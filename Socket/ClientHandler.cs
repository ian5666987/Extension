using System;
using System.Windows.Forms;
using System.Net;

namespace Extension.Socket
{
  public class ClientHandler
  {
    //The socket
    private ClientSocket clientSocket;

    //Timer
    private Timer timer = new Timer();
    private bool timeoutEnabled = false;
    public bool TimeoutEnabled {
      get { return timeoutEnabled; }
      set {
        timeoutEnabled = value;
        if (value) //If timeout is just enabled, always reset the last stream received time...
          clientSocket.ResetLastStreamReceivedTime();        
        timer.Enabled = clientSocket == null ? false : value;        
      }
    }
    private int timeoutValue = 15; //TODO makes all these configurable outside
    public int TimeoutValue { get { return timeoutValue; } set { timeoutValue = value; } }

    //The connected server
    public IPAddress ServerIPAddress { get { return IsConnected() ? (clientSocket.RemoteEndPoint as IPEndPoint).Address : null; } }

    //The synchronizer
    private System.Threading.SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current; //using qualified method to avoid conflict with Timer class

    //Events
    public delegate void ClientEventHandler(object sender, ClientEventArgs e);
    public event ClientEventHandler Timeout; //to represent occurance of timeout
    public event ClientEventHandler TimerTick;
    public event ClientEventHandler PackageReceived;
    public event ClientEventHandler ErrorMessageReceived;
    public event ClientEventHandler ConnectionMessageReceived;
    public event ClientEventHandler SuddenDisconnection;

    public ClientHandler() {
      timer.Interval = 250;
      timer.Enabled = false;
      timer.Tick += timer_Tick;
    }

    void checkTimeout(double timeSpan) {
      if (timeSpan >= timeoutValue) { //timeout        
        timer.Enabled = false;
        if (Timeout != null)
          Timeout(this, new ClientEventArgs("Server cannot be found! Preparing for disconnection...\n"));
      }
    }

    void timer_Tick(object sender, EventArgs e) {
      if (clientSocket != null && TimerTick != null) {
        double timeSpan = (DateTime.Now - clientSocket.LastStreamReceived).TotalSeconds;
        TimerTick(this, new ClientEventArgs(timeSpan));
        checkTimeout(timeSpan);
      }
    }

    public void InitiateSocket(IPAddress serverIPAddress, int serverPortNo) {
      if (clientSocket == null) {
        clientSocket = new ClientSocket(serverPortNo, serverIPAddress);
        clientSocket.PackageHandler += packageHandler;
        clientSocket.ConnectionHandler += connectionMessageHandler;
        clientSocket.ErrorMessageHandler += errorMessageHandler;
        clientSocket.Disconnected += clientSocket_Disconnected;
        clientSocket.LoopConnect(5, 3); //should be started after declaration of all the handler. TODO makes the values configurable
      }
      timer.Enabled = timeoutEnabled; //successful connection makes the timer started if the checkbox is checked
    }

    public void DisposeSocket() {
      if (clientSocket != null) {
        clientSocket.Close(); //Close seems to be safer so far...
        clientSocket = null;
      }
    }

    public bool Send(byte[] data) { //to send message to the client by index no
      if (clientSocket == null)
        return false;
      clientSocket.Send(data);
      return true;
    }

    public bool IsConnected() {
      return clientSocket != null && clientSocket.Connected;
    }

    #region client event handler
    private void connectionMessageHandler(string connMsg) {
      syncContext.Post(connectionMessageReceiver, connMsg);
    }

    private void errorMessageHandler(string errorStr) {
      syncContext.Post(errorMessageReceiver, errorStr);
    }

    private void packageHandler(byte[] package) {
      syncContext.Post(packageReceiver, package);
    }

    void clientSocket_Disconnected(object sender, EventArgs e) {
      syncContext.Post(disconnectionHandler, sender);
    }

    private void disconnectionHandler(object objSocket) {
      if (SuddenDisconnection != null)
        SuddenDisconnection(this, new ClientEventArgs());
    }

    private void connectionMessageReceiver(object connStr) {
      if (ConnectionMessageReceived != null)
        ConnectionMessageReceived(this, new ClientEventArgs(connStr as string));
    }
    private void errorMessageReceiver(object errorStr) {
      if (ErrorMessageReceived != null)
        ErrorMessageReceived(this, new ClientEventArgs(errorStr as string));
    }

    private void packageReceiver(object objPackage) {
      if (PackageReceived != null)
        PackageReceived(this, new ClientEventArgs(objPackage as byte[]));
    }
    #endregion
  }

}
