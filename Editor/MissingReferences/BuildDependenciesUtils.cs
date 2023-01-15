using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.MissingReferences
{
    public static class BuildDependenciesUtils
    {
        const string BuiltinResourcesRegex = @"(?!.*\/Editor\/)^Assets.*\/Resources\/.*$";

        [MenuItem("Assets/Dependencies/Find usages")]
        public static void FindDependants()
        {
            var tree = new Dictionary<string, List<string>>();
            var open = new List<string>();
            var closed = new List<string>();
            var target = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(target);
            open.Add(path);

            EditorUtility.DisplayProgressBar("Searching for usages", $"Found: {tree.Count()}", 0);
            var countdownProgress = 1f;
            var shallowDependencies = new Dictionary<string, string[]>();
            var deepDependencies = new Dictionary<string, string[]>();
            while (open.Count > 0)
            {
                var next = open[^1];
                open.RemoveAt(open.Count - 1);
                
                closed.Add(next);

                FindDependantsOf(path, next, tree, open, closed, shallowDependencies, deepDependencies);
                EditorUtility.DisplayProgressBar("Searching for usages", $"Found: {tree.Count()}",
                    1 - countdownProgress);
                countdownProgress /= 4;
            }

            Debug.Log($"Found {tree.Count() - 1} usages of {target}");

            PrintDependencies(path, 0, tree);

            EditorUtility.ClearProgressBar();
        }

        static void PrintDependencies(string path, int identation, Dictionary<string, List<string>> tree)
        {
            var regex = new Regex(BuiltinResourcesRegex);
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path);
            var isTarget = identation == 0;
            var isResources = regex.IsMatch(path);
            var isScene = scenes.Contains(path);

            var color = "white";
            var resourceColor = "#5588ff";
            var sceneColor = "yellow";
            if (isResources) color = resourceColor;
            else if (isScene) color = sceneColor;

            if (tree[path].Count > 0 || isTarget || isResources || isScene)
            {
                var pad = new string('\t', identation);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Debug.Log($"{pad}\t<color={color}>{path}</color>", asset);

                foreach (var d in tree[path])
                {
                    PrintDependencies(d, identation + 1, tree);
                }
            }
        }

        static void FindDependantsOf(string target, string next, Dictionary<string, List<string>> tree,
            List<string> open, List<string> closed,
            Dictionary<string, string[]> shallowDependencies, Dictionary<string, string[]> deepDependencies)
        {
            if (!tree.ContainsKey(next))
            {
                var dependants = new List<string>();
                tree.Add(next, dependants);

                var list = AssetDatabase.GetAllAssetPaths();
                foreach (var s in list)
                {
                    if (!shallowDependencies.TryGetValue(s, out var shallowDeps))
                    {
                        shallowDeps = AssetDatabase.GetDependencies(s, false);
                        shallowDependencies[s] = shallowDeps;
                    }

                    if (shallowDeps.Contains(next))
                    {
                        if (!deepDependencies.TryGetValue(s, out var deepDeps))
                        {
                            deepDeps = AssetDatabase.GetDependencies(s, true);
                            deepDependencies[s] = deepDeps;
                        }

                        if (deepDeps.Contains(target))
                        {
                            dependants.Add(s);
                            if (!closed.Contains(s) && !open.Contains(s))
                            {
                                open.Add(s);
                            }
                        }
                    }
                }
            }
        }

        public static string[] GetAllAssetsIncludedInBuild()
        {
            var regex = new Regex(BuiltinResourcesRegex);
            var list = AssetDatabase.GetAllAssetPaths()
                .Where(path => regex.IsMatch(path))
                .ToList();

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            list.AddRange(scenes);

            var result = AssetDatabase.GetDependencies(list.ToArray(), true);

            return result;
        }
    }
}