using System.Windows.Forms;

namespace Extension.Controls {
  public class ControlledFocusCueCheckBox : CheckBox {
    //Remove the focus cue: http://stackoverflow.com/questions/148729/how-to-set-change-remove-focus-style-on-a-button-in-c
    private bool disableFocusCue = false;
    public bool DisableFocusCue { get { return disableFocusCue; } set { disableFocusCue = value; } }

    protected override bool ShowFocusCues { get { return disableFocusCue ? false : base.ShowFocusCues; } }
  }
}
