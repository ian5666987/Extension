using System.Windows.Forms;
using System.Drawing;

namespace Extension.Controls {
  public static class RichTextBoxExtension {
    public static void AppendTextWithColor(this RichTextBox richTextBox, Color color, string text) {
      richTextBox.SelectionStart = richTextBox.TextLength;
      richTextBox.SelectionLength = 0;
      richTextBox.SelectionColor = color;
      richTextBox.AppendText(text);
      richTextBox.SelectionColor = richTextBox.ForeColor;
    }

    public static void AppendStyledText(this RichTextBox richTextBox, FontStyle style, string text) {
      richTextBox.SelectionStart = richTextBox.TextLength;
      richTextBox.SelectionLength = 0;
      richTextBox.SelectionFont = new Font(richTextBox.Font, style);
      richTextBox.AppendText(text);
      richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
    }

    public static void AppendStyledTextWithColor(this RichTextBox richTextBox, FontStyle style, Color color, string text) {
      richTextBox.SelectionStart = richTextBox.TextLength;
      richTextBox.SelectionLength = 0;
      richTextBox.SelectionColor = color;
      richTextBox.SelectionFont = new Font(richTextBox.Font, style);
      richTextBox.AppendText(text);
      richTextBox.SelectionColor = richTextBox.ForeColor;
      richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
    }
  }
}
