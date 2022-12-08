using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class TextureSizeDetail : ProjectWindowDetailBase
    {
        public TextureSizeDetail()
        {
            Name = "Texture Size";
            ColumnWidth = 80;
            Alignment = TextAlignment.Right;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
        {
            if (isFolder) return DetailContent.Empty;
            var texture = asset as Texture;
            return new DetailContent(texture == null ? string.Empty : $"{texture.width}x{texture.height}");
        }
    }
}
