using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Extension.Database.OldOracle {
	public partial class OracleTableViewForm : Form {
		OracleHandler handler = null;
		private string originalTitle;
		public string OriginalTitle { get { return originalTitle; } }
		public OracleTableViewForm() {
			InitializeComponent();
			originalTitle = Text;
		}

		public void SetOracleHandler(OracleHandler oracleHandler) {
			handler = oracleHandler;
		}

		public void DisplayTableResult(string tableName, string columnsClause = "", string whereClause = "", int maxRows = int.MaxValue, bool addIndex = false) { //the simplest command is to display this
			if (handler == null || !handler.IsConnectionUp())
				return;

			dgv.Rows.Clear(); //clear rows
			dgv.Columns.Clear(); //and then columns
			DataTable dataTable = handler.ReadTable(tableName, columnsClause, whereClause);

			if (dataTable == null || dataTable.Columns.Count <= 0) //table with no column
				return;

			if (addIndex)
				dgv.Columns.Add(new DataGridViewTextBoxColumn() {
					DataPropertyName = "MyIndex",
					HeaderText = "MyIndex",
					Name = "MyIndexDataGridViewTextBoxColumn"
				});

			dgv.Columns.AddRange(dataTable.Columns //Add all columns
				.Cast<DataColumn>()
				.Select(x => new DataGridViewTextBoxColumn() {
					DataPropertyName = x.ColumnName,
					HeaderText = x.ColumnName,
					Name = x.ColumnName + "DataGridViewTextBoxColumn"
				})
				.ToArray());

			if (dataTable.Rows == null || dataTable.Rows.Count <= 0) //table with no entry
				return;

			List<object[]> rows = dataTable.Rows //Add all rows
				.Cast<DataRow>()
				.Select(x => x.ItemArray)
				.ToList();

			for (int i = 0; i < Math.Min(rows.Count, maxRows); ++i) {
				List<object> row = rows[i].Select(x => x is DBNull || x == null ? "" : x).ToList();
				//TODO this fails when the object comes from number
				//The best is to get the actual type and then treats it correctly (later on)
				if (addIndex)
					row.Insert(0, (i + 1));
				dgv.Rows.Add(row.ToArray());
			}
		}

		public void DisplayTableResult(string tableName, IEnumerable<string> columns = null, string whereClause = "", int maxRows = int.MaxValue, bool addIndex = false) { //the simplest command is to display this
			DisplayTableResult(tableName, columns == null ? "" : string.Join(", ", columns), whereClause, maxRows, addIndex);
		}

		public void DisplayTableResult(string tableName, string whereClause = "", int maxRows = int.MaxValue, bool addIndex = false) { //the simplest command is to display this
			DisplayTableResult(tableName, "", whereClause, maxRows, addIndex);
		}

		public bool PreventClosing { get; set; }

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
	}
}
