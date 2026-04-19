namespace QobuzDownloaderX.Models.UI
{
    public class SearchResultRow
    {
        public string ThumbnailUrl { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public bool Explicit { get; set; }
        public string FormattedDuration { get; set; }
        public string WebPlayerUrl { get; set; }
        public string StoreUrl { get; set; }
        public int TrackCount { get; set; }
        public string ReleaseDate { get; set; }
        public bool HiRes { get; set; }
        public string FormattedQuality { get; set; }
        public string ResultType { get; set; }
    }
}