using QobuzDownloaderX.Models.Download;
using QobuzDownloaderX.Properties;
using System;
using TagLib;

namespace QobuzDownloaderX.Shared.Tools
{
    public static class AudioFileTagger
    {
        // Add Metadata to audio files in ID3v2 for mp3 and Vorbis Comment for FLAC
        public static void AddMetaDataTags(DownloadItemInfo fileInfo, string tagFilePath, string tagCoverArtFilePath, DownloadLogger logger)
        {
            // Set file to tag
            var tfile = TagLib.File.Create(tagFilePath);
            tfile.RemoveTags(TagTypes.Id3v1);
            // Use ID3v2.4 as default mp3 tag version
            TagLib.Id3v2.Tag.DefaultVersion = 4;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            //todo Synthesize the two cases (flac and mp3) to one, differ just the changes between the formats.
            //todo Instead of switch, do IF checks for the format.
            switch (Settings.Default.audioType)
            {
                case ".mp3":
                    // For custom / troublesome tags.
                    TagLib.Id3v2.Tag customId3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagTypes.Id3v2, true);
                    // Saving cover art to file(s)
                    if (Settings.Default.imageTag)
                    {
                        try
                        {
                            // Define cover art to use for MP3 file(s)
                            TagLib.Id3v2.AttachmentFrame pic = new TagLib.Id3v2.AttachmentFrame
                            {
                                TextEncoding = TagLib.StringType.Latin1,
                                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                                Type = TagLib.PictureType.FrontCover,
                                Data = TagLib.ByteVector.FromPath(tagCoverArtFilePath)
                            };
                            // Save cover art to MP3 file.
                            tfile.Tag.Pictures = new TagLib.IPicture[1] { pic };
                            tfile.Save();
                        }
                        catch
                        {
                            logger.AddDownloadLogErrorLine($"Cover art tag failed, .jpg still exists?...{Environment.NewLine}", true, true);
                        }
                    }
                    // Track Title tag, version is already added to name if available
                    if (Settings.Default.trackTitleTag)
                        tfile.Tag.Title = fileInfo.TrackName;
                    // Album Title tag, version is already added to name if available
                    if (Settings.Default.albumTag)
                        tfile.Tag.Album = fileInfo.AlbumName;
                    // Album Artist tag
                    if (Settings.Default.albumArtistTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.AlbumArtists = new string[] { fileInfo.AlbumArtist };
                        else
                            tfile.Tag.AlbumArtists = fileInfo.AlbumArtists;
                    }
                    // Track Artist tag
                    if (Settings.Default.artistTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.Performers = new string[] { fileInfo.PerformerName };
                        else
                            tfile.Tag.Performers = fileInfo.PerformerNames;
                    }
                    // Composer tag
                    if (Settings.Default.composerTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.Composers = new string[] { fileInfo.ComposerName };
                        else
                            tfile.Tag.Composers = fileInfo.ComposerNames;
                    }
                    // Label tag
                    if (Settings.Default.labelTag)
                        tfile.Tag.Publisher = fileInfo.LabelName;
                    // InvolvedPeople tag
                    if (Settings.Default.involvedPeopleTag)
                        customId3v2.SetTextFrame("TIPL", fileInfo.InvolvedPeople);
                    // Release Year tag (writes to "TDRC" (recording date) Frame)
                    if (Settings.Default.yearTag)
                        tfile.Tag.Year = UInt32.Parse(fileInfo.ReleaseDate.Substring(0, 4));
                    // Release Date tag (use "TDRL" (release date) Frame for full date)
                    if (Settings.Default.releaseDateTag)
                        customId3v2.SetTextFrame("TDRL", fileInfo.ReleaseDate);
                    // Genre tag
                    if (Settings.Default.genreTag)
                        tfile.Tag.Genres = new string[] { fileInfo.Genre };
                    // Disc Number tag
                    if (Settings.Default.discTag)
                        tfile.Tag.Disc = Convert.ToUInt32(fileInfo.DiscNumber);
                    // Total Discs tag
                    if (Settings.Default.totalDiscsTag)
                        tfile.Tag.DiscCount = Convert.ToUInt32(fileInfo.DiscTotal);
                    // Total Tracks tag
                    if (Settings.Default.totalTracksTag)
                        tfile.Tag.TrackCount = Convert.ToUInt32(fileInfo.TrackTotal);
                    // Track Number tag
                    // !! Set Track Number after Total Tracks to prevent taglib-sharp from re-formatting the field to a "two-digit zero-filled value" !!
                    if (Settings.Default.trackTag)
                    {
                        // Set TRCK tag manually to prevent using "two-digit zero-filled value"
                        // See https://github.com/mono/taglib-sharp/pull/240 where this change was introduced in taglib-sharp v2.3
                        // Original command: tfile.Tag.Track = Convert.ToUInt32(TrackNumber);
                        customId3v2.SetNumberFrame("TRCK", Convert.ToUInt32(fileInfo.TrackNumber), tfile.Tag.TrackCount);
                    }
                    // Comment tag
                    if (Settings.Default.commentTag)
                        tfile.Tag.Comment = Settings.Default.commentText;
                    // Copyright tag
                    if (Settings.Default.copyrightTag)
                        tfile.Tag.Copyright = fileInfo.Copyright;
                    // ISRC tag
                    if (Settings.Default.isrcTag)
                        tfile.Tag.ISRC = fileInfo.Isrc;
                    // Release Type tag
                    if (fileInfo.MediaType != null && Settings.Default.typeTag)
                        customId3v2.SetTextFrame("TMED", fileInfo.MediaType);
                    // Album store URL tag
                    if (fileInfo.Url != null && Settings.Default.urlTag)
                    {
                        customId3v2.SetTextFrame("WCOM", Globals.Login.User.Store == "fr-fr" ? fileInfo.Url : ("https://www.qobuz.com/" + Globals.Login.User.Store
                            + fileInfo.Url.Substring(fileInfo.Url.IndexOf("/album"))).ToLower());
                    }
                    // Save all selected tags to file
                    tfile.Save();
                    break;

                case ".flac":
                    // For custom / troublesome tags.
                    TagLib.Ogg.XiphComment custom = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph);
                    // Saving cover art to file(s)
                    if (Settings.Default.imageTag)
                    {
                        try
                        {
                            // Define cover art to use for FLAC file(s)
                            TagLib.Id3v2.AttachmentFrame pic = new TagLib.Id3v2.AttachmentFrame
                            {
                                TextEncoding = TagLib.StringType.Latin1,
                                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                                Type = TagLib.PictureType.FrontCover,
                                Data = TagLib.ByteVector.FromPath(tagCoverArtFilePath)
                            };
                            // Save cover art to FLAC file.
                            tfile.Tag.Pictures = new TagLib.IPicture[1] { pic };
                            tfile.Save();
                        }
                        catch
                        {
                            logger.AddDownloadLogErrorLine($"Cover art tag failed, .jpg still exists?...{Environment.NewLine}", true, true);
                        }
                    }
                    // Track Title tag, version is already added to name if available
                    if (Settings.Default.trackTitleTag)
                        tfile.Tag.Title = fileInfo.TrackName;
                    // Album Title tag, version is already added to name if available
                    if (Settings.Default.albumTag)
                        tfile.Tag.Album = fileInfo.AlbumName;
                    // Album Artist tag
                    if (Settings.Default.albumArtistTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.AlbumArtists = new string[] { fileInfo.AlbumArtist };
                        else
                            tfile.Tag.AlbumArtists = fileInfo.AlbumArtists;
                    }
                    // Track Artist tag
                    if (Settings.Default.artistTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.Performers = new string[] { fileInfo.PerformerName };
                        else
                            tfile.Tag.Performers = fileInfo.PerformerNames;
                    }
                    // Composer tag
                    if (Settings.Default.composerTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            tfile.Tag.Composers = new string[] { fileInfo.ComposerName };
                        else
                            tfile.Tag.Composers = fileInfo.ComposerNames;
                    }
                    // Label tag
                    if (Settings.Default.labelTag)
                    {
                        tfile.Tag.Publisher = fileInfo.LabelName; // Writes to the official ORGANIZATION field
                        custom.SetField("LABEL", fileInfo.LabelName);
                    }
                    // Producer tag
                    if (Settings.Default.producerTag)
                    {
                        if (Settings.Default.mergePerformersTag)
                            custom.SetField("PRODUCER", fileInfo.ProducerName);
                        else
                            custom.SetField("PRODUCER", fileInfo.ProducerNames);
                    }
                    // InvolvedPeople tag
                    if (Settings.Default.involvedPeopleTag)
                        custom.SetField("INVOLVEDPEOPLE", fileInfo.InvolvedPeople);
                    // Release Year tag (The "tfile.Tag.Year" field actually writes to the DATE tag, so use custom tag)
                    if (Settings.Default.yearTag)
                        custom.SetField("YEAR", fileInfo.ReleaseDate.Substring(0, 4));
                    // Release Date tag
                    if (Settings.Default.releaseDateTag)
                        custom.SetField("DATE", fileInfo.ReleaseDate);
                    // Genre tag
                    if (Settings.Default.genreTag)
                        tfile.Tag.Genres = new string[] { fileInfo.Genre };
                    // Track Number tag
                    if (Settings.Default.trackTag)
                    {
                        tfile.Tag.Track = Convert.ToUInt32(fileInfo.TrackNumber);
                        // Override TRACKNUMBER tag again to prevent using "two-digit zero-filled value"
                        // See https://github.com/mono/taglib-sharp/pull/240 where this change was introduced in taglib-sharp v2.3
                        custom.SetField("TRACKNUMBER", Convert.ToUInt32(fileInfo.TrackNumber));
                    }
                    // Disc Number tag
                    if (Settings.Default.discTag)
                        tfile.Tag.Disc = Convert.ToUInt32(fileInfo.DiscNumber);
                    // Total Discs tag
                    if (Settings.Default.totalDiscsTag)
                        tfile.Tag.DiscCount = Convert.ToUInt32(fileInfo.DiscTotal);
                    // Total Tracks tag
                    if (Settings.Default.totalTracksTag)
                        tfile.Tag.TrackCount = Convert.ToUInt32(fileInfo.TrackTotal);
                    // Comment tag
                    if (Settings.Default.commentTag)
                        tfile.Tag.Comment = Settings.Default.commentText;
                    // Copyright tag
                    if (Settings.Default.copyrightTag)
                        tfile.Tag.Copyright = fileInfo.Copyright;
                    // UPC tag
                    if (Settings.Default.upcTag)
                        custom.SetField("UPC", fileInfo.Upc);
                    // ISRC tag
                    if (Settings.Default.isrcTag)
                        tfile.Tag.ISRC = fileInfo.Isrc;
                    // Release Type tag
                    if (fileInfo.MediaType != null && Settings.Default.typeTag)
                        custom.SetField("MEDIATYPE", fileInfo.MediaType);
                    // Explicit tag
                    if (Settings.Default.explicitTag)
                    {
                        if (fileInfo.Advisory == true)
                            custom.SetField("ITUNESADVISORY", "1");
                        else
                            custom.SetField("ITUNESADVISORY", "0");
                    }
                    // Album store URL tag
                    if (fileInfo.Url != null && Settings.Default.urlTag)
                    {
                        custom.SetField("URL", Globals.Login.User.Store == "fr-fr" ? fileInfo.Url : ("https://www.qobuz.com/" + Globals.Login.User.Store
                            + fileInfo.Url.Substring(fileInfo.Url.IndexOf("/album"))).ToLower());
                    }
                    // Save all selected tags to file
                    tfile.Save();
                    break;
            }
        }
    }
}