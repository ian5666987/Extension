using System.Windows.Forms;
using System.Drawing;
using System;

namespace Extension.Controls {
  public class RainbowCheckBox : CheckBox {
    public RainbowCheckBox() : base(){
      AutoSize = false;      
    }

    public static Color DefaultBorderColor { get { return Color.LightGray; } }
    private Color borderColor = DefaultBorderColor;
    public Color BorderColor { get { return borderColor; } set { borderColor = value; } }

    public static ButtonBorderStyle DefaultAltBorderStyle { get { return ButtonBorderStyle.Solid; } }
    private ButtonBorderStyle altBorderStyle = DefaultAltBorderStyle;
    public ButtonBorderStyle AltBorderStyle { get { return altBorderStyle; } set { altBorderStyle = value; } }

    public static Color DefaultValueColor { get { return Color.Blue; } }
    private Color valueColor = DefaultValueColor;
    public Color ValueColor { get { return valueColor; } set { valueColor = value; } }

    private string valueText = "";
    public string ValueText { get { return valueText; } set { valueText = value; } }

    private Font valueFont = new Font(CheckBox.DefaultFont, FontStyle.Bold);
    public Font ValueFont { get { return valueFont; } set { valueFont = value; } }

    private ValueTextPositionType valueTextPosition = ValueTextPositionType.RightAligned;
    public ValueTextPositionType ValueTextPosition { get { return valueTextPosition; } set { valueTextPosition = value; } }

    private int fixedOffsetValueX = 0;
    public int FixedOffsetValueX { get { return fixedOffsetValueX; } set { fixedOffsetValueX = value; } }

    private int fixedOffsetValueY = 0;
    public int FixedOffsetValueY { get { return fixedOffsetValueY; } set { fixedOffsetValueY = value; } }

    private int valueWidthOffset = 17; //based on experiment
    public int ValueWidthOffset { get { return valueWidthOffset; } set { valueWidthOffset = value; } }

    private int valueHeightOffset = 1; //based on experiment
    public int ValueHeightOffset { get { return valueHeightOffset; } set { valueHeightOffset = value; } }

    private int afterTextSpace = 0; //based on experiment
    public int AfterTextSpace { get { return afterTextSpace; } set { afterTextSpace = value; } }

    private bool rightEndAutoAdjust = false;
    public bool RightEndAutoAdjust { get { return rightEndAutoAdjust; } set { rightEndAutoAdjust = value; } }

    private int rightEndAutoAdjustOffset = 3; //based on experiment
    public int RightEndAutoAdjustOffset { get { return rightEndAutoAdjustOffset; } set { rightEndAutoAdjustOffset = value; } }

    //Remove the focus cue: http://stackoverflow.com/questions/148729/how-to-set-change-remove-focus-style-on-a-button-in-c
    private bool disableFocusCue = false;
    public bool DisableFocusCue { get { return disableFocusCue; } set { disableFocusCue = value; } }

    protected override bool ShowFocusCues { get { return disableFocusCue ? false : base.ShowFocusCues; } }
    public override ContentAlignment TextAlign { get { return ContentAlignment.TopLeft; } } //return the base
    //public override bool AutoSize { get { return false; } } // cannot be autosized

#if DEBUG
    private string printItem;
    public string PrintItem { get { return printItem; } }
    private Color printColor;
    public Color PrintColor { get { return printColor; } }
    public event EventHandler PrintRequest;
    public void ClearPrint() {
      printItem = null;
      printColor = Color.Blue;
    }
#endif

    private bool isOnBox = false;
    public bool IsOnBox { get { return isOnBox; } }

    protected override void OnMouseDown(MouseEventArgs mevent) {
      isOnBox = mevent.Button == MouseButtons.Left && mevent.X <= BOX_POSITION;
      base.OnMouseDown(mevent);
    }

    private const int BOX_POSITION = 12;
    protected override void OnMouseClick(MouseEventArgs e) {
      isOnBox = e.Button == MouseButtons.Left && e.X <= BOX_POSITION;
      base.OnMouseClick(e);
    }

    protected override void OnPaint(PaintEventArgs e) { //We can see how this is drawn many times! How to limit? parent drawing, can it make the child draws often too?      
      SizeF textSizeF = e.Graphics.MeasureString(this.Text, this.Font);
      SizeF valueTextSizeF = e.Graphics.MeasureString(valueText, valueFont);
      float x = 0, y = 0;
      switch (ValueTextPosition) {
      case ValueTextPositionType.AfterText:
        x = textSizeF.Width + valueWidthOffset + afterTextSpace; //checkbox size + space, supposedly
        y = valueHeightOffset;
        break;
      case ValueTextPositionType.RightAligned:
        x = this.Width - valueTextSizeF.Width; //the padding is fixed?
        y = valueHeightOffset;
        break;
      case ValueTextPositionType.BelowText:
        x = valueWidthOffset;
        y = textSizeF.Height;
        break;
      case ValueTextPositionType.FixedOffset:
        x = FixedOffsetValueX + valueWidthOffset;
        y = FixedOffsetValueY;
        break;
      default:
        break;
      }
      if (rightEndAutoAdjust)
        this.Width = (int)(x + valueTextSizeF.Width + rightEndAutoAdjustOffset);

      //Drawing part
      base.OnPaint(e); //must be before the string and the border
      if (this.AltBorderStyle != ButtonBorderStyle.None)
        ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, borderColor, this.AltBorderStyle);
      using (SolidBrush solidBrush = new SolidBrush(ValueColor)) //to ensure that the brush is disposed afterwards
        e.Graphics.DrawString(valueText, this.ValueFont, solidBrush, x, y);

#if DEBUG
      printItem = this.Text;
      printColor = Color.Blue;
      if (string.IsNullOrWhiteSpace(printItem) && PrintRequest != null) {
        printItem += "\n";
        PrintRequest(this, e);
      }
#endif
    }

  }
}
