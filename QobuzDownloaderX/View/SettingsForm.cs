using QobuzDownloaderX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QobuzDownloaderX.Shared;

namespace QobuzDownloaderX.View
{
    public partial class SettingsForm : HeadlessForm
    {
        private readonly List<string> themeListData1;
        public SettingsForm()
        {
            InitializeComponent();
            AppIdTextBox.Text = Settings.Default.appId;
            AppSecretTextBox.Text = Settings.Default.appSecret;
            // Add clear buttons
            AddClearTextButton(AppIdTextBox);
            AddClearTextButton(AppSecretTextBox);
            // Don't select entire existing value in textbox when it's loaded
            AppIdTextBox.Select(AppIdTextBox.Text.Length, 0);
            Markdowns markdowns = new Markdowns();
            themeListData1 = markdowns.ThemeListData;
            themeComboBox.DataSource = themeListData1;
            themeComboBox.SelectedItem = Settings.Default.theme;
        }

        private void AddClearTextButton(TextBox textBox)
        {
            Button clearButton = new Button
            {
                Text = "X",
                Size = new Size(25, textBox.ClientSize.Height + 20),
                Location = new Point(textBox.Location.X + textBox.Width + 2, textBox.Location.Y - 10),
                Cursor = Cursors.Default,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += (s, e) => textBox.Clear();
            // Add the button to the same container as the text box
            textBox.Parent.Controls.Add(clearButton);
        }

        private void SettingsForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void LogoPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ExitLabel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExitLabel_MouseHover(object sender, EventArgs e)
        {
            exitlbl.ForeColor = Color.FromArgb(0, 112, 239);
        }

        private void ExitLabel_MouseLeave(object sender, EventArgs e)
        {
            exitlbl.ForeColor = Color.White;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Settings.Default.appId = AppIdTextBox.Text;
            Settings.Default.appSecret = AppSecretTextBox.Text;
            Settings.Default.theme = (string)themeComboBox.SelectedItem;
            Settings.Default.Save();
            MessageBox.Show(
                "Settings saved!",
                "Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            Globals.LoginForm.ApplyTheme();
            Globals.SettingsForm.ApplyTheme();
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.SetToolTips();
            this.ApplyTheme();
        }

        private void ApplyTheme()
        {
            switch (Settings.Default.theme)
            {
                case "Dark":
                    this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
                    this.AppIdTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
                    this.AppSecretTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
                    break;
                case "Light":
                    this.BackColor = System.Drawing.Color.Empty;
                    this.AppIdTextBox.BackColor = System.Drawing.SystemColors.Control;
                    this.AppSecretTextBox.BackColor = System.Drawing.SystemColors.Control;
                    break;
                case "Party":
                    int randomRed = Settings.GetRandom;
                    int randomGreen = Settings.GetRandom;
                    int randomBlue = Settings.GetRandom;
                    this.BackColor = System.Drawing.Color.FromArgb((byte)randomRed, (byte)randomGreen, (byte)randomBlue);
                    this.AppIdTextBox.BackColor = System.Drawing.Color.FromArgb((byte)randomRed, (byte)randomGreen, (byte)randomBlue);
                    this.AppSecretTextBox.BackColor = System.Drawing.Color.FromArgb((byte)randomRed, (byte)randomGreen, (byte)randomBlue);
                    break;
            }
        }

        private void SetToolTips()
        {
            // Set tooltips for all the interactable controls in the Settings window.
            new ToolTip().SetToolTip(exitlbl, "Close settings window.");
            new ToolTip().SetToolTip(AppIdTextBox, "Write custom AppID acquired from your browser.");
            new ToolTip().SetToolTip(AppSecretTextBox, "Write custom AppSecret acquired from your browser.");
            new ToolTip().SetToolTip(SaveButton, "Save settings and close window.");
        }
    }
}