using QobuzDownloaderX.Properties;
using QobuzDownloaderX.View;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QobuzDownloaderX
{
    public partial class SettingsForm : HeadlessForm
    {
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
        }

        private void AddClearTextButton(TextBox textBox)
        {
            Button clearButton = new Button
            {
                Text = "X",
                Size = new Size(25, textBox.ClientSize.Height + 2),
                Location = new Point(textBox.Location.X + textBox.Width + 2, textBox.Location.Y),
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

            Settings.Default.Save();

            FlexibleMessageBox.Show(
                "Settings saved!",
                "Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            this.Close();
        }
    }
}