using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

using Extension.Controls;

using Oracle.ManagedDataAccess.Client;

namespace Extension.Database.OldOracle {
	public partial class OracleMultipleTableViewersForm : Form {
		OracleHandler oracleHandler = new OracleHandler();

		//Typical initialization
		private string root;

		//Directories and files
		private string configFoldername = "configs";
		private string dbConnectionFilename = "dbconnection";

		//Drawing
		private Bitmap green_indicator, red_indicator, gray_indicator, seablue_indicator;

		//Connections
		private OracleConnectionSettings dbConnectionSettings = null;

		public OracleMultipleTableViewersForm() {
			InitializeComponent();

			//Directories
			root = Application.StartupPath;
			if (!Directory.Exists(root + "\\" + configFoldername))
				Directory.CreateDirectory(root + "\\" + configFoldername); //configuraton directory...

			//Initialize drawings and pictures
			try {
				gray_indicator = Properties.Resources.gray_indicator;
				red_indicator = Properties.Resources.red_indicator;
				green_indicator = Properties.Resources.green_indicator;
				seablue_indicator = Properties.Resources.seablue_indicator;
				pictureBoxConnection.Image = gray_indicator;
			} catch {
			}

			//Initialization variables
			FileStream filestream = null;
			string folderpath = root + "\\" + configFoldername;
			XmlSerializer serializerObj;

			try {
				filestream = new FileStream(folderpath + "\\" + dbConnectionFilename + ".xml", FileMode.Open, FileAccess.Read, FileShare.Read);
				serializerObj = new XmlSerializer(typeof(OracleConnectionSettings));
				dbConnectionSettings = (OracleConnectionSettings)serializerObj.Deserialize(filestream);
				filestream.Close();
				textBoxDBDataSource.Text = dbConnectionSettings.DataSource;
				textBoxDBUserID.Text = dbConnectionSettings.UserId;
				textBoxDBPassword.Text = dbConnectionSettings.Password;
				if (dbConnectionSettings.AutoConnect)
					connect();
			} catch {
				if (filestream != null)
					filestream.Close();
			}
		}

		List<ComboBox> comboBoxes = new List<ComboBox>();
		List<string> sources = new List<string>();

		void linkLabelFrom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			SingleTableSpecifierPanel panel = (sender as Control).Parent as SingleTableSpecifierPanel;
			if (panel.IncludeCount <= 0) //no item in the include, put everything!
				panel.SetColumns(oracleHandler.GetAllColumnNames(panel.TableName), null);
			OracleFromForm form = new OracleFromForm(panel.IncludeColumns, panel.ExcludeColumns, string.Concat(" [", panel.SpecifierId, "] ", panel.TableName));
			if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) { //Do something
				panel.SetColumns(form.IncludedColumns, form.ExcludedColumns); //record the new included and excluded
				panelControlDisplayHandler(sender as Control); //update the display control (if there is any)
			}
		}

		void linkLabelRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			panelControlDisplayHandler(sender as Control);
		}

		void checkBox_CheckedChanged(object sender, EventArgs e) {
			panelControlDisplayHandler(sender as Control);
		}

		void comboBox_SelectedIndexChanged(object sender, EventArgs e) { //The only thing which would cause the include and exclude to be reset		
			SingleTableSpecifierPanel panel = (sender as Control).Parent as SingleTableSpecifierPanel;
			panel.ResetColumns();
			panelControlDisplayHandler(sender as Control);
		}

		void panelControlDisplayHandler(Control ctrl) {
			if (!oracleHandler.IsConnectionUp())
				return;
			displayControl(ctrl.Parent as SingleTableSpecifierPanel);
		}

		List<OracleTableViewForm> oracleForms = new List<OracleTableViewForm>();
		private void displayControl(SingleTableSpecifierPanel panel) {
			if (string.IsNullOrWhiteSpace(panel.TableName)) //not found
				return;
			OracleTableViewForm form = oracleForms
				.Where(x => x.Name == string.Concat("oracleForm", panel.SpecifierId))
				.FirstOrDefault();
			if (!panel.ViewChecked) {
				if (form != null) //while unchecked, only needs to do something when it is found
					form.Hide();
				return;
			}
			
			if (form == null) { //form does not exist, create a new form
				form = new OracleTableViewForm();				
				form.Name = string.Concat("oracleForm", panel.SpecifierId);
				form.SetOracleHandler(oracleHandler);
				form.FormClosing += form_FormClosing;
				oracleForms.Add(form);
			}
			form.Text = string.Concat(form.OriginalTitle, " [", panel.SpecifierId, "] ", panel.TableName);
			form.DisplayTableResult(panel.TableName, panel.ColumnsClause, panel.WhereClause, panel.MaxRows, panel.IndexAdded);
			form.Show();
		}

		void form_FormClosing(object sender, FormClosingEventArgs e) {
			Form form = sender as Form;
			form.Hide();
			e.Cancel = true;
			string numstr = form.Name.Replace("oracleForm", string.Empty);
			SingleTableSpecifierPanel panel = specifiers[Convert.ToInt32(numstr) - 1];
			panel.ViewChecked = false;
		}

		private bool populateSources() {
			OracleConnection conn = oracleHandler.GetConnection();
			OracleCommand cmd;
			string[] seltext = { "table_name", "view_name" };
			string[] fromtext = { "all_tables", "all_views" };

			specifiers.ForEach(panel => this.Controls.Remove(panel));
			specifiers.Clear();
			sources.Clear();
			comboBoxes.ForEach(cb => cb.Items.Clear());
			comboBoxes.Clear();

			try {
				for (int i = 0; i < seltext.Length; ++i) {
					cmd = new OracleCommand("select " + seltext[i] +
						" from " + fromtext[i] +
						" where owner = '" + oracleHandler.GetConnectionSettings().UserId + "'"
						, conn);
					using (OracleDataReader reader = cmd.ExecuteReader()) {
						if (reader == null)
							return false; //at this moment, do not return error					
						while (reader.Read())
							sources.Add(reader.GetString(0));
					}
				}
			} catch { //TODO do something later on
				return false;
			}
			sources.Sort();

			int tableNo = (int)numericUpDownTableNumber.Value;
			for (int i = 0; i < tableNo; ++i)
				addSpecifier();

			return true;
		}

		private OracleConnectionSettings getDBConnectionSettingsFromGUI() {
			bool autoConnect = dbConnectionSettings != null && dbConnectionSettings.AutoConnect;
			OracleConnectionSettings settings = new OracleConnectionSettings();
			settings.DataSource = textBoxDBDataSource.Text;
			settings.UserId = textBoxDBUserID.Text;
			settings.Password = textBoxDBPassword.Text;
			settings.AutoConnect = autoConnect;
			dbConnectionSettings = settings;
			return settings;
		}

		private void updateControls(bool isConnected) {
			pictureBoxConnection.Image = isConnected ? green_indicator : red_indicator;
			labelDBConnStat.Text = isConnected ? "Connected" : "Disconnected";
			textBoxDBDataSource.Enabled = !isConnected;
			textBoxDBPassword.Enabled = !isConnected;
			textBoxDBUserID.Enabled = !isConnected;
			buttonConnect.Text = isConnected ? "Disconnect" : "Connect";
			int tableNo = isConnected ? (int)numericUpDownTableNumber.Value : 0;
			this.MaximumSize = new Size(this.Width, 76 + 34 * tableNo);
			this.Height = 76 + 34 * tableNo;
			specifiers.ForEach(sp => sp.Enabled = isConnected);
		}

		private void connect() {
			bool result = true;
			if (oracleHandler.IsConnectionUp()) {
				oracleHandler.CloseConnection();
				int tableNo = specifiers.Count;
				for (int i = 0; i < tableNo; ++i)
					removeSpecifier();
			} else {
				oracleHandler.OpenConnection(getDBConnectionSettingsFromGUI());
				result = populateSources();
			}
			if (result)
				updateControls(oracleHandler.IsConnectionUp()); //the latest status determine...
		}

		private void buttonConnect_Click(object sender, EventArgs e) {
			connect();
		}

		List<SingleTableSpecifierPanel> specifiers = new List<SingleTableSpecifierPanel>();
		private void addSpecifier() {
			int tableNo = specifiers.Count;

			SingleTableSpecifierPanel panel = new SingleTableSpecifierPanel();
			panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panel.IndexAdded = false;
			panel.Location = new System.Drawing.Point(0, 36 + tableNo * 34);
			panel.MaxRows = 200;
			panel.Name = string.Concat("singleTableSpecifierPanel", tableNo);
			panel.Size = new System.Drawing.Size(819, 35);
			panel.SpecifierId = specifiers.Count + 1;
			panel.TabIndex = 34;
			panel.ViewChecked = false;
			panel.WhereClause = "";
			specifiers.Add(panel);

			ComboBox cb = panel.Controls.Cast<Control>()
					.Where(y => y is ComboBox)
					.First() as ComboBox;
			cb.SelectedIndexChanged += comboBox_SelectedIndexChanged;			
			if (sources != null && sources.Count > 0) {
				cb.Items.AddRange(sources.ToArray());
				cb.SelectedIndex = 0;
			}
			comboBoxes.Add(cb);

			CheckBox chbView = panel.Controls.Cast<Control>()
					.Where(y => y is CheckBox && y.Name.StartsWith("checkBoxView"))
					.First() as CheckBox;
			chbView.CheckedChanged += checkBox_CheckedChanged;

			CheckBox chbAddIndex = panel.Controls.Cast<Control>()
					.Where(y => y is CheckBox && y.Name.StartsWith("checkBoxAddIndex"))
					.First() as CheckBox;
			chbAddIndex.CheckedChanged += checkBox_CheckedChanged;

			LinkLabel llRefresh = panel.Controls.Cast<Control>()
					.Where(y => y is LinkLabel && y.Name.StartsWith("linkLabelRefresh"))
					.First() as LinkLabel;
			llRefresh.LinkClicked += linkLabelRefresh_LinkClicked;

			LinkLabel llFrom = panel.Controls.Cast<Control>()
					.Where(y => y is LinkLabel && y.Name.StartsWith("linkLabelFrom"))
					.First() as LinkLabel;
			llFrom.LinkClicked += linkLabelFrom_LinkClicked;

			this.Controls.Add(panel);

			updateControls(oracleHandler.IsConnectionUp());
		}

		private void removeSpecifier() {
			if (specifiers.Count == 0)
				return;

			SingleTableSpecifierPanel panel = specifiers[specifiers.Count - 1];

			OracleTableViewForm form = oracleForms
				.Where(x => x.Name == string.Concat("oracleForm", panel.SpecifierId))
				.FirstOrDefault();
			if (form != null && form.Visible)
				form.Hide();

			ComboBox cb = panel.Controls.Cast<Control>()
					.Where(y => y is ComboBox)
					.First() as ComboBox;

			this.Controls.Remove(panel);
			specifiers.Remove(panel);
			comboBoxes.Remove(cb);

			updateControls(oracleHandler.IsConnectionUp());
		}

		private void numericUpDownTableNumber_ValueChanged(object sender, EventArgs e) {
			if (!oracleHandler.IsConnectionUp())
				return;
			if (specifiers.Count > (int)numericUpDownTableNumber.Value)
				removeSpecifier();
			else
				addSpecifier();
		}

		//TODO add serialization if necessary
		//Skills to pick up, multiple/complex XML classes serialization
		//#region serializable
		//[Serializable()]
		//public class AppSettings {
		//	//[XmlArrayItem("EquipmentID", typeof(string))]
		//	//public List<string> EquipmentIDList = new List<string>();
		//	public bool SaveWhenClosing = false;
		//	public bool LoadWhenOpening = false;
		//	public bool SaveBeforeRemoved = false;
		//	public bool LoadWhenAdded = false;
		//	public int NoOfTables = 5;
		//	//Has list of specifiers, each specifier has number, tablename, include, exclude, view, index, where clause
		//	//public int SearchParallelMax = 1;
		//	//public bool ShowLogBox = false;
		//	//public bool PCUsed = false;
		//	//public bool LockTable = false;
		//}
		//#endregion
	}
}
