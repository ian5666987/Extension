using System;
using System.Runtime.InteropServices;

namespace Extension.Drawing {
  class Control {
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

    private const int WM_SETREDRAW = 11;

		public static void SuspendDrawing(System.Windows.Forms.Control parent) {
      SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
    }

		public static void ResumeDrawing(System.Windows.Forms.Control parent) {
      SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
      parent.Refresh();
    }
  }
}
