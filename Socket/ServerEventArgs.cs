using System;
using System.Net;

namespace Extension.Socket {
	//Needed: no of client, no of accepted client, the received client socket IP address
	public class ServerEventArgs : EventArgs {
		public byte[] Package = null; //used for package received
		public int SenderClientNo = -1; //used for package received
		public int AcceptedClientNo = -1; //used for client accepted
		public string ErrorMessage = null; //used for error message received
		public IPAddress ClientIPAddress = null; //used for client accepted
		public int DisposedClientNo = -1; //used for client disposed

		public ServerEventArgs() { //used for non-defined call
		}

		public ServerEventArgs(byte[] package, int senderClientNo) { //used for package received
			Package = package;
			SenderClientNo = senderClientNo;
		}

		public ServerEventArgs(int acceptedClientNo, IPAddress ipAddress) { //used for client accepted
			AcceptedClientNo = acceptedClientNo;
			ClientIPAddress = ipAddress;
		}

		public ServerEventArgs(int disposedClientNo) { //used for client disposed
			DisposedClientNo = disposedClientNo;
		}

		public ServerEventArgs(string errorMessage) { //used for error messaging
			ErrorMessage = errorMessage;
		}
	}
}
