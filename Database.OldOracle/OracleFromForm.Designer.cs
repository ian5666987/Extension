namespace Extension.Database.OldOracle {
	partial class OracleFromForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OracleFromForm));
			this.listBoxInclude = new System.Windows.Forms.ListBox();
			this.listBoxExclude = new System.Windows.Forms.ListBox();
			this.buttonExclude = new System.Windows.Forms.Button();
			this.buttonInclude = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.labelInclude = new System.Windows.Forms.Label();
			this.labelExclude = new System.Windows.Forms.Label();
			this.linkLabelReverseInclude = new System.Windows.Forms.LinkLabel();
			this.linkLabelReverseExclude = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// listBoxInclude
			// 
			this.listBoxInclude.FormattingEnabled = true;
			this.listBoxInclude.Location = new System.Drawing.Point(12, 33);
			this.listBoxInclude.Name = "listBoxInclude";
			this.listBoxInclude.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxInclude.Size = new System.Drawing.Size(155, 212);
			this.listBoxInclude.TabIndex = 0;
			// 
			// listBoxExclude
			// 
			this.listBoxExclude.FormattingEnabled = true;
			this.listBoxExclude.Location = new System.Drawing.Point(235, 33);
			this.listBoxExclude.Name = "listBoxExclude";
			this.listBoxExclude.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxExclude.Size = new System.Drawing.Size(155, 212);
			this.listBoxExclude.TabIndex = 1;
			// 
			// buttonExclude
			// 
			this.buttonExclude.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonExclude.Location = new System.Drawing.Point(181, 78);
			this.buttonExclude.Name = "buttonExclude";
			this.buttonExclude.Size = new System.Drawing.Size(41, 33);
			this.buttonExclude.TabIndex = 2;
			this.buttonExclude.Text = ">";
			this.buttonExclude.UseVisualStyleBackColor = true;
			this.buttonExclude.Click += new System.EventHandler(this.buttonExclude_Click);
			// 
			// buttonInclude
			// 
			this.buttonInclude.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonInclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonInclude.Location = new System.Drawing.Point(181, 120);
			this.buttonInclude.Name = "buttonInclude";
			this.buttonInclude.Size = new System.Drawing.Size(41, 33);
			this.buttonInclude.TabIndex = 3;
			this.buttonInclude.Text = "<";
			this.buttonInclude.UseVisualStyleBackColor = true;
			this.buttonInclude.Click += new System.EventHandler(this.buttonInclude_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOK.Location = new System.Drawing.Point(11, 256);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(379, 31);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// labelInclude
			// 
			this.labelInclude.AutoSize = true;
			this.labelInclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInclude.Location = new System.Drawing.Point(8, 9);
			this.labelInclude.Name = "labelInclude";
			this.labelInclude.Size = new System.Drawing.Size(68, 20);
			this.labelInclude.TabIndex = 5;
			this.labelInclude.Text = "Include";
			// 
			// labelExclude
			// 
			this.labelExclude.AutoSize = true;
			this.labelExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExclude.Location = new System.Drawing.Point(231, 9);
			this.labelExclude.Name = "labelExclude";
			this.labelExclude.Size = new System.Drawing.Size(72, 20);
			this.labelExclude.TabIndex = 6;
			this.labelExclude.Text = "Exclude";
			// 
			// linkLabelReverseInclude
			// 
			this.linkLabelReverseInclude.AutoSize = true;
			this.linkLabelReverseInclude.Location = new System.Drawing.Point(120, 14);
			this.linkLabelReverseInclude.Name = "linkLabelReverseInclude";
			this.linkLabelReverseInclude.Size = new System.Drawing.Size(47, 13);
			this.linkLabelReverseInclude.TabIndex = 7;
			this.linkLabelReverseInclude.TabStop = true;
			this.linkLabelReverseInclude.Text = "Reverse";
			this.linkLabelReverseInclude.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReverseInclude_LinkClicked);
			// 
			// linkLabelReverseExclude
			// 
			this.linkLabelReverseExclude.AutoSize = true;
			this.linkLabelReverseExclude.Location = new System.Drawing.Point(343, 14);
			this.linkLabelReverseExclude.Name = "linkLabelReverseExclude";
			this.linkLabelReverseExclude.Size = new System.Drawing.Size(47, 13);
			this.linkLabelReverseExclude.TabIndex = 8;
			this.linkLabelReverseExclude.TabStop = true;
			this.linkLabelReverseExclude.Text = "Reverse";
			this.linkLabelReverseExclude.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReverseExclude_LinkClicked);
			// 
			// OracleFromForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(403, 298);
			this.Controls.Add(this.linkLabelReverseExclude);
			this.Controls.Add(this.linkLabelReverseInclude);
			this.Controls.Add(this.labelExclude);
			this.Controls.Add(this.labelInclude);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonInclude);
			this.Controls.Add(this.buttonExclude);
			this.Controls.Add(this.listBoxExclude);
			this.Controls.Add(this.listBoxInclude);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(419, 337);
			this.MinimumSize = new System.Drawing.Size(419, 337);
			this.Name = "OracleFromForm";
			this.Text = "From Clause";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxInclude;
		private System.Windows.Forms.ListBox listBoxExclude;
		private System.Windows.Forms.Button buttonExclude;
		private System.Windows.Forms.Button buttonInclude;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelInclude;
		private System.Windows.Forms.Label labelExclude;
		private System.Windows.Forms.LinkLabel linkLabelReverseInclude;
		private System.Windows.Forms.LinkLabel linkLabelReverseExclude;
	}
}