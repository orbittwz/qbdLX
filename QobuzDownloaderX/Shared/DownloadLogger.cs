using System.Collections.Generic;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace QobuzDownloaderX.Shared
{
    public class DownloadLogger
    {
        public readonly string downloadErrorLogPath = Path.Combine(Globals.LoggingDir, "Download_Errors.log");
        public delegate void DownloadEnded();
        private readonly DownloadEnded updateUiOnDownloadEnd;
        private TextBox ScreenOutputTextBox { get; }
        public string DownloadLogPath { get; set; }

        public DownloadLogger(TextBox outputTextBox, DownloadEnded updateUiOnDownloadEnd)
        {
            ScreenOutputTextBox = outputTextBox;
            this.updateUiOnDownloadEnd = updateUiOnDownloadEnd;
        }

        public void RemovePreviousErrorLog()
        {
            // Remove previous download error log
            if (System.IO.File.Exists(downloadErrorLogPath))
                System.IO.File.Delete(downloadErrorLogPath);
        }

        public void AddDownloadLogLine(string logEntry, bool logToFile, bool logToScreen = false)
        {
            if (string.IsNullOrEmpty(logEntry))
                return;
            if (logToScreen)
                ScreenOutputTextBox?.Invoke(new Action(() => ScreenOutputTextBox.AppendText(logEntry)));
            if (logToFile)
            {
                var logEntries = logEntry.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(logLine => string.IsNullOrWhiteSpace(logLine) ? logLine : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} : {logLine}");
                // Filter out all empty lines except if the logEntry started with an empty line to avoid blank lines for each newline in UI
                var filteredLogEntries = logEntries.Aggregate(new List<string>(), (accumulator, current) =>
                {
                    if (accumulator.Count == 0 || !string.IsNullOrWhiteSpace(current))
                        accumulator.Add(current);
                    return accumulator;
                });
                System.IO.File.AppendAllLines(DownloadLogPath, filteredLogEntries);
            }
        }

        public void AddDownloadLogErrorLine(string logEntry, bool logToFile, bool logToScreen = false)
        {
            AddDownloadLogLine($"[ERROR] {logEntry}", logToFile, logToScreen);
        }

        public void AddEmptyDownloadLogLine(bool logToFile, bool logToScreen = false)
        {
            AddDownloadLogLine($"{Environment.NewLine}{Environment.NewLine}", logToFile, logToScreen);
        }

        public void AddDownloadErrorLogLines(IEnumerable<string> logEntries)
        {
            if (logEntries == null && !logEntries.Any())
                return;
            System.IO.File.AppendAllLines(downloadErrorLogPath, logEntries);
        }

        public void AddDownloadErrorLogLine(string logEntry)
        {
            AddDownloadErrorLogLines(new string[] { logEntry });
        }

        public void LogDownloadTaskException(string downloadTaskType, Exception downloadEx)
        {
            // If there is an issue trying to, or during the download, show error info.
            ClearUILogComponent();
            AddDownloadLogErrorLine($"{downloadTaskType} Download Task ERROR. Details saved to error log.{Environment.NewLine}", true, true);
            AddDownloadErrorLogLine($"{downloadTaskType} Download Task ERROR.");
            AddDownloadErrorLogLine(downloadEx.ToString());
            AddDownloadErrorLogLine(Environment.NewLine);
            updateUiOnDownloadEnd?.Invoke();
        }

        public void LogFinishedDownloadJob(bool noErrorsOccured)
        {
            AddEmptyDownloadLogLine(true, true);
            // notify downloading is completed.
            if (noErrorsOccured)
            {
                AddDownloadLogLine("Download job completed! All downloaded files will be located in your chosen path.", true, true);
                this.ChangeTextColor(System.Drawing.Color.LawnGreen);
            }
            else
                AddDownloadLogLine("Download job completed with warnings and/or errors! Some or all files could be missing!", true, true);
            updateUiOnDownloadEnd?.Invoke();
        }

        public void ClearUILogComponent()
        {
            this.ScreenOutputTextBox.Invoke(new Action(() => ScreenOutputTextBox.Text = String.Empty));
            this.ScreenOutputTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        }

        public void ChangeTextColor(Color color)
        {
            //this.ScreenOutputTextBox.BackColor = System.Drawing.Color.Aquamarine;
            this.ScreenOutputTextBox.ForeColor = color;
        }
    }
}