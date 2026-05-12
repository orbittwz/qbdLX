using QobuzDownloaderX.Models.Download;
using System.Linq;
using System.Text.RegularExpressions;

namespace QobuzDownloaderX.Shared.Tools
{
    public static class DownloadUrlParser
    {
        // Pre-compiled supported download URL patterns
        private static readonly Regex[] DownloadUrlRegExes = {
            new Regex("https:\\/\\/(?:.*?).qobuz.com\\/(?<Type>.*?)\\/(?<id>.*?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("https:\\/\\/(?:.*?).qobuz.com\\/(?:.*?)\\/(?<Type>.*?)\\/(?<Slug>.*?)\\/(?<AlbumsTag>download-streaming-albums)\\/(?<id>.*?)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex("https:\\/\\/(?:.*?).qobuz.com\\/(?:.*?)\\/(?<Type>.*?)\\/(?<Slug>.*?)\\/(?<id>.*?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)
        };
        // Supported types of links. "interpreter" = "artist" in store links
        private static readonly string[] LinkTypes = { "album", "track", "artist", "label", "user", "playlist", "interpreter" };

        public static DownloadItem ParseDownloadUrl(string downloadUrl)
        {
            DownloadItem downloadItem = new DownloadItem(downloadUrl);
            if (string.IsNullOrEmpty(downloadUrl))
                return downloadItem;
            foreach (Regex regEx in DownloadUrlRegExes)
            {
                Match matches = regEx.Match(downloadUrl);
                if (matches.Success)
                {
                    if (!LinkTypes.Contains(matches.Result("${Type}")))
                        continue;
                    // Valid Type found, set DownloadItem values
                    downloadItem.Type = matches.Result("${Type}");
                    downloadItem.Id = matches.Result("${id}")?.TrimEnd('/');
                    // In store links, "interpreter" = "artist"
                    if (downloadItem.Type == "interpreter")
                        downloadItem.Type = "artist";
                    break;
                }
            }
            return downloadItem;
        }
    }
}