using System;
using System.Reflection;

namespace QobuzDownloaderX
{
    internal static class Utils
    {
        internal static Version GetProgramVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}