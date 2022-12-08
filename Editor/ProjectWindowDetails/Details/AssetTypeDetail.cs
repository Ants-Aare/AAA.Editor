using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class AssetTypeDetail : ProjectWindowDetailBase
    {
        protected override bool EnabledByDefault => true;

        public AssetTypeDetail()
        {
            Name = "Asset Type";
            ColumnWidth = 150;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
            =>  new DetailContent(isFolder
                ? "Folder"
                : asset?.GetType()?.Name
                , "Open Source Code");

        public override void OnClicked(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) return;
            Event.current.Use();

            var type = asset.GetType();
            if (type.IsSubclassOf(typeof(ScriptableObject)))
            {
                OpenScriptableObjectScript(type);
            }
        }

        public static void OpenScriptableObjectScript(Type type)
        {
            var scriptGuid = AssetDatabase.FindAssets(type.Name).FirstOrDefault();
            if (string.IsNullOrEmpty(scriptGuid)) return;
            var obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(scriptGuid));
            AssetDatabase.OpenAsset(obj);
        }
    }
}
