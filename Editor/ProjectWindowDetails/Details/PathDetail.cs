using System.IO;
using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class PathDetail : ProjectWindowDetailBase
    {
        static readonly string ProjectPath = Path.Combine(Application.dataPath, "../");
        public PathDetail()
        {
            Name = "Path";
            ColumnWidth = 400;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
            => new DetailContent(Path.GetDirectoryName(assetPath), "Reveal in Finder");

        public override void OnClicked(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var folderPath = Path.Combine(ProjectPath, Path.GetDirectoryName(assetPath) ?? string.Empty);
            Application.OpenURL($"file://{folderPath}");
            Event.current.Use();
        }
    }
}
