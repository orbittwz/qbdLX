namespace QobuzDownloaderX.Models.Download
{
    public class DownloadItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }

        public DownloadItem(string url)
        {
            Url = url;
        }

        public bool IsEmpty()
        {
            return Type == null || Id == null;
        }
    }
}