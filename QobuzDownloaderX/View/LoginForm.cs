using QobuzApiSharp.Exceptions;
using QobuzDownloaderX.Properties;
using QobuzDownloaderX.Shared;
using QobuzDownloaderX.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QobuzDownloaderX
{
    public partial class LoginForm : HeadlessForm
    {
        private readonly string dllCheck = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TagLibSharp.dll");

        private readonly string loginErrorLog = Path.Combine(Globals.LoggingDir, "Login_Errors.log");
        private readonly string versionCheckErrorLog = Path.Combine(Globals.LoggingDir, "VersionCheck_Errors.log");

        public LoginForm()
        {
            InitializeComponent();

            // Delete previous login error log
            if (System.IO.File.Exists(loginErrorLog)) System.IO.File.Delete(loginErrorLog);
            // Delete previous version check error log
            if (System.IO.File.Exists(versionCheckErrorLog)) System.IO.File.Delete(versionCheckErrorLog);

        }

        private string AltLoginValue { get; set; }

        private void QobuzDownloaderX_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void LoginFrm_Load(object sender, EventArgs e)
        {
            // Get and display version number.
            verNumlbl3.Text = Settings.Version;

            if (!System.IO.File.Exists(dllCheck))
            {
                MessageBox.Show("TagLibSharp.dll missing from folder!\r\nPlease Make sure the DLL is in the same folder as QobuzDownloaderX.exe!", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Bring to center of screen.
            CenterToScreen();

            /*
             * Only needed when using the default .NET SettingsProvider, not applicable when using PortableJsonSettingsProvider
            // Initialise new settings with values from previous version when assembly version changes.
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            */

            // Set saved settings to correct places.
            emailTextbox.Text = Settings.Default.savedEmail;
            passwordTextbox.Text = Settings.Default.savedPassword;
            userIdTextbox.Text = Settings.Default.savedUserID;
            userAuthTokenTextbox.Text = Settings.Default.savedUserAuthToken;
            AltLoginValue = Settings.Default.savedAltLoginValue;

            // Set alt login mode & label text based on saved value
            if (AltLoginValue == "0")
            {
                // Change alt login label text
                altLoginlbl.Text = "Can't login? Click here";

                // Hide alt login methods
                //altLoginTutLabel.Visible = false;
                userIdTextbox.Visible = false;
                userAuthTokenTextbox.Visible = false;

                // Unhide standard login methods
                emailTextbox.Visible = true;
                passwordTextbox.Visible = true;
            }
            else if (AltLoginValue == "1")
            {
                // Change alt login label text
                altLoginlbl.Text = "Login normally? Click here";

                // Hide standard login methods
                emailTextbox.Visible = false;
                passwordTextbox.Visible = false;

                // Unhide alt login methods
                //altLoginTutLabel.Visible = true;
                userIdTextbox.Visible = true;
                userAuthTokenTextbox.Visible = true;
            }

            // Set values for email textbox.
            if (emailTextbox.Text != "Email")
            {
                emailTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
            if (emailTextbox.Text == null || emailTextbox.Text == "")
            {
                emailTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                emailTextbox.Text = "Email";
            }

            // Set values for user_id textbox.
            if (userIdTextbox.Text != "user_id")
            {
                userIdTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
            if (userIdTextbox.Text == null || userIdTextbox.Text == "")
            {
                userIdTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                userIdTextbox.Text = "user_id";
            }

            // Set values for password textbox.
            if (passwordTextbox.Text != "Password")
            {
                passwordTextbox.PasswordChar = '*';
                passwordTextbox.UseSystemPasswordChar = false;
                passwordTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
            if (passwordTextbox.Text == null || passwordTextbox.Text == "")
            {
                passwordTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                passwordTextbox.UseSystemPasswordChar = true;
                passwordTextbox.Text = "Password";
            }

            // Set values for user_auth_token textbox.
            if (userAuthTokenTextbox.Text != "user_auth_token")
            {
                userAuthTokenTextbox.PasswordChar = '*';
                userAuthTokenTextbox.UseSystemPasswordChar = false;
                userAuthTokenTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
            if (userAuthTokenTextbox.Text == null || userAuthTokenTextbox.Text == "")
            {
                userAuthTokenTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                userAuthTokenTextbox.UseSystemPasswordChar = true;
                userAuthTokenTextbox.Text = "user_auth_token";
            }

            //try
            //{
            //    // Create HttpClient to grab version number from Github
            //    // Force minimum TLS 1.2 as Github does not support TLS 1.1 and lower
            //    var versionURLClient = new HttpClient(new HttpClientHandler { SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13});
            //    // Set user-agent to Firefox.
            //    versionURLClient.DefaultRequestHeaders.Add("User-Agent", Globals.USER_AGENT);

            //    // Grab response from Github to get latest application version.
            //    string versionURL = Globals.GITHUB_LATEST_VERSION_URL;
            //    var versionURLResponse = await versionURLClient.GetAsync(versionURL);
            //    string versionURLResponseString = await versionURLResponse.Content.ReadAsStringAsync();

            //    // Grab metadata from API JSON response
            //    JObject joVersionResponse = JObject.Parse(versionURLResponseString);

            //    // Grab latest version number
            //    string remoteVersionString = (string)joVersionResponse["tag_name"];
            //    // Grab changelog
            //    string changes = (string)joVersionResponse["body"];
            //    string currentVersionString = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //    Version remoteVersion = Version.Parse(remoteVersionString);
            //    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            //    if (currentVersion.CompareTo(remoteVersion) < 0)
            //    {
            //        // Remote version is newer, propose update.
            //        string updateDialogContents = "New version of QBDLX-MOD is available!\r\n\r\nInstalled Version - "
            //            + currentVersionString
            //            + "\r\nLatest version - "
            //            + remoteVersionString
            //            + "\r\n\r\nChangelog Below\r\n==============\r\n"
            //            + changes.Replace("\\r\\n", "\r\n")
            //            + "\r\n==============\r\n\r\nWould you like to update?";

            //        DialogResult dialogResult = FlexibleMessageBox.Show(updateDialogContents, "QBDLX-MOD | Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            // If "Yes" is clicked, open GitHub page and close QBDLX-MOD.
            //            Process.Start(Globals.GITHUB_LATEST_URL);
            //            Application.Exit();
            //        }
            //        else if (dialogResult == DialogResult.No)
            //        {
            //            // Ignore the update until next open.
            //        }
            //    }
            //    else
            //    {
            //        // Do nothing. All is good.
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // log the exeption details for info
            //    System.IO.File.WriteAllText(versionCheckErrorLog, $"Failed to compare GitHub version, exception details below:\r\n{ex}");

            //    DialogResult dialogResult = MessageBox.Show("Connection to GitHub to check for an update has failed.\r\nWould you like to check for an update manually?\r\n\r\nYour current version is " + Assembly.GetExecutingAssembly().GetName().Version.ToString(), "QBDLX | GitHub Connection Failed", MessageBoxButtons.YesNo);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        // If "Yes" is clicked, open GitHub page and close QBDLX.
            //        Process.Start(Globals.GITHUB_LATEST_URL);
            //        Application.Exit();
            //    }
            //    else if (dialogResult == DialogResult.No)
            //    {
            //        // Ignore the update until next open.
            //    }
            //}
        }

        private void OpenSettings_Click(object sender, EventArgs e)
        {
            Globals.SettingsForm.ShowDialog(this);
        }

        private void AboutLabel_Click(object sender, EventArgs e)
        {
            Globals.AboutForm.ShowDialog();
        }

        private void AltLoginLabel_Click(object sender, EventArgs e)
        {
            if (altLoginlbl.Text == "Can't login? Click here")
            {
                // Set value if alt login is needed.
                AltLoginValue = "1";

                // Change alt login label text
                altLoginlbl.Text = "Login normally? Click here";

                // Hide standard login methods
                emailTextbox.Visible = false;
                passwordTextbox.Visible = false;

                // Unhide alt login methods
                //altLoginTutLabel.Visible = true;
                userIdTextbox.Visible = true;
                userAuthTokenTextbox.Visible = true;
            }
            else
            {
                // Set value if alt login is not needed.
                AltLoginValue = "0";

                // Change alt login label text
                altLoginlbl.Text = "Can't login? Click here";

                // Hide alt login methods
                //altLoginTutLabel.Visible = false;
                userIdTextbox.Visible = false;
                userAuthTokenTextbox.Visible = false;

                // Unhide standard login methods
                emailTextbox.Visible = true;
                passwordTextbox.Visible = true;
            }
        }

        //private void AltLoginTutLabel_Click(object sender, EventArgs e)
        //{
        //    Process.Start(Globals.GITHUB_ALT_LOGIN_TUTORIAL_URL);
        //}

        private void ExitLabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FinishLogin(object sender, EventArgs e)
        {
            loginButton.Invoke(new Action(() => loginButton.Enabled = true));
            altLoginlbl.Invoke(new Action(() => altLoginlbl.Visible = true));

            // Login successful, create main forms
            Globals.QbdlxForm = new QobuzDownloaderX();
            Globals.SearchForm = new SearchForm();

            this.Invoke(new Action(() => this.Hide()));
            Application.Run(Globals.QbdlxForm);
        }

        private void LoginBG_DoWork(object sender, DoWorkEventArgs e)
        {
            loginBG.WorkerSupportsCancellation = true;

            String appId = Settings.Default.appId;
            String appSecret = Settings.Default.appSecret;
            bool useCustomAppIdAndSecret = !string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(appSecret);

            // Initialize QobuzApiServiceManager with default Web Player AppId and AppSecret if not set in settings
            // Otherwise, use the provided app_id & app_secret
            try
            {
                if (useCustomAppIdAndSecret)
                {
                    QobuzApiServiceManager.Initialize(appId, appSecret);
                }
                else
                {
                    // Dynamic retrieval of app_id & app_secret in QobuzApiService were valid as of bundle-7.2.0-b082e.js
                    QobuzApiServiceManager.Initialize();
                }
            }
            catch (Exception ex)
            {
                string errorMessage;

                switch (ex)
                {
                    case QobuzApiInitializationException _:
                        errorMessage = $"{ex.Message} Error Log saved";
                        break;
                    case ArgumentException _:
                        errorMessage = $"{ex.Message} Error Log saved";
                        break;
                    default:
                        errorMessage = "Unknown error initializing API connection. Error Log saved";
                        break;
                }

                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = errorMessage));
                System.IO.File.AppendAllText(loginErrorLog, ex.ToString());

                loginButton.Invoke(new Action(() => loginButton.Enabled = true));
                altLoginlbl.Invoke(new Action(() => altLoginlbl.Visible = true));
                return;
            }

            if (useCustomAppIdAndSecret)
            {
                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "Using custom App ID and Secret! Logging in..."));
            } else
            {
                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "ID and Secret Obtained! Logging in..."));
            }

            try
            {
                if (AltLoginValue == "0")
                {
                    Globals.Login = QobuzApiServiceManager.GetApiService().LoginWithEmail(emailTextbox.Text, passwordTextbox.Text);
                }
                else if (AltLoginValue == "1")
                {
                    Globals.Login = QobuzApiServiceManager.GetApiService().LoginWithToken(userIdTextbox.Text, userAuthTokenTextbox.Text);
                }
            }
            catch (Exception ex)
            {
                // If connection to API fails, or something is incorrect, show error info + log details.
                List<string> errorLines = new List<string>();

                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "Login Failed. Error Log saved"));

                switch (ex)
                {
                    case ApiErrorResponseException erEx:
                        errorLines.Add($"Failed API request: \r\n{erEx.RequestContent}");
                        errorLines.Add($"Api response code: {erEx.ResponseStatusCode}");
                        errorLines.Add($"Api response status: {erEx.ResponseStatus}");
                        errorLines.Add($"Api response reason: {erEx.ResponseReason}");
                        break;
                    case ApiResponseParseErrorException pEx:
                        errorLines.Add("Error parsing API response");
                        errorLines.Add($"Api response content: {pEx.ResponseContent}");
                        break;
                    default:
                        errorLines.Add($"{ex}");
                        break;
                }

                // Write detailed info to log
                System.IO.File.AppendAllLines(loginErrorLog, errorLines);
                loginButton.Invoke(new Action(() => loginButton.Enabled = true));
                altLoginlbl.Invoke(new Action(() => altLoginlbl.Visible = true));
                return;
            }

            if (!QobuzApiServiceManager.GetApiService().IsAppSecretValid())
            {
                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "Invalid App Credentials Obtained, Results logged."));
                System.IO.File.AppendAllText(loginErrorLog, "Test stream failed with obtained App data.\r\n");
                System.IO.File.AppendAllText(loginErrorLog, $"Retrieved app_id: {QobuzApiServiceManager.GetApiService().AppId}\r\n");
                System.IO.File.AppendAllText(loginErrorLog, $"Retrieved app_secret: {QobuzApiServiceManager.GetApiService().AppSecret}\r\n");
                loginButton.Invoke(new Action(() => loginButton.Enabled = true));
                altLoginlbl.Invoke(new Action(() => altLoginlbl.Visible = true));
                return;
            }

            loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "Login Successful! Launching QBDLX..."));
            FinishLogin(sender, e);

            loginBG.CancelAsync();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Hide alt login label until job is finished or failed
            altLoginlbl.Visible = false;

            switch (AltLoginValue)
            {
                // If logging in normally (email & password)
                case "0":

                    #region Normal Login

                    #region Check if textboxes are valid

                    if (emailTextbox.Text == "Email" || string.IsNullOrEmpty(emailTextbox.Text?.Trim()))
                    {
                        // If there's no email typed in.
                        loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "No email, please input email first."));
                        return;
                    }

                    if (passwordTextbox.Text == "Password" || string.IsNullOrEmpty(passwordTextbox.Text?.Trim()))
                    {
                        // If there's no password typed in.
                        loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "No password typed, please input password first."));
                        return;
                    }

                    #endregion Check if textboxes are valid

                    // Trim entered email and password to help copy/paste dummies...
                    emailTextbox.Text = emailTextbox.Text.Trim();
                    passwordTextbox.Text = passwordTextbox.Text.Trim();

                    string plainTextPW = passwordTextbox.Text;

                    var passMD5CheckLog = Regex.Match(plainTextPW, "(?<md5Test>^[0-9a-f]{32}$)").Groups;
                    var passMD5Check = passMD5CheckLog[1].Value;

                    if (string.IsNullOrEmpty(passMD5Check))
                    {
                        // Generate the MD5 hash using the string created above.
                        using (MD5 md5PassHash = MD5.Create())
                        {
                            string hashedPW = MD5Tools.GetMd5Hash(md5PassHash, plainTextPW);

                            if (MD5Tools.VerifyMd5Hash(md5PassHash, plainTextPW, hashedPW))
                            {
                                // If the MD5 hash is verified, proceed to get the streaming URL.
                                passwordTextbox.Text = hashedPW;
                            }
                            else
                            {
                                // If the hash can't be verified.
                                loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "Hashing failed. Please retry."));
                                return;
                            }
                        }
                    }

                    // Save info locally to be used on next launch.
                    Settings.Default.savedEmail = emailTextbox.Text;
                    Settings.Default.savedPassword = passwordTextbox.Text;
                    Settings.Default.savedAltLoginValue = AltLoginValue;
                    Settings.Default.Save();

                    #endregion Normal Login

                    break;

                default:

                    #region Alt Login

                    #region Check if textboxes are valid

                    if (userIdTextbox.Text == "user_id" || string.IsNullOrEmpty(userIdTextbox.Text?.Trim()))
                    {
                        // If there's no user_id  typed in.
                        loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "No user_id, please input user_id first."));
                        return;
                    }

                    if (userAuthTokenTextbox.Text == "user_auth_token" || string.IsNullOrEmpty(userAuthTokenTextbox.Text?.Trim()))
                    {
                        // If there's no password typed in.
                        loginTextlbl.Invoke(new Action(() => loginTextlbl.Text = "No user_auth_token typed, please input user_auth_token first."));
                        return;
                    }

                    #endregion Check if textboxes are valid

                    // Trim entered user_id and user_auth_token to help copy/paste dummies...
                    userIdTextbox.Text = userIdTextbox.Text.Trim();
                    userAuthTokenTextbox.Text = userAuthTokenTextbox.Text.Trim();

                    // Save info locally to be used on next launch.
                    Settings.Default.savedUserID = userIdTextbox.Text;
                    Settings.Default.savedUserAuthToken = userAuthTokenTextbox.Text;
                    Settings.Default.savedAltLoginValue = AltLoginValue;
                    Settings.Default.Save();

                    #endregion Alt Login

                    break;
            }

            loginButton.Enabled = false;
            loginTextlbl.Text = "Getting App ID and Secret...";
            loginBG.RunWorkerAsync();
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #region Textbox Focous & Text Change

        #region Email Textbox

        private void EmailTextbox_Click(object sender, EventArgs e)
        {
            if (emailTextbox.Text == "Email")
            {
                emailTextbox.Text = null;
                emailTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
        }

        private void EmailTextbox_Leave(object sender, EventArgs e)
        {
            if (emailTextbox.Text == null || emailTextbox.Text == "")
            {
                emailTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                emailTextbox.Text = "Email";
            }
        }

        #endregion Email Textbox

        #region Password Textbox

        private void PasswordTextbox_Click(object sender, EventArgs e)
        {
            if (passwordTextbox.Text == "Password")
            {
                passwordTextbox.Text = null;
                passwordTextbox.PasswordChar = '*';
                passwordTextbox.UseSystemPasswordChar = false;
                passwordTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
        }

        private void PasswordTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void PasswordTextbox_Leave(object sender, EventArgs e)
        {
            if (passwordTextbox.Text == null || passwordTextbox.Text == "")
            {
                passwordTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                passwordTextbox.UseSystemPasswordChar = true;
                passwordTextbox.Text = "Password";
            }
        }

        #endregion Password Textbox

        #region user_id Textbox

        private void UserIdTextbox_Click(object sender, EventArgs e)
        {
            if (userIdTextbox.Text == "user_id")
            {
                userIdTextbox.Text = null;
                userIdTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
        }

        private void UserIdTextbox_Leave(object sender, EventArgs e)
        {
            if (userIdTextbox.Text == null || userIdTextbox.Text == "")
            {
                userIdTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                userIdTextbox.Text = "user_id";
            }
        }

        #endregion user_id Textbox

        #region user_auth_token Textbox

        private void UserAuthTokenTextbox_Click(object sender, EventArgs e)
        {
            if (userAuthTokenTextbox.Text == "user_auth_token")
            {
                userAuthTokenTextbox.Text = null;
                userAuthTokenTextbox.PasswordChar = '*';
                userAuthTokenTextbox.UseSystemPasswordChar = false;
                userAuthTokenTextbox.ForeColor = Color.FromArgb(186, 186, 186);
            }
        }

        private void UserAuthTokenTextbox_Leave(object sender, EventArgs e)
        {
            if (userAuthTokenTextbox.Text == null || userAuthTokenTextbox.Text == "")
            {
                userAuthTokenTextbox.ForeColor = Color.FromArgb(88, 92, 102);
                userAuthTokenTextbox.UseSystemPasswordChar = true;
                userAuthTokenTextbox.Text = "user_auth_token";
            }
        }

        #endregion user_auth_token Textbox

        #endregion Textbox Focous & Text Change

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void VerNumLabel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void VisibleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (visableCheckbox.Checked)
            {
                passwordTextbox.UseSystemPasswordChar = true;
                userAuthTokenTextbox.UseSystemPasswordChar = true;
            }
            else
            {
                passwordTextbox.UseSystemPasswordChar = false;
                userAuthTokenTextbox.UseSystemPasswordChar = false;
            }
        }
    }
}