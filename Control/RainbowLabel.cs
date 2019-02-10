using System.Windows.Forms;
using System.Drawing;

namespace Extension.Controls {
  public enum ValueTextPositionType {
    AfterText,
    RightAligned,
    FixedOffset,
    BelowText
  }

  public class RainbowLabel : Label {
    public static Color DefaultBorderColor { get { return Color.LightGray; } }
    private Color borderColor = DefaultBorderColor;
    public Color BorderColor {
      get { return borderColor; }
      set {
        borderColor = value;
        //this.Invalidate();
        //this.Update();
      }
    }

    public static ButtonBorderStyle DefaultAltBorderStyle { get { return ButtonBorderStyle.Solid; } }
    private ButtonBorderStyle altBorderStyle = DefaultAltBorderStyle;
    public ButtonBorderStyle AltBorderStyle {
      get { return altBorderStyle; }
      set {
        altBorderStyle = value;
        //this.Invalidate();
        //this.Update();
      }
    }

    //this.Invalidate();  // request a delayed Repaint by the normal MessageLoop system    
    //this.Update();      // forces Repaint of invalidated area 
    //this.Refresh();     // Combines Invalidate() and Update()

    public static Color DefaultValueColor { get { return Color.Blue; } }
    private Color valueColor = DefaultValueColor;
    public Color ValueColor {
      get { return valueColor; }
      set { 
        valueColor = value; 
        //this.Invalidate(); 
        //this.Update();
      }
    }

    private string valueText = "";
    public string ValueText { 
      get { return valueText; } 
      set { 
        valueText = value; 
        //this.Invalidate(); 
        //this.Update();
      } 
    }

    private Font valueFont = new Font(Label.DefaultFont,FontStyle.Regular);
    public Font ValueFont {
      get { return valueFont; }
      set {
        valueFont = value;
        //this.Invalidate();
        //this.Update();
      }
    }

    private ValueTextPositionType valueTextPosition = ValueTextPositionType.RightAligned;
    public ValueTextPositionType ValueTextPosition {
      get { return valueTextPosition; }
      set { 
        valueTextPosition = value;
        //this.Invalidate();
        //this.Update();
      }
    }

    private int fixedOffsetValueX = 0;
    public int FixedOffsetValueX {
      get { return fixedOffsetValueX; }
      set { 
        fixedOffsetValueX = value; 
        //this.Invalidate(); 
        //this.Update();
      }
    }

    private int fixedOffsetValueY = 0;
    public int FixedOffsetValueY {
      get { return fixedOffsetValueY; }
      set {
        fixedOffsetValueY = value;
        //this.Invalidate();
        //this.Update();
      }
    }

    public override ContentAlignment TextAlign { get { return ContentAlignment.TopLeft; } }//always top-left, read only
    public override bool AutoSize { get { return false; } } // cannot be autosized

    protected override void OnPaint(PaintEventArgs e) {
      SolidBrush solidBrush = new SolidBrush(ValueColor);
      SizeF textSizeF = e.Graphics.MeasureString(this.Text, this.Font);
      SizeF valueTextSizeF = e.Graphics.MeasureString(valueText, valueFont);
      float x = 0, y = 0;
      switch (ValueTextPosition){
      case ValueTextPositionType.AfterText:
        x = textSizeF.Width; //+ e.Graphics.MeasureString(" ", this.Font).Width;
        break;
      case ValueTextPositionType.RightAligned:
        x = this.Width - valueTextSizeF.Width; //the padding is fixed?
        break;
      case ValueTextPositionType.BelowText:
        y = textSizeF.Height;
        break;
      case ValueTextPositionType.FixedOffset:
        x = FixedOffsetValueX;
        y = FixedOffsetValueY;
        break;
      default:
        break;
      }
      base.OnPaint(e);
      ControlPaint.DrawBorder(e.Graphics, this.DisplayRectangle, borderColor, this.AltBorderStyle);
      e.Graphics.DrawString(valueText, this.ValueFont, solidBrush, x, y);
    }
  }
}
