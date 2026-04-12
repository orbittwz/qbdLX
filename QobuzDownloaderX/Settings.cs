namespace QobuzDownloaderX.Properties
{
    internal sealed partial class Settings
    {
        private static string VERSION = Utils.GetProgramVersion().ToString().Replace(".0.0.0", ".0");

        internal static string Version
        {
            get
            {
                string result = VERSION;
                int index = result.LastIndexOf(".0");
                result = result.Remove(index);
                return result;
            }
        }
    }
}