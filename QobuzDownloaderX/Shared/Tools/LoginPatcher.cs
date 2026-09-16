using System;
using System.IO;
using CefSharp;
using CefSharp.WinForms;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QobuzDownloaderX.Shared.Tools
{
    public class LoginPatcher : Form
    {
        private readonly string _email;
        private readonly string _pwd;
        private int _id;
        private string _token;

        public LoginPatcher(string email, string pwd)
        {
            this.Name = "LoginPatcher";
            this.Text = "qbdLX | Login Patcher";
            this._email = email;
            this._pwd = pwd;
            this.Width = 1280;
            this.Height = 1024;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Dock = DockStyle.Fill;
        }

        public string Token
        {
            get
            {
                return this._token;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
        }

        public async Task<bool> Fetch(int waitMs = 3000)
        {
            // Initialize CefSharp (once per application)
            CefRuntime.SubscribeAnyCpuAssemblyResolver();
            string cefCachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CEF",
                "qbdLX");
            var settings = new CefSettings
            {
                RootCachePath = cefCachePath,
                CachePath = Path.Combine(cefCachePath, "Cache")
            };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            var browser = new ChromiumWebBrowser("https://play.qobuz.com");
            this.Controls.Add(browser);
            await browser.WaitForInitialLoadAsync();
            await Task.Delay(waitMs);
            var clickLoginBtn = await browser.EvaluateScriptAsync(@"document.querySelector('button[type=""button""]') != null");
            if (clickLoginBtn.Success && clickLoginBtn.Result is bool && (bool)clickLoginBtn.Result)
            {
                browser.ExecuteScriptAsync("document.querySelector('button[type=\"button\"]').click();");
            }
            await Task.Delay(waitMs);
            string script = @"
                (function () {
                    var email = document.querySelector('input[name=""_username""]');
                    var password = document.querySelector('input[name=""_password""]');

                    if (!email || !password)
                        return false;

                    email.value = """ + this._email.Replace("\\", "\\\\").Replace("\"", "\\\"") + @""";
                    password.value = """ + this._pwd.Replace("\\", "\\\\").Replace("\"", "\\\"") + @""";

                    email.dispatchEvent(new Event('input', { bubbles: true }));
                    email.dispatchEvent(new Event('change', { bubbles: true }));

                    password.dispatchEvent(new Event('input', { bubbles: true }));
                    password.dispatchEvent(new Event('change', { bubbles: true }));

                    return true;
                })();
                ";
            await browser.EvaluateScriptAsync(script);
            await browser.EvaluateScriptAsync(@"
                (function () {
                    var button = document.querySelector('#login');

                    if (!button)
                        return false;

                    button.click();

                    return true;
                })();
                ");
            // Wait for successful login
            while (browser.Address.Contains("play.qobuz.com/discover") == false)
                await Task.Delay(waitMs);
            await Task.Delay(waitMs);
            var idResponse = await browser.EvaluateScriptAsync(
                "localStorage.getItem('localuser') ? JSON.parse(localStorage.getItem('localuser')).id : 0;"
            );
            var tokenResponse = await browser.EvaluateScriptAsync(
                "localStorage.getItem('localuser') ? JSON.parse(localStorage.getItem('localuser')).token : '';"
            );
            if (idResponse.Success && idResponse.Result != null)
                _id = Convert.ToInt32(idResponse.Result);
            if (tokenResponse.Success && tokenResponse.Result != null)
                _token = tokenResponse.Result.ToString();
            if (this._id > 0 && string.IsNullOrEmpty(this._token) == false)
            {
                this.Close();
                return true;
            }
            else
                return false;
        }
    }
}