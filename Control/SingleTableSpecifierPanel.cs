using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Extension.Controls {
	public class SingleTableSpecifierPanel : Panel {
		private int specifierNo = 0;
		public int SpecifierId {
			get { return specifierNo; }
			set {
				specifierNo = value;
				string specifier = specifierNo.ToString();
				comboBox.Name = string.Concat("comboBox", specifier);
				checkBox.Name = string.Concat("checkBoxView", specifier);
				labelNo.Name = string.Concat("labelNo", specifier);
				numericUpDown.Name = string.Concat("numericUpDown", specifier);
				labelRows.Name = string.Concat("labelRows", specifier);
				checkBoxAddIndex.Name = string.Concat("checkBoxAddIndex", specifier);
				textBox.Name = string.Concat("textBox", specifier);
				labelWhere.Name = string.Concat("labelWhere", specifier);
				linkLabelRefresh.Name = string.Concat("linkLabelRefresh", specifier);
				linkLabelFrom.Name = string.Concat("linkLabelFrom", specifier);
				labelNo.Text = specifierNo.ToString();
			}
		}

		public bool ViewChecked { get { return checkBox.Checked; } set { checkBox.Checked = value; } }
		public bool IndexAdded { get { return checkBoxAddIndex.Checked; } set { checkBoxAddIndex.Checked = value; } }
		public string WhereClause { get { return textBox.Text; } set { textBox.Text = value; } }
		public int MaxRows { get { return (int)numericUpDown.Value; } set { numericUpDown.Value = new decimal(new int[] { value, 0, 0, 0 }); } }
		public string TableName { get { return comboBox.SelectedIndex >= 0 ? comboBox.SelectedItem.ToString() : null; } }

		public string ColumnsClause { get { return includeColumns == null || includeColumns.Count <= 0 ? "" : string.Join(", ", includeColumns); } }
		public int IncludeCount { get { return includeColumns.Count; } }
		public List<string> IncludeColumns { get { return new List<string>(includeColumns); } } //creates copy by design
		public List<string> ExcludeColumns { get { return new List<string>(excludeColumns); } } //creates copy by design
		private List<string> includeColumns = new List<string>(); //cannot be null by design
		private List<string> excludeColumns = new List<string>(); //cannot be null by design

		public SingleTableSpecifierPanel(){
			comboBox = new ComboBox();
			checkBox = new CheckBox();
			labelNo = new Label();
			numericUpDown = new NumericUpDown();
			labelRows = new Label();
			checkBoxAddIndex = new CheckBox();
			textBox = new TextBox();
			labelWhere = new Label();
			linkLabelRefresh = new LinkLabel();
			linkLabelFrom = new LinkLabel();

			Height = 35;
			Width = 819;

			string specifier = specifierNo.ToString();

			// 
			// labelNo
			// 
			labelNo.AutoSize = true;
			labelNo.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			labelNo.Location = new Point(6, 10);
			labelNo.Name = string.Concat("labelNo", specifier);
			labelNo.Size = new Size(16, 16);
			labelNo.TabIndex = 24;
			labelNo.Text = specifierNo.ToString();
			labelNo.TextAlign = ContentAlignment.MiddleRight;

			// 
			// comboBox
			// 
			comboBox.FormattingEnabled = true;
			comboBox.Location = new Point(35, 7);
			comboBox.Name = string.Concat("comboBox", specifier);
			comboBox.Size = new Size(199, 21);
			comboBox.TabIndex = 4;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

			// 
			// linkLabel
			// 
			linkLabelFrom.AutoSize = true;
			linkLabelFrom.Location = new Point(240, 10);
			linkLabelFrom.Name = string.Concat("linkLabelFrom", specifier);
			linkLabelFrom.TabIndex = 47;
			linkLabelFrom.TabStop = true;
			linkLabelFrom.Text = "From";

			// 
			// checkBoxView
			// 
			checkBox.AutoSize = true;
			checkBox.Location = new Point(275, 9);
			checkBox.Name = string.Concat("checkBoxView", specifier);
			checkBox.Size = new Size(49, 17);
			checkBox.TabIndex = 8;
			checkBox.Text = "View";
			checkBox.UseVisualStyleBackColor = true;

			// 
			// checkBoxAddIndex
			// 
			checkBoxAddIndex.AutoSize = true;
			checkBoxAddIndex.Location = new Point(325, 9);
			checkBoxAddIndex.Name = string.Concat("checkBoxAddIndex", specifier);
			checkBoxAddIndex.Size = new Size(52, 17);
			checkBoxAddIndex.TabIndex = 36;
			checkBoxAddIndex.Text = "Index";
			checkBoxAddIndex.UseVisualStyleBackColor = true;

			// 
			// labelRows
			// 
			labelRows.AutoSize = true;
			labelRows.Location = new Point(373, 10);
			labelRows.Name = string.Concat("labelRows", specifier);
			labelRows.Size = new Size(57, 13);
			labelRows.TabIndex = 35;
			labelRows.Text = "Max Rows";

			// 
			// numericUpDown
			// 
			numericUpDown.Location = new Point(446, 8);
			numericUpDown.Name = string.Concat("numericUpDown", specifier);
			numericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			numericUpDown.Size = new Size(55, 20);
			numericUpDown.TabIndex = 34;
			numericUpDown.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});

			// 
			// labelWhere
			// 
			labelWhere.AutoSize = true;
			labelWhere.Location = new Point(508, 10);
			labelWhere.Name = string.Concat("labelWhere", specifier);
			labelWhere.Size = new Size(39, 13);
			labelWhere.TabIndex = 38;
			labelWhere.Text = "Where";

			// 
			// textBox
			// 
			textBox.Location = new Point(553, 7);
			textBox.Name = string.Concat("textBox", specifier);
			textBox.Size = new Size(200, 20);
			textBox.TabIndex = 37;

			// 
			// linkLabel
			// 
			linkLabelRefresh.AutoSize = true;
			linkLabelRefresh.Location = new Point(763, 10);
			linkLabelRefresh.Name = string.Concat("linkLabelRefresh", specifier);
			linkLabelRefresh.Size = new Size(55, 13);
			linkLabelRefresh.TabIndex = 47;
			linkLabelRefresh.TabStop = true;
			linkLabelRefresh.Text = "Refresh";

			Controls.Add(linkLabelFrom);
			Controls.Add(linkLabelRefresh);
			Controls.Add(labelWhere);
			Controls.Add(textBox);
			Controls.Add(checkBoxAddIndex);
			Controls.Add(labelRows);
			Controls.Add(numericUpDown);
			Controls.Add(labelNo);
			Controls.Add(checkBox);
			Controls.Add(comboBox);
		}

		public SingleTableSpecifierPanel(int specifierNo) {
			SpecifierId = specifierNo;
		}

		public void SetColumns(List<string> includeColumns, List<string> excludeColumns) {
			this.includeColumns = includeColumns == null ? new List<string>() : includeColumns;
			this.excludeColumns = excludeColumns == null ? new List<string>() : excludeColumns;
		}

		public void ResetColumns() {
			includeColumns.Clear();
			excludeColumns.Clear();
		}

		private ComboBox comboBox;
		private CheckBox checkBox;
		private Label labelNo;
		private NumericUpDown numericUpDown;
		private Label labelRows;
		private CheckBox checkBoxAddIndex;
		private TextBox textBox;
		private Label labelWhere;
		private LinkLabel linkLabelRefresh;
		private LinkLabel linkLabelFrom;
	}
}
