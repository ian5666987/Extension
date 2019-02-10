namespace Extension.TcpWinForm
{
  partial class TcpClientForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TcpClientForm));
      this.groupBoxTCPIPClient = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxTimeOfTimeout = new System.Windows.Forms.TextBox();
      this.checkBoxTimeout = new System.Windows.Forms.CheckBox();
      this.numericUpDownTimeout = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.textBoxServerPortNo = new System.Windows.Forms.TextBox();
      this.textBoxServerIpAddress = new System.Windows.Forms.TextBox();
      this.labelClientIPPortNo = new System.Windows.Forms.Label();
      this.buttonClientTCPIPConnect = new System.Windows.Forms.Button();
      this.labelClientTCPIPPortAvailability = new System.Windows.Forms.Label();
      this.linkLabelClientRefreshTCPIP = new System.Windows.Forms.LinkLabel();
      this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
      this.buttonSend = new System.Windows.Forms.Button();
      this.checkBoxHexFormat = new System.Windows.Forms.CheckBox();
      this.groupBoxExchange = new System.Windows.Forms.GroupBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.tabControlExchange = new System.Windows.Forms.TabControl();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.splitContainer3 = new System.Windows.Forms.SplitContainer();
      this.groupBoxTCPIPClient.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeout)).BeginInit();
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
      // groupBoxTCPIPClient
      // 
      this.groupBoxTCPIPClient.Controls.Add(this.label2);
      this.groupBoxTCPIPClient.Controls.Add(this.textBoxTimeOfTimeout);
      this.groupBoxTCPIPClient.Controls.Add(this.checkBoxTimeout);
      this.groupBoxTCPIPClient.Controls.Add(this.numericUpDownTimeout);
      this.groupBoxTCPIPClient.Controls.Add(this.label3);
      this.groupBoxTCPIPClient.Controls.Add(this.textBoxServerPortNo);
      this.groupBoxTCPIPClient.Controls.Add(this.textBoxServerIpAddress);
      this.groupBoxTCPIPClient.Controls.Add(this.labelClientIPPortNo);
      this.groupBoxTCPIPClient.Controls.Add(this.buttonClientTCPIPConnect);
      this.groupBoxTCPIPClient.Controls.Add(this.labelClientTCPIPPortAvailability);
      this.groupBoxTCPIPClient.Controls.Add(this.linkLabelClientRefreshTCPIP);
      this.groupBoxTCPIPClient.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBoxTCPIPClient.Location = new System.Drawing.Point(0, 0);
      this.groupBoxTCPIPClient.Margin = new System.Windows.Forms.Padding(4);
      this.groupBoxTCPIPClient.Name = "groupBoxTCPIPClient";
      this.groupBoxTCPIPClient.Padding = new System.Windows.Forms.Padding(4);
      this.groupBoxTCPIPClient.Size = new System.Drawing.Size(824, 63);
      this.groupBoxTCPIPClient.TabIndex = 30;
      this.groupBoxTCPIPClient.TabStop = false;
      this.groupBoxTCPIPClient.Text = "Client TCP/IP";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(668, 26);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(12, 17);
      this.label2.TabIndex = 39;
      this.label2.Text = "/";
      // 
      // textBoxTimeOfTimeout
      // 
      this.textBoxTimeOfTimeout.Location = new System.Drawing.Point(616, 24);
      this.textBoxTimeOfTimeout.Name = "textBoxTimeOfTimeout";
      this.textBoxTimeOfTimeout.ReadOnly = true;
      this.textBoxTimeOfTimeout.Size = new System.Drawing.Size(50, 23);
      this.textBoxTimeOfTimeout.TabIndex = 38;
      this.textBoxTimeOfTimeout.Text = "0";
      // 
      // checkBoxTimeout
      // 
      this.checkBoxTimeout.AutoSize = true;
      this.checkBoxTimeout.Checked = true;
      this.checkBoxTimeout.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxTimeout.Location = new System.Drawing.Point(531, 26);
      this.checkBoxTimeout.Name = "checkBoxTimeout";
      this.checkBoxTimeout.Size = new System.Drawing.Size(78, 21);
      this.checkBoxTimeout.TabIndex = 37;
      this.checkBoxTimeout.Text = "Timeout";
      this.checkBoxTimeout.UseVisualStyleBackColor = true;
      this.checkBoxTimeout.CheckedChanged += new System.EventHandler(this.checkBoxTimeout_CheckedChanged);
      // 
      // numericUpDownTimeout
      // 
      this.numericUpDownTimeout.Location = new System.Drawing.Point(680, 24);
      this.numericUpDownTimeout.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
      this.numericUpDownTimeout.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.numericUpDownTimeout.Name = "numericUpDownTimeout";
      this.numericUpDownTimeout.Size = new System.Drawing.Size(63, 23);
      this.numericUpDownTimeout.TabIndex = 36;
      this.numericUpDownTimeout.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
      this.numericUpDownTimeout.ValueChanged += new System.EventHandler(this.numericUpDownTimeout_ValueChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(747, 26);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(15, 17);
      this.label3.TabIndex = 35;
      this.label3.Text = "s";
      // 
      // textBoxServerPortNo
      // 
      this.textBoxServerPortNo.Location = new System.Drawing.Point(199, 22);
      this.textBoxServerPortNo.Margin = new System.Windows.Forms.Padding(4);
      this.textBoxServerPortNo.Name = "textBoxServerPortNo";
      this.textBoxServerPortNo.Size = new System.Drawing.Size(55, 23);
      this.textBoxServerPortNo.TabIndex = 17;
      this.textBoxServerPortNo.Text = "9377";
      this.textBoxServerPortNo.TextChanged += new System.EventHandler(this.textBoxServerPortNo_TextChanged);
      // 
      // textBoxServerIpAddress
      // 
      this.textBoxServerIpAddress.Location = new System.Drawing.Point(69, 22);
      this.textBoxServerIpAddress.Margin = new System.Windows.Forms.Padding(4);
      this.textBoxServerIpAddress.Name = "textBoxServerIpAddress";
      this.textBoxServerIpAddress.Size = new System.Drawing.Size(121, 23);
      this.textBoxServerIpAddress.TabIndex = 20;
      this.textBoxServerIpAddress.Text = "127.0.0.1";
      this.textBoxServerIpAddress.TextChanged += new System.EventHandler(this.textBoxServerIpAddress_TextChanged);
      // 
      // labelClientIPPortNo
      // 
      this.labelClientIPPortNo.AutoSize = true;
      this.labelClientIPPortNo.Location = new System.Drawing.Point(8, 24);
      this.labelClientIPPortNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelClientIPPortNo.Name = "labelClientIPPortNo";
      this.labelClientIPPortNo.Size = new System.Drawing.Size(50, 17);
      this.labelClientIPPortNo.TabIndex = 18;
      this.labelClientIPPortNo.Text = "IP/Port";
      // 
      // buttonClientTCPIPConnect
      // 
      this.buttonClientTCPIPConnect.Location = new System.Drawing.Point(320, 19);
      this.buttonClientTCPIPConnect.Margin = new System.Windows.Forms.Padding(4);
      this.buttonClientTCPIPConnect.Name = "buttonClientTCPIPConnect";
      this.buttonClientTCPIPConnect.Size = new System.Drawing.Size(93, 30);
      this.buttonClientTCPIPConnect.TabIndex = 16;
      this.buttonClientTCPIPConnect.Text = "Connect";
      this.buttonClientTCPIPConnect.UseVisualStyleBackColor = true;
      this.buttonClientTCPIPConnect.Click += new System.EventHandler(this.buttonClientTCPIPConnect_Click);
      // 
      // labelClientTCPIPPortAvailability
      // 
      this.labelClientTCPIPPortAvailability.AutoSize = true;
      this.labelClientTCPIPPortAvailability.Location = new System.Drawing.Point(416, 18);
      this.labelClientTCPIPPortAvailability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelClientTCPIPPortAvailability.MaximumSize = new System.Drawing.Size(107, 34);
      this.labelClientTCPIPPortAvailability.MinimumSize = new System.Drawing.Size(107, 34);
      this.labelClientTCPIPPortAvailability.Name = "labelClientTCPIPPortAvailability";
      this.labelClientTCPIPPortAvailability.Size = new System.Drawing.Size(107, 34);
      this.labelClientTCPIPPortAvailability.TabIndex = 19;
      this.labelClientTCPIPPortAvailability.Text = "Initialized";
      this.labelClientTCPIPPortAvailability.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // linkLabelClientRefreshTCPIP
      // 
      this.linkLabelClientRefreshTCPIP.AutoSize = true;
      this.linkLabelClientRefreshTCPIP.Location = new System.Drawing.Point(259, 27);
      this.linkLabelClientRefreshTCPIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.linkLabelClientRefreshTCPIP.Name = "linkLabelClientRefreshTCPIP";
      this.linkLabelClientRefreshTCPIP.Size = new System.Drawing.Size(58, 17);
      this.linkLabelClientRefreshTCPIP.TabIndex = 32;
      this.linkLabelClientRefreshTCPIP.TabStop = true;
      this.linkLabelClientRefreshTCPIP.Text = "Refresh";
      this.linkLabelClientRefreshTCPIP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClientRefreshTCPIP_LinkClicked);
      // 
      // richTextBoxMessage
      // 
      this.richTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richTextBoxMessage.ForeColor = System.Drawing.Color.Red;
      this.richTextBoxMessage.Location = new System.Drawing.Point(0, 0);
      this.richTextBoxMessage.Name = "richTextBoxMessage";
      this.richTextBoxMessage.Size = new System.Drawing.Size(626, 70);
      this.richTextBoxMessage.TabIndex = 57;
      this.richTextBoxMessage.Text = "";
      this.richTextBoxMessage.TextChanged += new System.EventHandler(this.richTextBoxMessage_TextChanged);
      // 
      // buttonSend
      // 
      this.buttonSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonSend.Location = new System.Drawing.Point(7, 4);
      this.buttonSend.Name = "buttonSend";
      this.buttonSend.Size = new System.Drawing.Size(100, 30);
      this.buttonSend.TabIndex = 56;
      this.buttonSend.Text = "Send";
      this.buttonSend.UseVisualStyleBackColor = true;
      this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
      // 
      // checkBoxHexFormat
      // 
      this.checkBoxHexFormat.AutoSize = true;
      this.checkBoxHexFormat.Location = new System.Drawing.Point(8, 41);
      this.checkBoxHexFormat.Name = "checkBoxHexFormat";
      this.checkBoxHexFormat.Size = new System.Drawing.Size(99, 21);
      this.checkBoxHexFormat.TabIndex = 59;
      this.checkBoxHexFormat.Text = "Hex Format";
      this.checkBoxHexFormat.UseVisualStyleBackColor = true;
      this.checkBoxHexFormat.CheckedChanged += new System.EventHandler(this.checkBoxHexFormat_CheckedChanged);
      // 
      // groupBoxExchange
      // 
      this.groupBoxExchange.Controls.Add(this.splitContainer1);
      this.groupBoxExchange.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxExchange.Enabled = false;
      this.groupBoxExchange.Location = new System.Drawing.Point(0, 63);
      this.groupBoxExchange.Name = "groupBoxExchange";
      this.groupBoxExchange.Size = new System.Drawing.Size(824, 448);
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
      this.splitContainer1.Size = new System.Drawing.Size(818, 426);
      this.splitContainer1.SplitterDistance = 352;
      this.splitContainer1.TabIndex = 0;
      // 
      // tabControlExchange
      // 
      this.tabControlExchange.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControlExchange.Location = new System.Drawing.Point(0, 0);
      this.tabControlExchange.Name = "tabControlExchange";
      this.tabControlExchange.SelectedIndex = 0;
      this.tabControlExchange.Size = new System.Drawing.Size(818, 352);
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
      this.label1.TabIndex = 38;
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
      this.pictureBox1.TabIndex = 37;
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
      // TcpClientForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(824, 511);
      this.Controls.Add(this.groupBoxExchange);
      this.Controls.Add(this.groupBoxTCPIPClient);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MinimumSize = new System.Drawing.Size(840, 286);
      this.Name = "TcpClientForm";
      this.Text = "Generic TCP/IP Client Form v1.0";
      this.groupBoxTCPIPClient.ResumeLayout(false);
      this.groupBoxTCPIPClient.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeout)).EndInit();
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

    private System.Windows.Forms.GroupBox groupBoxTCPIPClient;
    private System.Windows.Forms.TextBox textBoxServerPortNo;
    private System.Windows.Forms.TextBox textBoxServerIpAddress;
    private System.Windows.Forms.Label labelClientIPPortNo;
    private System.Windows.Forms.Button buttonClientTCPIPConnect;
    private System.Windows.Forms.Label labelClientTCPIPPortAvailability;
    private System.Windows.Forms.LinkLabel linkLabelClientRefreshTCPIP;
    private System.Windows.Forms.RichTextBox richTextBoxMessage;
    private System.Windows.Forms.Button buttonSend;
    private System.Windows.Forms.CheckBox checkBoxHexFormat;
    private System.Windows.Forms.GroupBox groupBoxExchange;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TabControl tabControlExchange;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.SplitContainer splitContainer3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericUpDownTimeout;
    private System.Windows.Forms.CheckBox checkBoxTimeout;
    private System.Windows.Forms.TextBox textBoxTimeOfTimeout;
    private System.Windows.Forms.Label label2;
  }
}

