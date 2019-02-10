using System;
using System.Windows.Forms;
using System.Drawing;

using Extension.Controls;

namespace Extension.PageManager {
  public enum PageManagerValueAlignment {
    AfterText, //default style, everything is shown after text
    LongestText, //automatically adjust according to the longest text in column
    RightAligned, //automatically adjusted according to the shortest right aligned possible (in column)
    TwoLines //not implemented for now...
  }

  public class PageManagerOptionsPanel : ButtonPanel {
    Label filterLabel = new Label(); //left most
    TextBox filterTextBox = new TextBox(); //next after filterLabel
		Label showLabel = new Label(); //next after filterTextBox
		ComboBox showComboBox = new ComboBox(); //next after showLabel
		LinkLabel clearLinkLabel = new LinkLabel(); //next after showComboBox
		Label alignLabel = new Label(); //next after clearLinkLabel
		ComboBox alignComboBox = new ComboBox(); //next after alignLabel
		Label fontSizeLabel = new Label(); //next after alignComboBox
		NumericUpDown fontSizeUpDown = new NumericUpDown(); //next after fontSizeLabel
		Label gridLabel = new Label(); //next after fontSizeUpDown
		NumericUpDown gridUpDown = new NumericUpDown(); //next after gridLabel
		Label switchLabel = new Label(); //next after gridUpDown
		ComboBox switchComboBox = new ComboBox(); //next after switchLabel

    public event EventHandler FilterUpdated; //To create event when show filter text is changed
    public event EventHandler ShowStyleChanged; //To create event when show style comboBox selected index is changed
    public event EventHandler ClearClicked; //To create event for the clear link clicked
    public event EventHandler FontSizeUpDownChanged; //To create event when numeric up-down for font size is clicked
    public event EventHandler GridUpDownChanged; //To create event when numeric up-down for grid is clicked (may not be used, but prepare this anyway)
    public event EventHandler AlignmentChanged; //To create event when alignment comboBox selected index is changed
    public event EventHandler SwitchChanged; //To create event when switch comboBox selected index is changed

    int heightBaseOffset = 5;
    public PageManagerOptionsPanel() { //TODO Time to use xml serialization!
      //This panel, most of them must be declared outside!
      this.Font = new Font("Tahoma", 9.5f, FontStyle.Regular);
      this.Dock = DockStyle.Top;
      this.Height = 37;      

      //Filter Label
      filterLabel.Location = new Point(2, heightBaseOffset + 5);
      filterLabel.Text = "Filter";
      filterLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      filterLabel.Size = filterLabel.PreferredSize;
      this.Controls.Add(filterLabel);

      //Filter TextBox
      filterTextBox.Location = new Point(40, heightBaseOffset + 2);
      filterTextBox.ForeColor = Color.DarkGreen;
      filterTextBox.TabStop = false;
      filterTextBox.TextChanged += filterTextBox_TextChanged;
      this.Controls.Add(filterTextBox);

      //Show Label
      showLabel.Location = new Point(filterTextBox.Location.X + 110, heightBaseOffset + 5);
      showLabel.Text = "Show";
      showLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      showLabel.Size = showLabel.PreferredSize;
      this.Controls.Add(showLabel);

      //Show ComboBox
      foreach (var item in Enum.GetValues(typeof(GenericDataPanelShowStyle)))
        showComboBox.Items.Add(item);
      showComboBox.Location = new Point(showLabel.Location.X + 45, heightBaseOffset + 1);
      showComboBox.SelectedIndex = 0;
      showComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      showComboBox.ForeColor = Color.DarkGreen;
      showComboBox.TabStop = false;
      showComboBox.SelectedIndexChanged += showComboBox_SelectedIndexChanged;
      this.Controls.Add(showComboBox);

      //Clear link label
      clearLinkLabel.Location = new Point(showComboBox.Location.X + 130, heightBaseOffset + 4);
      clearLinkLabel.Text = "Clear";
      clearLinkLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      clearLinkLabel.Size = clearLinkLabel.PreferredSize;
      clearLinkLabel.TabStop = false;
      clearLinkLabel.Click += clearLinkLabel_Click;
      this.Controls.Add(clearLinkLabel);

      //Align Label
      alignLabel.Location = new Point(clearLinkLabel.Location.X + 50, heightBaseOffset + 5);
      alignLabel.Text = "Align";
      alignLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      alignLabel.Size = alignLabel.PreferredSize;
      this.Controls.Add(alignLabel);

      //Align ComboBox
      foreach (var item in Enum.GetValues(typeof(PageManagerValueAlignment)))
        alignComboBox.Items.Add(item);
      alignComboBox.Location = new Point(alignLabel.Location.X + 40, heightBaseOffset + 1);
      alignComboBox.SelectedIndex = 0;
      alignComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      alignComboBox.ForeColor = Color.DarkGreen;
      alignComboBox.TabStop = false;
      alignComboBox.SelectedIndexChanged += alignComboBox_SelectedIndexChanged;
      this.Controls.Add(alignComboBox);

      //Font Size Label
      fontSizeLabel.Location = new Point(alignComboBox.Location.X + 135, heightBaseOffset + 4);
      fontSizeLabel.Text = "Font";
      fontSizeLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      fontSizeLabel.Size = fontSizeLabel.PreferredSize;
      this.Controls.Add(fontSizeLabel);

      //Font Size Up Down
      fontSizeUpDown.Location = new Point(fontSizeLabel.Location.X + 36, heightBaseOffset + 2);
      fontSizeUpDown.Size = new Size(35, 25);
      fontSizeUpDown.Minimum = 10;
      fontSizeUpDown.Maximum = 50;
      fontSizeUpDown.ReadOnly = true;
      fontSizeUpDown.Value = (int)(this.Font.Size * 2);
      fontSizeUpDown.TabStop = false;
      fontSizeUpDown.ValueChanged += fontSizeUpDown_ValueChanged;
      this.Controls.Add(fontSizeUpDown);

      //Grid Label
      gridLabel.Location = new Point(fontSizeUpDown.Location.X + 45, heightBaseOffset + 4);
      gridLabel.Text = "Grid";
      gridLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      gridLabel.Size = gridLabel.PreferredSize;
      this.Controls.Add(gridLabel);

      //Grid Up Down
      gridUpDown.Location = new Point(gridLabel.Location.X + 36, heightBaseOffset + 2);
      gridUpDown.Size = new Size(35, 25);
      gridUpDown.Minimum = 1;
      gridUpDown.Maximum = 20;
      gridUpDown.ReadOnly = true;
      gridUpDown.Value = 3;
      gridUpDown.TabStop = false;
      gridUpDown.ValueChanged += gridUpDown_ValueChanged;
      this.Controls.Add(gridUpDown);
      gridValue = (int)gridUpDown.Value; //initializing the variable

      //Switch Label
      switchLabel.Location = new Point(gridUpDown.Location.X + 45, heightBaseOffset + 5);
      switchLabel.Text = "Switch";
      switchLabel.Font = this.Font; //without this, it cannot calculate the preferred size correctly!
      switchLabel.Size = switchLabel.PreferredSize;
      this.Controls.Add(switchLabel);

      //Switch ComboBox
      foreach (var item in Enum.GetValues(typeof(GenericDataPanelMode)))
        switchComboBox.Items.Add(item);
      switchComboBox.Location = new Point(switchLabel.Location.X + 50, heightBaseOffset + 1);
      switchComboBox.SelectedIndex = 0;
      switchComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      switchComboBox.ForeColor = Color.DarkGreen;
      switchComboBox.TabStop = false;
      switchComboBox.SelectedIndexChanged += switchComboBox_SelectedIndexChanged;
      this.Controls.Add(switchComboBox);
    }

    private GenericDataPanelMode panelMode;
    public GenericDataPanelMode PanelMode { get { return panelMode; } }
    void switchComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      panelMode = (GenericDataPanelMode)switchComboBox.SelectedItem;
      if (SwitchChanged != null)
        SwitchChanged(this, e);
    }

    private int gridValue;
    public int GridValue { get { return gridValue; } }
    void gridUpDown_ValueChanged(object sender, EventArgs e) {
      gridValue = (int)gridUpDown.Value;
      if (GridUpDownChanged != null)
        GridUpDownChanged(this, e);
    }

    private string filterText;
    public string FilterText { get { return filterText; } }
    void filterTextBox_TextChanged(object sender, EventArgs e) {
      filterText = filterTextBox.Text;
      if (FilterUpdated != null) //somebody creates the delegate for this event (must be done outside!)
        FilterUpdated(this, e);
    }

    private GenericDataPanelShowStyle showStyle;
    public GenericDataPanelShowStyle ShowStyle { get { return showStyle; } }
    void showComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showStyle = (GenericDataPanelShowStyle)showComboBox.SelectedItem;
      if (ShowStyleChanged != null) //somebody creates the delegate for this event (must be done outside!)
        ShowStyleChanged(this, e);
    }

    private PageManagerValueAlignment valueAlignment;
    public PageManagerValueAlignment ValueAlignment { get { return valueAlignment; } }
    void alignComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      valueAlignment = (PageManagerValueAlignment)alignComboBox.SelectedItem;
      if (AlignmentChanged != null) //somebody creates the delegate for this event (must be done outside!)
        AlignmentChanged(this, e);
    }

    private int fontSizeValue;
    public int FontSizeValue { get { return fontSizeValue; } }
    void fontSizeUpDown_ValueChanged(object sender, EventArgs e) {
      fontSizeValue = (int)fontSizeUpDown.Value;
      if (FontSizeUpDownChanged != null) //somebody creates the delegate for this event (must be done outside!)
        FontSizeUpDownChanged(this, e);       	    
    }

    void clearLinkLabel_Click(object sender, EventArgs e) { //must be implemented somewhere...
      if (ClearClicked != null) //somebody creates the delegate for this event (must be done outside!)
        ClearClicked(this, e);      
    }
  }
}

