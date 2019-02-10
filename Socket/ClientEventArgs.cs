using System;

namespace Extension.Socket {
	public class ClientEventArgs : EventArgs {
		public byte[] Package = null; //to show received package
		public string Message = null; //to show error or timeout message
		public double TimeSinceLastMessageReceived = 0;
		public ClientEventArgs() { //used for non-defined call, including sudden disconnection
		}

		public ClientEventArgs(string message) { //used for message callback
			Message = message;
		}

		public ClientEventArgs(double timeSinceLastMessageReceived) { //used tick callback
			TimeSinceLastMessageReceived = timeSinceLastMessageReceived;
		}

		public ClientEventArgs(byte[] package) {
			Package = package;
		}
	}
}
