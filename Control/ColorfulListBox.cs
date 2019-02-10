using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Extension.Controls {
  public class MyListBoxItem {
    public List<Color> ItemForeColors = new List<Color>();
    public List<Color> ItemBackColors = new List<Color>();
    public List<string> Message = new List<string>();
    public List<int> ItemMaxWidths = new List<int>();
    public int AssignedIndex;

    private bool isMultipleMessage = false;
    public bool IsMultipleMessage { get { return isMultipleMessage; } }
    private bool isMultipleColor = false;
    public bool IsMultipleColors { get { return isMultipleColor; } }

    #region Single message, single color constructor
    public MyListBoxItem()
      : this(ListBox.DefaultForeColor, ListBox.DefaultBackColor) { //Single message, single color, default colors
    }

    public MyListBoxItem(Color foreColor, Color backColor, string nameStr = null) { //Single message, single color
      ItemForeColors.Add(foreColor);
      ItemBackColors.Add(backColor);
      this.Message.Add(nameStr != null ? nameStr : null); //At least there is a single message, though null
      AssignedIndex = -1;
    }
    #endregion

    #region Multiple messages
    private void constructMaxWidths(int[] maxWidths, int msgIndex) {
      if (maxWidths != null) {
        int width = msgIndex >= maxWidths.Length ? 0 : maxWidths[msgIndex];
        ItemMaxWidths.Add(width);
      } else
        ItemMaxWidths.Add(0);
    }

    private void constructWithInvalidMessages(int[] maxWidths) {
      this.Message.Add(null); //There is at least one message, though null!!
      constructMaxWidths(maxWidths, 0);
    }

    #region Multiple messages, single color constructor
    public MyListBoxItem(Color foreColor, Color backColor, string[] msgStrArray, int[] maxWidths = null) { //Multiple messages, one color
      ItemForeColors.Add(foreColor);
      ItemBackColors.Add(backColor);
      if (msgStrArray != null && msgStrArray.Length > 0) {
        isMultipleMessage = msgStrArray.Length > 1;
        this.Message.AddRange(msgStrArray);
        for (int i = 0; i < msgStrArray.Length; ++i) //widths should depend on the strArray
          constructMaxWidths(maxWidths, i);
      } else
        constructWithInvalidMessages(maxWidths);
      AssignedIndex = -1;
    }
    #endregion

    #region Multiple messages, multiple colors constructor
    private void constructBackColors(Color[] backColors, int backColorIndex) {
      if (backColors != null) {
        Color back = backColorIndex >= backColors.Length ? ListBox.DefaultBackColor : backColors[backColorIndex];
        ItemBackColors.Add(back);
      } else
        ItemBackColors.Add(ListBox.DefaultBackColor);
    }

    private void constructWithInvalidForeColors(Color[] backColors) {
      ItemForeColors.Add(ListBox.DefaultForeColor);
      constructBackColors(backColors, 0);
    }

    //It is possible to have more color then the messages, yet the drawing must take care, that it depends on the message, not color
    //If color is available, uses it. If not, then just use the last color available
    public MyListBoxItem(Color[] foreColors, Color[] backColors, string[] msgStrArray, int[] maxWidths = null) { //Multiple messages, multiple colors
      if (foreColors != null && foreColors.Length > 0) {
        isMultipleColor = foreColors.Length > 1;
        ItemForeColors.AddRange(foreColors);
        for (int i = 0; i < foreColors.Length; ++i) //Back colors should depend on the fore colors length
          constructBackColors(backColors, i);
      } else
        constructWithInvalidForeColors(backColors);

      if (msgStrArray != null && msgStrArray.Length > 0) {
        isMultipleMessage = msgStrArray.Length > 1;
        this.Message.AddRange(msgStrArray);
        for (int i = 0; i < msgStrArray.Length; ++i) //widths should depend on the strArray
          constructMaxWidths(maxWidths, i);
      } else
        constructWithInvalidMessages(maxWidths);

      AssignedIndex = -1;
    }

    public MyListBoxItem(MyListBoxItem mlb)
      : this(mlb.ItemForeColors.ToArray(), mlb.ItemBackColors.ToArray(), mlb.Message.ToArray(), mlb.ItemMaxWidths.ToArray()) {
    }
    #endregion
    #endregion

    public override string ToString() { //Return the first message, it must have existed, though null
      return this.Message[0];
    }

    public string MessageToString(int index) { //Return other message, it could have not existed
      if (index < this.Message.Count)
        return this.Message[index];
      return null;
    }
  }

  public class ColorfulListBox : ListBox {
    #region Constructor
    private bool isColorfulListBoxInitialized = false;
    public ColorfulListBox()
      : base() {
      this.DrawMode = DrawMode.OwnerDrawFixed;
      isColorfulListBoxInitialized = true;
    }
    #endregion

    #region Public methods
    public List<string> GetMessageList(int index = 0) {
      List<string> msgList = new List<string>();
      ListBox.ObjectCollection coll = this.Items;
      foreach (MyListBoxItem mlbItem in coll)
        msgList.Add(mlbItem.Message[index]);
      return msgList;
    }

    public void RefreshMessage(int index = 0) {
      ListBox.ObjectCollection collOb = this.Items;
      int currentIndex = 0;
      foreach (MyListBoxItem mlbItem in collOb) {
        mlbItem.Message[index] = null;        
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs e = new DrawItemEventArgs(graph, this.Font, this.Bounds, currentIndex++, DrawItemState.None);
          drawMyListBoxItem(e, mlbItem, this.SelectedIndex);
        }
      }
    }

    public void SetAndDrawNamedMessage(string name, string msg, int index = 0) { //To update value of the named data
      MyListBoxItem item = getDataItemFromName(name);
      if (item != null && index < item.Message.Count) {
        item.Message[index] = msg;
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs e = new DrawItemEventArgs(graph, this.Font, this.Bounds, this.SelectedIndex == -1 ? item.AssignedIndex : this.SelectedIndex, DrawItemState.None);
          drawMyListBoxItem(e, item, this.SelectedIndex);
        }
      }
    }

    public void SetNamedMessage(string name, string msg, int index = 0) { //To update unit of the named data
      MyListBoxItem item = getDataItemFromName(name);
      if (item != null && index < item.Message.Count)
        item.Message[index] = msg;
    }

    public void SetAndDrawNamedMessages(string name, string[] msgs, int startIndex = 0, bool forceDraw = false, Color[] foreColorList = null) { //Length is implicit in the msgs itself
      MyListBoxItem item = getDataItemFromName(name);
      if (item != null && msgs != null) //if item is found
        if (msgs.Length + startIndex <= item.Message.Count || forceDraw) { //And not too many are updated. When it is still "Not available", actually the item could be very tricky! msgs.Length (2) + startIndex (1) = 3, but item.Message.Count
          if (forceDraw)
            while (item.Message.Count < msgs.Length + startIndex)
              item.Message.Add("");
          for (int i = 0; i < msgs.Length; ++i) {
            item.Message[startIndex + i] = msgs[i];
            if (foreColorList != null && foreColorList.Length > i) {
              if (item.ItemForeColors.Count - 1 < startIndex + i)
                item.ItemForeColors.Add(Color.Black);//temporarily, any color will do
              item.ItemForeColors[startIndex + i] = foreColorList[i];
            }
          }
          using (Graphics graph = this.CreateGraphics()) {
            DrawItemEventArgs e = new DrawItemEventArgs(graph, this.Font, this.Bounds, item.AssignedIndex, DrawItemState.None);
            drawMyListBoxItem(e, item, this.SelectedIndex);
          }
        } else if (forceDraw) {
        }
    }

    public void ChangeSelectedItemForeColor(Color color, int index = 0) {
      MyListBoxItem item = SelectedItem as MyListBoxItem;
      if (item != null) { //This necessarily must have selected item, if not skip
        item.ItemForeColors[index] = color;
        int appliedIndex = this.SelectedIndex == -1 ? 0 : this.SelectedIndex;
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs e = new DrawItemEventArgs(graph, this.Font, this.Bounds, appliedIndex, DrawItemState.None);
          drawMyListBoxItem(e, item, appliedIndex);
        }
      }
    }

    public void ChangeAndDrawNamedItemForeColor(string name, Color color, int index = 0) { //This should find the itemNo
      MyListBoxItem item = getDataItemFromName(name);
      if (item != null) {
        item.ItemForeColors[index] = color;
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs e = new DrawItemEventArgs(graph, this.Font, this.Bounds, item.AssignedIndex, DrawItemState.None);
          drawMyListBoxItem(e, item, this.SelectedIndex);
        }
      }
    }

    public void ChangeNamedItemForeColor(string name, Color color, int index = 0) { //This should find the itemNo
      MyListBoxItem item = getDataItemFromName(name);
      if (item != null)
        item.ItemForeColors[index] = color;
    }

    #endregion Public methods

    #region Properties (derived)
    public override DrawMode DrawMode {
      get { return base.DrawMode; }
      set { //Cannot change the DrawMode once set as OwnerDrawFixed
        if (!isColorfulListBoxInitialized)
          base.DrawMode = value;
      }
    }

    public bool FollowLastAvailableColor = true;
    public bool LastItemDrawTillTheEnd = true;
    #endregion Properties (derived)

    #region Event handlers (derived)
    private int prevSelectionIndex = -1;
    protected override void OnSelectedIndexChanged(EventArgs e) {      
      base.OnSelectedIndexChanged(e);
      prevSelectionIndex = SelectedIndex;
			List<MyListBoxItem> collObList = Items.Cast<MyListBoxItem>().ToList();
			for (int i = 0; i < collObList.Count; ++i) //Currently I re-draw everything, but it could be only visible items need to be redrawn. Isn't it?
        if (i != SelectedIndex) {
          MyListBoxItem item = collObList[i];
          using (Graphics graph = this.CreateGraphics()) {
            DrawItemEventArgs ed = new DrawItemEventArgs(graph, this.Font, this.Bounds, i, DrawItemState.None);
            drawMyListBoxItem(ed, item, this.SelectedIndex);
          }
        }
    }

    List<Color[]> enabledForeColorsList = new List<Color[]>();
    List<Color[]> enabledBackColorsList = new List<Color[]>();
    Color disabledForeColor = Color.Gray;
    Color disabledBackColor = Color.White;
    private void changeColorToDisabled() {
			List<MyListBoxItem> collObList = Items.Cast<MyListBoxItem>().ToList();
			for (int i = 0; i < collObList.Count; ++i) {
        MyListBoxItem item = collObList[i];
        Color[] enabledForeColors = new Color[item.ItemForeColors.Count];
        Color[] enabledBackColors = new Color[item.ItemBackColors.Count];
        for (int j = 0; j < item.ItemForeColors.Count; ++j) { //no of fore & back colors are identical
          enabledForeColors[j] = item.ItemForeColors[j];
          enabledBackColors[j] = item.ItemBackColors[j];
          item.ItemForeColors[j] = disabledForeColor;
          item.ItemBackColors[j] = disabledBackColor;
        }
        enabledForeColorsList.Add(enabledForeColors);
        enabledBackColorsList.Add(enabledBackColors);
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs ed = new DrawItemEventArgs(graph, this.Font, this.Bounds, i, DrawItemState.None);
          drawMyListBoxItem(ed, item, this.SelectedIndex);
        }
      }
    }

    private void changeColorToEnabled() {
      ListBox.ObjectCollection collOb = this.Items;
			List<MyListBoxItem> collObList = new List<MyListBoxItem>();
			foreach (object obj in collOb)
				collObList.Add((MyListBoxItem)obj);
			for (int i = 0; i < collObList.Count; ++i) {
        MyListBoxItem item = collObList[i];
        Color[] enabledForeColors = enabledForeColorsList[i];
        Color[] enabledBackColors = enabledBackColorsList[i];
        for (int j = 0; j < item.ItemForeColors.Count; ++j) { //no of fore & back colors are identical
          item.ItemForeColors[j] = enabledForeColors[j];
          item.ItemBackColors[j] = enabledBackColors[j];
        }
        using (Graphics graph = this.CreateGraphics()) {
          DrawItemEventArgs ed = new DrawItemEventArgs(graph, this.Font, this.Bounds, i, DrawItemState.None);
          drawMyListBoxItem(ed, item, this.SelectedIndex);
        }
      }
      enabledForeColorsList.Clear();
      enabledBackColorsList.Clear();
    }

    private delegate void processEnabledChangedDelegate();
    protected override void OnEnabledChanged(EventArgs e) {
      processEnabledChangedDelegate processEnabledChanged = this.Enabled ? new processEnabledChangedDelegate(changeColorToEnabled) : new processEnabledChangedDelegate(changeColorToDisabled);
      processEnabledChanged();
      base.OnEnabledChanged(e); //Need to handle the drawing myself when enabled/disabled!
    }

    protected override void OnDrawItem(DrawItemEventArgs e) { //This drawing must necessarily have item to draw
      base.OnDrawItem(e); //Remember that there can't be drawing when there is no proper indentified index for drawing!
      if (e.Index < 0 || this.Items.Count <= 0)
        return;
      MyListBoxItem item = e.Index >= 0 ? this.Items[e.Index] as MyListBoxItem : null; // Get the current item and cast it to MyListBoxItem
      int selectedIndex = this.SelectedIndex; //The current selected index is necessary to determine what color to draw
      this.ItemHeight = this.Font.Height; //This update is necessary before drawing
      if (item != null)
        if (e.Index >= this.TopIndex) //Only draw item that is visible
          drawMyListBoxItem(e, item, selectedIndex);
    }
    #endregion Event handlers (derived)

    #region private methods
    private const int RGBMAX = 255;
    private Color negativeColor(Color ColorToInvert) {
      return Color.FromArgb(RGBMAX - ColorToInvert.R,
        RGBMAX - ColorToInvert.G, RGBMAX - ColorToInvert.B);
    }

    private MyListBoxItem getDataItemFromName(string name) { //This will also trigger assignedIndex of the item
      ListBox.ObjectCollection collOb = this.Items;
			List<MyListBoxItem> collObList = new List<MyListBoxItem>();
			foreach (object obj in collOb)
				collObList.Add((MyListBoxItem)obj);
      List<string> collObMsgList = new List<string>();
      for (int i = 0; i < collObList.Count; ++i)
        collObMsgList.Add(collObList[i].Message[0]);
      MyListBoxItem namedItem = collObList[collObMsgList.IndexOf(name)];
      namedItem.AssignedIndex = collObMsgList.IndexOf(name);
      return namedItem;
    }

    private int getMaxWidth(DrawItemEventArgs e, Font font, string text) {
      SizeF stringSize = new SizeF();
      stringSize = e.Graphics.MeasureString(text, font);
      double width = stringSize.Width;
      return (int)width;
    }

    private void drawPrevItem(DrawItemEventArgs e, int prevIndex) {
      int relativeIndex = 0;
      int accWidth = 0;
      try {
        MyListBoxItem prevItem = this.Items[prevIndex] as MyListBoxItem; // Get the previous item and cast it to MyListBoxItem
        for (int i = 0; i < prevItem.Message.Count; ++i) { //Fore each prevItem which has message, draw
          Color appliedFore = ListBox.DefaultForeColor;
          Color appliedBack = ListBox.DefaultBackColor;
          if (FollowLastAvailableColor) { //Fore and back color counts would be identical in number...
            appliedFore = i < prevItem.ItemForeColors.Count ? prevItem.ItemForeColors[i] : prevItem.ItemForeColors[prevItem.ItemForeColors.Count - 1];
            appliedBack = i < prevItem.ItemBackColors.Count ? prevItem.ItemBackColors[i] : prevItem.ItemBackColors[prevItem.ItemBackColors.Count - 1];
          } else {
            appliedFore = i < prevItem.ItemForeColors.Count ? prevItem.ItemForeColors[i] : ListBox.DefaultForeColor;
            appliedBack = i < prevItem.ItemBackColors.Count ? prevItem.ItemBackColors[i] : ListBox.DefaultBackColor;
          }
          int appliedWidth = i < prevItem.ItemMaxWidths.Count ? prevItem.ItemMaxWidths[i] : getMaxWidth(e, this.Font, prevItem.Message[i]);
          if (appliedWidth == 0) //The same as no provided width
            appliedWidth = getMaxWidth(e, this.Font, prevItem.Message[i]);
          if (i == prevItem.Message.Count - 1 && LastItemDrawTillTheEnd) //last element
            appliedWidth = this.Width;
          string prevMsg = prevItem.Message[i];
          relativeIndex = prevIndex - this.TopIndex;
          e.Graphics.FillRectangle(new SolidBrush(appliedBack), accWidth, relativeIndex * this.Font.Height, appliedWidth, this.Font.Height); //Draw background
          e.Graphics.DrawString(prevMsg, this.Font, new SolidBrush(appliedFore), accWidth, relativeIndex * this.Font.Height); // Draw the appropriate text in the ListBox
          accWidth += appliedWidth;
        }
      } catch {
      }
    }

    private void drawMyListBoxItem(DrawItemEventArgs e, MyListBoxItem item, int selectedIndex) { //All to draw here is visible item, but selected index could be negative
      if (prevSelectionIndex != -1 && prevSelectionIndex != selectedIndex && prevSelectionIndex >= this.TopIndex) //This will just make the reaction faster, without this it will still work...
        drawPrevItem(e, prevSelectionIndex);
      int relativeIndex = e.Index - this.TopIndex; //The minimum relative index is one if followed carefully...
      int accWidth = 0;
      for (int i = 0; i < item.Message.Count; ++i) { //Fore each item which has message, draw
        Color appliedFore = ListBox.DefaultForeColor;
        Color appliedBack = ListBox.DefaultBackColor;
        if (e.Index == selectedIndex) {
          if (FollowLastAvailableColor)
            appliedFore = i < item.ItemForeColors.Count ? negativeColor(item.ItemForeColors[i]) : negativeColor(item.ItemForeColors[item.ItemForeColors.Count - 1]);
          else
            appliedFore = i < item.ItemForeColors.Count ? negativeColor(item.ItemForeColors[i]) : negativeColor(ListBox.DefaultForeColor);
          appliedBack = Color.DodgerBlue;
        } else {
          if (FollowLastAvailableColor) { //Fore and back color counts would be identical in number...
            appliedFore = i < item.ItemForeColors.Count ? item.ItemForeColors[i] : item.ItemForeColors[item.ItemForeColors.Count - 1];
            appliedBack = i < item.ItemBackColors.Count ? item.ItemBackColors[i] : item.ItemBackColors[item.ItemBackColors.Count - 1];
          } else {
            appliedFore = i < item.ItemForeColors.Count ? item.ItemForeColors[i] : ListBox.DefaultForeColor;
            appliedBack = i < item.ItemBackColors.Count ? item.ItemBackColors[i] : ListBox.DefaultBackColor;
          }
        }
        int appliedWidth = i < item.ItemMaxWidths.Count ? item.ItemMaxWidths[i] : getMaxWidth(e, this.Font, item.Message[i]);
        if (appliedWidth == 0) //The same as no provided width
          appliedWidth = getMaxWidth(e, this.Font, item.Message[i]);
        if (i == item.Message.Count - 1 && LastItemDrawTillTheEnd) //last element
          appliedWidth = this.Width;
        string msg = item.Message[i];
        e.Graphics.FillRectangle(new SolidBrush(appliedBack), accWidth, relativeIndex * this.Font.Height, appliedWidth, this.Font.Height); //Draw background
        e.Graphics.DrawString(msg, this.Font, new SolidBrush(appliedFore), accWidth, relativeIndex * this.Font.Height); // Draw the appropriate text in the ListBox
        accWidth += appliedWidth;
      }
    }
    #endregion private methods

    #region Incomplete
    private string GetChosenText() { //UNDONE Very specific just for textType item, to return the current chosen text item from coordinates and font size, to be made public when ready
      return null;
    }
    #endregion Incomplete

  }
}