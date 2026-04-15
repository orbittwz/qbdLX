using PlaylistsNET.Content;
using PlaylistsNET.Models;
using QobuzApiSharp.Exceptions;
using QobuzApiSharp.Models.Content;
using QobuzApiSharp.Service;
using QobuzDownloaderX.Models;
using QobuzDownloaderX.Models.Content;
using QobuzDownloaderX.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace QobuzDownloaderX.Shared
{
    public class DownloadManager
    {
        private readonly DownloadLogger logger;
        private CancellationTokenSource cancellationTokenSource;
        public delegate void DownloadTaskStatusChanged();
        public delegate void UpdateAlbumTagsUi(DownloadItemInfo downloadInfo);
        private readonly UpdateAlbumTagsUi UpdateAlbumUiTags;
        public delegate void UpdateDownloadSpeed(string speed);
        private readonly UpdateDownloadSpeed UpdateUiDownloadSpeed;
        public DownloadItemInfo DownloadInfo { get; private set; }
        public DownloadItemPaths DownloadPaths { get; private set; }
        public bool IsBusy { get; private set; }
        public bool CheckIfStreamable { get; set; }

        public DownloadManager(DownloadLogger logger, UpdateAlbumTagsUi updateAlbumTagsUi, UpdateDownloadSpeed updateUiDownloadSpeed)
        {
            IsBusy = false;
            CheckIfStreamable = true;
            this.logger = logger;
            UpdateUiDownloadSpeed = updateUiDownloadSpeed;
            UpdateAlbumUiTags = updateAlbumTagsUi;
        }

        private T ExecuteApiCall<T>(Func<QobuzApiService, T> apiCall)
        {
            try
            {
                return apiCall(QobuzApiServiceManager.GetApiService());
            }
            catch (Exception ex)
            {
                // If connection to API fails, or something is incorrect, show error info + log details.
                List<string> errorLines = new List<string>();
                logger.AddEmptyDownloadLogLine(false, true);
                logger.AddDownloadLogErrorLine($"Communication problem with Qobuz API. Details saved to error log{Environment.NewLine}", true, true);
                switch (ex)
                {
                    case ApiErrorResponseException erEx:
                        errorLines.Add("Failed API request:");
                        errorLines.Add(erEx.RequestContent);
                        errorLines.Add($"Api response code: {erEx.ResponseStatusCode}");
                        errorLines.Add($"Api response status: {erEx.ResponseStatus}");
                        errorLines.Add($"Api response reason: {erEx.ResponseReason}");
                        break;
                    case ApiResponseParseErrorException pEx:
                        errorLines.Add("Error parsing API response");
                        errorLines.Add($"Api response content: {pEx.ResponseContent}");
                        break;
                    default:
                        errorLines.Add("Unknown error trying API request:");
                        errorLines.Add($"{ex}");
                        break;
                }
                // Write detailed info to error log
                logger.AddDownloadErrorLogLines(errorLines);
            }
            return default;
        }

        public void StopDownloadTask()
        {
            this.cancellationTokenSource?.Cancel();
        }

        public bool IsStreamable(Track qobuzTrack, bool inPlaylist = false)
        {
            if (qobuzTrack.Streamable != false)
                return true;
            bool tryToStream = true;
            switch (CheckIfStreamable)
            {
                case true:
                    string trackReference;
                    if (inPlaylist)
                        trackReference = $"{qobuzTrack.Performer?.Name} - {qobuzTrack.Title}";
                    else
                        trackReference = $"{qobuzTrack.TrackNumber.GetValueOrDefault()} {StringTools.DecodeEncodedNonAsciiCharacters(qobuzTrack.Title.Trim())}";
                    logger.AddDownloadLogLine($"Track {trackReference} is not available for streaming. Unable to download.\r\n", true, true);
                    tryToStream = false;
                    break;

                default:
                    logger.AddDownloadLogLine("Track is not available for streaming. But streamable check is being ignored for debugging, or messed up releases. Attempting to download...\r\n",
                        tryToStream, tryToStream);
                    break;
            }
            return tryToStream;
        }

        public async Task DownloadFileAsync(HttpClient httpClient, string downloadUrl, string filePath)
        {
            using (Stream streamToReadFrom = await httpClient.GetStreamAsync(downloadUrl))
            {
                using (FileStream streamToWriteTo = System.IO.File.Create(filePath))
                {
                    long totalBytesRead = 0;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    byte[] buffer = new byte[32768]; // Use a 32KB buffer size for copying data
                    bool firstBufferRead = false;
                    int bytesRead;
                    while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        // Write only the minimum of buffer.Length and bytesRead bytes to the file
                        await streamToWriteTo.WriteAsync(buffer, 0, Math.Min(buffer.Length, bytesRead));
                        // Calculate download speed
                        totalBytesRead += bytesRead;
                        double speed = totalBytesRead / 1024d / 1024d / stopwatch.Elapsed.TotalSeconds;
                        // Update the downloadSpeedLabel with the current speed at download start and then max. every 100 ms, with 3 decimal places
                        if (!firstBufferRead || stopwatch.ElapsedMilliseconds >= 200)
                            UpdateUiDownloadSpeed.Invoke($"Downloading... {speed:F3} MB/s");
                        firstBufferRead = true;
                    }
                }
            }
            FileInfo fi = new FileInfo(filePath);
            if (File.Exists(filePath) && fi.Length == 0)
                File.Delete(filePath);
            // After download completes successfully
            UpdateUiDownloadSpeed.Invoke("Idle");
        }

        private async Task<bool> DownloadTrackAsync(CancellationToken cancellationToken, Track qobuzTrack, string basePath,
            bool isPartOfTracklist, bool isPartOfAlbum, bool removeTagArtFileAfterDownload = false, string albumPathSuffix = "")
        {
            // Just for good measure...
            // User requested task cancellation!
            cancellationToken.ThrowIfCancellationRequested();
            string trackIdString = qobuzTrack.Id.GetValueOrDefault().ToString();
            DownloadInfo.SetTrackTaggingInfo(qobuzTrack);
            // If track is downloaded as part of Album, Album related processes should already be done.
            // Only handle Album related processing when downloading a single track.
            if (!isPartOfAlbum)
            {
                // Get all album information and update UI fields via callback
                DownloadInfo.SetAlbumDownloadInfo(qobuzTrack.Album);
                UpdateAlbumUiTags.Invoke(DownloadInfo);
            }
            // Check if available for streaming.
            if (!IsStreamable(qobuzTrack))
                return false;
            // Create directories if they don't exist yet
            // Add Album ID to Album Path if requested (to avoid conflicts for similar albums with trimmed long names)
            CreateTrackDirectories(basePath, DownloadPaths.QualityPath, albumPathSuffix, isPartOfTracklist);
            // Set trackPath to the created directories
            string trackPath = DownloadInfo.CurrentDownloadPaths.Path4Full;
            // Create padded track number string with minimum of 2 integer positions based on number of total tracks
            string paddedTrackNumber = DownloadInfo.TrackNumber.ToString().PadLeft(Math.Max(2, (int)Math.Floor(Math.Log10(DownloadInfo.TrackTotal) + 1)), '0');
            // Create full track filename
            if (isPartOfTracklist)
                DownloadPaths.FinalTrackNamePath = string.Concat(DownloadPaths.PerformerNamePath, Globals.FileNameTemplateString, DownloadPaths.TrackNamePath).TrimEnd();
            else
                DownloadPaths.FinalTrackNamePath = string.Concat(paddedTrackNumber, Globals.FileNameTemplateString, DownloadPaths.TrackNamePath).TrimEnd();
            // Shorten full filename if over MaxLength to avoid errors with file names being too long
            DownloadPaths.FinalTrackNamePath = StringTools.TrimToMaxLength(DownloadPaths.FinalTrackNamePath, Globals.MaxLength);
            // Construct Full filename & file path
            DownloadPaths.FullTrackFileName = DownloadPaths.FinalTrackNamePath + Globals.AudioFileType;
            DownloadPaths.FullTrackFilePath = Path.Combine(trackPath, DownloadPaths.FullTrackFileName);
            // Check if the file already exists
            if (System.IO.File.Exists(DownloadPaths.FullTrackFilePath))
            {
                string message = $"File for \"{DownloadPaths.FinalTrackNamePath}\" already exists. Skipping.\r\n";
                logger.AddDownloadLogLine(message, true, true);
                return false;
            }
            // Notify UI of starting track download.
            logger.AddDownloadLogLine($"Downloading - {DownloadPaths.FinalTrackNamePath} ...... ", true, true);
            // Get track streaming URL, abort if failed.
            string streamUrl = ExecuteApiCall(apiService => apiService.GetTrackFileUrl(trackIdString, Globals.FormatIdString))?.Url;
            if (string.IsNullOrEmpty(streamUrl))
            {
                // Can happen with free accounts trying to download non-previewable tracks (or if API call failed).
                logger.AddDownloadLogLine($"Couldn't get streaming URL for Track \"{DownloadPaths.FinalTrackNamePath}\". Skipping.\r\n", true, true);
                return false;
            }
            try
            {
                // Create file path strings
                string coverArtFilePath = Path.Combine(DownloadPaths.Path3Full, "Cover.jpg");
                string coverArtTagFilePath = Path.Combine(DownloadPaths.Path3Full, Globals.TaggingOptions.ArtSize + ".jpg");
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", Globals.USER_AGENT);
                    // Save streamed file from link
                    await DownloadFileAsync(httpClient, streamUrl, DownloadPaths.FullTrackFilePath);
                    // Download selected cover art size for tagging files (if not exists)
                    if (!System.IO.File.Exists(coverArtTagFilePath))
                    {
                        try
                        {
                            await DownloadFileAsync(httpClient, DownloadInfo.FrontCoverImgTagUrl, coverArtTagFilePath);
                        }
                        catch (Exception ex)
                        {
                            // Qobuz servers throw a 404 as if the image doesn't exist.
                            logger.AddDownloadErrorLogLines(new string[] { "Error downloading image file for tagging.", ex.Message, Environment.NewLine });
                        }
                    }
                    // Download max quality Cover Art to "Cover.jpg" file in chosen path (if not exists & not part of playlist).
                    if (!isPartOfTracklist && !System.IO.File.Exists(coverArtFilePath))
                    {
                        try
                        {
                            await DownloadFileAsync(httpClient, DownloadInfo.FrontCoverImgUrl, coverArtFilePath);
                        }
                        catch (Exception ex)
                        {
                            // Qobuz servers throw a 404 as if the image doesn't exist.
                            logger.AddDownloadErrorLogLines(new string[] { "Error downloading full size cover image file.", ex.Message, Environment.NewLine });
                        }
                    }
                }
                // Tag metadata to downloaded track.
                AudioFileTagger.AddMetaDataTags(DownloadInfo, DownloadPaths.FullTrackFilePath, coverArtTagFilePath, logger);
                // Remove temp tagging art file if requested and exists.
                if (removeTagArtFileAfterDownload && System.IO.File.Exists(coverArtTagFilePath))
                    System.IO.File.Delete(coverArtTagFilePath);
                logger.AddDownloadLogLine("Track Download Done!\r\n", true, true);
                System.Threading.Thread.Sleep(100);
            }
            catch (AggregateException ae)
            {
                // When a Task fails, an AggregateException is thrown. Could be a HttpClient timeout or network error.
                logger.AddDownloadLogErrorLine($"Track Download canceled, probably due to network error or request timeout. Details saved to error log.{Environment.NewLine}",
                    true, true);
                logger.AddDownloadErrorLogLine("Track Download canceled, probably due to network error or request timeout.");
                logger.AddDownloadErrorLogLine(ae.ToString());
                logger.AddDownloadErrorLogLine(Environment.NewLine);
                return false;
            }
            catch (Exception downloadEx)
            {
                // If there is an unknown issue trying to, or during the download, show and log error info.
                logger.AddDownloadLogErrorLine($"Unknown error during Track Download. Details saved to error log.{Environment.NewLine}", true, true);
                logger.AddDownloadErrorLogLine("Unknown error during Track Download.");
                logger.AddDownloadErrorLogLine(downloadEx.ToString());
                logger.AddDownloadErrorLogLine(Environment.NewLine);
                return false;
            }
            return true;
        }

        private async Task<bool> DownloadAlbumAsync(CancellationToken cancellationToken, Album qobuzAlbum, string basePath, string albumPathSuffix = "")
        {
            bool noErrorsOccured = true;
            // Get Album model object with first batch of tracks
            const int tracksLimit = 100;
            qobuzAlbum = ExecuteApiCall(apiService => apiService.GetAlbum(qobuzAlbum.Id, true, null, tracksLimit, 0));
            // If API call failed, abort Album Download
            if (string.IsNullOrEmpty(qobuzAlbum.Id))
                return false;
            // Get all album information and update UI fields via callback
            DownloadInfo.SetAlbumDownloadInfo(qobuzAlbum);
            UpdateAlbumUiTags.Invoke(DownloadInfo);
            // Download all tracks of the Album in batches of {tracksLimit}, clean albumArt tag file after last track
            int tracksTotal = qobuzAlbum.Tracks.Total ?? 0;
            int tracksPageOffset = qobuzAlbum.Tracks.Offset ?? 0;
            int tracksLoaded = qobuzAlbum.Tracks.Items?.Count ?? 0;
            int i = 0;
            while (i < tracksLoaded)
            {
                // User requested task cancellation!
                cancellationToken.ThrowIfCancellationRequested();
                bool isLastTrackOfAlbum = (i + tracksPageOffset) == (tracksTotal - 1);
                Track qobuzTrack = qobuzAlbum.Tracks.Items[i];
                // Nested Album objects in Tracks are not always fully populated, inject current qobuzAlbum in Track to be downloaded
                qobuzTrack.Album = qobuzAlbum;
                if (!await DownloadTrackAsync(cancellationToken, qobuzTrack, basePath,
                        false, true, isLastTrackOfAlbum, albumPathSuffix)) noErrorsOccured = false;
                i++;
                if (i == tracksLoaded && tracksTotal > i + tracksPageOffset)
                {
                    // load next page of tracks
                    tracksPageOffset += tracksLimit;
                    qobuzAlbum = ExecuteApiCall(apiService => apiService.GetAlbum(qobuzAlbum.Id, true, null, tracksLimit, tracksPageOffset));
                    // If API call failed, abort Album Download
                    if (string.IsNullOrEmpty(qobuzAlbum.Id))
                        return false;
                    // If Album Track Items is empty, Qobuz max API offset might be reached
                    if (qobuzAlbum.Tracks?.Items?.Any() != true)
                        break;
                    // Reset counter for looping next batch of tracks
                    i = 0;
                    tracksLoaded = qobuzAlbum.Tracks.Items?.Count ?? 0;
                }
            }
            // Look for digital booklet(s) in "Goodies"
            // Don't fail on failed "Goodies" downloads, just log...
            if (Globals.TaggingOptions.WriteGetGoodiesTag == true)
            {
                if (!await DownloadBookletsAsync(qobuzAlbum, DownloadPaths.Path3Full))
                    noErrorsOccured = false;
            }
            return noErrorsOccured;
        }

        private async Task<bool> DownloadBookletsAsync(Album qobuzAlbum, string basePath)
        {
            bool noErrorsOccured = true;
            List<Goody> booklets = qobuzAlbum.Goodies?.Where(g => g.FileFormatId == (int)GoodiesFileType.BOOKLET).ToList();
            if (booklets == null || !booklets.Any())
            {
                // No booklets found, just return
                return noErrorsOccured;
            }
            logger.AddDownloadLogLine($"Goodies found, downloading...{Environment.NewLine}", true, true);
            using (HttpClient httpClient = new HttpClient())
            {
                int counter = 1;
                foreach (Goody booklet in booklets)
                {
                    string bookletFileName = counter == 1 ? "Digital Booklet.pdf" : $"Digital Booklet {counter}.pdf";
                    string bookletFilePath = Path.Combine(basePath, bookletFileName);
                    // Download booklet if file doesn't exist yet
                    if (System.IO.File.Exists(bookletFilePath))
                        logger.AddDownloadLogLine($"Booklet file for \"{bookletFileName}\" already exists. Skipping.{Environment.NewLine}", true, true);
                    else
                    {
                        // When a booklet download fails, mark error occured but continue downloading others if they exist
                        if (!await DownloadBookletAsync(booklet, httpClient, bookletFileName, bookletFilePath)) noErrorsOccured = false;
                    }
                    counter++;
                }
            }
            return noErrorsOccured;
        }

        private async Task<bool> DownloadBookletAsync(Goody booklet, HttpClient httpClient, string fileName, string filePath)
        {
            bool noErrorsOccured = true;
            try
            {
                // Download booklet
                await DownloadFileAsync(httpClient, booklet.Url, filePath);
                logger.AddDownloadLogLine($"Booklet \"{fileName}\" download complete!{Environment.NewLine}", true, true);
            }
            catch (AggregateException ae)
            {
                // When a Task fails, an AggregateException is thrown. Could be a HttpClient timeout or network error.
                logger.AddDownloadLogErrorLine($"Goodies Download canceled, probably due to network error or request timeout. Details saved to error log.{Environment.NewLine}",
                    true, true);
                logger.AddDownloadErrorLogLine("Goodies Download canceled, probably due to network error or request timeout.");
                logger.AddDownloadErrorLogLine(ae.ToString());
                logger.AddDownloadErrorLogLine(Environment.NewLine);
                noErrorsOccured = false;
            }
            catch (Exception downloadEx)
            {
                // If there is an unknown issue trying to, or during the download, show and log error info.
                logger.AddDownloadLogErrorLine($"Unknown error during Goodies Download. Details saved to error log.{Environment.NewLine}", true, true);
                logger.AddDownloadErrorLogLine("Unknown error during Goodies Download.");
                logger.AddDownloadErrorLogLine(downloadEx.ToString());
                logger.AddDownloadErrorLogLine(Environment.NewLine);
                noErrorsOccured = false;
            }
            return noErrorsOccured;
        }

        private async Task<bool> DownloadAlbumsAsync(CancellationToken cancellationToken, string basePath, List<Album> albums, bool isEndOfDownloadJob)
        {
            bool noAlbumErrorsOccured = true;
            foreach (Album qobuzAlbum in albums)
            {
                // User requested task cancellation!
                cancellationToken.ThrowIfCancellationRequested();
                // Empty output, then say Starting Downloads.
                logger.ClearUILogComponent();
                logger.AddEmptyDownloadLogLine(true, false);
                logger.AddDownloadLogLine($"Starting Downloads for album \"{qobuzAlbum.Title}\" with ID: <{qobuzAlbum.Id}>...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                bool albumDownloadOK = await DownloadAlbumAsync(cancellationToken, qobuzAlbum, basePath, $" [{qobuzAlbum.Id}]");
                // If album download failed, mark error occured and continue
                if (!albumDownloadOK)
                    noAlbumErrorsOccured = false;
            }
            if (isEndOfDownloadJob)
                logger.LogFinishedDownloadJob(noAlbumErrorsOccured);
            return noAlbumErrorsOccured;
        }

        // Convert release to album for download.
        private async Task<bool> DownloadReleasesAsync(CancellationToken cancellationToken, string basePath, List<Release> releases)
        {
            bool noAlbumErrorsOccured = true;
            foreach (Release qobuzRelease in releases)
            {
                // User requested task cancellation!
                cancellationToken.ThrowIfCancellationRequested();
                // Fetch Album object corresponding to release
                Album qobuzAlbum = ExecuteApiCall(apiService => apiService.GetAlbum(qobuzRelease.Id, true, null, 0, 0));
                // If API call failed, mark error occured and continue with next album
                if (string.IsNullOrEmpty(qobuzAlbum.Id))
                {
                    noAlbumErrorsOccured = false;
                    continue;
                }
                // Empty output, then say Starting Downloads.
                logger.ClearUILogComponent();
                logger.AddEmptyDownloadLogLine(true, false);
                logger.AddDownloadLogLine($"Starting Downloads for album \"{qobuzAlbum.Title}\" with ID: <{qobuzAlbum.Id}>...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                bool albumDownloadOK = await DownloadAlbumAsync(cancellationToken, qobuzAlbum, basePath, $" [{qobuzAlbum.Id}]");
                // If album download failed, mark error occured and continue
                if (!albumDownloadOK)
                    noAlbumErrorsOccured = false;
            }
            return noAlbumErrorsOccured;
        }

        private async Task<bool> DownloadArtistReleasesAsync(CancellationToken cancellationToken, Artist qobuzArtist, string basePath, string releaseType, bool isEndOfDownloadJob)
        {
            bool noErrorsOccured = true;
            // Get ReleasesList model object with first batch of releases
            const int releasesLimit = 100;
            int releasesOffset = 0;
            ReleasesList releasesList = ExecuteApiCall(apiService => apiService.GetReleaseList(qobuzArtist.Id.ToString(),
                true, releaseType, "release_date", "desc", 0, releasesLimit, releasesOffset));
            // If API call failed, abort Artist Download
            if (releasesList == null) { return false; }
            bool continueDownload = true;
            while (continueDownload)
            {
                // User requested task cancellation!
                cancellationToken.ThrowIfCancellationRequested();
                // If releases download failed, mark artist error occured and continue with next artist
                if (!await DownloadReleasesAsync(cancellationToken, basePath, releasesList.Items))
                    noErrorsOccured = false;
                if (releasesList.HasMore)
                {
                    // Fetch next batch of releases
                    releasesOffset += releasesLimit;
                    releasesList = ExecuteApiCall(apiService => apiService.GetReleaseList(qobuzArtist.Id.ToString(), true,
                        releaseType, "release_date", "desc", 0, releasesLimit, releasesOffset));
                }
                else
                    continueDownload = false;
            }
            if (isEndOfDownloadJob)
                logger.LogFinishedDownloadJob(noErrorsOccured);
            return noErrorsOccured;
        }

        public async Task StartDownloadItemTaskAsync(DownloadItem downloadItem, DownloadTaskStatusChanged downloadStartedCallback, DownloadTaskStatusChanged downloadStoppedCallback)
        {
            // Create new cancellation token source.
            using (this.cancellationTokenSource = new CancellationTokenSource())
            {
                IsBusy = true;
                try
                {
                    downloadStartedCallback?.Invoke();
                    // Link should be valid here, start new download log
                    logger.DownloadLogPath = Path.Combine(Globals.LoggingDir, $"Download_Log_{DateTime.Now:yyyy-MM-dd_HH.mm.ss.fff}.log");
                    string logLine = $"Downloading <{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(downloadItem.Type)}> from {downloadItem.Url}";
                    logger.AddDownloadLogLine(new string('=', logLine.Length).PadRight(logLine.Length), true);
                    logger.AddDownloadLogLine(logLine, true);
                    logger.AddDownloadLogLine(new string('=', logLine.Length).PadRight(logLine.Length), true);
                    logger.AddEmptyDownloadLogLine(true);
                    DownloadInfo = new DownloadItemInfo
                    {
                        DownloadItemID = downloadItem.Id
                    };
                    DownloadPaths = DownloadInfo.CurrentDownloadPaths;
                    switch (downloadItem.Type)
                    {
                        case "track":
                            await StartDownloadTrackTaskAsync(cancellationTokenSource.Token);
                            break;

                        case "album":
                            await StartDownloadAlbumTaskAsync(cancellationTokenSource.Token);
                            break;

                        case "artist":
                            await StartDownloadArtistDiscogTaskAsync(cancellationTokenSource.Token);
                            break;

                        case "label":
                            await StartDownloadLabelTaskAsync(cancellationTokenSource.Token);
                            break;

                        case "user":
                            if (DownloadInfo.DownloadItemID == @"library/favorites/albums")
                                await StartDownloadFaveAlbumsTaskAsync(cancellationTokenSource.Token);
                            else if (DownloadInfo.DownloadItemID == @"library/favorites/artists")
                                await StartDownloadFaveArtistsTaskAsync(cancellationTokenSource.Token);
                            else if (DownloadInfo.DownloadItemID == @"library/favorites/tracks")
                                await StartDownloadFaveTracksTaskAsync(cancellationTokenSource.Token);
                            else
                            {
                                logger.ClearUILogComponent();
                                logger.AddDownloadLogLine($"You entered an invalid user favorites link.{Environment.NewLine}", true, true);
                                logger.AddDownloadLogLine($"Favorite Tracks, Albums & Artists are supported with the following links:{Environment.NewLine}", true, true);
                                logger.AddDownloadLogLine($"Tracks - https://play.qobuz.com/user/library/favorites/tracks{Environment.NewLine}", true, true);
                                logger.AddDownloadLogLine($"Albums - https://play.qobuz.com/user/library/favorites/albums{Environment.NewLine}", true, true);
                                logger.AddDownloadLogLine($"Artists - https://play.qobuz.com/user/library/favorites/artists{Environment.NewLine}", true, true);
                            }
                            break;

                        case "playlist":
                            await StartDownloadPlaylistTaskAsync(cancellationTokenSource.Token);
                            break;

                        default:
                            // We shouldn't get here?!? I'll leave this here just in case...
                            logger.ClearUILogComponent();
                            logger.AddDownloadLogLine("URL not understood. Is there a typo?", true, true);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation.
                    logger.AddEmptyDownloadLogLine(true, true);
                    logger.AddDownloadLogLine("Download stopped by user!", true, true);
                }
                finally
                {
                    downloadStoppedCallback?.Invoke();
                    IsBusy = false;
                }
            }
        }

        // For downloading "track" links
        private async Task StartDownloadTrackTaskAsync(CancellationToken cancellationToken)
        {
            // Empty screen output, then say Grabbing info.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine($"Grabbing Track info...{Environment.NewLine}", true, true);
            // Set "basePath" as the selected path.
            string downloadBasePath = Settings.Default.savedFolder;
            try
            {
                Track qobuzTrack = ExecuteApiCall(apiService => apiService.GetTrack(DownloadInfo.DownloadItemID, true));
                // If API call failed, abort
                if (qobuzTrack == null)
                    return;
                logger.AddDownloadLogLine($"Track \"{qobuzTrack.Title}\" found. Starting Download...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                bool fileDownloaded = await DownloadTrackAsync(cancellationToken, qobuzTrack, downloadBasePath, true, false, true);
                // If download failed, abort
                if (!fileDownloaded)
                    return;
                // notify downloading is completed.
                logger.AddEmptyDownloadLogLine(true, true);
                logger.AddDownloadLogLine("Download job completed! All downloaded files will be located in your chosen path.", true, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.LogDownloadTaskException("Track", downloadEx);
            }
        }

        // For downloading "album" links
        private async Task StartDownloadAlbumTaskAsync(CancellationToken cancellationToken)
        {
            // Empty screen output, then say Grabbing info.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine($"Grabbing Album info...{Environment.NewLine}", true, true);
            // Set "basePath" as the selected path.
            String downloadBasePath = Settings.Default.savedFolder;
            try
            {
                // Get Album model object without tracks (tracks are loaded in batches later)
                Album qobuzAlbum = ExecuteApiCall(apiService => apiService.GetAlbum(DownloadInfo.DownloadItemID, true, null, 0));
                // If API call failed, abort
                if (qobuzAlbum == null)
                    return;
                logger.AddDownloadLogLine($"Album \"{qobuzAlbum.Title}\" found. Starting Downloads...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                logger.LogFinishedDownloadJob(await DownloadAlbumAsync(cancellationToken, qobuzAlbum, downloadBasePath));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.LogDownloadTaskException("Album", downloadEx);
            }
        }

        // For downloading "artist" links
        private async Task StartDownloadArtistDiscogTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path.
            String artistBasePath = Settings.Default.savedFolder;
            // Empty output, then say Grabbing IDs.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Artist info...", true, true);
            try
            {
                // Get Artist model object
                Artist qobuzArtist = ExecuteApiCall(apiService => apiService.GetArtist(DownloadInfo.DownloadItemID, true));
                // If API call failed, abort
                if (qobuzArtist == null)
                    return;
                logger.AddDownloadLogLine($"Starting Downloads for artist \"{qobuzArtist.Name}\" with ID: <{qobuzArtist.Id}>...", true, true);
                await DownloadArtistReleasesAsync(cancellationToken, qobuzArtist, artistBasePath, "all", true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Artist", downloadEx);
            }
        }

        // For downloading "label" links
        private async Task StartDownloadLabelTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path + "/- Labels".
            string labelBasePath = Path.Combine(Settings.Default.savedFolder, "- Labels");
            // Empty output, then say Grabbing IDs.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Label albums...", true, true);
            try
            {
                // Initialize full Album list
                QobuzApiSharp.Models.Content.Label qobuzLabel = null;
                List<Album> labelAlbums = new List<Album>();
                const int albumLimit = 500;
                int albumsOffset = 0;
                while (true)
                {
                    // Get Label model object with albums
                    qobuzLabel = ExecuteApiCall(apiService => apiService.GetLabel(DownloadInfo.DownloadItemID, true, "albums", albumLimit, albumsOffset));
                    // If API call failed, abort
                    if (qobuzLabel == null)
                        return;
                    // If resulting Label has no Album Items, Qobuz API maximum offset is reached
                    if (qobuzLabel.Albums?.Items?.Any() != true)
                        break;
                    labelAlbums.AddRange(qobuzLabel.Albums.Items);
                    // Exit loop when all albums are loaded or the Qobuz imposed limit of 10000 is reached
                    if ((qobuzLabel.Albums?.Total ?? 0) == labelAlbums.Count)
                        break;
                    albumsOffset += albumLimit;
                }
                // If label has no albums, log and abort
                if (!labelAlbums.Any())
                {
                    logger.AddDownloadLogLine($"No albums found for label \"{qobuzLabel.Name}\" with ID: <{qobuzLabel.Id}>, nothing to download.", true, true);
                    return;
                }
                logger.AddDownloadLogLine($"Starting Downloads for label \"{qobuzLabel.Name}\" with ID: <{qobuzLabel.Id}>...", true, true);
                // Add Label name to basePath
                string safeLabelName = StringTools.GetSafeFilename(StringTools.DecodeEncodedNonAsciiCharacters(qobuzLabel.Name));
                labelBasePath = Path.Combine(labelBasePath, safeLabelName);
                await DownloadAlbumsAsync(cancellationToken, labelBasePath, labelAlbums, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Label", downloadEx);
            }
        }

        // For downloading "favorites"
        // Favorite Albums
        private async Task StartDownloadFaveAlbumsTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path + "/- Favorites".
            string favoritesBasePath = Path.Combine(Settings.Default.savedFolder, "- Favorites");
            // Empty output, then say Grabbing IDs.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Favorite Albums...", true, true);
            try
            {
                // Initialize full Album list
                List<Album> favoriteAlbums = new List<Album>();
                const int albumLimit = 500;
                int albumsOffset = 0;
                while (true)
                {
                    // Get UserFavorites model object with albums
                    UserFavorites qobuzUserFavorites = ExecuteApiCall(apiService => apiService.GetUserFavorites(DownloadInfo.DownloadItemID, "albums", albumLimit, albumsOffset));
                    // If API call failed, abort
                    if (qobuzUserFavorites == null)
                        return;
                    // If resulting UserFavorites has no Album Items, Qobuz API maximum offset is reached
                    if (qobuzUserFavorites.Albums?.Items?.Any() != true)
                        break;
                    favoriteAlbums.AddRange(qobuzUserFavorites.Albums.Items);
                    // Exit loop when all albums are loaded
                    if ((qobuzUserFavorites.Albums?.Total ?? 0) == favoriteAlbums.Count)
                        break;
                    albumsOffset += albumLimit;
                }
                // If user has no favorite albums, log and abort
                if (!favoriteAlbums.Any())
                {
                    logger.AddDownloadLogLine("No favorite albums found, nothing to download.", true, true);
                    return;
                }
                // Download all favorite albums
                await DownloadAlbumsAsync(cancellationToken, favoritesBasePath, favoriteAlbums, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Favorite Albums", downloadEx);
            }
        }

        // Favorite Artists
        private async Task StartDownloadFaveArtistsTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path + "/- Favorites".
            string favoritesBasePath = Path.Combine(Settings.Default.savedFolder, "- Favorites");
            // Empty output, then say Grabbing IDs.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Favorite Artists...", true, true);
            try
            {
                bool noArtistErrorsOccured = true;
                // Get UserFavoritesIds model object, getting Id's allows all results at once.
                UserFavoritesIds qobuzUserFavoritesIds = ExecuteApiCall(apiService => apiService.GetUserFavoriteIds(DownloadInfo.DownloadItemID));
                // If API call failed, abort
                if (qobuzUserFavoritesIds == null)
                    return;
                // If user has no favorite artists, log and abort
                if (qobuzUserFavoritesIds.Artists?.Any() != true)
                {
                    logger.AddDownloadLogLine("No favorite artists found, nothing to download.", true, true);
                    return;
                }
                // Download favorite artists
                foreach (int favoriteArtistId in qobuzUserFavoritesIds.Artists)
                {
                    // User requested task cancellation!
                    cancellationToken.ThrowIfCancellationRequested();
                    // Get Artist model object
                    Artist qobuzArtist = ExecuteApiCall(apiService => apiService.GetArtist(favoriteArtistId.ToString(), true));
                    // If API call failed, mark artist error occured and continue with next artist
                    if (qobuzArtist == null)
                    {
                        noArtistErrorsOccured = false;
                        continue;
                    }
                    logger.AddEmptyDownloadLogLine(true, true);
                    logger.AddDownloadLogLine($"Starting Downloads for artist \"{qobuzArtist.Name}\" with ID: <{qobuzArtist.Id}>...", true, true);
                    // If albums download failed, mark artist error occured and continue with next artist
                    if (!await DownloadArtistReleasesAsync(cancellationToken, qobuzArtist, favoritesBasePath, "all", false)) noArtistErrorsOccured = false;
                }
                logger.LogFinishedDownloadJob(noArtistErrorsOccured);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Favorite Albums", downloadEx);
            }
        }

        // Favorite Tracks
        private async Task StartDownloadFaveTracksTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path + "/- Favorites".
            string favoriteTracksBasePath = Path.Combine(Settings.Default.savedFolder, "- Favorites");
            // Empty screen output, then say Grabbing info.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Favorite Tracks...", true, true);
            logger.AddEmptyDownloadLogLine(true, true);
            try
            {
                bool noTrackErrorsOccured = true;
                // Get UserFavoritesIds model object, getting Id's allows all results at once.
                UserFavoritesIds qobuzUserFavoritesIds = ExecuteApiCall(apiService => apiService.GetUserFavoriteIds(DownloadInfo.DownloadItemID));
                // If API call failed, abort
                if (qobuzUserFavoritesIds == null)
                    return;
                // If user has no favorite tracks, log and abort
                if (qobuzUserFavoritesIds.Tracks?.Any() != true)
                {
                    logger.AddDownloadLogLine("No favorite tracks found, nothing to download.", true, true);
                    return;
                }
                logger.AddDownloadLogLine("Favorite tracks found. Starting Downloads...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                // Download favorite tracks
                foreach (int favoriteTrackId in qobuzUserFavoritesIds.Tracks)
                {
                    // User requested task cancellation!
                    cancellationToken.ThrowIfCancellationRequested();
                    Track qobuzTrack = ExecuteApiCall(apiService => apiService.GetTrack(favoriteTrackId.ToString(), true));
                    // If API call failed, log and continue with next track
                    if (qobuzTrack == null)
                    {
                        noTrackErrorsOccured = false;
                        continue;
                    }
                    if (!await DownloadTrackAsync(cancellationToken, qobuzTrack, favoriteTracksBasePath, true, false, true))
                        noTrackErrorsOccured = false;
                }
                logger.LogFinishedDownloadJob(noTrackErrorsOccured);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Playlist", downloadEx);
            }
        }

        // For downloading "playlist" links
        private async Task StartDownloadPlaylistTaskAsync(CancellationToken cancellationToken)
        {
            // Set "basePath" as the selected path.
            String playlistBasePath = Settings.Default.savedFolder;
            // Empty screen output, then say Grabbing info.
            logger.ClearUILogComponent();
            logger.AddDownloadLogLine("Grabbing Playlist tracks...", true, true);
            logger.AddEmptyDownloadLogLine(true, true);
            try
            {
                // Get Playlist model object with all track_ids
                Playlist qobuzPlaylist = ExecuteApiCall(apiService => apiService.GetPlaylist(DownloadInfo.DownloadItemID, true, "track_ids", 10000));
                // If API call failed, abort
                if (qobuzPlaylist == null)
                    return;
                // If playlist empty, log and abort
                if (qobuzPlaylist.TrackIds?.Any() != true)
                {
                    logger.AddDownloadLogLine($"Playlist \"{qobuzPlaylist.Name}\" is empty, nothing to download.", true, true);
                    return;
                }
                logger.AddDownloadLogLine($"Playlist \"{qobuzPlaylist.Name}\" found. Starting Downloads...", true, true);
                logger.AddEmptyDownloadLogLine(true, true);
                // Create Playlist root directory.
                string playlistSafeName = StringTools.GetSafeFilename(StringTools.DecodeEncodedNonAsciiCharacters(qobuzPlaylist.Name));
                string playlistNamePath = StringTools.TrimToMaxLength(playlistSafeName, Globals.MaxLength);
                playlistBasePath = Path.Combine(playlistBasePath, "- Playlists", playlistNamePath);
                System.IO.Directory.CreateDirectory(playlistBasePath);
                // Download Playlist cover art to "Playlist.jpg" in root directory (if not exists)
                string coverArtFilePath = Path.Combine(playlistBasePath, "Playlist.jpg");
                if (!System.IO.File.Exists(coverArtFilePath))
                {
                    try
                    {
                        using (WebClient imgClient = new WebClient())
                        {
                            imgClient.DownloadFile(new Uri(qobuzPlaylist.ImageRectangle.FirstOrDefault<string>()), coverArtFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Qobuz servers throw a 404 as if the image doesn't exist.
                        logger.AddDownloadErrorLogLines(new string[] { "Error downloading full size playlist cover image file.", ex.Message, "\r\n" });
                    }
                }
                bool noTrackErrorsOccured = true;
                // Start new m3u Playlist file.
                M3uPlaylist m3uPlaylist = new M3uPlaylist
                {
                    IsExtended = true
                };
                // Download Playlist tracks
                foreach (long trackId in qobuzPlaylist.TrackIds)
                {
                    // User requested task cancellation!
                    cancellationToken.ThrowIfCancellationRequested();
                    // Fetch full Track info
                    Track qobuzTrack = ExecuteApiCall(apiService => apiService.GetTrack(trackId.ToString(), true));
                    // If API call failed, log and continue with next track
                    if (qobuzTrack == null)
                    {
                        noTrackErrorsOccured = false;
                        continue;
                    }
                    if (!IsStreamable(qobuzTrack, true))
                        continue;
                    if (!await DownloadTrackAsync(cancellationToken, qobuzTrack, playlistBasePath, true, false, true))
                        noTrackErrorsOccured = false;
                    AddTrackToPlaylistFile(m3uPlaylist, DownloadInfo, DownloadPaths);
                }
                // Write m3u playlist to file, override if exists
                string m3uPlaylistFile = Path.Combine(playlistBasePath, $"{playlistSafeName}.m3u8");
                System.IO.File.WriteAllText(m3uPlaylistFile, PlaylistToTextHelper.ToText(m3uPlaylist), System.Text.Encoding.UTF8);
                logger.LogFinishedDownloadJob(noTrackErrorsOccured);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow when cancellation requested by user.
                throw;
            }
            catch (Exception downloadEx)
            {
                logger.ClearUILogComponent();
                logger.LogDownloadTaskException("Playlist", downloadEx);
            }
        }

        public void AddTrackToPlaylistFile(M3uPlaylist m3uPlaylist, DownloadItemInfo downloadInfo, DownloadItemPaths downloadPaths)
        {
            // If the TrackFile doesn't exist, skip.
            if (!System.IO.File.Exists(downloadPaths.FullTrackFilePath))
                return;
            // Add successfully downloaded file to m3u playlist
            m3uPlaylist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Path = downloadPaths.FullTrackFilePath,
                Duration = TimeSpan.FromSeconds(DownloadInfo.Duration),
                Title = $"{downloadInfo.PerformerName} - {downloadInfo.TrackName}"
            });
        }

        public void CreateTrackDirectories(string basePath, string qualityPath, string albumPathSuffix = "", bool forTracklist = false)
        {
            if (forTracklist)
            {
                DownloadPaths.Path1Full = basePath;
                DownloadPaths.Path2Full = DownloadPaths.Path1Full;
                DownloadPaths.Path3Full = Path.Combine(basePath, qualityPath);
                DownloadPaths.Path4Full = DownloadPaths.Path3Full;
            }
            else
            {
                DownloadPaths.Path1Full = Path.Combine(basePath, DownloadPaths.AlbumArtistPath);
                DownloadPaths.Path2Full = Path.Combine(basePath, DownloadPaths.AlbumArtistPath, DownloadPaths.AlbumNamePath + albumPathSuffix);
                DownloadPaths.Path3Full = Path.Combine(basePath, DownloadPaths.AlbumArtistPath, DownloadPaths.AlbumNamePath + albumPathSuffix, qualityPath);
                // If more than 1 disc, create folders for discs. Otherwise, strings will remain null
                // Pad discnumber with minimum of 2 integer positions based on total number of disks
                if (DownloadInfo.DiscTotal > 1)
                {
                    // Create strings for disc folders
                    string discFolder = "CD " + DownloadInfo.DiscNumber.ToString().PadLeft(Math.Max(2, (int)Math.Floor(Math.Log10(DownloadInfo.DiscTotal) + 1)), '0');
                    DownloadPaths.Path4Full = Path.Combine(basePath, DownloadPaths.AlbumArtistPath, DownloadPaths.AlbumNamePath + albumPathSuffix, qualityPath, discFolder);
                }
                else
                    DownloadPaths.Path4Full = DownloadPaths.Path3Full;
            }
            System.IO.Directory.CreateDirectory(DownloadPaths.Path4Full);
        }
    }
}