using QobuzApiSharp.Models.User;

namespace QobuzDownloaderX.Shared
{
    internal static class Globals
    {
        public const string WEBPLAYER_BASE_URL = "https://play.qobuz.com";
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:150.0) Gecko/20100101 Firefox/150.0";
        public const string DEFAULT_COVER_ART_URL = "https://static.qobuz.com/images/covers/01/00/2013072600001_300.jpg";
        // Forms
        public static LoginForm LoginForm { get; set; }
        public static QobuzDownloaderX QbdlxForm { get; set; }
        public static SearchForm SearchForm { get; set; }
        public static SettingsForm SettingsForm { get; set; }
        // Login
        public static Login Login { get; set; }
        // Tagging options
        public static TaggingOptions TaggingOptions { get; set; }
        // Audio quality selection
        public static string FormatIdString { get; set; }
        public static string AudioFileType { get; set; }
        // Additional user selections
        public static int MaxLength { get; set; }
        // Logs
        public static string LoggingDir { get; set; }
    }
}