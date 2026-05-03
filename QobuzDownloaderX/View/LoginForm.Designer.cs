namespace QobuzDownloaderX
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.topPanel = new System.Windows.Forms.Panel();
            this.clearLoginInfobtn = new System.Windows.Forms.Button();
            this.settingsPictureBox = new System.Windows.Forms.PictureBox();
            this.verNumlbl3 = new System.Windows.Forms.Label();
            this.exitlbl = new System.Windows.Forms.Label();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.userAuthTokenTextbox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.emailDividerPanel = new System.Windows.Forms.Panel();
            this.passwordDividerPanel = new System.Windows.Forms.Panel();
            this.emailTextbox = new System.Windows.Forms.TextBox();
            this.passwordTextbox = new System.Windows.Forms.TextBox();
            this.loginTextlbl = new System.Windows.Forms.Label();
            this.loginBG = new System.ComponentModel.BackgroundWorker();
            this.visableCheckbox = new System.Windows.Forms.CheckBox();
            this.altLoginlbl = new System.Windows.Forms.Label();
            this.userIdTextbox = new System.Windows.Forms.TextBox();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackgroundImage = global::QobuzDownloaderX.Properties.Resources.login_frame;
            this.topPanel.Controls.Add(this.clearLoginInfobtn);
            this.topPanel.Controls.Add(this.settingsPictureBox);
            this.topPanel.Controls.Add(this.verNumlbl3);
            this.topPanel.Controls.Add(this.exitlbl);
            this.topPanel.Controls.Add(this.bannerPictureBox);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(282, 175);
            this.topPanel.TabIndex = 0;
            this.topPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopPanel_MouseMove);
            // 
            // clearLoginInfobtn
            // 
            this.clearLoginInfobtn.Font = new System.Drawing.Font("Trebuchet MS", 8F, System.Drawing.FontStyle.Bold);
            this.clearLoginInfobtn.Location = new System.Drawing.Point(12, 138);
            this.clearLoginInfobtn.Name = "clearLoginInfobtn";
            this.clearLoginInfobtn.Size = new System.Drawing.Size(105, 23);
            this.clearLoginInfobtn.TabIndex = 37;
            this.clearLoginInfobtn.Text = "Clear login info";
            this.clearLoginInfobtn.UseVisualStyleBackColor = true;
            this.clearLoginInfobtn.Click += new System.EventHandler(this.ClearLoginInfobtn_Click);
            // 
            // settingsPictureBox
            // 
            this.settingsPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.settingsPictureBox.Image = global::QobuzDownloaderX.Properties.Resources.settings_icon;
            this.settingsPictureBox.InitialImage = global::QobuzDownloaderX.Properties.Resources.settings_icon;
            this.settingsPictureBox.Location = new System.Drawing.Point(242, 4);
            this.settingsPictureBox.Name = "settingsPictureBox";
            this.settingsPictureBox.Size = new System.Drawing.Size(15, 15);
            this.settingsPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.settingsPictureBox.TabIndex = 36;
            this.settingsPictureBox.TabStop = false;
            this.settingsPictureBox.Click += new System.EventHandler(this.OpenSettings_Click);
            // 
            // verNumlbl3
            // 
            this.verNumlbl3.BackColor = System.Drawing.Color.Transparent;
            this.verNumlbl3.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verNumlbl3.ForeColor = System.Drawing.Color.White;
            this.verNumlbl3.Location = new System.Drawing.Point(194, 157);
            this.verNumlbl3.Name = "verNumlbl3";
            this.verNumlbl3.Size = new System.Drawing.Size(85, 18);
            this.verNumlbl3.TabIndex = 32;
            this.verNumlbl3.Text = "#.#.#";
            this.verNumlbl3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.verNumlbl3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VerNumLabel2_MouseMove);
            // 
            // exitlbl
            // 
            this.exitlbl.AutoSize = true;
            this.exitlbl.BackColor = System.Drawing.Color.Transparent;
            this.exitlbl.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitlbl.ForeColor = System.Drawing.Color.Black;
            this.exitlbl.Location = new System.Drawing.Point(262, 0);
            this.exitlbl.Name = "exitlbl";
            this.exitlbl.Size = new System.Drawing.Size(20, 23);
            this.exitlbl.TabIndex = 9;
            this.exitlbl.Text = "X";
            this.exitlbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exitlbl.Click += new System.EventHandler(this.ExitLabel_Click);
            // 
            // bannerPictureBox
            // 
            this.bannerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.bannerPictureBox.Image = global::QobuzDownloaderX.Properties.Resources.qbdlx_white;
            this.bannerPictureBox.Location = new System.Drawing.Point(12, 52);
            this.bannerPictureBox.Name = "bannerPictureBox";
            this.bannerPictureBox.Size = new System.Drawing.Size(258, 64);
            this.bannerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bannerPictureBox.TabIndex = 29;
            this.bannerPictureBox.TabStop = false;
            this.bannerPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            // 
            // userAuthTokenTextbox
            // 
            this.userAuthTokenTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.userAuthTokenTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userAuthTokenTextbox.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userAuthTokenTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.userAuthTokenTextbox.Location = new System.Drawing.Point(12, 255);
            this.userAuthTokenTextbox.Multiline = true;
            this.userAuthTokenTextbox.Name = "userAuthTokenTextbox";
            this.userAuthTokenTextbox.Size = new System.Drawing.Size(237, 23);
            this.userAuthTokenTextbox.TabIndex = 35;
            this.userAuthTokenTextbox.Text = "user_auth_token";
            this.userAuthTokenTextbox.Visible = false;
            this.userAuthTokenTextbox.Click += new System.EventHandler(this.UserAuthTokenTextbox_Click);
            this.userAuthTokenTextbox.Leave += new System.EventHandler(this.UserAuthTokenTextbox_Leave);
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(239)))));
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(12, 293);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(258, 30);
            this.loginButton.TabIndex = 2;
            this.loginButton.Text = "LOGIN";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // emailDividerPanel
            // 
            this.emailDividerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.emailDividerPanel.Location = new System.Drawing.Point(12, 221);
            this.emailDividerPanel.Name = "emailDividerPanel";
            this.emailDividerPanel.Size = new System.Drawing.Size(258, 1);
            this.emailDividerPanel.TabIndex = 2;
            // 
            // passwordDividerPanel
            // 
            this.passwordDividerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.passwordDividerPanel.Location = new System.Drawing.Point(12, 277);
            this.passwordDividerPanel.Name = "passwordDividerPanel";
            this.passwordDividerPanel.Size = new System.Drawing.Size(258, 1);
            this.passwordDividerPanel.TabIndex = 2;
            // 
            // emailTextbox
            // 
            this.emailTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.emailTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.emailTextbox.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.emailTextbox.Location = new System.Drawing.Point(12, 199);
            this.emailTextbox.Multiline = true;
            this.emailTextbox.Name = "emailTextbox";
            this.emailTextbox.Size = new System.Drawing.Size(258, 23);
            this.emailTextbox.TabIndex = 7;
            this.emailTextbox.Text = "Email";
            this.emailTextbox.Click += new System.EventHandler(this.EmailTextbox_Click);
            this.emailTextbox.Leave += new System.EventHandler(this.EmailTextbox_Leave);
            // 
            // passwordTextbox
            // 
            this.passwordTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.passwordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passwordTextbox.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.passwordTextbox.Location = new System.Drawing.Point(12, 255);
            this.passwordTextbox.Multiline = true;
            this.passwordTextbox.Name = "passwordTextbox";
            this.passwordTextbox.Size = new System.Drawing.Size(237, 23);
            this.passwordTextbox.TabIndex = 8;
            this.passwordTextbox.Text = "Password";
            this.passwordTextbox.Click += new System.EventHandler(this.PasswordTextbox_Click);
            this.passwordTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasswordTextbox_KeyDown);
            this.passwordTextbox.Leave += new System.EventHandler(this.PasswordTextbox_Leave);
            // 
            // loginTextlbl
            // 
            this.loginTextlbl.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginTextlbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.loginTextlbl.Location = new System.Drawing.Point(12, 349);
            this.loginTextlbl.Name = "loginTextlbl";
            this.loginTextlbl.Size = new System.Drawing.Size(258, 23);
            this.loginTextlbl.TabIndex = 30;
            this.loginTextlbl.Text = "Waiting for login...";
            this.loginTextlbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loginBG
            // 
            this.loginBG.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoginBG_DoWork);
            // 
            // visableCheckbox
            // 
            this.visableCheckbox.AutoSize = true;
            this.visableCheckbox.Location = new System.Drawing.Point(255, 256);
            this.visableCheckbox.Name = "visableCheckbox";
            this.visableCheckbox.Size = new System.Drawing.Size(15, 14);
            this.visableCheckbox.TabIndex = 31;
            this.visableCheckbox.UseVisualStyleBackColor = true;
            this.visableCheckbox.CheckedChanged += new System.EventHandler(this.VisibleCheckbox_CheckedChanged);
            // 
            // altLoginlbl
            // 
            this.altLoginlbl.Font = new System.Drawing.Font("Trebuchet MS", 8.25F);
            this.altLoginlbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.altLoginlbl.Location = new System.Drawing.Point(12, 326);
            this.altLoginlbl.Name = "altLoginlbl";
            this.altLoginlbl.Size = new System.Drawing.Size(258, 20);
            this.altLoginlbl.TabIndex = 32;
            this.altLoginlbl.Text = "Can\'t login? Click here";
            this.altLoginlbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.altLoginlbl.Click += new System.EventHandler(this.AltLoginLabel_Click);
            // 
            // userIdTextbox
            // 
            this.userIdTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.userIdTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userIdTextbox.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userIdTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.userIdTextbox.Location = new System.Drawing.Point(12, 199);
            this.userIdTextbox.Multiline = true;
            this.userIdTextbox.Name = "userIdTextbox";
            this.userIdTextbox.Size = new System.Drawing.Size(258, 23);
            this.userIdTextbox.TabIndex = 9;
            this.userIdTextbox.Text = "user_id";
            this.userIdTextbox.Visible = false;
            this.userIdTextbox.Click += new System.EventHandler(this.UserIdTextbox_Click);
            this.userIdTextbox.Leave += new System.EventHandler(this.UserIdTextbox_Leave);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(282, 392);
            this.Controls.Add(this.altLoginlbl);
            this.Controls.Add(this.visableCheckbox);
            this.Controls.Add(this.loginTextlbl);
            this.Controls.Add(this.passwordDividerPanel);
            this.Controls.Add(this.emailDividerPanel);
            this.Controls.Add(this.passwordTextbox);
            this.Controls.Add(this.emailTextbox);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.userIdTextbox);
            this.Controls.Add(this.userAuthTokenTextbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Text = "qbdLX | Login";
            this.Load += new System.EventHandler(this.LoginFrm_Load);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.PictureBox bannerPictureBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Panel emailDividerPanel;
        private System.Windows.Forms.Panel passwordDividerPanel;
        private System.Windows.Forms.TextBox emailTextbox;
        private System.Windows.Forms.TextBox passwordTextbox;
        private System.Windows.Forms.Label exitlbl;
        private System.Windows.Forms.Label loginTextlbl;
        private System.ComponentModel.BackgroundWorker loginBG;
        private System.Windows.Forms.Label verNumlbl3;
        private System.Windows.Forms.CheckBox visableCheckbox;
        private System.Windows.Forms.TextBox userAuthTokenTextbox;
        private System.Windows.Forms.Label altLoginlbl;
        private System.Windows.Forms.TextBox userIdTextbox;
        private System.Windows.Forms.PictureBox settingsPictureBox;
        private System.Windows.Forms.Button clearLoginInfobtn;
    }
}