using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Extension.Manipulator;

namespace Extension.Controls {
  public class TcpExchangeTabPage : TabPage {

    Label labelClientAddress = new Label();
    TextBox textBoxClientAddress = new TextBox();
    GroupBox groupBoxInfoSettingClient = new GroupBox();
    RichTextBox richTextBoxClientExchange = new RichTextBox();
    CheckBox checkBoxScrollToCaretClient = new CheckBox();
    CheckBox checkBoxHexFormatClient = new CheckBox();
    LinkLabel linkLabelClearClient = new LinkLabel();
    Button buttonAbort = new Button();

    public event EventHandler OnAbort = null;

    public TcpExchangeTabPage(int index) {
      string no = index.ToString();      
      this.Text = no;
      this.UseVisualStyleBackColor = true;

      labelClientAddress.AutoSize = true;
      labelClientAddress.Location = new Point(6, 23);
      labelClientAddress.Name = "labelClientAddress" + no;
      labelClientAddress.Size = new Size(60, 17);
      labelClientAddress.TabIndex = 0;
      labelClientAddress.Text = "Address";

      textBoxClientAddress.Enabled = false;
      textBoxClientAddress.Location = new Point(76, 20);
      textBoxClientAddress.Name = "textBoxClientAddress" + no;
      textBoxClientAddress.Size = new Size(171, 23);
      textBoxClientAddress.TabIndex = 1;

      groupBoxInfoSettingClient.Dock = DockStyle.Top;
      groupBoxInfoSettingClient.Location = new Point(0, 0);
      groupBoxInfoSettingClient.Name = "groupBoxInfoSettingClient" + no;
      groupBoxInfoSettingClient.Size = new Size(765, 55);
      groupBoxInfoSettingClient.TabIndex = 2;
      groupBoxInfoSettingClient.TabStop = false;
      groupBoxInfoSettingClient.Text = "Info && Settings";

      richTextBoxClientExchange.Dock = DockStyle.Fill;
      richTextBoxClientExchange.Location = new Point(0, 55);
      richTextBoxClientExchange.Name = "richTextBoxClientExchange" + no;
      richTextBoxClientExchange.Size = new Size(765, 319);
      richTextBoxClientExchange.TabIndex = 3;
      richTextBoxClientExchange.Text = "";
      richTextBoxClientExchange.ReadOnly = true;
      richTextBoxClientExchange.BackColor = Color.White;

      checkBoxScrollToCaretClient.AutoSize = true;
      checkBoxScrollToCaretClient.Checked = true;
      checkBoxScrollToCaretClient.CheckState = CheckState.Checked;
      checkBoxScrollToCaretClient.Location = new Point(255, 22);
      checkBoxScrollToCaretClient.Name = "checkBoxScrollToCaret" + no;
      checkBoxScrollToCaretClient.Size = new Size(113, 21);
      checkBoxScrollToCaretClient.TabIndex = 2;
      checkBoxScrollToCaretClient.Text = "Scroll To Caret";
      checkBoxScrollToCaretClient.UseVisualStyleBackColor = true;

      checkBoxHexFormatClient.AutoSize = true;
      checkBoxHexFormatClient.Location = new Point(377, 22);
      checkBoxHexFormatClient.Name = "checkBoxHexFormatClient" + no;
      checkBoxHexFormatClient.Size = new Size(110, 21);
      checkBoxHexFormatClient.TabIndex = 2;
      checkBoxHexFormatClient.Text = "Hex Format";
      checkBoxHexFormatClient.UseVisualStyleBackColor = true;

      linkLabelClearClient.AutoSize = true;
      linkLabelClearClient.Location = new Point(476, 23);
      linkLabelClearClient.Margin = new Padding(4, 0, 4, 0);
      linkLabelClearClient.Name = "linkLabelClearClient" + no;
      linkLabelClearClient.TabIndex = 32;
      linkLabelClearClient.TabStop = true;
      linkLabelClearClient.Text = "Clear";
      linkLabelClearClient.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClearClient_LinkClicked);

      buttonAbort.Location = new System.Drawing.Point(528, 17);
      buttonAbort.Name = "buttonAbort" + no;
      buttonAbort.Size = new System.Drawing.Size(65, 28);
      buttonAbort.TabIndex = 0;
      buttonAbort.Text = "Abort";
      buttonAbort.UseVisualStyleBackColor = true;
      buttonAbort.Click += buttonAbort_Click;

      groupBoxInfoSettingClient.Controls.Add(checkBoxHexFormatClient);
      groupBoxInfoSettingClient.Controls.Add(checkBoxScrollToCaretClient);
      groupBoxInfoSettingClient.Controls.Add(textBoxClientAddress);
      groupBoxInfoSettingClient.Controls.Add(labelClientAddress);
      groupBoxInfoSettingClient.Controls.Add(linkLabelClearClient);
      groupBoxInfoSettingClient.Controls.Add(buttonAbort);

      this.Controls.Add(richTextBoxClientExchange);
      this.Controls.Add(groupBoxInfoSettingClient);
    }

    void buttonAbort_Click(object sender, EventArgs e) {
      if (OnAbort != null)
        OnAbort(sender, e);
    }

    public void Write(byte[] bytes, Color color, string sender) {
      string msgContent = checkBoxHexFormatClient.Checked ? Data.GetVisualStringOfBytes(bytes) : Encoding.ASCII.GetString(bytes);
      string msgLog = "[" + DateTime.Now.ToString() + " | " + sender + "]: " + msgContent + "\n";
      directWrite(msgLog, color);
    }

    public void Write(string msgContent, Color color, string sender) {
      string msgLog = "[" + DateTime.Now.ToString() + " | " + sender + "]: " + msgContent + "\n";
      directWrite(msgLog, color);
    }

    public void WriteAddress(string addr) {
      textBoxClientAddress.Text = addr;
    }

    private void directWrite(string msgLog, Color color) {
      richTextBoxClientExchange.SelectionStart = richTextBoxClientExchange.TextLength;
      richTextBoxClientExchange.SelectionLength = 0;
      richTextBoxClientExchange.SelectionColor = color;
      richTextBoxClientExchange.AppendText(msgLog);
      richTextBoxClientExchange.SelectionColor = richTextBoxClientExchange.ForeColor;
      if (checkBoxScrollToCaretClient.Checked)
        richTextBoxClientExchange.ScrollToCaret();
    }

    private void linkLabelClearClient_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      richTextBoxClientExchange.Clear();
    }

    public bool IsHexFormat() {
      return checkBoxHexFormatClient.Checked;
    }

  }
}
