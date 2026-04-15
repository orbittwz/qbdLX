namespace QobuzDownloaderX
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.exitlbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.verNumlbl2 = new System.Windows.Forms.Label();
            this.inspiredBylbl = new System.Windows.Forms.Label();
            this.thankslbl = new System.Windows.Forms.Label();
            this.noticelbl = new System.Windows.Forms.Label();
            this.modDevLinklbl = new System.Windows.Forms.LinkLabel();
            this.origDevLinklbl = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::QobuzDownloaderX.Properties.Resources.login_frame;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.exitlbl);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(548, 146);
            this.panel1.TabIndex = 6;
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // exitlbl
            // 
            this.exitlbl.AutoSize = true;
            this.exitlbl.BackColor = System.Drawing.Color.Transparent;
            this.exitlbl.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitlbl.ForeColor = System.Drawing.Color.Black;
            this.exitlbl.Location = new System.Drawing.Point(519, 2);
            this.exitlbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.exitlbl.Name = "exitlbl";
            this.exitlbl.Size = new System.Drawing.Size(20, 23);
            this.exitlbl.TabIndex = 0;
            this.exitlbl.Text = "X";
            this.exitlbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exitlbl.Click += new System.EventHandler(this.exitLabel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::QobuzDownloaderX.Properties.Resources.qbdlx_white;
            this.pictureBox1.Location = new System.Drawing.Point(75, 18);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(334, 98);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // verNumlbl2
            // 
            this.verNumlbl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.verNumlbl2.BackColor = System.Drawing.Color.Transparent;
            this.verNumlbl2.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verNumlbl2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.verNumlbl2.Location = new System.Drawing.Point(224, 151);
            this.verNumlbl2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.verNumlbl2.Name = "verNumlbl2";
            this.verNumlbl2.Size = new System.Drawing.Size(96, 28);
            this.verNumlbl2.TabIndex = 5;
            this.verNumlbl2.Text = "#.#.#";
            this.verNumlbl2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.verNumlbl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.verNumLabel2_MouseMove);
            // 
            // inspiredBylbl
            // 
            this.inspiredBylbl.AutoSize = true;
            this.inspiredBylbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.inspiredBylbl.Location = new System.Drawing.Point(120, 274);
            this.inspiredBylbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.inspiredBylbl.Name = "inspiredBylbl";
            this.inspiredBylbl.Size = new System.Drawing.Size(309, 20);
            this.inspiredBylbl.TabIndex = 4;
            this.inspiredBylbl.Text = "Inspired By - Qo-DL by Sorrow and DashLt";
            this.inspiredBylbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // thankslbl
            // 
            this.thankslbl.AutoSize = true;
            this.thankslbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.thankslbl.Location = new System.Drawing.Point(70, 308);
            this.thankslbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.thankslbl.Name = "thankslbl";
            this.thankslbl.Size = new System.Drawing.Size(405, 40);
            this.thankslbl.TabIndex = 3;
            this.thankslbl.Text = "Thanks to the users on Github and Telegram for offering\r\nbug reports and ideas!";
            this.thankslbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noticelbl
            // 
            this.noticelbl.AutoSize = true;
            this.noticelbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.noticelbl.Location = new System.Drawing.Point(22, 365);
            this.noticelbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.noticelbl.Name = "noticelbl";
            this.noticelbl.Size = new System.Drawing.Size(501, 40);
            this.noticelbl.TabIndex = 2;
            this.noticelbl.Text = "IF YOU PAID FOR THIS PROGRAM, YOU HAVE BEEN SCAMMED!\r\nTHIS SOFTWARE IS COMPLETELY" +
    " FREE AND OPEN-SOURCE.";
            this.noticelbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // modDevLinklbl
            // 
            this.modDevLinklbl.ActiveLinkColor = System.Drawing.Color.DarkGray;
            this.modDevLinklbl.AutoSize = true;
            this.modDevLinklbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.modDevLinklbl.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.modDevLinklbl.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.modDevLinklbl.Location = new System.Drawing.Point(177, 192);
            this.modDevLinklbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.modDevLinklbl.Name = "modDevLinklbl";
            this.modDevLinklbl.Size = new System.Drawing.Size(184, 20);
            this.modDevLinklbl.TabIndex = 1;
            this.modDevLinklbl.TabStop = true;
            this.modDevLinklbl.Text = "Mod Developer - orbittwz";
            this.modDevLinklbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.modDevLinklbl.VisitedLinkColor = System.Drawing.Color.Gray;
            this.modDevLinklbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ModDevLinkLabel_LinkClicked);
            // 
            // origDevLinklbl
            // 
            this.origDevLinklbl.ActiveLinkColor = System.Drawing.Color.DarkGray;
            this.origDevLinklbl.AutoSize = true;
            this.origDevLinklbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.origDevLinklbl.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.origDevLinklbl.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(92)))), ((int)(((byte)(102)))));
            this.origDevLinklbl.Location = new System.Drawing.Point(120, 232);
            this.origDevLinklbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.origDevLinklbl.Name = "origDevLinklbl";
            this.origDevLinklbl.Size = new System.Drawing.Size(306, 20);
            this.origDevLinklbl.TabIndex = 0;
            this.origDevLinklbl.TabStop = true;
            this.origDevLinklbl.Text = "Original Developers - AiiR and DJDoubleD";
            this.origDevLinklbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.origDevLinklbl.VisitedLinkColor = System.Drawing.Color.Gray;
            this.origDevLinklbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OrigDevLinkLabel_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(548, 445);
            this.Controls.Add(this.origDevLinklbl);
            this.Controls.Add(this.modDevLinklbl);
            this.Controls.Add(this.noticelbl);
            this.Controls.Add(this.thankslbl);
            this.Controls.Add(this.inspiredBylbl);
            this.Controls.Add(this.verNumlbl2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QobuzDLX | About";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AboutForm_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label verNumlbl2;
        private System.Windows.Forms.Label exitlbl;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label inspiredBylbl;
        private System.Windows.Forms.Label thankslbl;
        private System.Windows.Forms.Label noticelbl;
        private System.Windows.Forms.LinkLabel modDevLinklbl;
        private System.Windows.Forms.LinkLabel origDevLinklbl;
    }
}