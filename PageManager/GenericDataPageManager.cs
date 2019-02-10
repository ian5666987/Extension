using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Extension.Developer;
using Extension.Controls;

//TODO currently, there isn't class called "page manager" because the only data needs the page manager is generic data
//In the future, this can be added
namespace Extension.PageManager {
  public class GenericDataPageManager : Panel { //It is a panel
    PageManagerOptionsPanel optionsPanel = new PageManagerOptionsPanel();
    PageManagerExecutionPanel executionPanel = new PageManagerExecutionPanel();
    public List<GenericData> GenericDataList = null; //the only things should be filled by the user
    List<GenericDataPanel> genericDataPanelList = new List<GenericDataPanel>(); //to prevent this from being null under worst case scenario
    List<GenericDataPanel> movablePanelList = new List<GenericDataPanel>();

#if DEBUG
    public event EventHandler PrintRequest;
    private string printItem;
    public string PrintItem { get { return printItem; } }
    private Color printColor;
    public Color PrintColor { get { return printColor; } }
    public void ClearPrint() {
      printItem = null;
      printColor = Color.Blue;
    }
#endif

    public GenericDataPageManager() {
      //Options Panel
      optionsPanel.FilterUpdated += optionsPanel_FilterUpdated;
      optionsPanel.ShowStyleChanged += optionsPanel_ShowStyleChanged;
      optionsPanel.ClearClicked += optionsPanel_ClearClicked;
      optionsPanel.AlignmentChanged += optionsPanel_AlignmentChanged;
      optionsPanel.FontSizeUpDownChanged += optionsPanel_FontSizeUpDownChanged;
      optionsPanel.SwitchChanged += optionsPanel_SwitchChanged;
      optionsPanel.GridUpDownChanged += optionsPanel_GridUpDownChanged;
      this.Controls.Add(optionsPanel);

      //Execution Panel
      executionPanel.BinStyleChanged += executionPanel_BinStyleChanged;
      executionPanel.BinCheckBoxClicked += executionPanel_BinCheckBoxClicked;
      executionPanel.SendClicked += executionPanel_SendClicked;
      this.Controls.Add(executionPanel);

      //This
      this.TabStop = false;
    }

    #region primary public methods
    public bool AddGenericDataPanel(string dataPanelName, int x = 0, int y = 0, bool isRegistered = false) { //The simplest way to show generic panel, by inputing the name, the x and the y
      if (string.IsNullOrWhiteSpace(dataPanelName))
        return false;
      GenericDataPanel dataPanel = genericDataPanelList.Find(panel => panel.Name == dataPanelName);
      if (dataPanel != null) { //found
        dataPanel.CheckBoxBorderColor = dataPanel.IsRegistered ? System.Drawing.Color.Red : RainbowCheckBox.DefaultBorderColor;
        return false;
      }
      
      dataPanel = new GenericDataPanel();
      dataPanel.Name = dataPanelName;
      dataPanel.Location = new Point(x, y);      
      dataPanel.MobilityChanged += OnMobilityChanged;
      dataPanel.CbMouseMoveOnMovable += OnCbMouseMoveOnMovable;
      dataPanel.CbMouseDownOnMovable += OnCbMouseDownOnMovable;
      dataPanel.CbMouseUpOnMovable += OnCbMouseUpOnMovable;
      dataPanel.CbMouseClickOnBoxOnMovable += OnCbMouseClickOnBoxOnMovable;
      dataPanel.MouseClickOnPanelModeMovable += OnMouseClickOnPanelModeMovable;
      dataPanel.MouseClickOnPanelShowStyleMovable += OnMouseClickOnPanelShowStyleMovable;
      dataPanel.GenPanelShowStyleChanged += OnGenPanelShowStyleChanged;
      dataPanel.IsNullingCompleteDisplayControl += OnNullingCompleteDisplayControl;
      dataPanel.CompleteDisplayControlAcquired += OnCompleteDisplayControlAcquired;
#if DEBUG
      dataPanel.PrintRequest += PrintRequest;
#endif
      if (isRegistered) //if the command to register is issued
        registerDataPanel(dataPanel);

      //Try to create pseudo-Generic Data? This seems to be the best choice if there isn't injection of the real generic data, otherwise we cannot show it apart from genData being given
      GenericData pseudoGenData = new GenericData(new string[] { dataPanelName });
      dataPanel.GenData = pseudoGenData;
      try {
        dataPanel.SetCheckBoxToSuggestedBackColor(); //cannot be completely correct without checking the data validity
        dataPanel.GenPanelShowStyle = optionsPanel.ShowStyle;
        dataPanel.GenPanelMode = optionsPanel.PanelMode;
        dataPanel.Show();
        if (dataPanel.Location.Y + dataPanel.Height < executionPanel.Location.Y)
          dataPanel.BringToFront(); //don't remove!!
        genericDataPanelList.Add(dataPanel); //alternatively, adds the new item
        this.Controls.Add(dataPanel);
#if DEBUG
      } catch (Exception exc){
        dataPanel.GenData = null;
        printError(exc.ToString() + "\n");
#else 
			} catch {
				dataPanel.GenData = null;
#endif
				return false;
      }
      return true;
    }

    public void MarkGenericDataPanelAsUnknown(string dataPanelName) {
      GenericDataPanel dataPanel = genericDataPanelList.Find(panel => panel.Name == dataPanelName);
      if (dataPanel == null) // not found
        return;
      dataPanel.MarkAsUnknown = true;
    }

    public void AttachGenericData(string dataPanelName, GenericData genData) { //can only be "attached" if the dataPanel is found among the displayed? TODO not sure if this is the best idea!
      GenericDataPanel dataPanel = genericDataPanelList.Find(panel => panel.Name == dataPanelName);
      if (dataPanel == null) // not found
        return;
      dataPanel.GenData = genData;
      dataPanel.SetCheckBoxToSuggestedBackColor();
      if (dataPanel.Location.Y + dataPanel.Height < executionPanel.Location.Y)
        dataPanel.BringToFront(); //don't remove!!
      //dataPanel.RefreshDisplayValue();
    }

    public void MergeGenericData(string dataPanelName, GenericData genData) {
      GenericDataPanel dataPanel = genericDataPanelList.Find(panel => panel.Name == dataPanelName);
      if (dataPanel == null)// not found
        return;
      dataPanel.GenData.CombineAndOverride(genData);
    }

    public void RefreshAllBorderColor() { //only used when filter is really empty!
      foreach (GenericDataPanel dataPanel in genericDataPanelList)
        if (dataPanel.CheckBoxBorderColor == System.Drawing.Color.Red)
          dataPanel.CheckBoxBorderColor = RainbowCheckBox.DefaultBorderColor;
    }

    #endregion primary public methods

#if DEBUG
    void printError(string str) {
      printItem = str;
      printColor = Color.Red;
      if (PrintRequest != null)
        PrintRequest(this, null);
    }
#endif

    #region other events
    private void addCompleteDisplayControl(GenericDataPanel dataPanel) {
      if (!dataPanel.CompleteDisplayControl.Visible)
        dataPanel.Show();
      dataPanel.CompleteDisplayControl.Location = new Point(dataPanel.Location.X, dataPanel.Location.Y + dataPanel.Height); //TODO consider font size      
      dataPanel.RefreshDisplayValue();
      this.Controls.Add(dataPanel.CompleteDisplayControl);
    }

    public void OnCompleteDisplayControlAcquired(object sender, EventArgs e) {
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      if (dataPanel == null || dataPanel.CompleteDisplayControl == null)
        return;
      if (!this.Controls.Contains(dataPanel.CompleteDisplayControl) && dataPanel.GenPanelShowStyle == GenericDataPanelShowStyle.Complete)
        addCompleteDisplayControl(dataPanel);
      //dataPanel.CompleteDisplayControl.SendToBack();
    }
    
    public void OnNullingCompleteDisplayControl(object sender, EventArgs e) {
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      if (dataPanel == null || dataPanel.CompleteDisplayControl == null)
        return;
      this.Controls.Remove(dataPanel.CompleteDisplayControl);
    }

    public void OnGenPanelShowStyleChanged(object sender, EventArgs e) { //Triggered by each GenericDataPanel in this page manager
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      if (dataPanel == null || !genericDataPanelList.Contains(dataPanel))
        return;
      switch (dataPanel.GenPanelShowStyle){ //The important part to handle the event to call this function
      case GenericDataPanelShowStyle.Complete:
        if (dataPanel.CompleteDisplayControl != null && !this.Controls.Contains(dataPanel.CompleteDisplayControl) && dataPanel.GenPanelShowStyle == GenericDataPanelShowStyle.Complete)
          addCompleteDisplayControl(dataPanel);
        //dataPanel.CompleteDisplayControl.SendToBack();
        break;
      case GenericDataPanelShowStyle.Compact:
        if (dataPanel.CompleteDisplayControl != null && this.Controls.Contains(dataPanel.CompleteDisplayControl))
          this.Controls.Remove(dataPanel.CompleteDisplayControl);
        break;
      }
    }
    #endregion other events

    #region mouse events
    private bool wheelIsFromParent = false;
    public void OnParentMouseWheel(MouseEventArgs e) {
      if (isMoved) { //if parent want to trigger move wheel, it can only be done when the child has not triggered the event
        isMoved = false;
        return; //does not follow parent if it is already moved once by the child for the same wheel
      }
      wheelIsFromParent = true;
      OnMouseWheel(e);
    }

    private Point mouseDownLocation;
    private bool isMovingObjects = false;
    public void OnMobilityChanged(object sender, EventArgs e) {
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      if (dataPanel == null)
        return;
      if (dataPanel.IsMoveable && !movablePanelList.Contains(dataPanel))
        movablePanelList.Add(dataPanel);
      if (!dataPanel.IsMoveable && movablePanelList.Contains(dataPanel))
        movablePanelList.Remove(dataPanel);
    }

    private void adjustDataPanelLayer(GenericDataPanel dataPanel) { //assuming data panel is not null, and it is movable
      if (dataPanel.Location.Y + dataPanel.Height < executionPanel.Location.Y && dataPanel.Location.Y > optionsPanel.Location.Y + optionsPanel.Height)
        dataPanel.BringToFront();
      else
        dataPanel.SendToBack();
    }

    private void adjustCompleteDisplayLayer(Control completeDisplayControl) {
      if (completeDisplayControl.Location.Y + completeDisplayControl.Height < executionPanel.Location.Y &&
        completeDisplayControl.Location.Y > optionsPanel.Location.Y + optionsPanel.Height)
        completeDisplayControl.BringToFront();
      else
        completeDisplayControl.SendToBack();
    }

    private void validateDataPanelPosition(GenericDataPanel dataPanel) {
      bool beyond_x_boundary = dataPanel.Left < -0.5 * dataPanel.Width || dataPanel.Left > this.Width - 0.5 * dataPanel.Width; //must be always executed, regardless whether x or y changed or not, otherwise = bug
      bool beyond_y_boundary = dataPanel.Top < optionsPanel.Height - 0.5 * dataPanel.Height || dataPanel.Top > this.Height - executionPanel.Height - 0.5 * dataPanel.Height;
      dataPanel.MarkAsRemoved = beyond_x_boundary || beyond_y_boundary;
    }

    private void adjustDataPanelLeft(GenericDataPanel dataPanel, int x) {
      dataPanel.Left += x;
      adjustDataPanelLayer(dataPanel);
      if (dataPanel.CompleteDisplayControl != null) {
        dataPanel.CompleteDisplayControl.Left += x;
        adjustCompleteDisplayLayer(dataPanel.CompleteDisplayControl);
      }
    }

    private void adjustDataPanelTop(GenericDataPanel dataPanel, int y) {
      dataPanel.Top += y;
      adjustDataPanelLayer(dataPanel);
      if (dataPanel.CompleteDisplayControl != null) {
        dataPanel.CompleteDisplayControl.Top += y;
        adjustCompleteDisplayLayer(dataPanel.CompleteDisplayControl);
      }
    }

    private void gridAdjustingDataPanel(GenericDataPanel dataPanel) {
      int grid = optionsPanel.GridValue;
      int excess_x = dataPanel.Left % grid;
      int excess_y = dataPanel.Top % grid;
      float limit = grid / 2;
      if (excess_x != 0)
        adjustDataPanelLeft(dataPanel, excess_x >= limit ? grid - excess_x : -excess_x); //negative sign must be put      
      if (excess_y != 0)
        adjustDataPanelTop(dataPanel, excess_y >= limit ? grid - excess_y : -excess_y); //negative sign must be put      
    }

    private void registerDataPanel(GenericDataPanel dataPanel) { //registration also means positioning
      gridAdjustingDataPanel(dataPanel); //should not be done before real place occur!
      validateDataPanelPosition(dataPanel);
      dataPanel.IsRegistered = true; //If data panel is moved, it is considered "registered" no matter what the previous state is
    }
    
    public void OnCbMouseMoveOnMovable(object sender, MouseEventArgs e) { //now, this does not make a panel registered
      if (e.Button != MouseButtons.Left || !isMovingObjects || movablePanelList == null || movablePanelList.Count <= 0)
        return;
      //Control parent = movablePanelList[0].Parent;
      //DrawingControl.SuspendDrawing(parent);
      foreach (GenericDataPanel dataPanel in movablePanelList) { //TODO Change everything then draw at once! to reduce the flickering effect!
        if (dataPanel == null || !dataPanel.IsMoveable || dataPanel.MouseDownLocation == null)
          continue;
        int x_changed = ((e.X - mouseDownLocation.X) / optionsPanel.GridValue) * optionsPanel.GridValue;
        int y_changed = ((e.Y - mouseDownLocation.Y) / optionsPanel.GridValue) * optionsPanel.GridValue;
        if (x_changed != 0)
          adjustDataPanelLeft(dataPanel, x_changed);
        if (y_changed != 0)
          adjustDataPanelTop(dataPanel, y_changed);
        validateDataPanelPosition(dataPanel);
      }
      //If any panel is out of bounds, must be removed from both: the movable panels list and the genericDataPanels list
      //DrawingControl.ResumeDrawing(parent);
    }

    public void OnMouseClickOnPanelModeMovable(object sender, MouseEventArgs e) {
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      List<GenericDataPanel> subList = movablePanelList.FindAll(x => x != sender && x.GenPanelMode != dataPanel.GenPanelMode);
      for (int i = 0; i < subList.Count; ++i)
        subList[i].GenPanelMode = dataPanel.GenPanelMode;
    }

    public void OnMouseClickOnPanelShowStyleMovable(object sender, MouseEventArgs e) {
      GenericDataPanel dataPanel = sender as GenericDataPanel;
      List<GenericDataPanel> subList = movablePanelList.FindAll(x => x != sender && x.GenPanelShowStyle != dataPanel.GenPanelShowStyle);
      for (int i = 0; i < subList.Count; ++i)
        subList[i].GenPanelShowStyle = dataPanel.GenPanelShowStyle;
    }

    public void OnCbMouseDownOnMovable(object sender, MouseEventArgs e) {
      if (!isMovingObjects) { //first time
        mouseDownLocation = e.Location;
        isMovingObjects = true;
      }
    }

    private void removeDataPanel (GenericDataPanel dataPanel) {
      if (dataPanel.CompleteDisplayControl != null) //if there is complete display control "attached" remove the display control first
        this.Controls.Remove(dataPanel.CompleteDisplayControl);
      movablePanelList.Remove(dataPanel);
      genericDataPanelList.Remove(dataPanel);
      this.Controls.Remove(dataPanel);
    }

    private void processMouseUp() { //consists of registration and removal
      isMovingObjects = false;
      int i = 0;
      while (i < movablePanelList.Count) { //remove everything marked as removed
        GenericDataPanel dataPanel = movablePanelList[i];
        if (dataPanel.MarkAsRemoved) {
          removeDataPanel(dataPanel);
        } else { //register the panel
          registerDataPanel(dataPanel);
          ++i;
        }
      }
    }

    public void OnCbMouseUpOnMovable(object sender, MouseEventArgs e) {
      processMouseUp();
    }

    protected override void OnMouseUp(MouseEventArgs e) { //To check if the mouseUp event will also be triggered here or not! TODO check for mouseUp event on a GenericDataPanel instead of here!
      processMouseUp();
      base.OnMouseUp(e);
    }

    public void OnCbMouseClickOnBoxOnMovable(object sender, MouseEventArgs e) { //this means the checked condition has been changed! the sender is the checkbox
      RainbowCheckBox rcb = sender as RainbowCheckBox;
      if (rcb == null)
        return;
      GenericDataPanel dataPanel = rcb.Parent as GenericDataPanel; //this must be true, given the condition
      if (dataPanel == null)
        return;
      List<GenericDataPanel> subList = movablePanelList.FindAll(x => x != dataPanel && x.CheckBoxChecked != dataPanel.CheckBoxChecked);
      for (int i = 0; i < subList.Count; ++i)
        subList[i].CheckBoxChecked = dataPanel.CheckBoxChecked;
    }

    private const int WHEEL_STEP = 1; //best is 1
    private const int WHEEL_FROM_PARENT_OFFSET = 24; //ideally, this is of the same size with the genericDataPanel (checkbox) size, buf offset of 5 is expected
    private bool isMoved = false; //to avoid multiple calls: check this flag first, drop down by the parent whenever 
    protected override void OnMouseWheel(MouseEventArgs e) { //Mouse wheel, when it is "wheeled" it will take its nearest "immovable" item to "movable"
      isMoved = false; //child can always respond to the request to move wheel
      if (e.Delta > 0) { // scroll up
        Point point = e.Location;
        int originalY = point.Y;
        int offset = wheelIsFromParent ? WHEEL_FROM_PARENT_OFFSET : 0; //TODO WHEEL_FROM_PARENT_OFFSET should depend on the fontsize
        point.Y = this.Height; //get from the bottom most!
        while (point.Y >= originalY - offset) {
          GenericDataPanel dataPanel = GetChildAtPoint(point) as GenericDataPanel;
          if (dataPanel == null) {
            point.Y -= WHEEL_STEP; //substract by WHEEL_STEP, should be enough for now, but TODO subjected to fontsize actually...
            continue;
          }
          if (dataPanel.IsMoveable) {
            dataPanel.SetMovability(false, e);
            break;
          }
          point.Y -= WHEEL_STEP; //substract by WHEEL_STEP, should be enough for now, but TODO subjected to fontsize actually...
        }
      } else if (e.Delta < 0) { // scroll down
        //TODO by default, WHEEL_FROM_PARENT_OFFSET is subjected to font size!
        Point point = wheelIsFromParent ? new Point(e.Location.X, e.Location.Y - WHEEL_FROM_PARENT_OFFSET) : e.Location;
        while (point.Y < this.Height + WHEEL_FROM_PARENT_OFFSET) { //WHEEL_FROM_PARENT_OFFSET is a buffer TODO, ideally subjected to fontsize
          GenericDataPanel dataPanel = GetChildAtPoint(point) as GenericDataPanel;
          if (dataPanel == null) {
            point.Y += WHEEL_STEP; //add by 15, should be enough for now, but TODO subjected to fontsize actually...
            continue;
          }
          if (!dataPanel.IsMoveable) {
            dataPanel.SetMovability(true, e);
            break;
          }
          point.Y += WHEEL_STEP; //add by 15, should be enough for now, but TODO subjected to fontsize actually...
        }
      }
      wheelIsFromParent = false; //always turn to false afterwards
      isMoved = true;
      //base.OnMouseWheel(e); //TODO Does not seem to be working by default, maybe a condition needs to be first satisfied
    }

    protected override void OnMouseClick(MouseEventArgs e) { //On mouseclick, all the "movable" items are set back to "immovable"
      base.OnMouseClick(e);
      if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
        return;
      foreach (Control ctrl in this.Controls) {
        GenericDataPanel dataPanel = ctrl as GenericDataPanel;
        if (dataPanel != null && dataPanel.IsMoveable)
          dataPanel.SetMovability(false, e);
      }
    }
    #endregion mouse events

    #region options events
    void optionsPanel_FilterUpdated(object sender, EventArgs e) {
      if (GenericDataList == null || GenericDataList.Count <= 0)
        return;

      List<GenericDataPanel> subDataPanel = genericDataPanelList.FindAll(x => !x.IsRegistered); //will return empty List<T> (not null) when failed
      movablePanelList.RemoveAll(x => !x.IsRegistered);
      genericDataPanelList.RemoveAll(x => !x.IsRegistered);
      for (int i = 0; i < subDataPanel.Count; ++i) {
        if (subDataPanel[i].CompleteDisplayControl != null && this.Controls.Contains(subDataPanel[i].CompleteDisplayControl))
          this.Controls.Remove(subDataPanel[i].CompleteDisplayControl);
        this.Controls.Remove(subDataPanel[i]);
      }
      RefreshAllBorderColor();

      if (string.IsNullOrWhiteSpace(optionsPanel.FilterText))
        return;

      List<GenericData> subList = GenericDataList.FindAll(x => x.DataName.ToLower().Contains(optionsPanel.FilterText.ToLower()));
      int heightNow = 0;
      for (int i = 0; i < subList.Count; ++i) {
        bool result = AddGenericDataPanel(subList[i].DataName, 2, optionsPanel.Height + heightNow);
        heightNow += result ? 20 : 0; //TODO consider font size
        AttachGenericData(subList[i].DataName, subList[i]);
      }

      List<GenericDataPanel> preExistingPanels = genericDataPanelList.FindAll(x => x.Name.ToLower().Contains(optionsPanel.FilterText.ToLower()));
      for (int i = 0; i < preExistingPanels.Count; ++i)
        AddGenericDataPanel(preExistingPanels[i].Name); //basically, just show
    }

    void optionsPanel_ShowStyleChanged(object sender, EventArgs e) { //either complete or compact
      List<GenericDataPanel> subList = genericDataPanelList.FindAll(x => x.GenPanelShowStyle != optionsPanel.ShowStyle);
      for (int i = 0; i < subList.Count; ++i)
        subList[i].GenPanelShowStyle = optionsPanel.ShowStyle;
      this.Focus();
    }

    void optionsPanel_ClearClicked(object sender, EventArgs e) { //Data in the current mode of the respective GenericDataBuffer mode will be cleared
      List<GenericDataPanel> hasReadList = genericDataPanelList.FindAll(x => !string.IsNullOrWhiteSpace(x.GenData.DataReadBuffer) 
        && (x.GenPanelMode == GenericDataPanelMode.ReadOnly || x.GenPanelMode == GenericDataPanelMode.ReadAndWrite));
      List<GenericDataPanel> hasWriteList = genericDataPanelList.FindAll(x => !string.IsNullOrWhiteSpace(x.GenData.DataWriteBuffer) 
        && (x.GenPanelMode == GenericDataPanelMode.WriteOnly || x.GenPanelMode == GenericDataPanelMode.ReadAndWrite));
      for (int i = 0; i < hasReadList.Count; ++i) {
        hasReadList[i].GenData.DataReadBuffer = null;
        hasReadList[i].Invalidate();
      }
      for (int i = 0; i < hasWriteList.Count; ++i) {
        hasWriteList[i].GenData.DataWriteBuffer = null;
        hasWriteList[i].Invalidate();
      }
      this.Focus();
    }

    void optionsPanel_AlignmentChanged(object sender, EventArgs e) {
      //TODO there are two ways to treat this: one is by brute search, another is by only focusing on the moving panel(s)
			//This is when the alignment is changed. It is important to get the value of the change
			switch (optionsPanel.ValueAlignment) { //Do something on the collections of the items
				case PageManagerValueAlignment.AfterText:
					break;
				case PageManagerValueAlignment.LongestText:
					break;
				case PageManagerValueAlignment.RightAligned:
					break;
				case PageManagerValueAlignment.TwoLines:
					break;
				default:
					break;
			}

      this.Focus();
    }

    void optionsPanel_FontSizeUpDownChanged(object sender, EventArgs e) {
      this.Focus();
    }

    void optionsPanel_SwitchChanged(object sender, EventArgs e) { //switch between read, r&w, and write
      List<GenericDataPanel> subList = genericDataPanelList.FindAll(x => x.GenPanelMode != optionsPanel.PanelMode);
      for (int i = 0; i < subList.Count; ++i)
        subList[i].GenPanelMode = optionsPanel.PanelMode;
      this.Focus();
    }

    void optionsPanel_GridUpDownChanged(object sender, EventArgs e) { //clearly, the grid is changed, affecting all the registered items
      List<GenericDataPanel> subList = genericDataPanelList.FindAll(x => x.IsRegistered && (x.Left % optionsPanel.GridValue != 0 || x.Top % optionsPanel.GridValue != 0));
      for (int j = 0; j < subList.Count; ++j) {
        gridAdjustingDataPanel(subList[j]);
        validateDataPanelPosition(subList[j]);
        if (subList[j].MarkAsRemoved) //this does not cause the panel to be removed from the subList, but it is ok, because the sublist will eventually be removed too!
          removeDataPanel(subList[j]);
      }
      this.Focus();
    }
    #endregion options events

    #region execution events
    void executionPanel_SendClicked(object sender, EventArgs e) {
      //TODO Adds
      this.Focus();
    }

    void executionPanel_BinCheckBoxClicked(object sender, EventArgs e) {
      //TODO Adds
      this.Focus();
    }

    void executionPanel_BinStyleChanged(object sender, EventArgs e) {
      //TODO Adds
      this.Focus();
    }
    #endregion execution events

  }
}

//this.Left = e.X + this.Left - mouseDownLocation.X;
//this.Top = e.Y + this.Top - mouseDownLocation.Y;
//GenericDataPanel dataPanel
//GenericDataPanel dataPanel = GetChildAtPoint(e.Location) as GenericDataPanel; //The location here is NOT the location of the mouse according to the manager, but according to the sender
//if (dataPanel.IsMoveable) { //falls into "moveable" object
//  mouseDownLocation = new Point(dataPanel.Location.X + e.Location.X, dataPanel.Location.Y + e.Location.Y);
//  isMovingObjects = true;
//}      
//string printthis = dataPanel.Name + ": " + dataPanel.Left.ToString() + " " + dataPanel.Top.ToString() + " "
//  + e.X.ToString() + " " + e.Y.ToString() + " " + dataPanel.MouseDownLocation.X.ToString() + " " + dataPanel.MouseDownLocation.Y.ToString() + "\n";
//printItem = printthis;
////The only moving element here is 
//if (PrintRequest != null)
//  PrintRequest(this, e);
//protected override void OnMouseDown(MouseEventArgs e) { //we cannot have mouseDown in here on top of the checkBox, because it will follow checkBox event
//  base.OnMouseDown(e); //Must check the mousedown event in the checkBox!
//  GenericDataPanel dataPanel = GetChildAtPoint(e.Location) as GenericDataPanel;
//  if (dataPanel == null)
//    return;
//  if (dataPanel.IsMoveable) { //falls into "moveable" object
//    mouseDownLocation = e.Location;
//    isMovingObjects = true;
//  }
//}

//protected override void OnMouseMove(MouseEventArgs e) {      
//  if (e.Button == MouseButtons.Left && isMovingObjects) {
//    foreach (GenericDataPanel dataPanel in movablePanels) {
//      if (dataPanel != null && dataPanel.IsMoveable && dataPanel.MouseDownLocation != null) {
//        dataPanel.Left = e.X + dataPanel.Left - dataPanel.MouseDownLocation.X;
//        dataPanel.Top = e.X + dataPanel.Top - dataPanel.MouseDownLocation.Y;
//      }
//    }
//    //this.Left = e.X + this.Left - mouseDownLocation.X;
//    //this.Top = e.Y + this.Top - mouseDownLocation.Y;
//  }
//  base.OnMouseMove(e);
//}

