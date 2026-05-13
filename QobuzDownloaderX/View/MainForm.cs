using QobuzDownloaderX.Models.Download;
using QobuzDownloaderX.Properties;
using QobuzDownloaderX.Shared;
using QobuzDownloaderX.Shared.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace QobuzDownloaderX.View
{
    public partial class QobuzDownloaderX : HeadlessForm
    {
        private readonly DownloadLogger logger;
        private readonly DownloadManager downloadManager;
        private readonly List<string> folderNameList1;
        private readonly List<string> fileNameList1;
        // Button color download inactive
        private readonly Color readyButtonBackColor = Color.FromArgb(0, 112, 239); // Windows Blue (Azure Blue)
        // Button color download active
        private readonly Color busyButtonBackColor = Color.FromArgb(200, 30, 0); // Red

        public QobuzDownloaderX()
        {
            InitializeComponent();
            logger = new DownloadLogger(output, UpdateControlsDownloadEnd);
            // Remove previous download error log
            logger.RemovePreviousErrorLog();
            downloadManager = new DownloadManager(logger, UpdateAlbumTagsUI, UpdateDownloadSpeedLabel)
            {
                CheckIfStreamable = true
            };
            Markdowns markdowns = new Markdowns();
            folderNameList1 = markdowns.FolderNameListData;
            fileNameList1 = markdowns.FileNameListData;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set main form size on launch and bring to left center.
            if (Settings.Default.hideTagsSection == true)
            {
                this.minimizelbl.Left -= 285;
                this.exitlbl.Left -= 285;
                this.Width = 615;
            }
            else
                this.Width = 900;
            this.Left = 30;
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            // Grab profile image
            string profilePic = Convert.ToString(Globals.Login.User.Avatar);
            profilePictureBox.ImageLocation = profilePic.Replace(@"\", null).Replace("s=50", "s=20");
            // Welcome the user after successful login.
            logger.ClearUILogComponent();
            // Get and display version number.
            verNumlbl.Text = Settings.Version;
            // Set a placeholder image for Cover Art box.
            albumArtPicBox.ImageLocation = Globals.DEFAULT_COVER_ART_URL;
            // Change account info for logout button
            string oldText = logoutlbl.Text;
            //logoutlbl.Text = oldText.Replace("%name%", Globals.Login.User?.DisplayName);
            this.BuildOutput();
            this.SetSettings();
            this.SetToolTips();
        }

        private void BuildOutput()
        {
            output.Invoke(new Action(() => output.AppendText("Welcome " + Globals.Login.User.DisplayName + " (" + Globals.Login.User.Email + ") !\r\n")));
            output.Invoke(new Action(() => output.AppendText("User Zone - " + Globals.Login.User.Zone + "\r\n\r\n")));
            output.Invoke(new Action(() => output.AppendText("Qobuz Credential Description - " + Globals.Login.User.Credential.Description + "\r\n")));
            output.Invoke(new Action(() => output.AppendText("\r\n")));
            output.Invoke(new Action(() => output.AppendText("Qobuz Subscription Details\r\n")));
            output.Invoke(new Action(() => output.AppendText("==========================\r\n")));
            if (Globals.Login.User.Subscription != null)
            {
                output.Invoke(new Action(() => output.AppendText("Offer Type - " + Globals.Login.User.Subscription.Offer + "\r\n")));
                output.Invoke(new Action(() => output.AppendText("Start Date - ")));
                output.Invoke(new Action(() => output.AppendText(Globals.Login.User.Subscription.StartDate != null ? ((DateTimeOffset)Globals.Login.User.Subscription.StartDate).ToString("dd-MM-yyyy") : "?")));
                output.Invoke(new Action(() => output.AppendText("\r\n")));
                output.Invoke(new Action(() => output.AppendText("End Date - ")));
                output.Invoke(new Action(() => output.AppendText(Globals.Login.User.Subscription.EndDate != null ? ((DateTimeOffset)Globals.Login.User.Subscription.EndDate).ToString("dd-MM-yyyy") : "?")));
                output.Invoke(new Action(() => output.AppendText("\r\n")));
                output.Invoke(new Action(() => output.AppendText("Periodicity - " + Globals.Login.User.Subscription.Periodicity + "\r\n")));
                output.Invoke(new Action(() => output.AppendText("==========================\r\n\r\n")));
            }
            else if (Globals.Login.User?.Credential?.Parameters?.Source == "household" && Globals.Login.User.Credential.Parameters?.HiresStreaming == true)
            {
                output.Invoke(new Action(() => output.AppendText("Active Family sub-account, unknown End Date \r\n")));
                output.Invoke(new Action(() => output.AppendText("Credential Label - " + Globals.Login.User.Credential.Label + "\r\n")));
                output.Invoke(new Action(() => output.AppendText("==========================\r\n\r\n")));
            }
            else
            {
                output.Invoke(new Action(() => output.AppendText("No active subscriptions, only sample downloads possible!\r\n")));
                output.Invoke(new Action(() => output.AppendText("==========================\r\n\r\n")));
            }
            output.Invoke(new Action(() => output.AppendText("Your user_auth_token has been set for this session!")));
            // Check if there's no selected path saved.
            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                // If there is NOT a saved path.
                output.Invoke(new Action(() => output.AppendText("\r\n\r\n")));
                output.Invoke(new Action(() => output.AppendText("No default path has been set! Remember to Choose a Folder!\r\n")));
            }
            else
            {
                // If there is a saved path.
                output.Invoke(new Action(() => output.AppendText("\r\n\r\n")));
                output.Invoke(new Action(() => output.AppendText("Using the last folder you've selected as your selected path!\r\n")));
                output.Invoke(new Action(() => output.AppendText("\r\n")));
                output.Invoke(new Action(() => output.AppendText("Default Folder:\r\n")));
                output.Invoke(new Action(() => output.AppendText(folderBrowserDialog.SelectedPath + "\r\n")));
            }
        }

        private void SetSettings()
        {
            // Set saved settings to correct places.
            folderBrowserDialog.SelectedPath = Settings.Default.savedFolder;
            albumCheckbox.Checked = Settings.Default.albumTag;
            albumArtistCheckbox.Checked = Settings.Default.albumArtistTag;
            artistCheckbox.Checked = Settings.Default.artistTag;
            commentCheckbox.Checked = Settings.Default.commentTag;
            commentTextbox.Text = Settings.Default.commentText;
            composerCheckbox.Checked = Settings.Default.composerTag;
            producerCheckbox.Checked = Settings.Default.producerTag;
            labelCheckbox.Checked = Settings.Default.labelTag;
            involvedPeopleCheckBox.Checked = Settings.Default.involvedPeopleTag;
            mergePerformersCheckBox.Checked = Settings.Default.mergePerformersTag;
            InitialListSeparatorTextbox.Text = Settings.Default.initialListSeparator;
            ListEndSeparatorTextbox.Text = Settings.Default.listEndSeparator;
            copyrightCheckbox.Checked = Settings.Default.copyrightTag;
            discNumberCheckbox.Checked = Settings.Default.discTag;
            discTotalCheckbox.Checked = Settings.Default.totalDiscsTag;
            genreCheckbox.Checked = Settings.Default.genreTag;
            isrcCheckbox.Checked = Settings.Default.isrcTag;
            typeCheckbox.Checked = Settings.Default.typeTag;
            explicitCheckbox.Checked = Settings.Default.explicitTag;
            trackTitleCheckbox.Checked = Settings.Default.trackTitleTag;
            trackNumberCheckbox.Checked = Settings.Default.trackTag;
            trackTotalCheckbox.Checked = Settings.Default.totalTracksTag;
            upcCheckbox.Checked = Settings.Default.upcTag;
            releasYearCheckbox.Checked = Settings.Default.yearTag;
            releaseDateCheckbox.Checked = Settings.Default.releaseDateTag;
            imageCheckbox.Checked = Settings.Default.imageTag;
            urlCheckBox.Checked = Settings.Default.urlTag;
            mp3RadioBtn.Checked = Settings.Default.quality == "q1";
            flacLowRadioBtn.Checked = Settings.Default.quality == "q2";
            flacMidRadioBtn.Checked = Settings.Default.quality == "q3";
            flacHighRadioBtn.Checked = Settings.Default.quality == "q4";
            goodiesCheckBox.Checked = Settings.Default.getGoodiesTag;
            //Globals.FormatIdString = Settings.Default.qualityFormat;
            //Globals.AudioFileType = Settings.Default.audioType;
            artSizeSelect.Text = Settings.Default.savedArtSize;
            foldernameTempSelect.DataSource = folderNameList1;
            foldernameTempSelect.SelectedItem = Settings.Default.savedFolderNameTemplate;
            foldernameTempSelect.Width += folderNameList1.Max(st => st.Length) + 20;
            filenameTempSelect.DataSource = fileNameList1;
            filenameTempSelect.SelectedItem = Settings.Default.savedFileNameTemplate;
            filenameTempSelect.Width += fileNameList1.Max(st => st.Length) + 10;
            //Globals.MaxLength = Settings.Default.savedMaxLength;
            maxLengthTextbox.Text = Settings.Default.savedMaxLength.ToString();
        }

        private void SetToolTips()
        {
            // Set tooltips for all the interactable controls in the Main window.
            new ToolTip().SetToolTip(selectFolderButton, "Choose folder to save downloaded items.");
            new ToolTip().SetToolTip(openFolderButton, "Open downloaded items base folder.");
            new ToolTip().SetToolTip(openLogFolderButton, "Open the program's log folder.");
            new ToolTip().SetToolTip(mp3RadioBtn, "Select to download items at MP3 quality.");
            new ToolTip().SetToolTip(flacLowRadioBtn, "Select to download items at FLAC 16-Bit quality.");
            new ToolTip().SetToolTip(flacMidRadioBtn, "Select to download items at FLAC 24-Bit quality.");
            new ToolTip().SetToolTip(flacHighRadioBtn, "Select to download items at FLAC 24-Bit HiRes quality.");
            new ToolTip().SetToolTip(tagsLabel, "Click to show/hide tags and options.");
            new ToolTip().SetToolTip(openSearchButton, "Open search window.");
            new ToolTip().SetToolTip(downloadButton, "Download the item using the address bar.");
            new ToolTip().SetToolTip(downloadUrl, "The item URL address in Qobuz website.");
            new ToolTip().SetToolTip(logoutlbl, "Logout and quit the current session. (return to login window)\n-= Logged as " + Globals.Login.User?.DisplayName + " =-");
            new ToolTip().SetToolTip(albumArtPicBox, "The item cover art.");
            new ToolTip().SetToolTip(albumArtistTextBox, "The item album artist.");
            new ToolTip().SetToolTip(albumTextBox, "The item album name.");
            new ToolTip().SetToolTip(qualityTextbox, "The item quality.");
            new ToolTip().SetToolTip(totalTracksTextbox, "The item total tracks.");
            new ToolTip().SetToolTip(upcTextBox, "The item UPC.");
            new ToolTip().SetToolTip(releaseDateTextBox, "The item release date.");
            new ToolTip().SetToolTip(minimizelbl, "Minimize the window.");
            new ToolTip().SetToolTip(exitlbl, "Quit program.");
            new ToolTip().SetToolTip(goodiesCheckBox, "Whether to download the item's booklet if available.");
            new ToolTip().SetToolTip(imageCheckbox, "Whether to save embedded cover art image in each track.");
            new ToolTip().SetToolTip(artSizeSelect, "Choose embedded cover art image size to save.");
            new ToolTip().SetToolTip(filenameTempSelect, "Choose file name template to save each track.");
            new ToolTip().SetToolTip(foldernameTempSelect, "Choose folder name template to save the item.");
            new ToolTip().SetToolTip(InitialListSeparatorTextbox, "Character to indicate how the program divide multiple artists.");
            new ToolTip().SetToolTip(ListEndSeparatorTextbox, "String to add at the end of multiple composers.");
        }

        private void UpdateDownloadSpeedLabel(string speed)
        {
            downloadSpeedlbl.Invoke(new Action(() => downloadSpeedlbl.Text = speed));
        }

        private void OpenSearch_Click(object sender, EventArgs e)
        {
            if (Globals.SearchForm.Visible == false)
                Globals.SearchForm.Show(this);
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!downloadManager.IsBusy)
                await StartLinkItemDownloadAsync(downloadUrl.Text);
            else
                downloadManager.StopDownloadTask();
        }

        private async void DownloadUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                await StartLinkItemDownloadAsync(downloadUrl.Text);
            }
        }

        public async Task StartLinkItemDownloadAsync(string downloadLink)
        {
            // Check if there's no selected path.
            if (string.IsNullOrEmpty(Settings.Default.savedFolder))
            {
                // If there is NOT a saved path.
                logger.ClearUILogComponent();
                output.Invoke(new Action(() => output.AppendText($"No path has been set, please choose a Folder!{Environment.NewLine}")));
                return;
            }
            // Get download item type and ID from url
            DownloadItem downloadItem = DownloadUrlParser.ParseDownloadUrl(downloadLink);
            // If download item could not be parsed, abort
            if (downloadItem.IsEmpty())
            {
                logger.ClearUILogComponent();
                output.Invoke(new Action(() => output.AppendText("URL not understood... is there a typo?")));
                return;
            }
            // If for some reason a download is still busy, do nothing
            if (downloadManager.IsBusy)
                return;
            // Run the StartDownloadItemTaskAsync method on a background thread & wait for the task to complete
            await Task.Run(() => downloadManager.StartDownloadItemTaskAsync(downloadItem, UpdateControlsDownloadStart, UpdateControlsDownloadEnd));
        }

        private void UpdateControlsDownloadStart()
        {
            downloadUrl.Invoke(new Action(() => downloadUrl.Enabled = false));
            selectFolderButton.Invoke(new Action(() => selectFolderButton.Enabled = false));
            openSearchButton.Invoke(new Action(() => openSearchButton.Enabled = false));
            downloadButton.Invoke(new Action(() => {
                downloadButton.Text = "Stop Download";
                downloadButton.BackColor = busyButtonBackColor;
            }));
        }

        private void UpdateControlsDownloadEnd()
        {
            downloadUrl.Invoke(new Action(() => downloadUrl.Enabled = true));
            selectFolderButton.Invoke(new Action(() => selectFolderButton.Enabled = true));
            openSearchButton.Invoke(new Action(() => openSearchButton.Enabled = true));
            downloadButton.Invoke(new Action(() => {
                downloadButton.Text = "Download";
                downloadButton.BackColor = readyButtonBackColor;
            }));
        }

        private void SelectFolder_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                // Open Folder Browser to select path & save the selection
                folderBrowserDialog.ShowDialog();
                Settings.Default.savedFolder = folderBrowserDialog.SelectedPath;
                Settings.Default.Save();
            });
            // Run your code from a thread that joins the STA Thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            // Open selected folder
            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                // If there's no selected path.
                MessageBox.Show("No path selected!", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UpdateControlsDownloadEnd();
            }
            else
            {
                // If selected path doesn't exist, create it. (Will be ignored if it does)
                System.IO.Directory.CreateDirectory(folderBrowserDialog.SelectedPath);
                // Open selected folder
                Process.Start(folderBrowserDialog.SelectedPath);
            }
        }

        private void OpenLogFolderButton_Click(object sender, EventArgs e)
        {
            // Open log folder. Folder should exist here so no extra check
            Process.Start(Globals.LoggingDir);
        }

        // Update UI for downloading album
        private void UpdateAlbumTagsUI(DownloadItemInfo downloadInfo)
        {
            // Display album art
            albumArtPicBox.Invoke(new Action(() => albumArtPicBox.ImageLocation = downloadInfo.FrontCoverImgBoxUrl));
            // Display album quality in Quality textbox.
            qualityTextbox.Invoke(new Action(() => qualityTextbox.Text = downloadInfo.DisplayQuality));
            // Display album info text fields
            albumArtistTextBox.Invoke(new Action(() => albumArtistTextBox.Text = downloadInfo.AlbumArtist));
            albumTextBox.Invoke(new Action(() => albumTextBox.Text = downloadInfo.AlbumName));
            releaseDateTextBox.Invoke(new Action(() => releaseDateTextBox.Text = downloadInfo.ReleaseDate));
            upcTextBox.Invoke(new Action(() => upcTextBox.Text = downloadInfo.Upc));
            totalTracksTextbox.Invoke(new Action(() => totalTracksTextbox.Text = downloadInfo.TrackTotal.ToString()));
        }

        private void AlbumCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.albumTag = albumCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteAlbumNameTag = albumCheckbox.Checked;
        }

        private void AlbumArtistCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.albumArtistTag = albumArtistCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteAlbumArtistTag = albumArtistCheckbox.Checked;
        }

        private void TrackTitleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.trackTitleTag = trackTitleCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteTrackTitleTag = trackTitleCheckbox.Checked;
        }

        private void ArtistCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.artistTag = artistCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteTrackArtistTag = artistCheckbox.Checked;
        }

        private void TrackNumberCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.trackTag = trackNumberCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteTrackNumberTag = trackNumberCheckbox.Checked;
        }

        private void TrackTotalCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.totalTracksTag = trackTotalCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteTrackTotalTag = trackTotalCheckbox.Checked;
        }

        private void DiscNumberCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.discTag = discNumberCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteDiscNumberTag = discNumberCheckbox.Checked;
        }

        private void DiscTotalCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.totalDiscsTag = discTotalCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteDiscTotalTag = discTotalCheckbox.Checked;
        }

        private void ReleaseYearCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.yearTag = releasYearCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteReleaseYearTag = releasYearCheckbox.Checked;
        }

        private void ReleaseDateCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.releaseDateTag = releaseDateCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteReleaseDateTag = releaseDateCheckbox.Checked;
        }

        private void GenreCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.genreTag = genreCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteGenreTag = genreCheckbox.Checked;
        }

        private void ComposerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.composerTag = composerCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteComposerTag = composerCheckbox.Checked;
        }

        private void CopyrightCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.copyrightTag = copyrightCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteCopyrightTag = copyrightCheckbox.Checked;
        }

        private void IsrcCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.isrcTag = isrcCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteISRCTag = isrcCheckbox.Checked;
        }

        private void TypeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.typeTag = typeCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteMediaTypeTag = typeCheckbox.Checked;
        }

        private void UpcCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.upcTag = upcCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteUPCTag = upcCheckbox.Checked;
        }

        private void ExplicitCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.explicitTag = explicitCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteExplicitTag = explicitCheckbox.Checked;
        }

        private void CommentCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.commentTag = commentCheckbox.Checked;
            if (Settings.Default.commentTag == false)
            {
                Settings.Default.commentText = string.Empty;
                commentTextbox.Text = string.Empty;
                commentTextbox.Enabled = false;
            }
            else
                commentTextbox.Enabled = true;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteCommentTag = commentCheckbox.Checked;
        }

        private void ImageCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.imageTag = imageCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteCoverImageTag = imageCheckbox.Checked;
        }

        private void CommentTextbox_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.commentText = commentTextbox.Text;
            Settings.Default.Save();
            //Globals.TaggingOptions.CommentTag = commentTextbox.Text;
        }

        private void ArtSizeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set ArtSize to selected value, and save selected option to settings.
            if (artSizeSelect.Text == "org")
                MessageBox.Show("Choosing this embedded cover art size may cause issues!", "Notice", MessageBoxButtons.OK);
            //Globals.TaggingOptions.ArtSize = artSizeSelect.Text;
            Settings.Default.savedArtSize = artSizeSelect.Text;
            Settings.Default.Save();
        }

        private void MaxLengthTextbox_TextChanged(object sender, EventArgs e)
        {
            if (maxLengthTextbox.Text != null)
            {
                try
                {
                    if (maxLengthTextbox.Text.All(char.IsDigit) == false || Convert.ToInt32(maxLengthTextbox.Text) > 150)
                        maxLengthTextbox.Text = "150";
                    Settings.Default.savedMaxLength = Convert.ToInt32(maxLengthTextbox.Text);
                    Settings.Default.Save();
                    //Globals.MaxLength = Convert.ToInt32(maxLengthTextbox.Text);
                }
                catch (Exception)
                {
                    Settings.Default.savedMaxLength = 150;
                }
            }
            else
                Settings.Default.savedMaxLength = 150;
        }

        private void ProducerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.producerTag = producerCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteProducerTag = producerCheckbox.Checked;
        }

        private void LabelCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.labelTag = labelCheckbox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteLabelTag = labelCheckbox.Checked;
        }

        private void InvolvedPeopleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.involvedPeopleTag = involvedPeopleCheckBox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteInvolvedPeopleTag = involvedPeopleCheckBox.Checked;
        }

        private void MergePerformersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.mergePerformersTag = mergePerformersCheckBox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.MergePerformers = mergePerformersCheckBox.Checked;
        }

        private void InitialListSeparatorTextbox_TextChanged(object sender, EventArgs e)
        {
            if (InitialListSeparatorTextbox.Text != null)
            {
                Settings.Default.initialListSeparator = InitialListSeparatorTextbox.Text;
                Settings.Default.Save();
                //Globals.TaggingOptions.PrimaryListSeparator = InitialListSeparatorTextbox.Text;
            }
            else
            {
                Settings.Default.initialListSeparator = ";";
                Settings.Default.Save();
                //Globals.TaggingOptions.PrimaryListSeparator = InitialListSeparatorTextbox.Text;
            }
        }

        private void ListEndSeparatorTextbox_TextChanged(object sender, EventArgs e)
        {
            if (ListEndSeparatorTextbox.Text != null)
            {
                Settings.Default.listEndSeparator = ListEndSeparatorTextbox.Text;
                Settings.Default.Save();
                //Globals.TaggingOptions.ListEndSeparator = ListEndSeparatorTextbox.Text;
            }
            else
            {
                Settings.Default.listEndSeparator = " & ";
                Settings.Default.Save();
                //Globals.TaggingOptions.ListEndSeparator = ListEndSeparatorTextbox.Text;
            }
        }

        private void UrlCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.urlTag = urlCheckBox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteURLTag = urlCheckBox.Checked;
        }

        private void ExitLabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MinimizeLabel_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void MinimizeLabel_MouseHover(object sender, EventArgs e)
        {
            minimizelbl.ForeColor = Color.FromArgb(0, 112, 239);
        }

        private void MinimizeLabel_MouseLeave(object sender, EventArgs e)
        {
            minimizelbl.ForeColor = Color.White;
        }

        private void ExitLabel_MouseHover(object sender, EventArgs e)
        {
            exitlbl.ForeColor = Color.FromArgb(0, 112, 239);
        }

        private void ExitLabel_MouseLeave(object sender, EventArgs e)
        {
            exitlbl.ForeColor = Color.White;
        }

        private void QobuzDownloaderX_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void QobuzDownloaderX_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.SearchForm.Close();
            Application.Exit();
        }

        private void LogoutLabel_MouseHover(object sender, EventArgs e)
        {
            logoutlbl.ForeColor = Color.FromArgb(0, 112, 239);
        }

        private void LogoutLabel_MouseLeave(object sender, EventArgs e)
        {
            logoutlbl.ForeColor = Color.FromArgb(88, 92, 102);
        }

        private void LogoutLabel_Click(object sender, EventArgs e)
        {
            // Could use some work, but this works.
            Process.Start("QobuzDownloaderX.exe");
            Application.Exit();
        }

        private void QualityRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            string formatIdString = string.Empty;
            string audioFileType = string.Empty;
            RadioButton rb = sender as RadioButton;
            if (rb == null)
            {
                MessageBox.Show("Sender is not a RadioButton");
                return;
            }
            if (rb.Checked)
            {
                if ((string)rb.Tag == "q4")
                {
                    formatIdString = "27";
                    audioFileType = ".flac";
                }
                if ((string)rb.Tag == "q3")
                {
                    formatIdString = "7";
                    audioFileType = ".flac";
                }
                if ((string)rb.Tag == "q2")
                {
                    formatIdString = "6";
                    audioFileType = ".flac";
                }
                if ((string)rb.Tag == "q1")
                {
                    formatIdString = "5";
                    audioFileType = ".mp3";
                }
                Settings.Default.qualityFormat = formatIdString;
                Settings.Default.audioType = audioFileType;
                downloadButton.Enabled = true;
            }
            Settings.Default.quality = (string)rb.Tag;
            Settings.Default.Save();
        }

        private void GoodiesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.getGoodiesTag = goodiesCheckBox.Checked;
            Settings.Default.Save();
            //Globals.TaggingOptions.WriteGetGoodiesTag = goodiesCheckBox.Checked;
        }

        public TextBox DownloadUrl
        {
            get
            {
                return this.downloadUrl;
            }
            set
            {
                this.downloadUrl = value;
            }
        }

        private void FoldernameTempSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.Default.savedFolderNameTemplate = (string)foldernameTempSelect.SelectedItem;
            Settings.Default.Save();
            //Globals.TaggingOptions.FolderNameTemplate = (string)foldernameTempSelect.SelectedItem;
        }

        private void FilenameTempSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.Default.savedFileNameTemplate = (string)filenameTempSelect.SelectedItem;
            Settings.Default.Save();
            //Globals.TaggingOptions.FileNameTemplate = (string)filenameTempSelect.SelectedItem;
        }

        private void TagsLabel_Click(object sender, EventArgs e)
        {
            if (this.Width == 900)
            {
                this.Width = 615;
                this.minimizelbl.Left -= 285;
                this.exitlbl.Left -= 285;
                Globals.SearchForm.Left -= 285;
                this.tagsLabel.Text = "Show/Hide 🠈 tags/options 🠈";
                Settings.Default.hideTagsSection = true;
                Settings.Default.Save();
            }
            else if (this.Width == 615)
            {
                this.Width = 900;
                this.minimizelbl.Left += 285;
                this.exitlbl.Left += 285;
                Globals.SearchForm.Left += 285;
                this.tagsLabel.Text = "Show/Hide 🠊 tags/options 🠊";
                Settings.Default.hideTagsSection = false;
                Settings.Default.Save();
            }
        }
    }
}