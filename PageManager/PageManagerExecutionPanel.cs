using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Extension.Controls;

namespace Extension.PageManager {
  public enum PageManagerBinShowStyle {
    Simple,
    Programming,
    ProgComma
  }

  public class PageManagerExecutionPanel : ButtonPanel {
    ControlledFocusCueCheckBox binShowCheckBox = new ControlledFocusCueCheckBox();
    RichTextBox binRichTextBox = new RichTextBox();
    Label binStyleLabel = new Label();
    ComboBox binStyleComboBox = new ComboBox();
    Button sendButton = new Button();

    public List<byte> Bytes;

    public event EventHandler BinStyleChanged; //To create event when bin style comboBox selected index is changed
    public event EventHandler BinCheckBoxClicked; //To create event when the bin checkbox state is changed
    public event EventHandler SendClicked; //To create event send button is clicked

    int heightBaseOffset = 5;
    public PageManagerExecutionPanel() {
      //This panel, most of them must be declared outside!
      this.Font = new Font("Tahoma", 9.5f, FontStyle.Regular);
      this.Dock = DockStyle.Bottom;
      this.Height = 37;

      //Binary Style Label
      binStyleLabel.Location = new Point(4, heightBaseOffset + 4);
      binStyleLabel.Text = "Style";
      binStyleLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      binStyleLabel.Size = binStyleLabel.PreferredSize;
      this.Controls.Add(binStyleLabel);

      //Binary Style Label ComboBox
      foreach (PageManagerBinShowStyle item in Enum.GetValues(typeof(PageManagerBinShowStyle)))
        binStyleComboBox.Items.Add(item);
      binStyleComboBox.Location = new Point(binStyleLabel.Location.X + 40, heightBaseOffset + 1);
      binStyleComboBox.SelectedIndex = 0;
      binStyleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      binStyleComboBox.ForeColor = Color.DarkGreen;
      binStyleComboBox.TabStop = false;      
      binStyleComboBox.SelectedIndexChanged += binStyleComboBox_SelectedIndexChanged;
      this.Controls.Add(binStyleComboBox);

      //Binary Show checkbox
      binShowCheckBox.Location = new Point(binStyleComboBox.Location.X + 140, heightBaseOffset + 3);
      binShowCheckBox.Text = "Bin";
      //binShowCheckBox.CheckAlign = ContentAlignment.MiddleRight;
      binShowCheckBox.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      binShowCheckBox.Size = binShowCheckBox.PreferredSize;
      binShowCheckBox.Checked = false;
      binShowCheckBox.TabStop = false;
      binShowCheckBox.DisableFocusCue = true;
      binShowCheckBox.Click += binShowCheckBox_Click;
      this.Controls.Add(binShowCheckBox);

      //Binary RichTextBox
      binRichTextBox.Location = new Point(binShowCheckBox.Location.X + 50, heightBaseOffset + 1);
      binRichTextBox.ForeColor = Color.DarkGreen;
      binRichTextBox.Size = new Size(400, 25);
      binRichTextBox.TabStop = false;
      binRichTextBox.Enabled = false;
      this.Controls.Add(binRichTextBox);

      //Send Button
      sendButton.Location = new Point(binRichTextBox.Location.X + binRichTextBox.Size.Width + 10, heightBaseOffset + 1);
      sendButton.Text = "Send";
      sendButton.FlatStyle = FlatStyle.Popup;
      sendButton.TabStop = false;
      sendButton.Size = new Size(80, 25);
      sendButton.Click += sendButton_Click;
      this.Controls.Add(sendButton);
    }

    void sendButton_Click(object sender, EventArgs e) {
      //TODO adds
      if (SendClicked != null)
        SendClicked(sender, e);
    }

    void binStyleComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (BinStyleChanged != null)
        BinStyleChanged(sender, e);
      //throw new NotImplementedException();
    }

    void binShowCheckBox_Click(object sender, EventArgs e) {
      CheckBox cb = sender as CheckBox;
      if (binRichTextBox.Text.Length > 0)
        binRichTextBox.Clear();
      binRichTextBox.Enabled = cb.Checked;
      if (BinCheckBoxClicked != null)
        BinCheckBoxClicked(sender, e);
      if (!cb.Checked || Bytes == null || Bytes.Count <= 0)
        return;
      PageManagerBinShowStyle item = (PageManagerBinShowStyle)binStyleComboBox.SelectedItem;
      for (int i = 0; i < Bytes.Count; ++i)
        binRichTextBox.AppendText((item == PageManagerBinShowStyle.Simple ? "" : "0x") + Bytes[i].ToString("X2") + (i == Bytes.Count - 1 ? "" : ((item == PageManagerBinShowStyle.ProgComma ? ", " : " "))));
    }

    protected override void OnSizeChanged(EventArgs e) {
      binRichTextBox.Size = new Size(this.Size.Width - 340, 25);
      sendButton.Location = new Point(binRichTextBox.Location.X + binRichTextBox.Size.Width + 10, heightBaseOffset + 1);
      base.OnSizeChanged(e);
    }
  }
}

