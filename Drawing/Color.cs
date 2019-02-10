namespace Extension.Drawing {
  public class Color {
    //adopted from http://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
    /// <summary>Blends the specified colors together.</summary>
    /// <param name="color">Color to blend onto the background color.</param>
    /// <param name="backColor">Color to blend the other color onto.</param>
    /// <param name="amount">How much of <paramref name="color"/> to keep,
    /// “on top of” <paramref name="backColor"/>.</param>
    /// <returns>The blended colors.</returns>
		public static System.Drawing.Color Blend(System.Drawing.Color color, System.Drawing.Color backColor, double amount) {
      byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
      byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
      byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
			return System.Drawing.Color.FromArgb(r, g, b);
    }    
  }
}
