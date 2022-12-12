using System;
using System.Collections.Generic;
using Plugins.AAA.Editor.Editor.Extensions;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowItems.FolderIconProviders
{
    public class UnityFolderIconProvider : IFolderIconProvider
    {
        readonly Dictionary<string, Texture2D> _folderIcons = new()
        {
            {"Editor", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Prefabs", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("5cd3c4c13c88e49aaa83df78d04240c0")},
            {"Materials", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("978d8d848bcc643e385c80f8ef91107b")},
            {"Data", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("f793caf3eb5f1469cb3ed16f1ec7a425")},
            {"Resources", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("3925bc528b90e4a51ab9913e5427e938")},
            {"Scripts", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("b16d8fec8f5d443e2b49e3df2ee140c9")},
            {"Textures", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("431ec7565c981484ca47a6c85ff5d99e")},
            // {"Audio", AssetDatabase.LoadAssetAtPath<Texture2D>("")},
            // {"Models", AssetDatabase.LoadAssetAtPath<Texture2D>("")},
            // {_Scenes", AssetDatabase.LoadAssetAtPath<Texture2D>("")},
            // {_Animations", AssetDatabase.LoadAssetAtPath<Texture2D>("")},
        };
        public Texture2D TryGetFolderIcon(string path)
        {
            foreach (var folderIcon in _folderIcons)
            {
                var indexOf = path.IndexOf(folderIcon.Key, StringComparison.Ordinal);

                if (indexOf < 0) continue;

                var lastSlashIndex = path.IndexOf('/', indexOf);

                if (lastSlashIndex == -1)
                    return folderIcon.Value;
            }
            return null;
        }


        public string TagIconPath(string path) => null;
    }
}
