using System;
using System.Windows.Forms;

namespace Extension.Database.OldOracle {
	public partial class OracleConnectionSettingsForm : Form {
		OracleHandler oracleHandler;
		OracleConnectionSettings settings;
		public OracleConnectionSettingsForm(OracleHandler handler, OracleConnectionSettings settings) { //must be constructed with Oracle Connection Settings
			InitializeComponent();
			if (handler == null || settings == null) {
				MessageBox.Show("Invalid Oracle Handler/Settings!", "Error");
				this.Close();
				return;
			}
			oracleHandler = handler;
			this.settings = settings;
			textBoxDBDataSource.Text = settings.DataSource;
			textBoxDBUserID.Text = settings.UserId;
			textBoxDBPassword.Text = settings.Password;
			buttonConnect.Text = AsInfo ? "OK" : oracleHandler.IsConnectionUp() ? "Disconnect" : "Connect";			
		}

		private void updateDBConnectionSettingsFromGUI() {
			bool autoConnect = oracleHandler.IsAutoConnect;
			settings.DataSource = textBoxDBDataSource.Text;
			settings.UserId = textBoxDBUserID.Text;
			settings.Password = textBoxDBPassword.Text;
			settings.AutoConnect = autoConnect;			
		}

		private bool asInfo = false;
		public bool AsInfo { get { return asInfo; }
			set {
				textBoxDBDataSource.Enabled = !value;
				textBoxDBUserID.Enabled = !value;
				textBoxDBPassword.Enabled = !value;
				buttonCancel.Visible = !value;
				if (value)
					buttonConnect.Text = "OK";
				asInfo = value;
			}
		}

		private void buttonConnect_Click(object sender, EventArgs e) {
			try {
				if (AsInfo) {
					Close();
					return;
				}
				if (oracleHandler.IsConnectionUp()) {
					oracleHandler.CloseConnection();
				} else {
					updateDBConnectionSettingsFromGUI();
					oracleHandler.OpenConnection(settings); //get with the password
				}
				DialogResult = DialogResult.OK;
				Close();
			} catch (Exception ex) {
				DialogResult = DialogResult.Abort;
				MessageBox.Show("Connection/Disconnection failed with Exception!\n" + ex.ToString(), "Error");				
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			updateDBConnectionSettingsFromGUI();
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
