using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Extensions
{
    public static class AssetDatabaseExtension
    {
        public static IEnumerable<T> FindAssetsByType<T>(bool includeEditor = false) where T : Object
        {
            var type = typeof(T)
                .ToString()
                .Split(".")
                .Last();

            var guids = AssetDatabase.FindAssets($"t:{type}", new[] { "Assets" });

            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            if (!includeEditor)
                paths = paths.Where(x => !x.Contains("/Editor/") && !x.Contains(@"\Editor\"));

            return paths.Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(asset => asset != null);
        }

        public static T LoadAssetFromGUID<T>(string guid)
            where T : Object
            => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        
        public static void EnsureLabel(this Object target, string label)
        {
            var labels = new List<string>(AssetDatabase.GetLabels(target));

            if (!labels.Contains(label))
            {
                labels.Add(label);
                AssetDatabase.SetLabels(target, labels.ToArray());
            }
        }
        
        private static string _projectPath;

        // Application.dataPath returns the path including /Assets, which we need to strip off
        public static string GetProjectPath()
        {
            if (!string.IsNullOrEmpty(_projectPath))
                return _projectPath;

            var path = Application.dataPath;
            var directory = new DirectoryInfo(path);
            var parent = directory.Parent;

            _projectPath = parent != null ? parent.FullName : path;

            return _projectPath;
        }
    }
}