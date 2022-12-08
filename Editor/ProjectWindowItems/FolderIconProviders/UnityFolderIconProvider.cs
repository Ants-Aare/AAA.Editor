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
            {"Prefabs", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Materials", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Data", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Resources", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Scripts", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
            {"Textures", AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("971f072eab62f40b49be119632a54381")},
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
