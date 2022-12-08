using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Labs.SuperScience;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Plugins.AAA.Editor.Editor.MissingReferences
{
    /// <summary>
    /// Scans the project for serialized references to missing (deleted) assets and displays the results in an EditorWindow
    /// </summary>
    sealed class MissingProjectReferences : MissingReferencesWindow
    {
        /// <summary>
        /// Tree structure for folder scan results
        /// This is the root object for the project scan, and represents the results in a hierarchy that matches the
        /// project's folder structure for an easy to read presentation of assets with missing references.
        /// When the Scan method encounters a subfolder, we initialize one of these using that path as an argument.
        /// When the scan encounters an asset, it either uses an AssetContainer or a GameObjectContainer, which is
        /// defined in MissingReferencesWindow. This object contains three separate lists of the different types of
        /// containers for display in the GUI. The window calls into these helper objects to draw them, as well.
        /// </summary>
        class Folder
        {
            /// <summary>
            /// Container for asset scan results. Just as with GameObjectContainer, we initialize one of these
            /// using an asset object to scan it for missing references and retain the results
            /// </summary>
            class AssetContainer : MissingReferencesContainer
            {
                const string k_SubAssetsLabelFormat = "Sub-assets: {0}";

                readonly UnityObject m_Object;
                bool m_SubAssetsVisible;
                public readonly List<MissingReferencesContainer> SubAssets = new List<MissingReferencesContainer>();
                public readonly List<SerializedProperty> PropertiesWithMissingReferences = new List<SerializedProperty>();

                public override UnityObject Object => m_Object;

                /// <summary>
                /// Initialize an AssetContainer to represent the given UnityObject
                /// This will scan the object for missing references and retain the information for display in
                /// the given window.
                /// </summary>
                /// <param name="unityObject">The main UnityObject for this asset</param>
                /// <param name="path">The path to this asset, for gathering sub-assets</param>
                /// <param name="options">User-configurable options for this view</param>
                public AssetContainer(UnityObject unityObject, string path, Options options)
                {
                    m_Object = unityObject;
                    CheckForMissingReferences(unityObject, PropertiesWithMissingReferences, options);

                    // Collect any sub-asset references
                    foreach (var asset in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                    {
                        if (asset is GameObject prefab)
                        {
                            var gameObjectContainer = new GameObjectContainer(prefab, options);
                            if (gameObjectContainer.Count > 0)
                                SubAssets.Add(gameObjectContainer);
                        }
                        else
                        {
                            var assetContainer = new AssetContainer(asset, options);
                            if (assetContainer.PropertiesWithMissingReferences.Count > 0)
                                SubAssets.Add(assetContainer);
                        }
                    }
                }

                AssetContainer(UnityObject unityObject, Options options)
                {
                    m_Object = unityObject;
                    CheckForMissingReferences(unityObject, PropertiesWithMissingReferences, options);
                }

                /// <summary>
                /// Draw the missing references UI for this asset
                /// </summary>
                public override void Draw()
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        DrawPropertiesWithMissingReferences(PropertiesWithMissingReferences);

                        var count = SubAssets.Count;
                        if (count == 0)
                            return;

                        m_SubAssetsVisible = EditorGUILayout.Foldout(m_SubAssetsVisible, string.Format(k_SubAssetsLabelFormat, count));
                        if (!m_SubAssetsVisible)
                            return;

                        foreach (var asset in SubAssets)
                        {
                            asset.Draw();
                        }
                    }
                }

                public override void SetVisibleRecursively(bool visible)
                {
                    m_SubAssetsVisible = visible;
                }
            }

            const string k_LabelFormat = "{0}: {1}";
            readonly SortedDictionary<string, Folder> m_Subfolders = new SortedDictionary<string, Folder>();
            readonly List<MissingReferencesContainer> m_Assets = new List<MissingReferencesContainer>();
            bool m_Visible;

            internal List<MissingReferencesContainer> Assets => m_Assets;
            internal SortedDictionary<string, Folder> Subfolders => m_Subfolders;

            /// <summary>
            /// The number of assets in this folder with missing references
            /// </summary>
            public int Count;

            /// <summary>
            /// Clear the contents of this container
            /// </summary>
            public void Clear()
            {
                m_Subfolders.Clear();
                m_Assets.Clear();
                Count = 0;
            }

            /// <summary>
            /// Scan the contents of a given path and add the results as a subfolder to this container
            /// </summary>
            /// <param name="path">The path to scan</param>
            /// <param name="options">User-configurable options for this view</param>
            public void AddAssetAtPath(string path, Options options)
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityObject>(path);

                // Prefabs are processed differently so that we can scan components and children
                // Model prefabs may contain materials which we want to scan. The "real prefab" as a sub-asset
                if (asset is GameObject prefab && PrefabUtility.GetPrefabAssetType(asset) != PrefabAssetType.Model)
                {
                    var gameObjectContainer = new GameObjectContainer(prefab, options);
                    if (gameObjectContainer.Count > 0)
                        GetOrCreateFolderForAssetPath(path).m_Assets.Add(gameObjectContainer);
                }
                else
                {
                    var assetContainer = new AssetContainer(asset, path, options);
                    if (assetContainer.PropertiesWithMissingReferences.Count > 0 || assetContainer.SubAssets.Count > 0)
                        GetOrCreateFolderForAssetPath(path).m_Assets.Add(assetContainer);
                }
            }

            /// <summary>
            /// Get the Folder object which corresponds to the folder containing the asset at a given path which is
            /// known to have missing references.
            /// If this is the first asset encountered for a given folder, create a chain of folder objects
            /// rooted with this one and return the folder at the end of that chain.
            /// Every time a folder is accessed, its Count property is incremented to indicate that it contains one
            /// more asset with missing references.
            /// </summary>
            /// <param name="path">Path to an asset with missing references relative to this folder</param>
            /// <returns>The folder object corresponding to the folder containing the asset at the given path</returns>
            Folder GetOrCreateFolderForAssetPath(string path)
            {
                var directories = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var folder = this;
                folder.Count++;
                var length = directories.Length - 1;
                for (var i = 0; i < length; i++)
                {
                    var directory = directories[i];
                    var subfolders = folder.m_Subfolders;
                    if (!subfolders.TryGetValue(directory, out var subfolder))
                    {
                        subfolder = new Folder();
                        subfolders[directory] = subfolder;
                    }

                    folder = subfolder;
                    folder.Count++;
                }

                return folder;
            }

            /// <summary>
            /// Draw missing reference information for this Folder
            /// </summary>
            /// <param name="name">The name of the folder</param>
            public void Draw(string name)
            {
                // A foldout structure here was removed to reduce clutter

                foreach (var kvp in m_Subfolders)
                {
                    kvp.Value.Draw($"{(string.IsNullOrEmpty(name) ? string.Empty : $"{name}/")}{kvp.Key}");
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    foreach (var asset in m_Assets)
                    {
                        var unityObject = asset.Object;
                        EditorGUILayout.PrefixLabel($"{name}/");
                        EditorGUILayout.ObjectField(asset.Object, typeof(UnityObject), false);

                        // Check for null in case  of destroyed object
                        if (unityObject != null)
                            asset.Draw();

                        DrawLine(Color.gray, 1);
                    }
                }
            }

            static void DrawLine(Color color, int thickness = 2, int padding = 10)
            {
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
                r.height = thickness;
                r.y += padding * 0.5f;
                r.x -= 2;
                r.width += 6;
                EditorGUI.DrawRect(r, color);
            }

            /// <summary>
            /// Set the visibility state of this folder, its contents and their children and all of its subfolders and their contents and children
            /// </summary>
            /// <param name="visible">Whether this object and its children should be visible in the GUI</param>
            void SetVisibleRecursively(bool visible)
            {
                m_Visible = visible;
                foreach (var asset in m_Assets)
                {
                    asset.SetVisibleRecursively(visible);
                }

                foreach (var kvp in m_Subfolders)
                {
                    kvp.Value.SetVisibleRecursively(visible);
                }
            }

            /// <summary>
            /// Sort the contents of this folder and all subfolders by name
            /// </summary>
            public void SortContentsRecursively()
            {
                m_Assets.Sort((a, b) => a.Object.name.CompareTo(b.Object.name));
                foreach (var kvp in m_Subfolders)
                {
                    kvp.Value.SortContentsRecursively();
                }
            }
        }

        const string k_NoMissingReferences = "No missing references in project";

        // Bool fields will be serialized to maintain state between domain reloads, but our list of GameObjects will not
        [NonSerialized]
        bool m_Scanned;

        bool _searchAssetsInBuildOnly = true;

        static readonly string SearchAssetsInBuildOnlyPref = "GameKit.MissingProjectReferences.SearchAssetsInBuildOnly";

        Vector2 m_ScrollPosition;
        readonly Folder m_ParentFolder = new Folder();

        ILookup<string, MissingReferencesContainer> m_AllMissingReferences;

        [MenuItem("GameKit/Missing Project References")]
        static void OnMenuItem()
        {
            var window = GetWindow<MissingProjectReferences>("Missing Project References");
            window._searchAssetsInBuildOnly = EditorPrefs.GetBool(SearchAssetsInBuildOnlyPref, true);
        }

        static class Styles {
            const string k_SearchAssetsInBuildOnlyLabel = "Search Assets In Build Only";
            const string k_SearchAssetsInBuildOnlyTooltip = "Search only though the assets included in the build.";

            public static readonly GUIContent SearchAssetsInBuildOnlyContent;

            static Styles()
            {
                SearchAssetsInBuildOnlyContent = new GUIContent(k_SearchAssetsInBuildOnlyLabel, k_SearchAssetsInBuildOnlyTooltip);
            }
        }

        protected override void OnSpecificGUI(ref Options options)
        {
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                _searchAssetsInBuildOnly = EditorGUILayout.Toggle(Styles.SearchAssetsInBuildOnlyContent, _searchAssetsInBuildOnly);
                if (change.changed)
                {
                    EditorPrefs.SetBool(SearchAssetsInBuildOnlyPref, _searchAssetsInBuildOnly);
                }
            }
        }

        /// <summary>
        /// Load all assets in the AssetDatabase and check them for missing serialized references
        /// </summary>
        /// <param name="options">User-configurable options for this view</param>
        protected override void Scan(Options options)
        {
            m_Scanned = true;
            m_ParentFolder.Clear();
            var allAssets = _searchAssetsInBuildOnly ? BuildDependenciesUtils.GetAllAssetsIncludedInBuild() : AssetDatabase.GetAllAssetPaths();

            foreach (var path in allAssets)
            {
                // Only include local paths (relative to project folder)
                if (Path.IsPathRooted(path))
                    continue;

                m_ParentFolder.AddAssetAtPath(path, options);
            }

            m_ParentFolder.SortContentsRecursively();

            var allMissingReferencesContainers = new List<MissingReferencesContainer>();

            void AddToList(List<MissingReferencesContainer> list, Folder folder)
            {
                list.AddRange(folder.Assets);
                foreach (var subfolder in folder.Subfolders)
                {
                    AddToList(list, subfolder.Value);
                }
            }

            AddToList(allMissingReferencesContainers, m_ParentFolder);

            m_AllMissingReferences = allMissingReferencesContainers.ToLookup(container =>
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(container.Object, out var guid, out long _);
                return guid;
            });

            foreach (var reference in allMissingReferencesContainers)
            {
                EditorGUIUtility.PingObject(reference.Object);
                reference.SetVisibleRecursively(true);
            }

            EditorApplication.RepaintProjectWindow();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (!m_Scanned)
            {
                GUIUtility.ExitGUI();
            }

            if (m_ParentFolder.Count == 0)
            {
                GUILayout.Label(k_NoMissingReferences);
            }
            else
            {
                using (var scrollView = new GUILayout.ScrollViewScope(m_ScrollPosition))
                {
                    m_ScrollPosition = scrollView.scrollPosition;
                    m_ParentFolder.Draw(string.Empty);
                }
            }
        }

        void OnEnable()
        {
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
            EditorApplication.RepaintProjectWindow();
        }

        void OnDisable()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;
            EditorApplication.RepaintProjectWindow();
        }

        void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (m_AllMissingReferences == null)
                return;

            if (!m_AllMissingReferences.Contains(guid))
                return;

            DrawItem(selectionRect);
        }
    }
}
