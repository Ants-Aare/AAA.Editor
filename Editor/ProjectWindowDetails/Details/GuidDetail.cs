using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class GuidDetail : ProjectWindowDetailBase
    {
        public GuidDetail()
        {
            Name = "Guid";
            ColumnWidth = 240;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
            => new DetailContent(guid, "Copy to Clipboard");

        public override void OnClicked(string guid)
        {
            GUIUtility.systemCopyBuffer = guid;
            Event.current.Use();
            Debug.Log("Copied GUID to Clipboard! guid: " + guid);
        }
    }
}
