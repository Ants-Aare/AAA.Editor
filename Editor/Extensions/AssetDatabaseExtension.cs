using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Extensions
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
    }
}