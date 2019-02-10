using System;
using System.Net.Sockets;
using System.Net;
using System.Threading; //for thread sleep

namespace Extension.Socket
{
	public class ClientSocket : System.Net.Sockets.Socket
  {
    private static int DEFAULT_PORT_NO = 5123;
    protected int portNo = DEFAULT_PORT_NO;
    public int PortNo { get { return portNo; } }

    private IPAddress ipv4Address;

    public delegate void PackageHandlerCallback(byte[] package); //the delegate should have a "signature"
    public PackageHandlerCallback PackageHandler = null; //initialized as null, must be stated outside

    public delegate void ConnectionHandlerCallback(string connMsg); //the delegate should have a "signature"
    public ConnectionHandlerCallback ConnectionHandler = null; //initialized as null, must be stated outside

    public delegate void ErrorMessageHandlerCallback(string errorStr); //the delegate should have a "signature"
    public ErrorMessageHandlerCallback ErrorMessageHandler = null; //initialized as null, must be stated outside

    public event EventHandler Disconnected = null;

    private const int BUFFER_SIZE = 4096;
    private byte[] buffer = new byte[BUFFER_SIZE]; //buffer size is limited to BUFFER_SIZE per message

    protected DateTime lastStreamReceived = DateTime.Now;
    public DateTime LastStreamReceived { get { return lastStreamReceived; } }

    public ClientSocket()
      : this(DEFAULT_PORT_NO, IPAddress.Loopback) { //constructor should take care of as many things as possible
    }

    public ClientSocket(IPAddress ipv4Address) :
      this(DEFAULT_PORT_NO, ipv4Address) {
    }

    public ClientSocket(int portNo)
      : this(portNo, IPAddress.Loopback) {
    }

    public ClientSocket(int portNo, IPAddress ipv4Address)
      : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
      this.portNo = portNo;
      this.ipv4Address = ipv4Address;
    }

    protected override void Dispose(bool disposing) {
      PackageHandler = null;
      ConnectionHandler = null;
      ErrorMessageHandler = null;
      base.Dispose(disposing);                  
    }

		public void LoopConnect(int noOfRetry, int attemptPeriodInSeconds) {
			int attempts = 0;
			while (!this.Connected && attempts < noOfRetry) {
				try {
					++attempts;
					IAsyncResult result = this.BeginConnect(ipv4Address, portNo, endConnect, null);
					result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(attemptPeriodInSeconds));
					Thread.Sleep(attemptPeriodInSeconds * 1000);
				} catch (Exception e) {
					if (ConnectionHandler != null)
						ConnectionHandler("Unsuccessful connecting attempt\n");
					if (ErrorMessageHandler != null)
						ErrorMessageHandler("Error: " + e.ToString() + "\n");
				}
				if (ConnectionHandler != null)
					ConnectionHandler("Connection attempt: " + attempts.ToString() + "\n");
			}
			if (ErrorMessageHandler != null && !this.Connected) {
				ErrorMessageHandler("Connection attempt is unsuccessful!\n");
				return;
			}
			if (ConnectionHandler != null)
				ConnectionHandler("Connection attempt is " + (this.Connected ? "successful!" : "unsuccessful!") + "\n");
		}

    private void endConnect(IAsyncResult result) {
      try {
        this.EndConnect(result);
        if (this.Connected) {
          this.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), this);
          lastStreamReceived = DateTime.Now;
        } else if (ErrorMessageHandler != null)
          ErrorMessageHandler("End of connection attempt, fail to connect...\n");
      } catch (Exception e) {
        if (ErrorMessageHandler != null)
          ErrorMessageHandler("End-connection attempt is unsuccessful! " + e.ToString() + "\n");
      }
    }

    public void ResetLastStreamReceivedTime() {
      lastStreamReceived = DateTime.Now;
    }

    private bool isNullData(byte[] data, int maxEvaluation) {
      if (data == null || maxEvaluation < 1 || data.Length < 1)
        return true;
      if (data.Length > maxEvaluation) //if data length is greater than to be evaluate, assume not null...
        return false;
      for (int i = 0; i < Math.Min(data.Length, maxEvaluation); ++i)
        if (data[i] != 0)
          return false;
      return true;
    }

		const int MAX_NULL_DATA_EVALUATION = 10;
		const int MAX_RECEIVE_ATTEMPT = 10;
    int receiveAttempt = 0;
    private void receiveCallback(IAsyncResult result) {
			System.Net.Sockets.Socket socket = null;
      try {
				socket = (System.Net.Sockets.Socket)result.AsyncState; //this is itself...?
        if (socket.Connected) {
          int received = socket.EndReceive(result);
          if (received > 0) {
            receiveAttempt = 0;
            byte[] data = new byte[received];
            Buffer.BlockCopy(buffer, 0, data, 0, data.Length); //There are several way to do this according to http://stackoverflow.com/questions/5099604/any-faster-way-of-copying-arrays-in-c in general, System.Buffer.memcpyimpl is the fastest
            lastStreamReceived = DateTime.Now;
            if (PackageHandler != null && !isNullData(data, MAX_NULL_DATA_EVALUATION)) //ping is to be ignored...              
              PackageHandler(data);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
          } else if (receiveAttempt < MAX_RECEIVE_ATTEMPT) {
            ++receiveAttempt;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
          } else { //completely fails!
            if (ErrorMessageHandler != null)
              ErrorMessageHandler("receiveCallback is failed!\n");
            receiveAttempt = 0;
            this.Close();
          }
        }
      } catch (Exception e) { // this exception will happen when "this" is be disposed...
        if (ErrorMessageHandler != null)
          ErrorMessageHandler("receiveCallback is failed! " + e.ToString() + "\n");
        if (Disconnected != null)
          Disconnected(this, new EventArgs());
        //Do socket closing on failure! but at this moment, this must be done outside! Not sure ie this is the best idea...
      }
    }
  }
}
