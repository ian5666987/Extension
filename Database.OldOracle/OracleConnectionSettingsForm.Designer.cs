namespace Extension.Database.OldOracle {
	partial class OracleConnectionSettingsForm {
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
			this.textBoxDBPassword = new System.Windows.Forms.TextBox();
			this.textBoxDBUserID = new System.Windows.Forms.TextBox();
			this.textBoxDBDataSource = new System.Windows.Forms.TextBox();
			this.buttonConnect = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxDBPassword
			// 
			this.textBoxDBPassword.Location = new System.Drawing.Point(127, 76);
			this.textBoxDBPassword.Name = "textBoxDBPassword";
			this.textBoxDBPassword.PasswordChar = '*';
			this.textBoxDBPassword.Size = new System.Drawing.Size(100, 23);
			this.textBoxDBPassword.TabIndex = 17;
			// 
			// textBoxDBUserID
			// 
			this.textBoxDBUserID.Location = new System.Drawing.Point(127, 42);
			this.textBoxDBUserID.Name = "textBoxDBUserID";
			this.textBoxDBUserID.Size = new System.Drawing.Size(100, 23);
			this.textBoxDBUserID.TabIndex = 16;
			// 
			// textBoxDBDataSource
			// 
			this.textBoxDBDataSource.Location = new System.Drawing.Point(127, 9);
			this.textBoxDBDataSource.Name = "textBoxDBDataSource";
			this.textBoxDBDataSource.Size = new System.Drawing.Size(100, 23);
			this.textBoxDBDataSource.TabIndex = 15;
			// 
			// buttonConnect
			// 
			this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonConnect.Location = new System.Drawing.Point(127, 111);
			this.buttonConnect.Margin = new System.Windows.Forms.Padding(4);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(100, 28);
			this.buttonConnect.TabIndex = 22;
			this.buttonConnect.Text = "Connect";
			this.buttonConnect.UseVisualStyleBackColor = true;
			this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(10, 79);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(69, 17);
			this.label9.TabIndex = 13;
			this.label9.Text = "Password";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 45);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 17);
			this.label2.TabIndex = 12;
			this.label2.Text = "User ID";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 12);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "Data Source";
			// 
			// buttonCancel
			// 
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonCancel.Location = new System.Drawing.Point(13, 111);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(100, 28);
			this.buttonCancel.TabIndex = 23;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// OracleConnectionSettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(240, 151);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.textBoxDBPassword);
			this.Controls.Add(this.textBoxDBUserID);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxDBDataSource);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonConnect);
			this.Controls.Add(this.label9);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "OracleConnectionSettingsForm";
			this.Text = "Connection Settings [Oracle]";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxDBPassword;
		private System.Windows.Forms.TextBox textBoxDBUserID;
		private System.Windows.Forms.TextBox textBoxDBDataSource;
		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCancel;
	}
}