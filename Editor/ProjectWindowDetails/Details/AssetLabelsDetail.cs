using System.IO;
using System.Text;
using AAA.Editor.Editor.ProjectWindowDetails;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using static System.String;

namespace AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class AssetLabelsDetail : ProjectWindowDetailBase
    {
        protected override bool EnabledByDefault => true;
        readonly StringBuilder _builder = new();

        public AssetLabelsDetail()
        {
            Name = "Asset Labels";
            ColumnWidth = 250;
            Alignment = TextAlignment.Left;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
        {
            var strings = AssetDatabase.GetLabels(new GUID(guid));
            _builder.Clear();
            for (var i = 0; i < strings.Length; i++)
            {
                if(i != 0)
                    _builder.Append('|');
                _builder.Append(strings[i]);
            }
            return new DetailContent(_builder.ToString());
        }
    }
}
