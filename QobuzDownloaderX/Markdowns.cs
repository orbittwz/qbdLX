using System.Collections.Generic;

namespace QobuzDownloaderX
{
    internal class Markdowns
    {
        private List<string> folderNameListData = new List<string> { "Artist - Album - Quality", "Artist - Album - Quality - Year", "Artist \\ Album \\ Quality" };

        private List<string> fileNameListData = new List<string> { "Track - Title", "Track - Artist - Title", "Track Title" };
        // for now checks are - no "-" means "Track Title", Contains "Artist" means "Track - Artist - Title", else "Track - Title"

        private List<string> themeListData = new List<string> { "Dark", "Light", "Party" };

        internal List<string> FolderNameListData
        {
            get
            {
                return folderNameListData;
            }
        }

        internal List<string> FileNameListData
        {
            get
            {
                return fileNameListData;
            }
        }

        internal List<string> ThemeListData
        {
            get
            {
                return themeListData;
            }
        }
    }
}