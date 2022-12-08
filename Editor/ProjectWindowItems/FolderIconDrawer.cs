using System;
using Plugins.AAA.Editor.Editor.ProjectWindowItems.FolderIconProviders;
using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowItems
{
    [InitializeOnLoad]
    public class FolderIconDrawer
    {
        const string DrawFolderIconsKey = "DrawFolderIcons";
        static readonly IFolderIconProvider[] ProjectWindowItems =
        {
            new UnityFolderIconProvider(),
            new DefaultFolderIconProvider(),
        };

        static FolderIconDrawer()
        {
            EditorApplication.projectWindowItemOnGUI -= ReplaceFolderIcon;
            EditorApplication.projectWindowItemOnGUI += ReplaceFolderIcon;
        }

        [MenuItem("AAA/Editor/Toggle Folder Icons")]
        public static void ToggleFolderIcons()
            => EditorPrefs.SetBool(DrawFolderIconsKey, !EditorPrefs.GetBool(DrawFolderIconsKey, true));
        static void ReplaceFolderIcon(string guid, Rect rect)
        {
            if (!EditorPrefs.GetBool(DrawFolderIconsKey, true)) return;

            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;

            Texture2D folderIcon = null;
            foreach (var item in ProjectWindowItems)
            {
                folderIcon = item.TryGetFolderIcon(path);
                if (folderIcon != null)
                    break;
            }
            if (folderIcon == null) return;

            if (rect.width > rect.height)
                rect.width = rect.height;
            else
                rect.height = rect.width;

            if (Math.Abs(rect.x - 14) < 1)
                rect.x += 3;

            GUI.DrawTexture(rect, folderIcon);
        }
    }
}
