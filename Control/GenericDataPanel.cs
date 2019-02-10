using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using Extension.Developer;

namespace Extension.Controls {
  public enum GenericDataPanelShowStyle {
    Complete, //default style, everything is shown
    Compact //page manager additions are not shown
  }

  public enum GenericDataPanelMode {
    ReadOnly,
    ReadAndWrite,
    WriteOnly
  }

  public class GenericDataPanel : Panel { //By default, this can only display "Compact", to display "Complete", something must be added by the page manager!
    private RainbowCheckBox checkBox = new RainbowCheckBox(); //basic control
    public Control CompleteDisplayControl = null; //this is the one which is shown "outside". It can be of any control type. (default is null, to indicate that no referenced item to this)
    //private ColorfulListBox completeDisplayListBox = new ColorfulListBox(); //it is always there, this is to be "attached" outside of this function itself. But it is private, because what is "attached" is variable
    //public Point CompleteDisplayPoint = new Point(); //this is what is to be used by the "outside" control to determine where this is to be placed (may not be necessary)
    //To make things more generic, actually, it depends entirely on what is "attached" by this panel to be displayed outside!

    public bool MarkAsRemoved = false; //use when needed
    public bool IsRegistered = false; //Will be true when dragged into the PageManager
    private bool markAsUnknown = false;
    public bool MarkAsUnknown { //cannot change background color other than moveable and the specified color
      get { return markAsUnknown; }
      set {
        markAsUnknown = true;
        checkBox.BackColor = isMoveable ? isMoveableColor : unknownDataColor;        
        Invalidate(); //TODO not sure if this is the best way of doing it
      }
    }

    public event EventHandler MobilityChanged;
    public event MouseEventHandler CbMouseMoveOnMovable;
    public event MouseEventHandler CbMouseDownOnMovable;
    public event MouseEventHandler CbMouseUpOnMovable;
    public event MouseEventHandler CbMouseClickOnBoxOnMovable;
    public event EventHandler GenPanelShowStyleChanged;
    public event EventHandler IsNullingCompleteDisplayControl;
    public event EventHandler CompleteDisplayControlAcquired;
    public event MouseEventHandler MouseClickOnPanelModeMovable;
    public event MouseEventHandler MouseClickOnPanelShowStyleMovable;

    private const int RCB_X_POSITION = 14;

		private System.Drawing.Color uninitializedDataColor = System.Drawing.Color.FromArgb(0xff, 0xff, 0xff, 0xc0);
		private System.Drawing.Color normalDataColor = System.Drawing.Color.FromArgb(0xff, 0xc8, 0xff, 0xc8); //lighter than Color.PaleGreen
		private System.Drawing.Color abnormalDataColor = System.Drawing.Color.Pink;
		private System.Drawing.Color mixedDataColor = System.Drawing.Color.Transparent;
		private System.Drawing.Color unknownDataColor = System.Drawing.Color.Orange;
		private System.Drawing.Color isMoveableColor = System.Drawing.Color.LightGray;
		private System.Drawing.Color aliveLineColor = System.Drawing.Color.Silver;
		private System.Drawing.Color deadLineColor = System.Drawing.Color.White; //will be derived from backcolor of this

    public GenericDataPanel() : base() {
      //CheckBox
      checkBox.Location = new Point(RCB_X_POSITION, 0);
      checkBox.Size = new Size(350, 19);
      checkBox.Text = "checkbox";
      checkBox.BackColor = uninitializedDataColor;
      checkBox.ValueTextPosition = ValueTextPositionType.AfterText;
      checkBox.RightEndAutoAdjust = true;
      checkBox.CheckState = CheckState.Checked;
      checkBox.AutoCheck = false; //To control the check/uncheck from outside!
      checkBox.DisableFocusCue = true;
      checkBox.MouseDown += checkBox_MouseDown;
      checkBox.MouseMove += checkBox_MouseMove;
      checkBox.MouseClick += checkBox_MouseClick;
      checkBox.Resize += checkBox_Resize;
      checkBox.MouseUp += checkBox_MouseUp;
#if DEBUG
      checkBox.PrintRequest += checkBox_PrintRequest;
#endif
      checkBox.Show();
      this.Controls.Add(checkBox);

      //The main panel
			this.BackColor = System.Drawing.Color.FromArgb(0, checkBox.BackColor);
      mixedDataColor = Extension.Drawing.Color.Blend(normalDataColor, abnormalDataColor, 0.5);
      adjustSize();
    }

    void checkBox_MouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left && isMoveable && CbMouseUpOnMovable != null)
        CbMouseUpOnMovable(sender, e);      
    }

#if DEBUG
    private string printItem;
    public string PrintItem { get { return printItem; } }
    private Color printColor;
    public Color PrintColor { get { return printColor; } }
    public event EventHandler PrintRequest;
    void checkBox_PrintRequest(object sender, EventArgs e) {
      RainbowCheckBox rcb = sender as RainbowCheckBox;
      if (rcb == null)
        return;
      if (PrintRequest != null) {
        printItem = rcb.PrintItem;
        printColor = rcb.PrintColor;
        PrintRequest(sender, e);
      }
    }

    public void ClearPrint() {
      printItem = null;
      printColor = Color.Blue;
    }
#endif

    public bool CheckBoxChecked { get { return checkBox.Checked; } set { checkBox.Checked = value; } }
    public System.Drawing.Color CheckBoxBorderColor { get { return checkBox.BorderColor; } set { checkBox.BorderColor = value; checkBox.Invalidate(); } } //TODO not sure if it is best to invalidate here!

    void checkBox_Resize(object sender, EventArgs e) {
      adjustSize();
    }

    void checkBox_MouseClick(object sender, MouseEventArgs e) {
      RainbowCheckBox cb = sender as RainbowCheckBox;
      if (cb == null)
        return;
      if (cb.IsOnBox) {
        cb.Checked = !cb.Checked;
        if (isMoveable && CbMouseClickOnBoxOnMovable != null)
          CbMouseClickOnBoxOnMovable(sender, e); //notice that the sender is the original sender not this
      }
    }

    void checkBox_MouseMove(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left && isMoveable && CbMouseMoveOnMovable != null)
        CbMouseMoveOnMovable(sender, e); //notice that the sender is the original sender not this
    }

    void checkBox_MouseDown(object sender, MouseEventArgs e) {
      RainbowCheckBox cb = sender as RainbowCheckBox;
      if (cb == null)
        return;
      if (e.Button == MouseButtons.Left && !cb.IsOnBox && !isMoveable) //wants to move the item
        SetMovability(true, e);
      if (e.Button == MouseButtons.Left && isMoveable && CbMouseDownOnMovable != null)
        CbMouseDownOnMovable(sender, e);
    }

    protected override void OnMouseClick(MouseEventArgs e) {
      if (e.Button == MouseButtons.Left && e.X >= 1 && e.X <= RCB_X_POSITION - 3 && e.Y >= 1 && e.Y <= 8) { //GenPanelMode area
        GenPanelMode = (GenericDataPanelMode)((int)(GenPanelMode + 1) % 3);
        if (isMoveable && MouseClickOnPanelModeMovable != null)
          MouseClickOnPanelModeMovable(this, e);
      }
      if (e.Button == MouseButtons.Left && e.X >= 1 && e.X <= RCB_X_POSITION - 3 && e.Y >= 10 && e.Y <= 18) { //GenPanelShowStyle area
        GenPanelShowStyle = (GenericDataPanelShowStyle)((int)(GenPanelShowStyle + 1) % 2);
        if (isMoveable && MouseClickOnPanelShowStyleMovable != null)
          MouseClickOnPanelShowStyleMovable(this, e);
      }
    }

    private void assignCompleteDisplayControl() {
      if (genData.ValueList != null && genData.ValueList.Count > 0) {
        DoubleListBoxPanel completeDisplayPanel = new DoubleListBoxPanel();
        completeDisplayPanel.DataType = genData.DataType;
        completeDisplayPanel.DisableFocusCue = true;
        if (genData.ComboList != null && genData.ComboList.Count > 0) { //combo precedes bits
          completeDisplayPanel.ListBoxStyle = DoubleListBoxPanelStyle.Combo;
          completeDisplayPanel.SetLists(genData.ComboList, genData.ValueList);
        } else if (genData.BitsList != null && genData.BitsList.Count > 0) {
          completeDisplayPanel.ListBoxStyle = DoubleListBoxPanelStyle.Bits;
          completeDisplayPanel.SetLists(genData.BitsList, genData.ValueList);
        }
        CompleteDisplayControl = completeDisplayPanel;
        if (CompleteDisplayControlAcquired != null)
          CompleteDisplayControlAcquired(this, new EventArgs());
      } //TODO additional way to determine "attachment" and "text" should be considered later
    }

    private GenericData genData;
    public GenericData GenData { //It is possible to change the display based on the gen data attached to this panel!
      get { return genData; } 
      set { 
        genData = value;
        
        //Reinitialization
        if (CompleteDisplayControl != null && IsNullingCompleteDisplayControl != null) //If the complete display control is going to be "changed", notify the parent
          IsNullingCompleteDisplayControl(this, new EventArgs()); //only when it is displaying complete show style this needs to be taken cared of
        CompleteDisplayControl = null; //reinitiates as null unless proven otherwise
        if (genData == null || string.IsNullOrWhiteSpace(genData.DataName))
          return;
        genData.DataReadBufferChanged += OnDataReadBufferChanged;
        genData.DataWriteBufferChanged += OnDataWriteBufferChanged;
        OnDataReadBufferChanged(this, null); //just call once to ensure that this is called without the real change: may cause some delay... very important! null sender here indicates the first time
        
        //Assign complete display control
        assignCompleteDisplayControl();
      } 
    }

    #region data read/write buffer changed
    private const int CYCLE_NO = 4;
    private int cycle = 0;
    private void removeAliveLine() {
      using (Pen pen = new Pen(deadLineColor, 1)) {
        int x_pos = checkBox.Location.X - 1; //checkBox.Location.X + checkBox.Width;
        this.CreateGraphics().DrawLine(pen, x_pos, checkBox.Location.Y, x_pos, checkBox.Location.Y + checkBox.Height);
      }
    }

    private void drawAliveLine(bool isNewData = false) { //If this is not included in OnPaint, likely it will be there...
      using (Pen pen = new Pen(aliveLineColor, 1)) {
        int linelength = (int)(checkBox.Height / CYCLE_NO + 0.5); //rounded
        int linestart = checkBox.Location.Y + cycle * linelength;
        int x_pos = checkBox.Location.X - 1; //checkBox.Location.X + checkBox.Width;
        this.CreateGraphics().DrawLine(pen, x_pos, linestart, x_pos, linestart + linelength);
        if (isNewData)
          cycle = (cycle + 1) % CYCLE_NO;
      }
    }

    private bool isLineAlive = false;
    private bool lifeLineUpdateRequest = false;
    
    //this can only be valid if the data validity is previously checked (Invalidate is called by the attached GenData) 
    //or if both the DataReadBufferChanged and DataWriteBufferChanged events are handled
    private System.Drawing.Color suggestBackColor() {
      if (isMoveable) // the simplest case
        return isMoveableColor;
      if (MarkAsUnknown) // the second simplest case
        return isMoveable ? isMoveableColor : unknownDataColor;
      switch (GenPanelMode){
      case GenericDataPanelMode.ReadOnly:
        return string.IsNullOrWhiteSpace(genData.DataReadBuffer) ? uninitializedDataColor : (genData.IsReadDataNormal ? normalDataColor : abnormalDataColor);
      case GenericDataPanelMode.ReadAndWrite:
        bool readUninitialized = string.IsNullOrWhiteSpace(genData.DataReadBuffer);
        bool writeUninitialized = string.IsNullOrWhiteSpace(genData.DataWriteBuffer);
        if (readUninitialized && writeUninitialized)
          return uninitializedDataColor;
        if ((readUninitialized && genData.IsWriteDataNormal) || (writeUninitialized && genData.IsReadDataNormal) || (genData.IsReadDataNormal && genData.IsWriteDataNormal))
          return normalDataColor;
        if (((readUninitialized && !genData.IsWriteDataNormal) || (writeUninitialized && !genData.IsReadDataNormal) || (!genData.IsReadDataNormal && !genData.IsWriteDataNormal)))
          return abnormalDataColor;
        if ((genData.IsWriteDataNormal && !genData.IsReadDataNormal) || (genData.IsReadDataNormal && !genData.IsWriteDataNormal))
          return mixedDataColor;
        break;
      case GenericDataPanelMode.WriteOnly:
        return string.IsNullOrWhiteSpace(genData.DataWriteBuffer) ? uninitializedDataColor : (genData.IsWriteDataNormal ? normalDataColor : abnormalDataColor);
      default:
        break;
      }
      return unknownDataColor; //worst case
    }

    public void SetCheckBoxToSuggestedBackColor() {      
      if (genData == null) //failed case, because there is no data in the first place
        return;
      genData.Invalidate(); //Do something before set to suggested back color, namely to make IsReadDataNormal && IsWriteDataNormal triggered
      checkBox.BackColor = suggestBackColor();
    }

    public void RefreshDisplayValue() {
      if (CompleteDisplayControl == null)
        return;
      DoubleListBoxPanel testPanel = CompleteDisplayControl as DoubleListBoxPanel; //test if the complete display is of this type
      if (testPanel == null || GenPanelShowStyle == GenericDataPanelShowStyle.Compact) //if it is not compact sytle then it shows something
        return;
      switch (GenPanelMode) {
      case GenericDataPanelMode.ReadOnly:
        testPanel.DisplayValue = genData.DataReadBuffer;
        break;
      case GenericDataPanelMode.ReadAndWrite:
        string val = string.IsNullOrWhiteSpace(genData.DataReadBuffer) ? " |" : genData.DataReadBuffer + "|";
        val += string.IsNullOrWhiteSpace(genData.DataWriteBuffer) ? " " : genData.DataWriteBuffer;
        testPanel.DisplayValue = val;
        break;
      case GenericDataPanelMode.WriteOnly:
        testPanel.DisplayValue = genData.DataWriteBuffer;
        break;
      default:
        break;
      }
      //CompleteDisplayControl.SendToBack(); //may not be necessary
    }

    private void OnDataWriteBufferChanged(object sender, EventArgs e) {
      checkBox.BackColor = suggestBackColor();
      RefreshDisplayValue();
      Invalidate(); //to trigger paint  
    }

    private void OnDataReadBufferChanged(object sender, EventArgs e) {
      lifeLineUpdateRequest = e != null; //null sender here means the first time
      isLineAlive = !string.IsNullOrWhiteSpace(genData.DataReadBuffer);
      checkBox.BackColor = suggestBackColor();
      RefreshDisplayValue();
      Invalidate(); //to trigger paint  
    }
    #endregion data read/write buffer changed

    private GenericDataPanelMode genPanelMode = GenericDataPanelMode.ReadOnly;
    public GenericDataPanelMode GenPanelMode { 
      get { return genPanelMode; } 
      set { 
        genPanelMode = value;
        checkBox.BackColor = suggestBackColor();
        RefreshDisplayValue();
        Invalidate(); //TODO not sure if it is the best idea, but so far seems to be okay...
      } 
    }

    private GenericDataPanelShowStyle genPanelShowStyle = GenericDataPanelShowStyle.Complete;
    public GenericDataPanelShowStyle GenPanelShowStyle {
      get { return genPanelShowStyle; }
      set {
        genPanelShowStyle = value;
        if (GenPanelShowStyleChanged != null)
          GenPanelShowStyleChanged(this, new EventArgs()); //report if there is any changed on this show style! because this cannot be handled alone
        Invalidate(); //TODO not sure if it is the best idea, but so far seems to be okay...
      }
    }

    private bool isMoveable = false;
    public bool IsMoveable { get { return isMoveable; } }

    private Point mouseDownLocation;
    public Point MouseDownLocation { get { return mouseDownLocation; } }

    public void SetMovability(bool movability, EventArgs e = null) {
      isMoveable = movability;
      checkBox.BackColor = suggestBackColor();
      if (isMoveable)
        mouseDownLocation = this.Location;
      if (MobilityChanged != null && e != null)
        MobilityChanged(this, e);
    }

    void adjustSize() {
      int width = checkBox.Width;
      int height = checkBox.Height;
      this.Size = new Size(width + RCB_X_POSITION + 1, height); //TODO currently this is only done for for normal data type (text), originally, this is +1
    }

		private void drawUpperRectangleIcon(Graphics graphics, System.Drawing.Color pencolor, System.Drawing.Color fillcolor) {
      using (Pen pen = new Pen(pencolor, 2))
      using (Brush brush = new SolidBrush(fillcolor)) {
        graphics.DrawRectangle(pen, 1, 1, RCB_X_POSITION - 3, 7); //TODO consider font size...
        graphics.FillRectangle(brush, 1, 1, RCB_X_POSITION - 3, 7);
      }
    }

		private void drawLowerRectangleIcon(Graphics graphics, System.Drawing.Color pencolor, System.Drawing.Color fillcolor) {
      using (Pen pen = new Pen(pencolor, 2))
      using (Brush brush = new SolidBrush(fillcolor)) {
        graphics.DrawRectangle(pen, 1, 11, RCB_X_POSITION - 3, 7); //TODO consider font size...
        graphics.FillRectangle(brush, 1, 11, RCB_X_POSITION - 3, 7);
      }
    }

    //private void drawTriangleIcon(Graphics graphics, Color pencolor, Color fillcolor) {
    //  using (Pen pen = new Pen(pencolor, 2))
    //  using (Brush brush = new SolidBrush(fillcolor)) {
    //    Point[] trianglePoints = new Point[] 
    //    { new Point(1 + (RCB_X_POSITION - 4) / 2, 11), //top most of the triangle (3 pixel from the rectangle)
    //      new Point(1, 18), //left bottom of the triangle
    //      new Point(RCB_X_POSITION - 3, 18) }; //right bottom of the triangle (1 pixel less from checkbox)
    //    graphics.DrawPolygon(pen, trianglePoints);
    //    graphics.FillPolygon(brush, trianglePoints);
    //  }
    //}

    protected override void OnPaint(PaintEventArgs e) {
      //Error checking portion
      if (GenData == null || string.IsNullOrWhiteSpace(GenData.DataName)) {
        if (checkBox.Visible)
          checkBox.Hide();
        base.OnPaint(e);
        return;
      }

      //Setting portion
      checkBox.Text = GenData.DataName;

      //Additional drawing portion
      e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
      string beforeVal = checkBox.ValueText;
      switch (GenPanelMode) {
      case GenericDataPanelMode.ReadOnly:
        System.Drawing.Color color = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0x08, 0x5C); //light crimson
				drawUpperRectangleIcon(e.Graphics, System.Drawing.Color.LightGray, color);
        checkBox.ValueText = GenData.DataReadBuffer == null ? "" : GenData.DataReadBuffer + (GenData.DataUnit == null ? "" : " " + GenData.DataUnit);
        break;
      case GenericDataPanelMode.ReadAndWrite:
				drawUpperRectangleIcon(e.Graphics, System.Drawing.Color.LightGray, System.Drawing.Color.Yellow);
        checkBox.ValueText = (GenData.DataReadBuffer == null ? "" : GenData.DataReadBuffer)
          + (GenData.DataReadBuffer == null && GenData.DataWriteBuffer == null ? "" : " | ")
          + (GenData.DataWriteBuffer == null ? "" : GenData.DataWriteBuffer)
          + (GenData.DataUnit == null ? "" : (GenData.DataReadBuffer == null && GenData.DataWriteBuffer == null ? "" : " " + GenData.DataUnit));
        break;
      case GenericDataPanelMode.WriteOnly:
				drawUpperRectangleIcon(e.Graphics, System.Drawing.Color.LightGray, System.Drawing.Color.LightGreen);
        checkBox.ValueText = GenData.DataWriteBuffer == null ? "" : GenData.DataWriteBuffer + (GenData.DataUnit == null ? "" : " " + GenData.DataUnit);
        break;
      default:
        break;
      }

      switch (GenPanelShowStyle) {
      case GenericDataPanelShowStyle.Compact:
				drawLowerRectangleIcon(e.Graphics, System.Drawing.Color.LightGray, System.Drawing.Color.Gold);
        break;
      case GenericDataPanelShowStyle.Complete:
				drawLowerRectangleIcon(e.Graphics, System.Drawing.Color.LightGray, System.Drawing.Color.AliceBlue);
        break;
      default:
        break;
      }

      //Life line update
      if (isLineAlive)
        drawAliveLine(lifeLineUpdateRequest);
      else
        removeAliveLine();

      //Checkbox update
      if (!checkBox.Visible) //at this point, the checkBox must be shown
        checkBox.Show();
      if (checkBox.ValueText != beforeVal || lifeLineUpdateRequest) //If there isn't this, it will keep drawing
        checkBox.Invalidate(); //this does not mean that the size of the checkbox is immediately changed!

      lifeLineUpdateRequest = false; //always false after drawing
    }

    //Old drawing portion    
    //base.OnPaint(e);

    //protected override CreateParams CreateParams {
    //  get {
    //    CreateParams createParams = base.CreateParams;
    //    createParams.ExStyle |= 0x00000020;
    //    return createParams;
    //  }
    //}

    //protected override void OnPaintBackground(PaintEventArgs e) {
    //  //base.OnPaintBackground(e); //Must be commented. Otherwise it would not give "transparent" background
    //}

  }
}

    ////From: http://www.gamedev.net/topic/321029-how-to-simulate-a-mouse-click-in-c/
    //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    //private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    //private const uint MOUSEEVENTF_LEFTUP = 0x04;
    //private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
    //private const uint MOUSEEVENTF_RIGHTUP = 0x10;

    //public void DoMouseClick() {
    //  //Call the imported function with the cursor's current position
    //  //int X = Cursor.Position.X;
    //  //int Y = Cursor.Position.Y;
    //  mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)this.Location.X, (uint)this.Location.Y, 0, 0);
    //}

//if (listBox.Visible) {
//  width = Math.Max(checkBox.Width, listBox.Width);
//  height += listBox.Height;
//}
//if (GenData.ComboList != null && GenData.ComboList.Count > 0) { //combo type
//  if (!listBox.Visible)
//    listBox.Show();
//} else if (GenData.BitsList != null && GenData.BitsList.Count > 0) { //bits type
//  if (!listBox.Visible)
//    listBox.Show();
//}      //using (SolidBrush solidBrush = new SolidBrush(checkBox.BackColor)) {
//}

//this.BackColor = checkBox.BackColor;
//After base paint
//ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, checkBox.BorderColor, ButtonBorderStyle.Solid);
//protected override void OnMouseDown(MouseEventArgs e) {
//  base.OnMouseDown(e);
//  if (e.Button == MouseButtons.Left)
//    mouseDownLocation = e.Location;      
//}

//protected override void OnMouseMove(MouseEventArgs e) {
//  base.OnMouseMove(e);
//  if (e.Button == MouseButtons.Left) {
//    this.Left = e.X + this.Left - mouseDownLocation.X;
//    this.Top = e.Y + this.Top - mouseDownLocation.Y;
//  }
//}

//protected override void OnMouseUp(MouseEventArgs e) {
//  base.OnMouseUp(e);
//  //if (e.Button == MouseButtons.Left) {
//  //  this.Left = e.X + this.Left - mouseDownLocation.X;
//  //  this.Top = e.Y + this.Top - mouseDownLocation.Y;
//  //}
//}

//using (Brush brush = new SolidBrush(this.BackColor))
//  e.Graphics.FillRectangle(brush, this.ClientRectangle);      

