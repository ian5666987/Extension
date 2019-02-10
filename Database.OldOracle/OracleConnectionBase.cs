using System;
using System.Data;
using System.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Extension.Database.OldOracle {
	public class OracleConnectionBase {
		protected OracleConnection conn = null;
		public bool IsAutoConnect = false;
		protected string connectionString = "";
		protected OracleConnectionSettings connSettings;

		public bool OpenConnection(OracleConnectionSettings connSettings) {
			return OpenConnection(connSettings.DataSource, connSettings.UserId, connSettings.Password);
		}

		public bool OpenConnection(string dataSource, string userId, string password, bool autoConnect = false) {
			IsAutoConnect = autoConnect;
			return OpenConnection(dataSource, userId, password);
		}

		private void connect(string connString) {
			if (conn != null)
				conn.Dispose(); //probably unmanaged resources
			conn = new OracleConnection(connString);
			conn.Open(); //this should open the connection
			connectionString = connString;
			string[] connElements = connString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			string datasource = connElements.Where(x => x.ToLower().StartsWith("data source")).FirstOrDefault().Split('=')[1];
			string userid = connElements.Where(x => x.ToLower().StartsWith("user id")).FirstOrDefault().Split('=')[1];
			string password = connElements.Where(x => x.ToLower().StartsWith("password")).FirstOrDefault().Split('=')[1];
			connSettings = new OracleConnectionSettings() { DataSource = datasource, UserId = userid, Password = password };
		}

		public bool OpenConnection(string connString) {
			try {
				connect(connString);
				return true;
			} catch (Exception e){
				throw (e);
			}
		}

		public bool OpenConnection(string dataSource, string userId, string password) {
			try {
				string connString = "DATA SOURCE=" + dataSource.Trim() +
					";PERSIST SECURITY INFO=True;PASSWORD=" + password + ";USER ID=" +
					userId.Trim();
				connect(connString);
				return true;
			} catch (Exception e) {
				throw (e);
			}
		}

		public void CloseConnection() { //always successful
			if (conn == null)
				return;
			conn.Close();
		}

		public bool IsConnectionUp() {
			return conn != null && conn.State == ConnectionState.Open;
		}

		public bool IsConnectionUp(out string errMsg) {
			errMsg = "";
			if (!IsConnectionUp()) {
				errMsg = "No connection";
				return false;
			}
			return true;
		}

		public OracleConnection GetConnection() {
			return conn;
		}

		public OracleConnectionSettings GetConnectionSettings() {
			return connSettings;
		}
	}

	[Serializable()]
	public class OracleConnectionSettings {
		public string DataSource;
		public string UserId;
		public string Password;
		public bool AutoConnect = false;
	}

}
