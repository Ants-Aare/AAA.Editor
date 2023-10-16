using System.IO;
using static System.String;
using Object = UnityEngine.Object;

namespace AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class FileSuffixDetail : ProjectWindowDetailBase
    {
        public FileSuffixDetail()
        {
            Name = "File Suffix";
            ColumnWidth = 60;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
            => new DetailContent(isFolder ? Empty : Path.GetExtension(assetPath), "FileType");
    }
}
