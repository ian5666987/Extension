using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Extension.Database.OldOracle {
	public partial class OracleFromForm : Form {
		private string originalTitle;
		public string OriginalTitle { get { return originalTitle; } }
		public OracleFromForm(List<string> includes, List<string> excludes, string addText = "") {
			InitializeComponent();
			listBoxInclude.Items.Clear();
			listBoxExclude.Items.Clear();
			listBoxInclude.Items.AddRange(includes.ToArray());
			listBoxExclude.Items.AddRange(excludes.ToArray());
			originalTitle = Text;
			Text = originalTitle + (string.IsNullOrWhiteSpace(addText) ? "" : addText);
		}

		public List<string> IncludedColumns;
		public List<string> ExcludedColumns;

		private void buttonExclude_Click(object sender, EventArgs e) {
			IEnumerable<string> selected = listBoxInclude.SelectedItems.Cast<object>().Select(x => x.ToString());
			List<int> selectedInts = listBoxInclude.SelectedIndices.Cast<int>().OrderByDescending(x => x).ToList();
			if (selectedInts.Count >= listBoxInclude.Items.Count) {
				MessageBox.Show("You have to include at least one column!", "Error");
				return;
			}
			listBoxExclude.Items.AddRange(selected.ToArray());
			selectedInts.ForEach(x => listBoxInclude.Items.RemoveAt(x));
		}

		private void buttonInclude_Click(object sender, EventArgs e) {
			IEnumerable<string> selected = listBoxExclude.SelectedItems.Cast<object>().Select(x => x.ToString());
			List<int> selectedInts = listBoxExclude.SelectedIndices.Cast<int>().OrderByDescending(x => x).ToList();
			listBoxInclude.Items.AddRange(selected.ToArray());
			selectedInts.ForEach(x => listBoxExclude.Items.RemoveAt(x));
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			IncludedColumns = listBoxInclude.Items.Cast<object>().Select(x => x.ToString()).ToList();
			ExcludedColumns = listBoxExclude.Items.Cast<object>().Select(x => x.ToString()).ToList();
			DialogResult = DialogResult.OK;
		}

		private void linkLabelReverseInclude_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			var includes = listBoxInclude.Items.Cast<object>().Select(x => x.ToString()).Reverse().ToList();
			listBoxInclude.Items.Clear();
			listBoxInclude.Items.AddRange(includes.ToArray());
		}

		private void linkLabelReverseExclude_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			var excludes = listBoxExclude.Items.Cast<object>().Select(x => x.ToString()).Reverse().ToList();
			listBoxExclude.Items.Clear();
			listBoxExclude.Items.AddRange(excludes.ToArray());
		}
	}
}
