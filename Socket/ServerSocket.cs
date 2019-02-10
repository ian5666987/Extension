using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace Extension.Socket
{
	public class ServerSocket : System.Net.Sockets.Socket //so this is a serverSocket
  {
    private static int DEFAULT_PORT_NO = 5123;
    protected int portNo = DEFAULT_PORT_NO;
    public int PortNo { get { return portNo; } }

    private static int DEFAULT_MAX_PENDING_CLIENT = 5;
    protected int maxPendingClients = 5;
    public int MaxPendingClients { get { return maxPendingClients; } }

		List<KeyValuePair<int, System.Net.Sockets.Socket>> clientNoSocketList =
			new List<KeyValuePair<int, System.Net.Sockets.Socket>>(); //to keep track of the clients (for keeping Alive purpose, this is also useful...)
    protected int noOfClient = 0;
    public int NoOfClient { get { return noOfClient; } }

    private int noOfAcceptedClient = 0; //always increasing throughout the session... for creation purpose
    public int NoOfAcceptedClient { get { return noOfAcceptedClient; } }

    private int beingDisposedClientNo = 0; //record the disposed client before real disposion, for destruction purpose
    public int BeingDisposedClientNo { get { return beingDisposedClientNo; } }

    private int senderClientNo = 0; //record the senderClientNo before the actual sending
    public int SenderClientNo { get { return senderClientNo; } }

    public delegate void PackageHandlerCallback(byte[] package); //the delegate should have a "signature". event is just a pre-defined delegate
    public PackageHandlerCallback PackageHandler = null; //initialized as null, must be stated outside

    public delegate void ErrorMessageHandlerCallback(string errorStr);
    public ErrorMessageHandlerCallback ErrorMessageHandler = null;

		public delegate void ClientAcceptedHandlerCallback(System.Net.Sockets.Socket clientSocket);
    public ClientAcceptedHandlerCallback ClientAcceptedHandler = null;

		public delegate void ClientDisposedHandlerCallback(System.Net.Sockets.Socket clientSocket);
    public ClientDisposedHandlerCallback ClientDisposedHandler = null;

    private const int BUFFER_SIZE = 4096;
    private byte[] buffer = new byte[BUFFER_SIZE]; //buffer size is limited to BUFFER_SIZE per message

    public ServerSocket()
      : this(DEFAULT_PORT_NO) { //constructor should take care of as many things as possible
    }

    public ServerSocket(int portNo) 
      : this(portNo, DEFAULT_MAX_PENDING_CLIENT) {
    }

    public ServerSocket(int portNo, int maxPendingClients)
      : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
      //SocketType.
      //ProtocolType.
      this.portNo = portNo;
      this.maxPendingClients = maxPendingClients;
      this.Bind(new IPEndPoint(IPAddress.Any, portNo));
      this.Listen(maxPendingClients);
      this.BeginAccept(new AsyncCallback(acceptCallback), null);      
    }

    protected override void Dispose(bool disposing) {
      for (int i = clientNoSocketList.Count - 1; i >= 0; --i)
        DisposeIndexedClientSocket(clientNoSocketList[i].Key);
      ErrorMessageHandler = null; //is this necessary? needs to test
      PackageHandler = null;
      ClientAcceptedHandler = null;
      ClientDisposedHandler = null;
      base.Dispose(disposing);
    }

		public byte[] PingMessage = new byte[1] { 0 };
    public void PingAllClients() {      
			foreach (System.Net.Sockets.Socket socket in clientNoSocketList.Select(x => x.Value).ToList())
				socket.Send(PingMessage);
				//socket.BeginSend(ping, 0, ping.Length, SocketFlags.None, endSend, socket);
		}

		//private void endSend(IAsyncResult result) {			
		//}

		private void acceptSocket(System.Net.Sockets.Socket socket) {
      ++noOfAcceptedClient;
      ++noOfClient;
			clientNoSocketList.Add(new KeyValuePair<int, System.Net.Sockets.Socket>(noOfAcceptedClient, socket));
      ClientAcceptedHandler(socket);
    }

		private void disposeSocket(System.Net.Sockets.Socket socket, string errorStr = null) {
      if (socket != null) {
        int index = clientNoSocketList.FindIndex(x => x.Value == socket);
        beingDisposedClientNo = clientNoSocketList[index].Key;
        --noOfClient;
        ClientDisposedHandler(socket);
        clientNoSocketList.RemoveAt(index);
        socket.Dispose();
        socket = null;
      }
      if (ErrorMessageHandler != null && !string.IsNullOrEmpty(errorStr))
        ErrorMessageHandler(errorStr);
    }

    public void DisposeIndexedClientSocket(int clientIndex) {
			System.Net.Sockets.Socket socket = clientNoSocketList.Find(x => x.Key == clientIndex).Value;
      disposeSocket(socket, "Client " + clientIndex.ToString()
        + " [" + (socket.RemoteEndPoint as IPEndPoint).Address.ToString() + "]"
        + " is disposed by server request...\n");
    }

		public System.Net.Sockets.Socket GetIndexedClientSocket(int index) {
      return clientNoSocketList.Find(x => x.Key == index).Value;
    }

    private void acceptCallback(IAsyncResult result) { //if the buffer is old, then there might already be something there...
			System.Net.Sockets.Socket socket = null;
      try {
        socket = EndAccept(result); // The objectDisposedException will come here... thus, it is to be expected!
        acceptSocket(socket);
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
        BeginAccept(new AsyncCallback(acceptCallback), null);        
      } catch (Exception e) { // this exception will happen when "this" is be disposed...        
        disposeSocket(socket, e.ToString() + "\n");				
      }
    }

		int MAX_RECEIVE_ATTEMPT = 10;
		int receiveAttempt = 0;
		private void receiveCallback(IAsyncResult result) {
			System.Net.Sockets.Socket socket = null;
			try {
				socket = (System.Net.Sockets.Socket)result.AsyncState;
				if (socket.Connected) {
					int received = socket.EndReceive(result);
					int index = clientNoSocketList.FindIndex(x => x.Value == socket);
					if (received > 0) {
						byte[] data = new byte[received];
						Buffer.BlockCopy(buffer, 0, data, 0, data.Length); //There are several way to do this according to http://stackoverflow.com/questions/5099604/any-faster-way-of-copying-arrays-in-c in general, System.Buffer.memcpyimpl is the fastest
						if (PackageHandler != null) {
							senderClientNo = clientNoSocketList[index].Key;
							PackageHandler(data);
						}
						receiveAttempt = 0;
						socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
					} else if (receiveAttempt < MAX_RECEIVE_ATTEMPT) {
						++receiveAttempt;
						socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
					} else { //completely fails!
						disposeSocket(socket, "receiveCallback fails! Client " + clientNoSocketList[index].Key.ToString() 
							+ " [" + (socket.RemoteEndPoint as IPEndPoint).Address.ToString() + "] socket is disposed...\n");
						receiveAttempt = 0;
					}
				}
			} catch (Exception e) { // this exception will happen when "this" is be disposed...
				disposeSocket(socket, e.ToString() + "\n");
			}
		}
  }
}
