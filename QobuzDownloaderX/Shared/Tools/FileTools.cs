using System.IO;
using System;
using System.Windows.Forms;

namespace QobuzDownloaderX.Shared.Tools
{
    internal static class FileTools
    {
        private static void DeleteFilesByAge(string folderPath, int maxAgeInDays)
        {
            if (!Directory.Exists(folderPath))
                return;
            DateTime thresholdDate = DateTime.Now.AddDays(-maxAgeInDays);
            foreach (var file in Directory.GetFiles(folderPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTime < thresholdDate)
                    File.Delete(file);
            }
        }

        public static string GetInitializedLogDir()
        {
            string logDirPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logs");
            if (!System.IO.Directory.Exists(logDirPath))
                System.IO.Directory.CreateDirectory(logDirPath);
            DeleteFilesByAge(logDirPath, 1);
            return logDirPath;
        }

        public static string GetInitializedSettingsDir()
        {
            string settingsDirPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "settings");
            if (!System.IO.Directory.Exists(settingsDirPath))
                System.IO.Directory.CreateDirectory(settingsDirPath);
            return settingsDirPath;
        }
    }
}