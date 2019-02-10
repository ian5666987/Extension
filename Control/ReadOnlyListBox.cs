using System.Windows.Forms;

//Adopted from: http://stackoverflow.com/questions/8197923/winforms-how-to-prevent-listbox-item-selection
//Original source: http://ajeethtechnotes.blogspot.sg/2009/02/readonly-listbox.html
namespace Extension.Controls {
  public class ReadOnlyListBox : ListBox {
    private bool readOnly = false;
    public bool ReadOnly {
      get { return readOnly; }
      set { readOnly = value; }
    }

    //Remove the focus cue: http://stackoverflow.com/questions/148729/how-to-set-change-remove-focus-style-on-a-button-in-c
    private bool disableFocusCue = false;
    public bool DisableFocusCue { get { return disableFocusCue; } set { disableFocusCue = value; } }

    protected override bool ShowFocusCues { get { return disableFocusCue ? false : base.ShowFocusCues; } }

    protected override void DefWndProc(ref Message m) {
      // If ReadOnly is set to true, then block any messages 
      // to the selection area from the mouse or keyboard. 
      // Let all other messages pass through to the 
      // Windows default implementation of DefWndProc.
      if (!readOnly || ((m.Msg <= 0x0200 || m.Msg >= 0x020E)
      && (m.Msg <= 0x0100 || m.Msg >= 0x0109)
      && m.Msg != 0x2111
      && m.Msg != 0x87)) {
        base.DefWndProc(ref m);
      }
    }
   
    public ReadOnlyListBox() : base () {
    }
  }
}