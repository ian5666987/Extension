using System;
using System.Windows.Forms;

namespace Extension.Controls {
  public class ButtonPanel : Button {
    public ButtonPanel() : base() {
      this.SetStyle(ControlStyles.Selectable, false); //important to make the "button" unselectable
      this.TabStop = false;
    }
    protected override void OnMouseEnter(EventArgs e) { }
    protected override void OnMouseLeave(EventArgs e) { }
    protected override void OnMouseClick(MouseEventArgs e) { }
    protected override void OnMouseDown(MouseEventArgs mevent) { }
    protected override void OnMouseUp(MouseEventArgs mevent) { }
    protected override void OnClick(EventArgs e) { }
    protected override void OnEnter(EventArgs e) { }
  }
}
