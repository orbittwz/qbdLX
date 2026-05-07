using System.Collections.Generic;

namespace QobuzDownloaderX
{
    internal class Markdowns
    {
        private List<string> folderNameListData = new List<string> { "Artist - Album - Quality", "Artist - Album - Quality - Year" };

        private List<string> fileNameListData = new List<string> { "Track - Title", "Track - Artist - Title", "Track Title" };
        // for now checks are - no "-" means "Track Title", Contains "Artist" means "Track - Artist - Title", else "Track - Title"

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
    }
}