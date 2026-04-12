using Bluegrams.Application;
using QobuzDownloaderX.Shared;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace QobuzDownloaderX
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Make the default settings class portable
            PortableSettingsProviderBase.SettingsDirectory = FileTools.GetInitializedSettingsDir();
            // Global override of every setting "Roaming" property.
            PortableSettingsProviderBase.AllRoaming = true;
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);

            // Use en-US formatting everywhere for consistency
            var culture = CultureInfo.GetCultureInfo("en-US");

            //Culture for any thread
            CultureInfo.DefaultThreadCurrentCulture = culture;

            //Culture for UI in any thread
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create logging dir and clean older logs if present
            Globals.LoggingDir = FileTools.GetInitializedLogDir();

            // Initialise forms
            Globals.LoginForm = new LoginForm();
            Globals.AboutForm = new AboutForm();
            Globals.SettingsForm = new SettingsForm();

            // Register EventHandler to release resources on exit
            Application.ApplicationExit += ApplicationExit;

            Application.Run(Globals.LoginForm);
        }

        private static void ApplicationExit(object sender, EventArgs e)
        {
            QobuzApiServiceManager.ReleaseApiService();
        }
    }
}