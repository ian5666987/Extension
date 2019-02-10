using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Extension.Debugger;
using Extension.Versioning;
using Extension.Checker;
using Extension.Controls;
using Extension.Socket;

namespace Extension.TcpWinForm
{
  public partial class TcpClientForm : Form
  {
    //Form
    private LogBoxForm logBox = new LogBoxForm();
    private SynchronizationContext syncContext = SynchronizationContext.Current;

    //Client, Server
    private ClientHandler clientHandler;
    private IPAddress myServerIpv4Address = Dns.GetHostEntry("localhost").AddressList[0]; //assuming local server until it is proven otherwise
    private IPAddress myClientIpv4Address;
    private int serverPortNo = 5123; //default port of mine
		private Color serverColor = Color.Blue;
    private TcpExchangeTabPage myTcpExchangeTabPage = null;

    public TcpClientForm() {
      InitializeComponent();

      //TCP/IP
      IPAddress[] ipv4Addresses = Array.FindAll(
          Dns.GetHostEntry(string.Empty).AddressList,
          a => a.AddressFamily == AddressFamily.InterNetwork);

      //The (assumed) Server
      myServerIpv4Address = ipv4Addresses[0];
      textBoxServerIpAddress.Text = myServerIpv4Address.ToString();
      textBoxServerPortNo.Text = serverPortNo.ToString();

      //The Client
      myClientIpv4Address = ipv4Addresses[0];
      clientHandler = new ClientHandler();
      clientHandler.TimerTick += clientHandler_TimerTick;
      clientHandler.Timeout += clientHandler_Timeout;
      clientHandler.PackageReceived += clientHandler_PackageReceived;
      clientHandler.ConnectionMessageReceived += clientHandler_ConnectionMessageReceived;
      clientHandler.ErrorMessageReceived += clientHandler_ErrorMessageReceived;
      clientHandler.SuddenDisconnection += clientHandler_SuddenDisconnection;
      updateClientIPPortStatus();
      
      //Log box initialization
      DateTime dt = TimeStamp.RetrieveLinkerTimestamp();
      logBox.Show();
      logBox.WriteTimedLog(this.Text.ToString() + " (c)" + dt.ToString("yyyy") + " - by Ian. Released: " + dt.ToString() + " (Singapore Time)\n"); //The first to be printed by the end of the initialization
    }

    void clientHandler_SuddenDisconnection(object sender, ClientEventArgs e) {
      closeConnection();
    }

    void clientHandler_ErrorMessageReceived(object sender, ClientEventArgs e) {
      logBox.WriteTimedLog(e.Message, Color.Red);
    }

    void clientHandler_ConnectionMessageReceived(object sender, ClientEventArgs e) {
      logBox.WriteTimedLog(e.Message, Color.Blue);
    }

    void clientHandler_PackageReceived(object sender, ClientEventArgs e) {
      myTcpExchangeTabPage.Write(e.Package, serverColor, "Server");
    }

    void clientHandler_Timeout(object sender, ClientEventArgs e) {
      logBox.WriteTimedLog(e.Message, Color.Red);
      closeConnection();
    }

    void clientHandler_TimerTick(object sender, ClientEventArgs e) {
      textBoxTimeOfTimeout.Text = ((int)e.TimeSinceLastMessageReceived).ToString();
    }

    private void checkBoxTimeout_CheckedChanged(object sender, EventArgs e) {
      clientHandler.TimeoutEnabled = (sender as CheckBox).Checked;
      if (clientHandler.TimeoutEnabled)
        textBoxTimeOfTimeout.Text = "0"; //always zeroed when it is just enabled...
    }

    private void updateClientConnectionControl(bool isConnected) {
      textBoxServerIpAddress.Enabled = !isConnected;
      textBoxServerPortNo.Enabled = !isConnected;
      linkLabelClientRefreshTCPIP.Enabled = !isConnected;
      groupBoxExchange.Enabled = isConnected;
      if (!isConnected)
        clientHandler.DisposeSocket();
    }

    private void updateClientIPPortStatus() {
      labelClientTCPIPPortAvailability.Text = "Not connected";
      labelClientTCPIPPortAvailability.ForeColor = Label.DefaultForeColor;
    }

    #region exchange page
    private void removeExhangePage() {
      if (myTcpExchangeTabPage != null) {
        tabControlExchange.Controls.Remove(myTcpExchangeTabPage);
        myTcpExchangeTabPage.Dispose();
        myTcpExchangeTabPage = null;
      }
    }
    #endregion

    private void textBoxServerIpAddress_TextChanged(object sender, EventArgs e) {
      TextBox thisTextBox = sender as TextBox;
      thisTextBox.ForeColor = Extension.Checker.Text.CheckTcpIpFormatValidity(thisTextBox.Text) ? TextBox.DefaultForeColor : Color.Red;
      if (thisTextBox.ForeColor == Color.Red)
        labelClientTCPIPPortAvailability.Text = "Invalid input";
      else {
        myServerIpv4Address = IPAddress.Parse(textBoxServerIpAddress.Text);
        updateClientIPPortStatus();
      }
    }

    private void textBoxServerPortNo_TextChanged(object sender, EventArgs e) {
      TextBox thisTextBox = sender as TextBox;
			if (Extension.Checker.Text.CheckTextValidity(thisTextBox.Text, TextType.IntegerType, 0, 99999)) { //increased from up to 65535 to up to 99999
        thisTextBox.ForeColor = TextBox.DefaultForeColor;
        serverPortNo = Convert.ToInt32(thisTextBox.Text);
        updateClientIPPortStatus();
      } else {
        thisTextBox.ForeColor = Color.Red;
        labelClientTCPIPPortAvailability.Text = "Invalid input";
        labelClientTCPIPPortAvailability.ForeColor = Color.Red;
      }
    }

    private void buttonClientTCPIPConnect_Click(object sender, EventArgs e) {
      if (textBoxServerIpAddress.ForeColor != Color.Red) {
        Button thisButton = sender as Button;
        myServerIpv4Address = IPAddress.Parse(textBoxServerIpAddress.Text);
        updateClientIPPortStatus();
        if (thisButton.Text == "Connect") {
          logBox.WriteTimedLog("Attempting TCP/IP connection...\n", Color.Blue);
          try {
            startConnection();
          } catch (Exception exc) {
            updateClientConnectionControl(false);
            labelClientTCPIPPortAvailability.ForeColor = Color.Red;
            labelClientTCPIPPortAvailability.Text = "Connecting failed!";
            logBox.WriteTimedLog("Connecting attempt through TCP/IP is failed!\n", Color.Red);
            logBox.WriteTimedLog(exc.ToString() + "\n", Color.Red);
            removeExhangePage();
          }
        } else if (thisButton.Text == "Disconnect") {
          try {
            closeConnection();
          } catch (Exception exc) {
            labelClientTCPIPPortAvailability.Text = "Disconnecting failed!";
            logBox.WriteTimedLog("Disconnecting attempt from TCP/IP is failed!\n", Color.Red);
            logBox.WriteTimedLog(exc.ToString() + "\n", Color.Red);
          }
        }
      }
    }

    //For the first time I will learn about callback as well...
    private void startConnection() {
      logBox.WriteTimedLog("Attempting to connect to the TCP/IP server...\n", Color.Blue);
      labelClientTCPIPPortAvailability.ForeColor = Label.DefaultForeColor;
      buttonClientTCPIPConnect.Enabled = false;
      labelClientTCPIPPortAvailability.Text = "Connecting...";
      clientHandler.InitiateSocket(myServerIpv4Address, serverPortNo);
      buttonClientTCPIPConnect.Enabled = true;
      updateClientConnectionControl(clientHandler.IsConnected()); //using real connectivity value
      if (clientHandler.IsConnected()) {
        buttonClientTCPIPConnect.Text = "Disconnect";
        labelClientTCPIPPortAvailability.Text = "Connected";
        logBox.WriteTimedLog("Connected to IP address: " + myServerIpv4Address.ToString() + ", Port: " + serverPortNo.ToString() + "\n", Color.Blue);
        logBox.WriteTimedLog("Connecting attempt through TCP/IP is successful!\n", Color.Green);
        if (myTcpExchangeTabPage == null) {
          myTcpExchangeTabPage = new TcpExchangeTabPage(1);
          myTcpExchangeTabPage.OnAbort += myTcpExchangeTabPage_OnAbort;
        }
        tabControlExchange.Controls.Add(myTcpExchangeTabPage);
        myTcpExchangeTabPage.WriteAddress(clientHandler.ServerIPAddress.ToString());
      } else {
        labelClientTCPIPPortAvailability.ForeColor = Color.Red;
        labelClientTCPIPPortAvailability.Text = "Timeout!";
        logBox.WriteTimedLog("Connecting attempt through TCP/IP is failed! Timeout Occurs!\n", Color.Red);
        removeExhangePage();
      }
    }

    void myTcpExchangeTabPage_OnAbort(object sender, EventArgs e) {
      if (MessageBox.Show("Do you really want to remove this connection?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        closeConnection(); //discarding here would also mean closing of connection by necessity (unlike the server)
    }

    private void closeConnection() {
      logBox.WriteTimedLog("Attempting to close the TCP/IP connection...\n", Color.Blue);
      buttonClientTCPIPConnect.Text = "Connect";
      textBoxTimeOfTimeout.Text = "0";
      clientHandler.DisposeSocket();
      labelClientTCPIPPortAvailability.Text = "Disconnected";
      logBox.WriteTimedLog("TCP/IP connection is gracefully closed\n", Color.Green);
      updateClientConnectionControl(false);
      removeExhangePage();
    }

    private void linkLabelClientRefreshTCPIP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      updateClientIPPortStatus();
      buttonClientTCPIPConnect.Enabled = true;
    }

    private void checkMessage() {
      richTextBoxMessage.ForeColor = checkBoxHexFormat.Checked ? Color.Red : TextBox.DefaultForeColor;
      if (checkBoxHexFormat.Checked) {
        try {
					if (Extension.Checker.Text.IsSpacedHexString(richTextBoxMessage.Text))
            richTextBoxMessage.ForeColor = TextBox.DefaultForeColor;
        } catch (Exception e) {
          logBox.WriteTimedLog(e.ToString() + "\n");
        }
      }
    }

    private void buttonSend_Click(object sender, EventArgs e) {
      if (richTextBoxMessage.ForeColor == Color.Red || richTextBoxMessage.TextLength < 1) {
        MessageBox.Show("Invalid or null message!", "Error");
        logBox.WriteTimedLog("Invalid or null message!\n", Color.Red);
        return;
      }
      if (!clientHandler.IsConnected()) {
        MessageBox.Show("No connection!", "Error");
        logBox.WriteTimedLog("No connection!\n", Color.Red);
        return;
      }
      List<byte> bytes = new List<byte>();
      if (checkBoxHexFormat.Checked) {
        byte[] data = Extension.Manipulator.Data.SpacedHexStringToBytes(richTextBoxMessage.Text);
        if (data == null || data.Length < 1)
          return; //something must be wrong
        bytes.AddRange(data);
      } else //ASCII format
        bytes.AddRange(Encoding.ASCII.GetBytes(richTextBoxMessage.Text));
      if (bytes.Count > 0) {
        myTcpExchangeTabPage.Write(bytes.ToArray(), RichTextBox.DefaultForeColor, "This Client");
        try {
          clientHandler.Send(bytes.ToArray());
          richTextBoxMessage.Clear();
        } catch (Exception exc) {
          clientHandler.DisposeSocket();
          logBox.WriteTimedLog("Client socket is not found! " + exc.ToString() + "\n");
        }
      }
    }

    private void richTextBoxMessage_TextChanged(object sender, EventArgs e) {
      checkMessage();
    }

    private void checkBoxHexFormat_CheckedChanged(object sender, EventArgs e) {
      checkMessage();
    }

    private void numericUpDownTimeout_ValueChanged(object sender, EventArgs e) {
      clientHandler.TimeoutValue = (int)((sender as NumericUpDown).Value);
    }

  }
}