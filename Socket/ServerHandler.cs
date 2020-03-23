using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Extension.Socket {
	public class ServerHandler { //This class helping to synchronize the context with the main thread and calls it event based, rather than having the form to handle the synchronization
		//The socket
		private ServerSocket serverSocket; //is not made public for good reason...
		public int ClientNo { get { return serverSocket == null ? -1 : serverSocket.NoOfClient; } }
		public TCPIPServerSettings Settings = new TCPIPServerSettings();
    public List<System.Net.Sockets.Socket> ClientSockets => serverSocket?.GetAllClientSockets();

		//Timer
		private Timer timer = new Timer();
		private DateTime lastHeartBeatBroadcast;
		private DateTime nextHeartBeatBroadcast; //TODO concept of next heart beat broadcast may be used to prevent shifting heart-beat time
		private bool heartBeatIsEnabled = false; //when enabled, the "tick" matters (maybe redundant) must be false by default!
		public bool HeartBeatIsEnabled {
			get { return heartBeatIsEnabled; }
			set {
				heartBeatIsEnabled = value;
				timer.Enabled = serverSocket == null ? false : value; //if the server socket is null, this is always false... Else, this can be set true
			}
		}

		//The synchronizer
		private System.Threading.SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current; //using qualified method to avoid conflict with Timer class

		//The events and the callbacks
		public delegate void ServerEventHandler(object sender, ServerEventArgs e);
		public event ServerEventHandler PackageReceived; //to represent receiving a package
		public event ServerEventHandler ClientAccepted;
		public event ServerEventHandler ClientDisposed;
		public event ServerEventHandler ErrorMessageReceived;
		public event ServerEventHandler SendCompleted;

		//The messages
		private string initMessage = "uninitialized";
		public string InitMessage { get { return initMessage; } }
		private bool initResult = false;
		public bool InitResult { get { return initResult; } }
		private bool autoOpenResult = false;
		public bool AutoOpenResult { get { return autoOpenResult; } }

		public ServerHandler() {
		}

		public ServerHandler(string settingsFolderpath, string settingsFilenameWithoutXMLExtension) {
			SetSettingsFromXMLFileSecure(settingsFolderpath, settingsFilenameWithoutXMLExtension);
		}

		public void SetPingMessage(byte[] msg) {
			if (msg == null || msg.Length < 1)
				return;
			serverSocket.PingMessage = msg;
		}

		public void InitHeartBeatTimerSettings() {
      //The Timer
      timer.Tick -= timer_Tick;
      timer.Interval = Settings.MinimumHeartBeatRate; //the timer interval is faster, to check if it is time to do the heart-beating!
			timer.Enabled = HeartBeatIsEnabled;
			timer.Tick += timer_Tick;
			lastHeartBeatBroadcast = DateTime.Now;
			nextHeartBeatBroadcast = DateTime.Now.AddMilliseconds(Settings.HeartBeatRate);
		}

		public void InitiateMySocket() { //server port number is the only input needed, this also means request connection
			if (serverSocket == null) {
				serverSocket = new ServerSocket(Settings.PortNo, Settings.MaxNoOfPendingClient); //For now, number of pending client is fixed...
				serverSocket.PackageHandler += packageHandler; //perhaps, this is the best way to do it
				serverSocket.ErrorMessageHandler += errorMessageHandler;
				serverSocket.ClientAcceptedHandler += clientAcceptedHandler;
				serverSocket.ClientDisposedHandler += clientDisposedHandler;
			}
			timer.Enabled = HeartBeatIsEnabled; //if server is initialized when the heartBeat is initialized, start the timer. Otherwise, wait...
		}

		public void DisposeSocket() { //this also means request disconnection
			timer.Enabled = false;
			serverSocket.Close(); //all others don't work well
			serverSocket = null;
		}

		public void DisposeClientSocket(int clientIndex) {
			serverSocket.DisposeIndexedClientSocket(clientIndex);
		}

		public bool Send(int clientIndexNo, byte[] data) { //to send message to the client by index no
			System.Net.Sockets.Socket clientSocket = serverSocket.GetIndexedClientSocket(clientIndexNo);
			if (clientSocket == null)
				return false;
			clientSocket.Send(data);
			return true;
		}

		private bool isSendingAsync = false;
		public bool IsSendAsync { get { return isSendingAsync; } set { isSendingAsync = value; } }
		public bool SendAsync(int clientIndexNo, byte[] data) {
			System.Net.Sockets.Socket clientSocket = serverSocket.GetIndexedClientSocket(clientIndexNo);
			if (clientSocket == null)
				return false;
			isSendingAsync = true;
			clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
			return true;
		}

		private void sendCallback(IAsyncResult ar) {
			System.Net.Sockets.Socket clientSocket = ar.AsyncState as System.Net.Sockets.Socket;
			try {
				SocketError errorCode;
				clientSocket.EndSend(ar, out errorCode);
				errorMessageHandler(errorCode.ToString());
				isSendingAsync = false;
				sendCompletedHandler(); //Wrap this with EventHandler this would handle such event				
			} catch (Exception e) {
				errorMessageHandler(e.ToString());
			}
		}

		public void SetMyIPToLocalIP() {
			IPAddress ip = Array.Find( //Whether it is necessary to do this each time the server info is refreshed is not sure, but this is simply to "play-safe"
					Dns.GetHostEntry(string.Empty).AddressList,
					a => a.AddressFamily == AddressFamily.InterNetwork);
			Settings.IPV4Address = ip.ToString();
		}

		public bool IsMyTargetPortBusy() {
			return isEndPointBusy(Settings.IPV4Address, Settings.PortNo);
		}

		public void SetSettingsFromXMLFileSecure(string settingsFolderpath, string settingsFilenameWithoutXMLExtension, bool forceAutoOpen = false) {
			TCPIPServerSettings settings = new TCPIPServerSettings();
			initResult = false;
			if (string.IsNullOrWhiteSpace(settingsFolderpath) || string.IsNullOrWhiteSpace(settingsFilenameWithoutXMLExtension)) {
				Settings = settings;
				initMessage = "File or folder path is invalid. Default setting is used.";
				return;
			}
			string settingsFilepath = Path.Combine(settingsFolderpath, settingsFilenameWithoutXMLExtension + ".xml");
			try {
				if (!Directory.Exists(settingsFolderpath)) //create directory if the directory does not exist
					Directory.CreateDirectory(settingsFolderpath); //possible for this to fail if access is not correct				
				XmlSerializer serializerObj = new XmlSerializer(typeof(TCPIPServerSettings));
				if (File.Exists(settingsFilepath)) { //the filepath exists
					FileStream filestream = new FileStream(settingsFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
					settings = (TCPIPServerSettings)serializerObj.Deserialize(filestream);
					filestream.Close();
					initMessage = "Load setting is successful. Filepath: " + settingsFilepath;
					initResult = true;
				} else { //the filepath doesn't exist create one
					TextWriter configWriteFileStream = new StreamWriter(settingsFilepath);
					serializerObj.Serialize(configWriteFileStream, settings);
					configWriteFileStream.Close();
					initMessage = "Filepath doesn't exist. Default setting is used.";
				}
			} catch (Exception e) {
				initMessage = "Unable to load default TCP/IP settings config file(s)! Filepath: " +
					settingsFilepath + ", Exception error message: " + e.ToString();
			}
			Settings = settings;

			//Local IP
			if (Settings.FindLocalIP)
				SetMyIPToLocalIP(); //based on the settings, set/not set to local IP

			//Auto-open
			if (Settings.AutoOpen || forceAutoOpen)
				autoOpenResult = tryOpen();

			//Final touch
			InitHeartBeatTimerSettings();
		}

		private string tryOpenFailureMessage = "";
		public string TryOpenFailureMessage { get { return tryOpenFailureMessage; } }
		private bool tryOpen() {
			if (IsMyTargetPortBusy())
				return false;
			try {
				InitiateMySocket();
				return true;
			} catch (Exception e) {
				tryOpenFailureMessage = e.ToString();
				return false;
			}
		}

		private bool isEndPointBusy(string ipv4Str, int port) { //TODO not sure what it really does, may not be the correct name, but leave it for now...
			IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();
			if (endpoints == null || endpoints.Length == 0)
				return false;
			for (int i = 0; i < endpoints.Length; i++)
				if (endpoints[i].Port == port && endpoints[i].Address.ToString() == ipv4Str)
					return true;
			return false;
		}

		private void packageHandler(byte[] package) {
			if (syncContext != null)
				syncContext.Post(packageReceiver, package); //make sure to handle the package
			else {
				MethodInvoker minv = new MethodInvoker(() => packageReceiver(package));
				minv.Invoke();
			}

		}

		private string lastErrorMessageHandlerFailed = "";
		public string LastErrorMessageHandlerFailed { get { return lastErrorMessageHandlerFailed; } }
		private void errorMessageHandler(string errorStr) {
			try {
				if (syncContext != null)
					syncContext.Post(errorMessageReceiver, errorStr);
				else {
					MethodInvoker minv = new MethodInvoker(() => errorMessageReceiver(errorStr));
					minv.Invoke();
				}
			} catch (Exception e) { //at this moment don't do anything...
				lastErrorMessageHandlerFailed = DateTime.Now.ToString() + ": " + e.ToString();
			}
		}

		private void clientAcceptedHandler(System.Net.Sockets.Socket socket) { //TODO note that it is possible for the syncPost to be done later... thus the number will be changed if not recorded here, somehow..
			if (syncContext != null)
				syncContext.Post(clientAccepted, socket);
			else {
				MethodInvoker minv = new MethodInvoker(() => clientAccepted(socket));
				minv.Invoke();
			}
		}

		private void sendCompletedHandler() {
			if (syncContext != null)
				syncContext.Post(sendCompleted, null); //TODO later needs to consider what is to be sent
			else {
				MethodInvoker minv = new MethodInvoker(() => sendCompleted(null));
				minv.Invoke();
			}
		}

		private string lastClientDisposedMessageFailed = "";
		public string LastClientDisposedMessageFailed { get { return lastClientDisposedMessageFailed; } }
		private void clientDisposedHandler(System.Net.Sockets.Socket socket) {
			try {
				if (syncContext != null)
					syncContext.Post(clientDisposed, socket);
				else {
					MethodInvoker minv = new MethodInvoker(() => clientDisposed(socket));
					minv.Invoke();
				}
			} catch (Exception e) { //at this moment don't do anything...
				lastClientDisposedMessageFailed = DateTime.Now.ToString() + ": " + e.ToString();
			}
		}

		//Needed: no of client, no of accepted client, the received client socket IP address
		//so, we can create a delegate and to call it
		private void clientAccepted(object objSocket) {
			if (serverSocket != null && ClientAccepted != null)
				ClientAccepted(this, new ServerEventArgs(serverSocket.NoOfAcceptedClient, ((objSocket as System.Net.Sockets.Socket).RemoteEndPoint as IPEndPoint).Address));
		}

		//only needs the current number of client and the client number being disposed...
		private void clientDisposed(object objSocket) {
			if (serverSocket != null && ClientDisposed != null)
				ClientDisposed(this, new ServerEventArgs(serverSocket.BeingDisposedClientNo)); //the serverSocket here may have gone...
		}

		private void packageReceiver(object objPackage) { //only needs the package and the client sender no...
			if (serverSocket != null && PackageReceived != null)
				PackageReceived(this, new ServerEventArgs(objPackage as byte[], serverSocket.SenderClientNo));
		}

		private void errorMessageReceiver(object errorStr) {
			if (ErrorMessageReceived != null)
				ErrorMessageReceived(this, new ServerEventArgs(errorStr as string));
		}

		private void sendCompleted(object obj) {
			if (SendCompleted != null)
				SendCompleted(this, new ServerEventArgs()); //TODO later needs to decide what is inside
		}

		public void Ping() {
			serverSocket.PingAllClients();
		}

		void timer_Tick(object sender, EventArgs e) {
			if (serverSocket == null || !HeartBeatIsEnabled)
				return; //the server must be opened to continue
			if ((DateTime.Now - lastHeartBeatBroadcast).TotalMilliseconds >= Settings.HeartBeatRate) {
				serverSocket.PingAllClients();
				nextHeartBeatBroadcast = lastHeartBeatBroadcast.AddMilliseconds(Settings.HeartBeatRate);
				lastHeartBeatBroadcast = DateTime.Now;
			}
		}
	}
}