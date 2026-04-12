namespace QobuzDownloaderX.Shared
{
    internal class TaggingOptions
    {
        public bool WriteAlbumNameTag { get; set; }
        public bool WriteAlbumArtistTag { get; set; }
        public bool WriteTrackTitleTag { get; set; }
        public bool WriteTrackArtistTag { get; set; }
        public bool WriteTrackNumberTag { get; set; }
        public bool WriteTrackTotalTag { get; set; }
        public bool WriteDiskNumberTag { get; set; }
        public bool WriteDiskTotalTag { get; set; }
        public bool WriteReleaseYearTag { get; set; }
        public bool WriteReleaseDateTag { get; set; }
        public string WriteQualityTag { get; set; }
        public bool WriteGenreTag { get; set; }
        public bool WriteComposerTag { get; set; }
        public bool WriteCopyrightTag { get; set; }
        public bool WriteISRCTag { get; set; }
        public bool WriteMediaTypeTag { get; set; }
        public bool WriteUPCTag { get; set; }
        public bool WriteExplicitTag { get; set; }
        public bool WriteCommentTag { get; set; }
        public bool WriteCoverImageTag { get; set; }
        public bool WriteProducerTag { get; set; }
        public bool WriteLabelTag { get; set; }
        public bool WriteInvolvedPeopleTag { get; set; }
        public bool MergePerformers { get; set; }
        public bool WriteURLTag { get; set; }
        public string CommentTag { get; set; }
        public string ArtSize { get; set; }
        public string PrimaryListSeparator { get; set; }
        public string ListEndSeparator { get; set; }
        public bool WriteGetGoodiesTag { get; set; }
    }
}