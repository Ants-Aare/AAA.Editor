using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Plugins.AAA.Editor.Editor.Utility
{
    public static class DeleteEmptyFoldersUtility
    {
        [MenuItem("AAA/Utility/Delete empty folders", false, 14)]
        public static void DeleteEmptyFolders()
        {
            var directoryInfos = new DirectoryInfo(Application.dataPath)
                .GetDirectories("*.*", SearchOption.AllDirectories)
                .OrderBy(f => f.FullName)
                .Reverse();

            foreach (var dir in directoryInfos)
            {
                if (dir.Exists)
                {
                    var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                    if (files.Length == 0 || files.All(file => file.FullName.EndsWith(".meta") || file.Name.Equals(".DS_Store")))
                    {
                        var meta = dir.Parent?.GetFiles(dir.Name + ".meta").FirstOrDefault();
                        meta?.Delete();
                        dir.Delete(true);
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}