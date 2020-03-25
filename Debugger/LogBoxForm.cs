using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Extension.Debugger
{
  public partial class LogBoxForm : Form
  {
    private DateTime startDt = DateTime.Now;
    public static string LogFilenameFormat = "yyyyMMdd_HHmmss_fff";
		public LogBoxForm(bool showLogBox = false, string streamRootFolder = null, string logFilenameFormat = null) {
			InitializeComponent();
			initComponent();
			for (int i = 0; i < NO_OF_STOPWATCH; ++i) {
				Stopwatch stopWatch = new Stopwatch();
				stopWatchList.Add(stopWatch);
			}
			for (int i = 0; i < NO_OF_STOPWATCH; ++i)
				stopWatchList[i].Start();
			this.Visible = showLogBox;
      this.streamRootFolder = streamRootFolder;
      if (!(string.IsNullOrEmpty(logFilenameFormat)))
        LogFilenameFormat = logFilenameFormat;
		}

		#region entry criteria

		private class EntryCriteria {
      public List<Color> EntryColorList; //to determine what color list can write message
      public List<Color> NonEntryColorList; //to determine what color list is excluded from writing a message. Non-entry takes precedence over entry
      public bool MustCallByName; //to determine if the richTextBox can only be filled when it is called by name. If it is not filled, then when no name or other name is put it *may* respond (read next flag)
      public bool ExcludedWhenOtherIsCalled; //to determine the behavior of the richTextBox when somebody else is called. If this is true, when somebody else is called, this will not respond. But if no one is specifically called, this assume itself called if MustCallByName is false.
      public EntryCriteria(List<Color> entryColorList = null, List<Color> nonEntryColorList = null, bool mustCallByName = false, bool excludedWhenOtherIsCalled = true) {
        EntryColorList = entryColorList;
        NonEntryColorList = nonEntryColorList;
        MustCallByName = mustCallByName;
        ExcludedWhenOtherIsCalled = excludedWhenOtherIsCalled;
      }

      private bool isCallable(string pageName, string callName) {
        bool isCalledByName = !string.IsNullOrEmpty(pageName) && !string.IsNullOrEmpty(callName) && pageName.ToUpper() == callName.ToUpper();
        bool isGenericCall = string.IsNullOrEmpty(callName); // if this is a generic call, then the callName will be null, empty, or white space
        if (isCalledByName) //if it is called by name, then it is always true
          return true;
        if (!isGenericCall) //if somebody else is called, then it is possible to include or exclude
          return !ExcludedWhenOtherIsCalled; //it is to be excluded when we are to exclude when this is somebody else
        return !MustCallByName; //If it is generic call, and is not your own name, then, we will only write if we can be called by any name. Otherwise, don't write
      }

      private bool isColorAllowed(Color color) {
        if (NonEntryColorList != null && NonEntryColorList.Contains(color)) //If non entry color list is empty or does not contain such color, then we can proceed
          return false; //always reject when it is in the non entry list
        return EntryColorList == null || EntryColorList.Count < 1 || EntryColorList.Contains(color); //only writes if no entry color list (all is allowed) or it is within the entry color allowed
      }

      public bool IsWritable(string pageName, string callName, Color color) {
        return isCallable(pageName, callName) && isColorAllowed(color);
      }
    }

    private class EntrySet
    {
      public EntryCriteria Criteria;
      public RichTextBox WriteBox;
      public CheckBox CheckBoxScrollToCaret;
      public CheckBox StreamCheckBox;
      public string LogFilename;
      public EntrySet(EntryCriteria criteria = null, RichTextBox writeBox = null, CheckBox checkBoxScrollToCaret = null, 
        CheckBox streamCheckBox = null, string logFilename = null) {
        Criteria = criteria;
        WriteBox = writeBox;
        CheckBoxScrollToCaret = checkBoxScrollToCaret;
        StreamCheckBox = streamCheckBox;
        LogFilename = logFilename;
      }
    }

		#endregion entry criteria

		#region timing

		private const int NO_OF_STOPWATCH = 30;
    List<Stopwatch> stopWatchList = new List<Stopwatch>();
    private long[] elapsedTimeLast = new long[NO_OF_STOPWATCH];
    public string GetTimeLapse(int index) {
      long elapsedTimeValue = GetTimeLapseValue(index);
      return elapsedTimeValue >= 0 ? elapsedTimeValue.ToString() + " ms" : "STOPWATCH_IS_NOT_FOUND";
    }

    public string GetTimeLapse() {
      return GetTimeLapse(0);
    }

    public long GetTimeLapseValue(int index) {
      if (index >= 0 && index < NO_OF_STOPWATCH) {
        long elapsedTimeNow = stopWatchList[index].ElapsedMilliseconds;
        long elapsedTime = elapsedTimeNow - elapsedTimeLast[index];
        elapsedTimeLast[index] = elapsedTimeNow;
        return elapsedTime;
      } else
        return -1;
    }

    public long GetTimeLapseValue() {
      return GetTimeLapseValue(0);
    }

		public delegate void WriteTimeLapseDelegate(string logString, string callName = "");
		public delegate void WriteTimeLapseColoredDelegate(string logString, Color color, string callName = "");

		public void WriteTimeLapse(int no = 0, bool timed = true, bool lined = true, string callName = "") {
			string logString = "The elapsed time is: " + GetTimeLapse(no);
			WriteTimeLapseDelegate[] writers = { WriteLog, WriteLogLine, WriteTimedLog, WriteTimedLogLine };
			writers[(timed ? 2 : 0) + (lined ? 1 : 0)](logString, callName);
		}

		public void WriteTimeLapse(Color color, int no = 0, bool timed = true, bool lined = true, string callName = "") {
			string logString = "The elapsed time is: " + GetTimeLapse(no);
			WriteTimeLapseColoredDelegate[] writers = { WriteLog, WriteLogLine, WriteTimedLog, WriteTimedLogLine };
			writers[(timed ? 2 : 0) + (lined ? 1 : 0)](logString, color, callName);
		}

		#endregion timing

		#region behavior and init

		private bool preventClosing = true; //by default, this is true for logBox
		public bool PreventClosing { get { return preventClosing; } set { preventClosing = value; } }

		private const int CP_NOCLOSE_BUTTON = 0x200;
		protected override CreateParams CreateParams { //to make this unable to be closed
			get {
				if (PreventClosing) {
					CreateParams myCp = base.CreateParams;
					myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
					return myCp;
				}
				return base.CreateParams;
			}
		}

    private string streamRootFolder;

    string[] rtb_names = new string[] { "All", "Error", "Others", "External" };    
    TabControl tabControl = new TabControl(); //the main tab control log, inside there are appropriate button and combobox
    List<EntryCriteria> rtb_criteria_list = new List<EntryCriteria>() {
      new EntryCriteria( //for "All"
        entryColorList: new List<Color>() {Color.Red, Color.Green, Color.Blue, RichTextBox.DefaultForeColor}),
      new EntryCriteria( //for "Error"
        entryColorList: new List<Color>() {Color.Red}),
      new EntryCriteria( //for "Others"
        entryColorList: null,
        nonEntryColorList: new List<Color>() {Color.Red, Color.Green, Color.Blue, RichTextBox.DefaultForeColor}),
      new EntryCriteria( //for "External"
        entryColorList: null, nonEntryColorList: null, mustCallByName: true)
    };
    Dictionary<string, EntrySet> entrySetDict = new Dictionary<string, EntrySet>(); //this is tied to rtb_names     
    string tabPageNameHeader = "tabPageLog";
    int tabPageNameHeaderLength = 0; //initialize as zero but will follow tabPageNameHeader later
    private void initComponent() {
      //Prepare tab control
      tabControl.Location = new System.Drawing.Point(0, 0);
      tabControl.Name = "tabControlLog";
      tabControl.SelectedIndex = 0;
      tabControl.Size = new System.Drawing.Size(1095, 568);
      tabControl.TabIndex = 9;
			tabControl.Dock = DockStyle.Fill;

      //Prepare color list
      ArrayList ColorList = new ArrayList();
      Type colorType = typeof(Color);
      PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static |
                                    BindingFlags.DeclaredOnly | BindingFlags.Public);
			string[] colors_string = new string[propInfoList.Length];
			for (int i = 0; i < propInfoList.Length; ++i)
				colors_string[i] = propInfoList[i].Name;

      //Prepare tabPageHeader length
      tabPageNameHeaderLength = tabPageNameHeader.Length;

      //Creates common controls: buttons, label, checkbox
      for (int i = 0; i < rtb_names.Length; ++i) { //TODO may need to change this to a panel, then simply put this wherever we want
        //Creates tab page
        TabPage tabPage = new TabPage();
        tabPage.Location = new Point(4, 22);
        tabPage.Name = tabPageNameHeader + rtb_names[i];
        tabPage.Padding = new Padding(3);
        tabPage.Size = new Size(1087, 542);
        tabPage.TabIndex = 0;
        tabPage.Text = rtb_names[i];
        tabPage.UseVisualStyleBackColor = true;

				SplitContainer splitContainer = new SplitContainer();
				splitContainer.Dock = DockStyle.Fill;
				splitContainer.Orientation = Orientation.Horizontal;
				splitContainer.IsSplitterFixed = true;
				splitContainer.FixedPanel = FixedPanel.Panel2;
				tabPage.Controls.Add(splitContainer);
				splitContainer.SplitterDistance = splitContainer.Size.Height - 40;

        //Creates richTextBoxt
        RichTextBox richTextBox = new RichTextBox();
        richTextBox.BackColor = Color.White;
        richTextBox.ForeColor = Color.DarkBlue;
        richTextBox.Location = new Point(6, 6);
        richTextBox.Name = "richTextBox" + rtb_names[i];
        richTextBox.ReadOnly = true;
				//richTextBox.Size = new Size(1065, 490);
        richTextBox.TabIndex = 0;
        richTextBox.Text = "";
				richTextBox.Dock = DockStyle.Fill;
				splitContainer.Panel1.Controls.Add(richTextBox);

				Panel bottomLeftPanel = new Panel();
				bottomLeftPanel.Dock = DockStyle.Left;
				bottomLeftPanel.Width = 300;
				splitContainer.Panel2.Controls.Add(bottomLeftPanel);

				Panel bottomRightPanel = new Panel();
				bottomRightPanel.Dock = DockStyle.Right;
				bottomRightPanel.Width = 240;
				splitContainer.Panel2.Controls.Add(bottomRightPanel);

				//Creates comboBox
        ComboBox comboBoxBackground = new ComboBox();
        comboBoxBackground.FormattingEnabled = true;
        comboBoxBackground.Location = new Point(105, 8);
        comboBoxBackground.Name = "comboBoxBackgroundColor" + rtb_names[i];
        comboBoxBackground.Size = new Size(81, 21);
        comboBoxBackground.TabIndex = 11;
        comboBoxBackground.Items.AddRange(colors_string);
        comboBoxBackground.Text = "White";
        comboBoxBackground.SelectedIndexChanged += new EventHandler(comboBoxBackgroundColor_SelectedIndexChanged);

        Label labelBackgroundColor = new Label();
        labelBackgroundColor.AutoSize = true;
        labelBackgroundColor.Location = new Point(8, 11);
        labelBackgroundColor.Name = "labelBackgroundColor";
        labelBackgroundColor.Size = new Size(92, 13);
        labelBackgroundColor.TabIndex = 13;
        labelBackgroundColor.Text = "Background Color";

        CheckBox checkBoxScrollToCaret = new CheckBox();
        checkBoxScrollToCaret.AutoSize = true; //Creates scroll to Caret
        checkBoxScrollToCaret.Checked = false; //by default don't scroll!
        checkBoxScrollToCaret.Location = new Point(202, 10);
        checkBoxScrollToCaret.Name = "checkBoxScrollToCaret";
        checkBoxScrollToCaret.Size = new Size(92, 17);
        checkBoxScrollToCaret.TabIndex = 6;
        checkBoxScrollToCaret.Text = "Scroll to Caret";
        checkBoxScrollToCaret.UseVisualStyleBackColor = true;

        //Creation of common controls outside of this function may create blinking
        CheckBox streamCheckBox = new CheckBox();
        streamCheckBox.Location = new Point(10, 7);
        streamCheckBox.Name = "checkBoxStream";
        streamCheckBox.Size = new Size(65, 23);
        streamCheckBox.TabIndex = 10;
        streamCheckBox.Text = "Stream";
        streamCheckBox.UseVisualStyleBackColor = true;

        entrySetDict.Add(rtb_names[i], new EntrySet(rtb_criteria_list[i], richTextBox, checkBoxScrollToCaret, streamCheckBox,
          startDt.ToString(LogFilenameFormat))); //EntrySet is created for each page

        Button saveLogButton = new Button(); //common controls are defined outside;
				saveLogButton.Location = new Point(80, 7);
				saveLogButton.Name = "buttonSaveLog";
				saveLogButton.Size = new Size(75, 23);
				saveLogButton.TabIndex = 11;
				saveLogButton.Text = "Save Log";
				saveLogButton.UseVisualStyleBackColor = true;
				saveLogButton.Click += new EventHandler(buttonSaveLog_Click);

				Button clearLogButton = new Button();
				clearLogButton.Location = new Point(160, 7);
				clearLogButton.Name = "buttonClearLog";
				clearLogButton.Size = new Size(75, 23);
				clearLogButton.TabIndex = 12;
				clearLogButton.Text = "Clear Log";
				clearLogButton.UseVisualStyleBackColor = true;
				clearLogButton.Click += new EventHandler(buttonClearLog_Click);

        //Add the created controls to the tabPage
        bottomLeftPanel.Controls.Add(comboBoxBackground);

        //Add common controls - bottom left (purposely duplicated to avoid flickering issue)
				bottomLeftPanel.Controls.Add(labelBackgroundColor);
				bottomLeftPanel.Controls.Add(checkBoxScrollToCaret);

        //Add common controls - bottom right (purposely duplicated to avoid flickering issue)
        bottomRightPanel.Controls.Add(streamCheckBox);
        bottomRightPanel.Controls.Add(saveLogButton);
        bottomRightPanel.Controls.Add(clearLogButton);

        //Add the tab page to the tab control
        tabControl.Controls.Add(tabPage);
      }
      this.Controls.Add(tabControl); //the tabControl is added to the form      
    }
    #endregion behavior and init

    #region write log

    public static int DEFAULT_LINES_LIMIT = 2000;
    public static int DEFAULT_RESET_LINES = 100;
    public int LinesLimit = DEFAULT_LINES_LIMIT;
    public int ResetLines = DEFAULT_RESET_LINES;
    public bool ImplementLinesLimit = true;
    public string TimeDisplayFormat = "yyyy-MM-dd HH:mm:ss.fff";
    public static Color DefaultColor = Color.Blue; //Default color for logging
		public void WriteTimedLogLine(string logString, string callName = "") {
			WriteTimedLog(logString + "\n", callName);
		}

		public void WriteTimedLogLine(string logString, Color color, string callName = "") {
			WriteTimedLog(logString + "\n", color, callName);
		}

		public void WriteTimedLogLines(IEnumerable<string> logs, string callName = "") {
			foreach (string log in logs)
				WriteTimedLogLine(log, callName);
		}

		public void WriteTimedLogLines(IEnumerable<string> logs, Color color, string callName = "") {
			foreach (string log in logs)
				WriteTimedLogLine(log, color, callName);
		}

		public void WriteTimedLog(string logString, string callName = "") {
      WriteLog("[" + DateTime.UtcNow.ToString(TimeDisplayFormat) + " UTC] " + logString, callName);
    }

    public void WriteTimedLog(string logString, Color color, string callName = "") {
      WriteLog("[" + DateTime.UtcNow.ToString(TimeDisplayFormat) + " UTC] " + logString, color, callName);
    }

		public void WriteLogLine(string logString, string callName = "") {
			WriteLog(logString + "\n", callName);
		}

		public void WriteLogLine(string logString, Color color, string callName = "") {
			WriteLog(logString + "\n", color, callName);			
		}

		public void WriteLogLines(IEnumerable<string> logs, string callName = "") {
			foreach (string log in logs)
				WriteLogLine(log, callName);
		}

		public void WriteLogLines(IEnumerable<string> logs, Color color, string callName = "") {
			foreach (string log in logs)
				WriteLogLine(log, color, callName);
		}

		public void WriteLog(string logString, string callName = "") {
      WriteLog(logString, DefaultColor, callName);
    }

    public void WriteLog(string logString, Color color, string callName = "") { //be default, it does not call name
      try { //Try catch just in case it fails
        foreach (TabPage tabPage in tabControl.TabPages) {
          string pageName = tabPage.Name.Substring(tabPageNameHeaderLength);
          EntrySet entrySet = entrySetDict[pageName];
          if (!entrySet.Criteria.IsWritable(pageName, callName, color))
            continue;
          //If stream checkbox is checked
          if (!string.IsNullOrEmpty(streamRootFolder) && entrySet.StreamCheckBox.Checked) { //Do the streaming, but must have the stream root folder too            
            Directory.CreateDirectory(streamRootFolder); //create the stream folder if necessary
            string streamFolder = streamRootFolder + "\\Logs\\" + pageName;
            Directory.CreateDirectory(streamFolder);
            string filepath = streamFolder + "\\" + entrySet.LogFilename + ".txt";
            using (StreamWriter writer = File.AppendText(filepath)) {
              writer.Write(logString);
              writer.Close();
            }
          }

          RichTextBox richTextBox = entrySet.WriteBox;
          richTextBox.SelectionStart = richTextBox.TextLength;
          richTextBox.SelectionLength = 0;
          richTextBox.SelectionColor = color;
          richTextBox.AppendText(logString);
          richTextBox.SelectionColor = richTextBox.ForeColor;
          if (entrySet.CheckBoxScrollToCaret.Checked)
            richTextBox.ScrollToCaret();
          if (ImplementLinesLimit && richTextBox.Lines.Length > LinesLimit) {
            richTextBox.ReadOnly = false;
            richTextBox.Select(0, richTextBox.GetFirstCharIndexFromLine(richTextBox.Lines.Length - ResetLines));
            richTextBox.SelectedText = "";
            richTextBox.ReadOnly = true;
          }
        }
      } catch (Exception e) {
				throw e;
      } 
    }

		#endregion write log

		#region event handlers
    private void comboBoxBackgroundColor_SelectedIndexChanged(object sender, EventArgs e) {
			Control spc = (sender as ComboBox).Parent.Parent.Parent;
			SplitterPanel spcPanel = (spc as SplitContainer).Panel1;
			RichTextBox richTextBox = spcPanel.GetChildAtPoint(new Point(6, 6)) as RichTextBox; //WARNING, get child at is only applicable for selected tab!
      richTextBox.BackColor = Color.FromName((sender as ComboBox).Text);
    }

    private void buttonSaveLog_Click(object sender, EventArgs e) {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.FileName = "";
      saveFileDialog.Title = "Save Log";
      saveFileDialog.Filter = "Text Documents|*.txt";
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {//The rich text box must be obtained from the parent
				Control spc = (sender as Button).Parent.Parent.Parent; //now this is a Button -> Panel -> SplitContainer.Panel -> SplitContainer
				SplitterPanel spcPanel = (spc as SplitContainer).Panel1;
				RichTextBox richTextBox = spcPanel.GetChildAtPoint(new Point(6, 6)) as RichTextBox;
        richTextBox.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
      }
    }

    private void buttonClearLog_Click(object sender, EventArgs e) {
			Control spc = (sender as Button).Parent.Parent.Parent; //now this is a Button -> Panel -> SplitContainer.Panel -> SplitContainer
			SplitterPanel spcPanel = (spc as SplitContainer).Panel1;
			RichTextBox richTextBox = spcPanel.GetChildAtPoint(new Point(6, 6)) as RichTextBox;
      richTextBox.Clear();
		}
    #endregion event handlers

    private void LogBoxForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (PreventClosing) //if closing is to be prevented
        e.Cancel = true; //do not allow the form to be closed using conventional manner
    }
  }
}
