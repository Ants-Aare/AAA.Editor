using System.IO;
using System.Text;
using AAA.Editor.Editor.ProjectWindowDetails;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using static System.String;

namespace AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class AssetInfoDetail : ProjectWindowDetailBase
    {
        protected override bool EnabledByDefault => true;
        readonly StringBuilder _builder = new();

        public AssetInfoDetail()
        {
            Name = "Asset Info";
            ColumnWidth = 150;
            Alignment = TextAlignment.Right;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
        {
            if (isFolder || asset is {name:"Assets"}) return DetailContent.Empty;

            return asset switch
            {
                Texture2D texture => new DetailContent(texture.format.ToString()),
                RenderTexture renderTexture => new DetailContent(renderTexture.format.ToString()),
                Material material => new DetailContent(material.shader.name, "Select Shader"),
                AudioClip audioClip => new DetailContent(audioClip.loadType.ToString()),
                AnimationClip animation => new DetailContent($"{animation.length:F3}s"),
                Font font => new DetailContent(font.material.name, "Select Font Material"),
                Mesh mesh => new DetailContent($"{mesh.vertexCount.ToString()} verts"),
                GameObject prefab => new DetailContent(GetPrefabInfo(prefab)),
                ScriptableObject scriptableObject => new DetailContent(scriptableObject.GetType().Name, "Open Source Code"),
                MonoScript script => new DetailContent(script.GetClass()?.Namespace),
                TextAsset textAsset => new DetailContent(Path.GetExtension(assetPath)),
                VideoClip video => new DetailContent($"{video.length:F3}s"),
                _ => DetailContent.Empty
            };
        }

        public override void OnClicked(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var isValidFolder = AssetDatabase.IsValidFolder(assetPath);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (isValidFolder || asset.name == "Assets") return;

            switch (asset)
            {
                case Material material:
                    Selection.activeObject = material.shader;
                    Event.current.Use();
                    break;
                case ScriptableObject scriptableObject:
                    AssetTypeDetail.OpenScriptableObjectScript(scriptableObject.GetType());
                    Event.current.Use();
                    break;
                case Font font:
                    Selection.activeObject = font.material;
                    Event.current.Use();
                    break;
            }
        }

        string GetPrefabInfo(GameObject prefab)
        {
            _builder.Clear();
            // if (prefab.GetComponent<UiElement>() != null)
            //     _builder.Append("UI ");
            // _builder.Append(GetMainPrefabInfo(prefab));
            // if (prefab.GetComponent<IView>() != null)
            //     _builder.Append(" View");
            return _builder.ToString();
        }

        string GetMainPrefabInfo(GameObject prefab)
        {
            if (prefab.GetComponent<Camera>() != null)
                return "Camera";
            if (prefab.GetComponent<Canvas>() != null)
                return "Canvas";
            if (prefab.GetComponent<ParticleSystem>() != null)
                return "ParticleSystem";
            if (prefab.GetComponent<AudioSource>() != null)
                return "AudioSource";
            return Empty;
        }
    }
}
