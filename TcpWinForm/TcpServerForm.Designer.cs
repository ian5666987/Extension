namespace Extension.TcpWinForm
{
  partial class TcpServerForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      saveSettingsBeforeClosing();
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TcpServerForm));
			this.groupBoxTCPIPServer = new System.Windows.Forms.GroupBox();
			this.checkBoxHeartBeat = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownHeartBeat = new System.Windows.Forms.NumericUpDown();
			this.textBoxNoOfClient = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxServerIpAddress = new System.Windows.Forms.TextBox();
			this.labelServerTCPIPPortAvailability = new System.Windows.Forms.Label();
			this.buttonServerTCPIPOpen = new System.Windows.Forms.Button();
			this.linkLabelServerRefreshTCPIP = new System.Windows.Forms.LinkLabel();
			this.labelServerIPPortNo = new System.Windows.Forms.Label();
			this.textBoxServerPortNo = new System.Windows.Forms.TextBox();
			this.groupBoxExchange = new System.Windows.Forms.GroupBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControlExchange = new System.Windows.Forms.TabControl();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
			this.checkBoxHexFormat = new System.Windows.Forms.CheckBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.checkBoxScrollToCaret = new System.Windows.Forms.CheckBox();
			this.groupBoxTCPIPServer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeartBeat)).BeginInit();
			this.groupBoxExchange.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxTCPIPServer
			// 
			this.groupBoxTCPIPServer.Controls.Add(this.checkBoxHeartBeat);
			this.groupBoxTCPIPServer.Controls.Add(this.label3);
			this.groupBoxTCPIPServer.Controls.Add(this.numericUpDownHeartBeat);
			this.groupBoxTCPIPServer.Controls.Add(this.textBoxNoOfClient);
			this.groupBoxTCPIPServer.Controls.Add(this.label2);
			this.groupBoxTCPIPServer.Controls.Add(this.textBoxServerIpAddress);
			this.groupBoxTCPIPServer.Controls.Add(this.labelServerTCPIPPortAvailability);
			this.groupBoxTCPIPServer.Controls.Add(this.buttonServerTCPIPOpen);
			this.groupBoxTCPIPServer.Controls.Add(this.linkLabelServerRefreshTCPIP);
			this.groupBoxTCPIPServer.Controls.Add(this.labelServerIPPortNo);
			this.groupBoxTCPIPServer.Controls.Add(this.textBoxServerPortNo);
			this.groupBoxTCPIPServer.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBoxTCPIPServer.Location = new System.Drawing.Point(0, 0);
			this.groupBoxTCPIPServer.Margin = new System.Windows.Forms.Padding(4);
			this.groupBoxTCPIPServer.Name = "groupBoxTCPIPServer";
			this.groupBoxTCPIPServer.Padding = new System.Windows.Forms.Padding(4);
			this.groupBoxTCPIPServer.Size = new System.Drawing.Size(824, 63);
			this.groupBoxTCPIPServer.TabIndex = 53;
			this.groupBoxTCPIPServer.TabStop = false;
			this.groupBoxTCPIPServer.Text = "Server TCP/IP";
			// 
			// checkBoxHeartBeat
			// 
			this.checkBoxHeartBeat.AutoSize = true;
			this.checkBoxHeartBeat.Checked = true;
			this.checkBoxHeartBeat.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxHeartBeat.Location = new System.Drawing.Point(643, 24);
			this.checkBoxHeartBeat.Name = "checkBoxHeartBeat";
			this.checkBoxHeartBeat.Size = new System.Drawing.Size(66, 21);
			this.checkBoxHeartBeat.TabIndex = 42;
			this.checkBoxHeartBeat.Text = "HBeat";
			this.checkBoxHeartBeat.UseVisualStyleBackColor = true;
			this.checkBoxHeartBeat.CheckedChanged += new System.EventHandler(this.checkBoxHeartBeat_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(779, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 17);
			this.label3.TabIndex = 41;
			this.label3.Text = "ms";
			// 
			// numericUpDownHeartBeat
			// 
			this.numericUpDownHeartBeat.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericUpDownHeartBeat.Location = new System.Drawing.Point(713, 23);
			this.numericUpDownHeartBeat.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.numericUpDownHeartBeat.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericUpDownHeartBeat.Name = "numericUpDownHeartBeat";
			this.numericUpDownHeartBeat.Size = new System.Drawing.Size(63, 23);
			this.numericUpDownHeartBeat.TabIndex = 40;
			this.numericUpDownHeartBeat.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownHeartBeat.ValueChanged += new System.EventHandler(this.numericUpDownHeartBeat_ValueChanged);
			// 
			// textBoxNoOfClient
			// 
			this.textBoxNoOfClient.Location = new System.Drawing.Point(531, 22);
			this.textBoxNoOfClient.Name = "textBoxNoOfClient";
			this.textBoxNoOfClient.ReadOnly = true;
			this.textBoxNoOfClient.Size = new System.Drawing.Size(41, 23);
			this.textBoxNoOfClient.TabIndex = 39;
			this.textBoxNoOfClient.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(576, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 17);
			this.label2.TabIndex = 38;
			this.label2.Text = "Clients";
			// 
			// textBoxServerIpAddress
			// 
			this.textBoxServerIpAddress.Location = new System.Drawing.Point(68, 22);
			this.textBoxServerIpAddress.Margin = new System.Windows.Forms.Padding(4);
			this.textBoxServerIpAddress.Name = "textBoxServerIpAddress";
			this.textBoxServerIpAddress.ReadOnly = true;
			this.textBoxServerIpAddress.Size = new System.Drawing.Size(121, 23);
			this.textBoxServerIpAddress.TabIndex = 37;
			this.textBoxServerIpAddress.Text = "127.0.0.1";
			// 
			// labelServerTCPIPPortAvailability
			// 
			this.labelServerTCPIPPortAvailability.AutoSize = true;
			this.labelServerTCPIPPortAvailability.Location = new System.Drawing.Point(416, 18);
			this.labelServerTCPIPPortAvailability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelServerTCPIPPortAvailability.MaximumSize = new System.Drawing.Size(107, 34);
			this.labelServerTCPIPPortAvailability.MinimumSize = new System.Drawing.Size(107, 34);
			this.labelServerTCPIPPortAvailability.Name = "labelServerTCPIPPortAvailability";
			this.labelServerTCPIPPortAvailability.Size = new System.Drawing.Size(107, 34);
			this.labelServerTCPIPPortAvailability.TabIndex = 36;
			this.labelServerTCPIPPortAvailability.Text = "Initialized";
			this.labelServerTCPIPPortAvailability.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonServerTCPIPOpen
			// 
			this.buttonServerTCPIPOpen.Location = new System.Drawing.Point(320, 19);
			this.buttonServerTCPIPOpen.Margin = new System.Windows.Forms.Padding(4);
			this.buttonServerTCPIPOpen.Name = "buttonServerTCPIPOpen";
			this.buttonServerTCPIPOpen.Size = new System.Drawing.Size(93, 30);
			this.buttonServerTCPIPOpen.TabIndex = 33;
			this.buttonServerTCPIPOpen.Text = "Open";
			this.buttonServerTCPIPOpen.UseVisualStyleBackColor = true;
			this.buttonServerTCPIPOpen.Click += new System.EventHandler(this.buttonServerTCPIPOpen_Click);
			// 
			// linkLabelServerRefreshTCPIP
			// 
			this.linkLabelServerRefreshTCPIP.AutoSize = true;
			this.linkLabelServerRefreshTCPIP.Location = new System.Drawing.Point(259, 27);
			this.linkLabelServerRefreshTCPIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.linkLabelServerRefreshTCPIP.Name = "linkLabelServerRefreshTCPIP";
			this.linkLabelServerRefreshTCPIP.Size = new System.Drawing.Size(58, 17);
			this.linkLabelServerRefreshTCPIP.TabIndex = 33;
			this.linkLabelServerRefreshTCPIP.TabStop = true;
			this.linkLabelServerRefreshTCPIP.Text = "Refresh";
			this.linkLabelServerRefreshTCPIP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelServerRefreshTCPIP_LinkClicked);
			// 
			// labelServerIPPortNo
			// 
			this.labelServerIPPortNo.AutoSize = true;
			this.labelServerIPPortNo.Location = new System.Drawing.Point(8, 24);
			this.labelServerIPPortNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelServerIPPortNo.Name = "labelServerIPPortNo";
			this.labelServerIPPortNo.Size = new System.Drawing.Size(50, 17);
			this.labelServerIPPortNo.TabIndex = 35;
			this.labelServerIPPortNo.Text = "IP/Port";
			// 
			// textBoxServerPortNo
			// 
			this.textBoxServerPortNo.Location = new System.Drawing.Point(199, 22);
			this.textBoxServerPortNo.Margin = new System.Windows.Forms.Padding(4);
			this.textBoxServerPortNo.Name = "textBoxServerPortNo";
			this.textBoxServerPortNo.Size = new System.Drawing.Size(55, 23);
			this.textBoxServerPortNo.TabIndex = 34;
			this.textBoxServerPortNo.Text = "9377";
			this.textBoxServerPortNo.TextChanged += new System.EventHandler(this.textBoxServerPortNo_TextChanged);
			// 
			// groupBoxExchange
			// 
			this.groupBoxExchange.Controls.Add(this.splitContainer1);
			this.groupBoxExchange.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxExchange.Enabled = false;
			this.groupBoxExchange.Location = new System.Drawing.Point(0, 63);
			this.groupBoxExchange.Name = "groupBoxExchange";
			this.groupBoxExchange.Size = new System.Drawing.Size(824, 500);
			this.groupBoxExchange.TabIndex = 59;
			this.groupBoxExchange.TabStop = false;
			this.groupBoxExchange.Text = "Exchange";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(3, 19);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tabControlExchange);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(818, 478);
			this.splitContainer1.SplitterDistance = 404;
			this.splitContainer1.TabIndex = 61;
			// 
			// tabControlExchange
			// 
			this.tabControlExchange.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlExchange.Location = new System.Drawing.Point(0, 0);
			this.tabControlExchange.Name = "tabControlExchange";
			this.tabControlExchange.SelectedIndex = 0;
			this.tabControlExchange.Size = new System.Drawing.Size(818, 404);
			this.tabControlExchange.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			this.splitContainer2.Panel1.Controls.Add(this.pictureBox1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(818, 70);
			this.splitContainer2.SplitterDistance = 71;
			this.splitContainer2.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.DarkBlue;
			this.label1.Location = new System.Drawing.Point(5, 4);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 17);
			this.label1.TabIndex = 36;
			this.label1.Text = "Message";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImage = global::Extension.TcpWinForm.Properties.Resources.Writing;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(22, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(40, 40);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer3.IsSplitterFixed = true;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.richTextBoxMessage);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.checkBoxHexFormat);
			this.splitContainer3.Panel2.Controls.Add(this.buttonSend);
			this.splitContainer3.Size = new System.Drawing.Size(743, 70);
			this.splitContainer3.SplitterDistance = 626;
			this.splitContainer3.TabIndex = 0;
			// 
			// richTextBoxMessage
			// 
			this.richTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxMessage.ForeColor = System.Drawing.Color.Blue;
			this.richTextBoxMessage.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxMessage.Name = "richTextBoxMessage";
			this.richTextBoxMessage.Size = new System.Drawing.Size(626, 70);
			this.richTextBoxMessage.TabIndex = 2;
			this.richTextBoxMessage.Text = "";
			this.richTextBoxMessage.TextChanged += new System.EventHandler(this.richTextBoxMessage_TextChanged);
			// 
			// checkBoxHexFormat
			// 
			this.checkBoxHexFormat.AutoSize = true;
			this.checkBoxHexFormat.Location = new System.Drawing.Point(8, 41);
			this.checkBoxHexFormat.Name = "checkBoxHexFormat";
			this.checkBoxHexFormat.Size = new System.Drawing.Size(99, 21);
			this.checkBoxHexFormat.TabIndex = 0;
			this.checkBoxHexFormat.Text = "Hex Format";
			this.checkBoxHexFormat.UseVisualStyleBackColor = true;
			this.checkBoxHexFormat.CheckedChanged += new System.EventHandler(this.checkBoxHexFormat_CheckedChanged);
			// 
			// buttonSend
			// 
			this.buttonSend.Location = new System.Drawing.Point(7, 4);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(100, 30);
			this.buttonSend.TabIndex = 3;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// checkBoxScrollToCaret
			// 
			this.checkBoxScrollToCaret.AutoSize = true;
			this.checkBoxScrollToCaret.Checked = true;
			this.checkBoxScrollToCaret.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxScrollToCaret.Location = new System.Drawing.Point(255, 22);
			this.checkBoxScrollToCaret.Name = "checkBoxScrollToCaret";
			this.checkBoxScrollToCaret.Size = new System.Drawing.Size(113, 21);
			this.checkBoxScrollToCaret.TabIndex = 2;
			this.checkBoxScrollToCaret.Text = "ScrollToCaret";
			this.checkBoxScrollToCaret.UseVisualStyleBackColor = true;
			// 
			// TcpServerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(824, 563);
			this.Controls.Add(this.groupBoxExchange);
			this.Controls.Add(this.groupBoxTCPIPServer);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimumSize = new System.Drawing.Size(840, 286);
			this.Name = "TcpServerForm";
			this.Text = "Generic TCP/IP Server Form v1.4";
			this.groupBoxTCPIPServer.ResumeLayout(false);
			this.groupBoxTCPIPServer.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeartBeat)).EndInit();
			this.groupBoxExchange.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxTCPIPServer;
    private System.Windows.Forms.TextBox textBoxServerIpAddress;
    private System.Windows.Forms.Label labelServerTCPIPPortAvailability;
    private System.Windows.Forms.Button buttonServerTCPIPOpen;
    private System.Windows.Forms.LinkLabel linkLabelServerRefreshTCPIP;
    private System.Windows.Forms.Label labelServerIPPortNo;
    private System.Windows.Forms.TextBox textBoxServerPortNo;
    private System.Windows.Forms.GroupBox groupBoxExchange;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.CheckBox checkBoxHexFormat;
    private System.Windows.Forms.SplitContainer splitContainer3;
    private System.Windows.Forms.Button buttonSend;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxNoOfClient;
    private System.Windows.Forms.RichTextBox richTextBoxMessage;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox checkBoxScrollToCaret;
    private System.Windows.Forms.TabControl tabControlExchange;
    private System.Windows.Forms.NumericUpDown numericUpDownHeartBeat;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox checkBoxHeartBeat;
  }
}

