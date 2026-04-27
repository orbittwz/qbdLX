using QobuzApiSharp.Models.Content;
using QobuzDownloaderX.Shared;
using System.Collections.Generic;
using System.Linq;

namespace QobuzDownloaderX.Models
{
    public class DownloadItemInfo
    {
        public string DisplayQuality { get; set; }
        public string DownloadItemID { get; set; }
        //public string Stream { get; set; }
        public DownloadItemPaths CurrentDownloadPaths { get; set; }
        private string TrackVersionName { get; set; }
        public bool? Advisory { get; set; }
        public string AlbumArtist { get; set; }
        public string[] AlbumArtists { get; set; }
        public string AlbumName { get; set; }
        public string PerformerName { get; set; }
        public string[] PerformerNames { get; set; }
        public string ComposerName { get; set; }
        public string[] ComposerNames { get; set; }
        public string ProducerName { get; set; }
        public string[] ProducerNames { get; set; }
        public string LabelName { get; set; }
        public string InvolvedPeople { get; set; }
        public string TrackName { get; set; }
        public string Copyright { get; set; }
        public string Genre { get; set; }
        public string ReleaseDate { get; set; }
        public string Isrc { get; set; }
        public string Upc { get; set; }
        public string FrontCoverImgUrl { get; set; }
        public string FrontCoverImgTagUrl { get; set; }
        public string FrontCoverImgBoxUrl { get; set; }
        public string MediaType { get; set; }
        public string Url { get; set; }
        public int DiscNumber { get; set; }
        public int DiscTotal { get; set; }
        public int TrackNumber { get; set; }
        public int TrackTotal { get; set; }
        public long Duration { get; set; }

        public DownloadItemInfo()
        {
            CurrentDownloadPaths = new DownloadItemPaths();
        }

        public void SetAlbumDownloadInfo(Album qobuzAlbum)
        {
            SetAlbumCoverArtUrls(qobuzAlbum);
            SetAlbumTaggingInfo(qobuzAlbum);
            SetAlbumPaths(qobuzAlbum);
        }

        private void ClearAlbumTaggingInfo()
        {
            // Clear tag strings
            AlbumArtist = null;
            AlbumArtists = null;
            AlbumName = null;
            LabelName = null;
            Genre = null;
            ReleaseDate = null;
            Upc = null;
            MediaType = null;
            Url = null;
            // Clear tag numbers
            TrackTotal = 0;
            DiscTotal = 0;
            // Clear tag based Paths
            CurrentDownloadPaths.AlbumArtistPath = null;
            CurrentDownloadPaths.AlbumNamePath = null;
        }

        private void ClearTrackTaggingInfo()
        {
            // Clear tag strings
            PerformerName = null;
            PerformerNames = null;
            ComposerName = null;
            ComposerNames = null;
            ProducerName = null;
            ProducerNames = null;
            InvolvedPeople = null;
            TrackName = null;
            TrackVersionName = null;
            Advisory = null;
            Copyright = null;
            Isrc = null;
            // Clear tag numbers
            TrackNumber = 0;
            DiscNumber = 0;
            Duration = 0;
            // Clear tag based Paths
            CurrentDownloadPaths.TrackNamePath = null;
        }

        private void SetAlbumCoverArtUrls(Album qobuzAlbum)
        {
            // Grab cover art link
            FrontCoverImgUrl = qobuzAlbum.Image.Large;
            // Get 150x150 artwork for cover art box
            FrontCoverImgBoxUrl = FrontCoverImgUrl.Replace("_600.jpg", "_150.jpg");
            // Get selected artwork size for tagging
            FrontCoverImgTagUrl = FrontCoverImgUrl.Replace("_600.jpg", $"_{Globals.TaggingOptions.ArtSize}.jpg");
            // Get max sized artwork ("_org.jpg" is compressed version of the original "_org.jpg")
            FrontCoverImgUrl = FrontCoverImgUrl.Replace("_600.jpg", "_org.jpg");
        }

        // Set Album tagging info
        private void SetAlbumTaggingInfo(Album qobuzAlbum)
        {
            ClearAlbumTaggingInfo();
            AlbumArtists = GetArtistNames(qobuzAlbum.Artists, InvolvedPersonRoleType.MainArtist);
            string[] featuredArtists = GetArtistNames(qobuzAlbum.Artists, InvolvedPersonRoleType.FeaturedArtist);
            string albumArtists = MergeFeaturedArtistsWithMainArtists(AlbumArtists, featuredArtists);
            // Add Features Artists to Album Artists.
            AlbumArtists = AlbumArtists.Concat(featuredArtists).ToArray();
            if (!string.IsNullOrEmpty(albumArtists) && Globals.TaggingOptions.MergePerformers)
                AlbumArtist = albumArtists; // User Main-Artists by default
            else
                AlbumArtist = StringTools.DecodeEncodedNonAsciiCharacters(qobuzAlbum.Artist.Name);
            // Qobuz doesn't return an array of album artists for compilations, so use singular AlbumArtist
            if (AlbumArtists.Length < 1)
                AlbumArtists = new string[] { AlbumArtist };
            AlbumName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzAlbum.Title.Trim());
            string albumVersionName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzAlbum.Version?.Trim());
            // Add album version to AlbumName if present
            AlbumName += (albumVersionName == null ? "" : " (" + albumVersionName + ")");
            LabelName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzAlbum.Label.Name);
            Genre = StringTools.DecodeEncodedNonAsciiCharacters(qobuzAlbum.Genre.Name);
            ReleaseDate = StringTools.FormatDateTimeOffset(qobuzAlbum.ReleaseDateStream);
            Upc = qobuzAlbum.Upc;
            MediaType = qobuzAlbum.ReleaseType;
            Url = qobuzAlbum.Url;
            // Grab tag ints
            TrackTotal = qobuzAlbum.TracksCount.GetValueOrDefault();
            DiscTotal = qobuzAlbum.MediaCount.GetValueOrDefault();
        }

        // Set Album tag based Paths
        private void SetAlbumPaths(Album qobuzAlbum)
        {
            // Grab sample rate and bit depth for album.
            (string displayQuality, string qualityPathLocal) = QualityStringMappings.GetQualityStrings(Globals.FormatIdString, qobuzAlbum);
            DisplayQuality = displayQuality;
            CurrentDownloadPaths.QualityPath = qualityPathLocal;
            // If AlbumArtist or AlbumName goes over set MaxLength number of characters, limit them to the MaxLength
            CurrentDownloadPaths.AlbumArtistPath = StringTools.TrimToMaxLength(StringTools.GetSafeFilename(AlbumArtist), Globals.MaxLength);
            CurrentDownloadPaths.AlbumNamePath = StringTools.TrimToMaxLength(StringTools.GetSafeFilename(AlbumName), Globals.MaxLength);
        }

        // Set Track tagging info
        public void SetTrackTaggingInfo(Track qobuzTrack)
        {
            ClearTrackTaggingInfo();
            PerformersParser performersParser = new PerformersParser(qobuzTrack.Performers);
            PerformerNames = performersParser.GetPerformersWithRole(InvolvedPersonRoleType.MainArtist);
            string[] featuredArtists = performersParser.GetPerformersWithRole(InvolvedPersonRoleType.FeaturedArtist);
            string trackArtists = MergeFeaturedArtistsWithMainArtists(PerformerNames, featuredArtists);
            // Add Features Artists to Album Artists.
            PerformerNames = PerformerNames.Concat(featuredArtists).ToArray();
            if (!string.IsNullOrEmpty(trackArtists) && Globals.TaggingOptions.MergePerformers)
                PerformerName = trackArtists; // User MainArtist Performers by default
            else
                PerformerName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Performer?.Name);
            // If no performer name, use album artist
            if (string.IsNullOrEmpty(PerformerName))
                PerformerName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Album?.Artist?.Name);
            // Qobuz could return an unknown role for the Track Artists. Use singular PerformerName as fallback
            if (PerformerNames.Length < 1)
                PerformerNames = new string[] { PerformerName };
            ComposerNames = performersParser.GetPerformersWithRole(InvolvedPersonRoleType.Composer).ToArray();
            string composers = StringTools.MergeDoubleDelimitedList(ComposerNames, Globals.TaggingOptions.PrimaryListSeparator, Globals.TaggingOptions.ListEndSeparator);
            if (!string.IsNullOrEmpty(composers) && Globals.TaggingOptions.MergePerformers)
                ComposerName = composers;
            else
                ComposerName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Composer?.Name);
            // Qobuz could return an unknown role for the Composers. Use singular ComposerName as fallback
            if (ComposerNames.Length < 1)
                ComposerNames = new string[] { ComposerName };
            ProducerNames = performersParser.GetPerformersWithRole(InvolvedPersonRoleType.Producer).ToArray();
            ProducerName = StringTools.MergeDoubleDelimitedList(ProducerNames, Globals.TaggingOptions.PrimaryListSeparator, Globals.TaggingOptions.ListEndSeparator);
            InvolvedPeople = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Performers);
            TrackName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Title.Trim());
            TrackVersionName = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Version?.Trim());
            // Add track version to TrackName
            TrackName += (TrackVersionName == null ? "" : " (" + TrackVersionName + ")");
            Advisory = qobuzTrack.ParentalWarning;
            Copyright = StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Copyright);
            Isrc = qobuzTrack.Isrc;
            // Grab tag numbers
            TrackNumber = qobuzTrack.TrackNumber.GetValueOrDefault();
            DiscNumber = qobuzTrack.MediaNumber.GetValueOrDefault();
            Duration = qobuzTrack.Duration.GetValueOrDefault();
            // Set Track tag based Paths
            CurrentDownloadPaths.PerformerNamePath = StringTools.TrimToMaxLength(StringTools.GetSafeFilename(PerformerName), Globals.MaxLength);
            CurrentDownloadPaths.TrackNamePath = StringTools.GetSafeFilename(TrackName);
        }

        private string[] GetArtistNames(List<Artist> artists, InvolvedPersonRoleType role)
        {
            return artists.Where(artist => artist.Roles.Exists(roleString => InvolvedPersonRoleMapping.GetRoleByString(roleString) == role))
                .Select(artist => artist.Name)
                .ToArray();
        }

        private string MergeFeaturedArtistsWithMainArtists(string[] mainArtists, string[] featuresArtists)
        {
            string mergedMainArtists = StringTools.MergeDoubleDelimitedList(mainArtists,
                Globals.TaggingOptions.PrimaryListSeparator, Globals.TaggingOptions.ListEndSeparator);
            string mergedFeaturedArtists = StringTools.MergeDoubleDelimitedList(featuresArtists,
                Globals.TaggingOptions.PrimaryListSeparator, Globals.TaggingOptions.ListEndSeparator);
            if (string.IsNullOrEmpty(mergedFeaturedArtists))
                return mergedMainArtists;
            else
                return $"{mergedMainArtists} Feat. {mergedFeaturedArtists}";
        }
    }
}