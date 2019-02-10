using System;

namespace Extension.Socket {
	[Serializable()]
	public class TCPIPServerSettings {
		public bool FindLocalIP = true;
		public string IPV4Address = "127.0.0.1";
		public int PortNo = 51293;
		public bool EnableHeartBeat;
		public int MinimumHeartBeatRate = 100;
		private int heartBeatRate = 1000;
		public bool AutoOpen;
		public int MaxPortNo = 99999;
		public int MaxNoOfPendingClient = 50;
		public int HeartBeatRate {
			get { return heartBeatRate; }
			set { heartBeatRate = value < MinimumHeartBeatRate ? MinimumHeartBeatRate : value; }
		}
	}
}
