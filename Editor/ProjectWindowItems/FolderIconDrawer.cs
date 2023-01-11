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
        const string MenuItem = "AAA/Editor/Folder Icons";

        static readonly IFolderIconProvider[] ProjectWindowItems =
        {
            new UnityFolderIconProvider(),
            new DefaultFolderIconProvider(),
        };

        static bool DrawFoldersEnabled
        {
            get => EditorPrefs.GetBool(DrawFolderIconsKey, true);
            set => EditorPrefs.SetBool(DrawFolderIconsKey, value);
        }

        static FolderIconDrawer()
        {
            var enabled = DrawFoldersEnabled;
            Menu.SetChecked(MenuItem, enabled);

            if (enabled)
            {
                EditorApplication.projectWindowItemOnGUI -= ReplaceFolderIcon;
                EditorApplication.projectWindowItemOnGUI += ReplaceFolderIcon;
            }
        }

        [MenuItem(MenuItem)]
        public static void ToggleFolderIcons()
        {
            var enabled = !DrawFoldersEnabled;
            DrawFoldersEnabled = enabled;
            Menu.SetChecked(MenuItem, enabled);

            EditorApplication.projectWindowItemOnGUI -= ReplaceFolderIcon;
            if (enabled)
            {
                EditorApplication.projectWindowItemOnGUI += ReplaceFolderIcon;
            }
        }

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