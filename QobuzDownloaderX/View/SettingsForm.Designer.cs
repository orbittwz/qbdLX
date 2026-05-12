namespace QobuzDownloaderX.View
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.SystemGroupBox = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.DividerPanel2 = new System.Windows.Forms.Panel();
            this.DividerPanel1 = new System.Windows.Forms.Panel();
            this.AppSecretTextBox = new System.Windows.Forms.TextBox();
            this.appSecretlbl = new System.Windows.Forms.Label();
            this.AppIdTextBox = new System.Windows.Forms.TextBox();
            this.appIDlbl = new System.Windows.Forms.Label();
            this.exitlbl = new System.Windows.Forms.Label();
            this.settingsTitlelbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SystemGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.LogoPictureBox.Image = global::QobuzDownloaderX.Properties.Resources.qbdlx_white;
            this.LogoPictureBox.Location = new System.Drawing.Point(12, 12);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(105, 26);
            this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LogoPictureBox.TabIndex = 91;
            this.LogoPictureBox.TabStop = false;
            this.LogoPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LogoPictureBox_MouseMove);
            // 
            // SystemGroupBox
            // 
            this.SystemGroupBox.Controls.Add(this.SaveButton);
            this.SystemGroupBox.Controls.Add(this.DividerPanel2);
            this.SystemGroupBox.Controls.Add(this.DividerPanel1);
            this.SystemGroupBox.Controls.Add(this.AppSecretTextBox);
            this.SystemGroupBox.Controls.Add(this.appSecretlbl);
            this.SystemGroupBox.Controls.Add(this.AppIdTextBox);
            this.SystemGroupBox.Controls.Add(this.appIDlbl);
            this.SystemGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.SystemGroupBox.Location = new System.Drawing.Point(12, 56);
            this.SystemGroupBox.Name = "SystemGroupBox";
            this.SystemGroupBox.Size = new System.Drawing.Size(476, 159);
            this.SystemGroupBox.TabIndex = 93;
            this.SystemGroupBox.TabStop = false;
            this.SystemGroupBox.Text = "System";
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(239)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.ForeColor = System.Drawing.Color.White;
            this.SaveButton.Location = new System.Drawing.Point(14, 126);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(120, 23);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DividerPanel2
            // 
            this.DividerPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.DividerPanel2.Location = new System.Drawing.Point(14, 103);
            this.DividerPanel2.Name = "DividerPanel2";
            this.DividerPanel2.Size = new System.Drawing.Size(445, 1);
            this.DividerPanel2.TabIndex = 95;
            // 
            // DividerPanel1
            // 
            this.DividerPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.DividerPanel1.Location = new System.Drawing.Point(14, 61);
            this.DividerPanel1.Name = "DividerPanel1";
            this.DividerPanel1.Size = new System.Drawing.Size(445, 1);
            this.DividerPanel1.TabIndex = 94;
            // 
            // AppSecretTextBox
            // 
            this.AppSecretTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.AppSecretTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AppSecretTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.AppSecretTextBox.Location = new System.Drawing.Point(14, 87);
            this.AppSecretTextBox.Multiline = true;
            this.AppSecretTextBox.Name = "AppSecretTextBox";
            this.AppSecretTextBox.Size = new System.Drawing.Size(427, 20);
            this.AppSecretTextBox.TabIndex = 2;
            this.AppSecretTextBox.WordWrap = false;
            // 
            // appSecretlbl
            // 
            this.appSecretlbl.AutoSize = true;
            this.appSecretlbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.appSecretlbl.Location = new System.Drawing.Point(11, 68);
            this.appSecretlbl.Name = "appSecretlbl";
            this.appSecretlbl.Size = new System.Drawing.Size(60, 13);
            this.appSecretlbl.TabIndex = 92;
            this.appSecretlbl.Text = "App Secret";
            // 
            // AppIdTextBox
            // 
            this.AppIdTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.AppIdTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AppIdTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.AppIdTextBox.Location = new System.Drawing.Point(14, 45);
            this.AppIdTextBox.Multiline = true;
            this.AppIdTextBox.Name = "AppIdTextBox";
            this.AppIdTextBox.Size = new System.Drawing.Size(427, 20);
            this.AppIdTextBox.TabIndex = 1;
            this.AppIdTextBox.WordWrap = false;
            // 
            // appIDlbl
            // 
            this.appIDlbl.AutoSize = true;
            this.appIDlbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.appIDlbl.Location = new System.Drawing.Point(11, 26);
            this.appIDlbl.Name = "appIDlbl";
            this.appIDlbl.Size = new System.Drawing.Size(40, 13);
            this.appIDlbl.TabIndex = 90;
            this.appIDlbl.Text = "App ID";
            // 
            // exitlbl
            // 
            this.exitlbl.AutoSize = true;
            this.exitlbl.BackColor = System.Drawing.Color.Transparent;
            this.exitlbl.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitlbl.ForeColor = System.Drawing.Color.White;
            this.exitlbl.Location = new System.Drawing.Point(468, 9);
            this.exitlbl.Name = "exitlbl";
            this.exitlbl.Size = new System.Drawing.Size(20, 23);
            this.exitlbl.TabIndex = 4;
            this.exitlbl.Text = "X";
            this.exitlbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exitlbl.Click += new System.EventHandler(this.ExitLabel_Click);
            this.exitlbl.MouseLeave += new System.EventHandler(this.ExitLabel_MouseLeave);
            this.exitlbl.MouseHover += new System.EventHandler(this.ExitLabel_MouseHover);
            // 
            // settingsTitlelbl
            // 
            this.settingsTitlelbl.AutoSize = true;
            this.settingsTitlelbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsTitlelbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.settingsTitlelbl.Location = new System.Drawing.Point(218, 15);
            this.settingsTitlelbl.Name = "settingsTitlelbl";
            this.settingsTitlelbl.Size = new System.Drawing.Size(90, 25);
            this.settingsTitlelbl.TabIndex = 95;
            this.settingsTitlelbl.Text = "Settings";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(500, 227);
            this.Controls.Add(this.settingsTitlelbl);
            this.Controls.Add(this.exitlbl);
            this.Controls.Add(this.SystemGroupBox);
            this.Controls.Add(this.LogoPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "qbdLX | Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SettingsForm_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.SystemGroupBox.ResumeLayout(false);
            this.SystemGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.GroupBox SystemGroupBox;
        private System.Windows.Forms.Panel DividerPanel2;
        private System.Windows.Forms.Panel DividerPanel1;
        private System.Windows.Forms.TextBox AppSecretTextBox;
        private System.Windows.Forms.Label appSecretlbl;
        private System.Windows.Forms.TextBox AppIdTextBox;
        private System.Windows.Forms.Label appIDlbl;
        private System.Windows.Forms.Label exitlbl;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label settingsTitlelbl;
    }
}