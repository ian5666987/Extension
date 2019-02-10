using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using Extension.Debugger;
using Extension.Versioning;
using Extension.Manipulator;
using Extension.Checker;
using Extension.Controls;
using Extension.Socket;

namespace Extension.TcpWinForm
{
  public partial class TcpServerForm : Form
  {
    //Typical initialization
    private LogBoxForm logBox = new LogBoxForm();
    private string root;

    //Directories and files
    private string configFoldername = "configs";
    private string tcpConnectionFilename = "tcpserverconnection";

    //Server
    private Color serverColor = Color.Blue;
		private ServerHandler serverHandler;

    public TcpServerForm() {
      InitializeComponent();
      
      //Directories
      root = Application.StartupPath;
      if (!Directory.Exists(root + "\\" + configFoldername))
        Directory.CreateDirectory(root + "\\" + configFoldername); //configuraton directory...

      //Configuration
			//FileStream filestream = null;
      string folderpath = root + "\\" + configFoldername;
			//XmlSerializer serializerObj;

      //Typical initialization
      DateTime dt = TimeStamp.RetrieveLinkerTimestamp();
      logBox.Show();
      logBox.WriteTimedLog(this.Text.ToString() + " (c)" + dt.ToString("yyyy") + " - by Ian. Released: " + dt.ToString() + " (Singapore Time)\n"); //The first to be printed by the end of the initialization      

      //Initialize the app settings, the most important...      
			serverHandler = new ServerHandler(folderpath, tcpConnectionFilename);
			logBox.WriteTimedLogLine(serverHandler.InitMessage, serverHandler.InitResult ? Color.Green : Color.Red);
      initGUISharedSettings(serverHandler.Settings);

      //Component initialization
      richTextBoxMessage.ForeColor = serverColor;

      //The Server
      updateServerIPPortStatus();
      serverHandler.ClientAccepted += serverHandler_ClientAccepted;
      serverHandler.ClientDisposed += serverHandler_ClientDisposed;
      serverHandler.PackageReceived += serverHandler_PackageReceived;
      serverHandler.ErrorMessageReceived += serverHandler_ErrorMessageReceived;

      //Auto-open
      if (serverHandler.Settings.AutoOpen)
				if (serverHandler.AutoOpenResult)
          updateServerConnectionControls("Open");
        else {
          logBox.WriteTimedLog("Auto-opening failed!\n", Color.Red);
          labelServerTCPIPPortAvailability.Text = "Opening failed!";
        }
    }

    #region GUI save and load
    private TCPIPServerSettings getTCPServerSettingsFromGUI() {
      bool autoOpen = serverHandler.Settings != null && serverHandler.Settings.AutoOpen;
      bool findLocalIP = serverHandler.Settings != null && serverHandler.Settings.FindLocalIP;
      int maxPortNo = serverHandler.Settings.MaxPortNo;
      int maxNoOfPendingClient = serverHandler.Settings.MaxNoOfPendingClient;
      int minimumHeartBeatRate = serverHandler.Settings.MinimumHeartBeatRate;
      TCPIPServerSettings settings = new TCPIPServerSettings();
      settings.IPV4Address = textBoxServerIpAddress.Text;
      settings.PortNo = Convert.ToInt32(textBoxServerPortNo.Text);
      settings.EnableHeartBeat = checkBoxHeartBeat.Checked;
      settings.HeartBeatRate = (int)numericUpDownHeartBeat.Value;
      settings.AutoOpen = autoOpen;
      settings.FindLocalIP = findLocalIP;
      settings.MaxPortNo = maxPortNo;
      settings.MaxNoOfPendingClient = maxNoOfPendingClient;
      settings.MinimumHeartBeatRate = minimumHeartBeatRate;
      return settings;
    }

    private void initGUISharedSettings(TCPIPServerSettings settings){
      textBoxServerIpAddress.Text = settings.IPV4Address == null ? "" : settings.IPV4Address;
      textBoxServerPortNo.Text = settings.PortNo.ToString();
      checkBoxHeartBeat.Checked = settings.EnableHeartBeat;
      numericUpDownHeartBeat.Value = settings.HeartBeatRate;      
    }

    private void saveSettingsBeforeClosing() {
      string folderpath = root + "\\" + configFoldername;
      if (!Directory.Exists(folderpath))
        Directory.CreateDirectory(folderpath); //configuration directory...

      //Shared settings
      TCPIPServerSettings tcpServerSettings = getTCPServerSettingsFromGUI();
      XmlSerializer serializerObj = new XmlSerializer(typeof(TCPIPServerSettings));
      string filepath = folderpath + "\\" + tcpConnectionFilename + ".xml";
      TextWriter configWriteFileStream = new StreamWriter(filepath);
      serializerObj.Serialize(configWriteFileStream, tcpServerSettings);
      configWriteFileStream.Close();
    }
    #endregion

    #region my server events
    void serverHandler_ErrorMessageReceived(object sender, ServerEventArgs e) {
      logBox.WriteTimedLog(e.ErrorMessage, Color.Red);
    }

    void serverHandler_PackageReceived(object sender, ServerEventArgs e) {
      TcpExchangeTabPage tabPage = exchangePageFind(e.SenderClientNo) as TcpExchangeTabPage;
      if (tabPage == null) //it means the tabPage is not created for this client, impossible actually
        return; //simply return for now
      tabPage.Write(e.Package, RichTextBox.DefaultForeColor, "Client " + e.SenderClientNo.ToString());
    }

    void serverHandler_ClientDisposed(object sender, ServerEventArgs e) {
      textBoxNoOfClient.Text = serverHandler.ClientNo.ToString();
      exchangePageDestruction(e.DisposedClientNo); //important thing is to determine the number for this destruction
    }

    void serverHandler_ClientAccepted(object sender, ServerEventArgs e) {
      textBoxNoOfClient.Text = serverHandler.ClientNo.ToString();
      TcpExchangeTabPage tabPage = new TcpExchangeTabPage(e.AcceptedClientNo);
      tabPage.OnAbort += tabPage_OnAbort;
      tabControlExchange.Controls.Add(tabPage);
      string ipStr = e.ClientIPAddress.ToString();
      tabPage.WriteAddress(ipStr);
      logBox.WriteTimedLog("Client " + e.AcceptedClientNo.ToString() + " [" + ipStr + "] " + "is accepted!\n", Color.Green); 
    }

    void tabPage_OnAbort(object sender, EventArgs e) {
      Button button = sender as Button;
      int clientIndex = Convert.ToInt32(button.Name.Substring(("buttonAbort").Length));
      if (MessageBox.Show("Do you really want to remove this connection?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        serverHandler.DisposeClientSocket(clientIndex); //when the socket is disposed, this will trigger ClientDisposed...
    }
    #endregion

    #region connectivity
    private void updateServerIPPortStatus() {
      bool isBusy = serverHandler.IsMyTargetPortBusy();
      labelServerTCPIPPortAvailability.Text = "Port is " + (isBusy ? "busy" : "available");
      labelServerTCPIPPortAvailability.ForeColor = isBusy ? Color.Red : Label.DefaultForeColor;
    }

    private void updateServerConnectionControls(string connectivityString) {
      bool isConnected = connectivityString == "Open";
      string pastParticiple = isConnected ? "ed" : "d";
      buttonServerTCPIPOpen.Text = isConnected ? "Close" : "Open";
      textBoxServerPortNo.Enabled = !isConnected;
      linkLabelServerRefreshTCPIP.Enabled = !isConnected;
      labelServerTCPIPPortAvailability.Text = "Port " + connectivityString + pastParticiple;
      logBox.WriteTimedLog("Port " + connectivityString + pastParticiple + ": " + serverHandler.Settings.IPV4Address + 
        ", Port: " + serverHandler.Settings.PortNo.ToString() + "\n");
      groupBoxExchange.Enabled = isConnected;
    }

    private void buttonServerTCPIPOpen_Click(object sender, EventArgs e) {
      Button thisButton = sender as Button;
      if (thisButton.Text == "Open") {
        if (!serverHandler.IsMyTargetPortBusy()) { //If client port is available (no need to check local port again, given this)... TODO may be improved... bad idea in fact..
          try {
            serverHandler.InitiateMySocket();
          } catch (Exception exc){
            logBox.WriteTimedLog("Opening failed! " + exc.ToString() + "\n", Color.Red);
            labelServerTCPIPPortAvailability.Text = "Opening failed!";
            return;
          }
          updateServerConnectionControls(thisButton.Text);
        }
      } else if (thisButton.Text == "Close") {
        updateServerConnectionControls(thisButton.Text);
        removeAllExchangePages();
        //serverHandler.DisposeClientSocket();
        textBoxNoOfClient.Text = "0"; //no client is excpected when socket is completely closed        
        serverHandler.DisposeSocket();
        logBox.WriteTimedLog("Connectivity closed!\n");
      }
    }
    #endregion

    #region exchange page
    private TabPage exchangePageFind(int index) {
      string no = index.ToString();
      foreach (TabPage tabPage in tabControlExchange.TabPages)
        if (tabPage.Text == no)
          return tabPage;
      return null; //fails!
    }

    private void exchangePageDestruction(int index) {
      string no = index.ToString();
      foreach (TabPage tabPage in tabControlExchange.TabPages)
        if (tabPage.Text == no)
          tabControlExchange.Controls.Remove(tabPage);      
    }

    private void removeAllExchangePages() {
      while (tabControlExchange.TabPages.Count > 0) //actually can go one round...
        foreach (TabPage tabPage in tabControlExchange.TabPages) {
          //serverHandler.DisposeClientSocket(Convert.ToInt32(tabPage.Text));
          logBox.WriteTimedLog("TabPage: " + tabPage.Text + " is removed!\n", Color.Red);
          tabControlExchange.Controls.Remove(tabPage);
        }
    }
    #endregion

    #region message checking
    private void textBoxServerPortNo_TextChanged(object sender, EventArgs e) {
      TextBox thisTextBox = sender as TextBox;
      if (Extension.Checker.Text.CheckTextValidity(thisTextBox.Text, TextType.IntegerType, 0, serverHandler.Settings.MaxPortNo)) {
        thisTextBox.ForeColor = TextBox.DefaultForeColor;
        serverHandler.Settings.PortNo = Convert.ToInt32(thisTextBox.Text);
        updateServerIPPortStatus();
      } else {
        thisTextBox.ForeColor = Color.Red;
        labelServerTCPIPPortAvailability.Text = "Invalid input";
        labelServerTCPIPPortAvailability.ForeColor = Color.Red;
      }
    }

    private void checkBoxHexFormat_CheckedChanged(object sender, EventArgs e) {
      checkMessage();
    }

    private void checkMessage() {
      richTextBoxMessage.ForeColor = checkBoxHexFormat.Checked ? Color.Red : serverColor;
      if (!checkBoxHexFormat.Checked)
        return;
      try {
        if (Extension.Checker.Text.IsSpacedHexString(richTextBoxMessage.Text))
          richTextBoxMessage.ForeColor = serverColor;
      } catch (Exception e) {
        logBox.WriteTimedLog(e.ToString() + "\n");
      }
    }
    #endregion

    #region component event handler
    private void linkLabelServerRefreshTCPIP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      updateServerIPPortStatus();
    }

    private void buttonSend_Click(object sender, EventArgs e) {
      TcpExchangeTabPage tabPage = tabControlExchange.SelectedTab as TcpExchangeTabPage;
      if (tabPage == null) {//there must be a selected tabPage (just in case if no tabPage is present, it won't crash)
        MessageBox.Show("Target not selected or there is no target.", "Error");
        logBox.WriteTimedLog("Target not selected or there is no target.\n", Color.Red);
        return;
      }
      if (richTextBoxMessage.ForeColor == Color.Red || richTextBoxMessage.TextLength < 1) {
        MessageBox.Show("Invalid or null message!", "Error");
        logBox.WriteTimedLog("Invalid or null message!\n", Color.Red);
        return;
      }
      List<byte> bytes = new List<byte>();
      if (checkBoxHexFormat.Checked) {
        byte[] data = Data.SpacedHexStringToBytes(richTextBoxMessage.Text);
        if (data == null || data.Length < 1) //this is correct as for sending, because it detects something is wrong... however, this is undetected outside?
          return; //something must be wrong
        bytes.AddRange(data);
      } else //ASCII format
        bytes.AddRange(Encoding.ASCII.GetBytes(richTextBoxMessage.Text));
      if (bytes.Count > 0) {
        tabPage.Write(bytes.ToArray(), serverColor, "Server"); //serverColor is blue...
        serverHandler.Send(Convert.ToInt32(tabPage.Text), bytes.ToArray());
        richTextBoxMessage.Clear();
      }
    }

    private void checkBoxHeartBeat_CheckedChanged(object sender, EventArgs e) {
      if (serverHandler != null)
        serverHandler.HeartBeatIsEnabled = checkBoxHeartBeat.Checked;
    }

    private void numericUpDownHeartBeat_ValueChanged(object sender, EventArgs e) {
      if (serverHandler != null)
        serverHandler.Settings.HeartBeatRate = (int)numericUpDownHeartBeat.Value;
    }

    private void richTextBoxMessage_TextChanged(object sender, EventArgs e) {
      checkMessage();
    }
    #endregion
  }
}