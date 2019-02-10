using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Extension.Drawing {
  public static class RectangleDrawer {
    private static Form mMask;
    private static Point mPos;
    public static Rectangle Draw(Form parent) {
      // Record the start point
			mPos = parent.PointToClient(System.Windows.Forms.Control.MousePosition);
      // Create a transparent form on top of <frm>
      mMask = new Form();
      mMask.FormBorderStyle = FormBorderStyle.None;
			mMask.BackColor = System.Drawing.Color.Magenta;
      mMask.TransparencyKey = mMask.BackColor;
      mMask.ShowInTaskbar = false;
      mMask.StartPosition = FormStartPosition.Manual;
      mMask.Size = parent.ClientSize;
      mMask.Location = parent.PointToScreen(Point.Empty);
      mMask.MouseMove += MouseMove;
      mMask.MouseUp += MouseUp;
      mMask.Paint += PaintRectangle;
      mMask.Load += DoCapture;
      // Display the overlay
      mMask.ShowDialog(parent);
      // Clean-up and calculate return value
      mMask.Dispose();
      mMask = null;
			Point pos = parent.PointToClient(System.Windows.Forms.Control.MousePosition);
      int x = Math.Min(mPos.X, pos.X);
      int y = Math.Min(mPos.Y, pos.Y);
      int w = Math.Abs(mPos.X - pos.X);
      int h = Math.Abs(mPos.Y - pos.Y);
      return new Rectangle(x, y, w, h);
    }
    private static void DoCapture(object sender, EventArgs e) {
      // Grab the mouse
      mMask.Capture = true;
    }
    private static void MouseMove(object sender, MouseEventArgs e) {
      // Repaint the rectangle
      mMask.Invalidate();
    }
    private static void MouseUp(object sender, MouseEventArgs e) {
      // Done, close mask
      mMask.Close();
    }
    private static void PaintRectangle(object sender, PaintEventArgs e) {
      // Draw the current rectangle
			Point pos = mMask.PointToClient(System.Windows.Forms.Control.MousePosition);
      using (Pen pen = new Pen(Brushes.Black)) {
        pen.DashStyle = DashStyle.Dot;
        e.Graphics.DrawLine(pen, mPos.X, mPos.Y, pos.X, mPos.Y);
        e.Graphics.DrawLine(pen, pos.X, mPos.Y, pos.X, pos.Y);
        e.Graphics.DrawLine(pen, pos.X, pos.Y, mPos.X, pos.Y);
        e.Graphics.DrawLine(pen, mPos.X, pos.Y, mPos.X, mPos.Y);
      }
    }
  }
}