using System.Collections.Generic;

namespace QobuzDownloaderX
{
    internal class Markdowns
    {
        private List<string> folderNameListData = new List<string> { "Artist-Album-Quality", "Artist-Album-Quality-Year" };

        internal List<string> FolderNameListData
        {
            get
            {
                return folderNameListData;
            }
        }
    }
}