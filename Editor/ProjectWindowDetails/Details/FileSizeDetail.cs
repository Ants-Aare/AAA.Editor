using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public class FileSizeDetail : ProjectWindowDetailBase
    {
        const string ToolTip = "Filesize:\nRed: 64MB\nOrange: 32MB\nYellow: 16MB\nGreen: 8MB\nLightGreen: 1MB";
        protected override bool EnabledByDefault => true;
        public override int Order => 1000;
        static readonly string ProjectPath = Path.Combine(Application.dataPath, "../");
        static readonly Dictionary<string, long> DirectorySizeCache = new();
        static readonly Color Red = new(0.88235295F, 0.34509805F, 0.22745098F);
        static readonly Color Orange = new(0.8980392F, 0.5058824F, 0.22745098F);
        static readonly Color Yellow = new(0.99607843F, 0.73333335F, 0.1882353F);
        static readonly Color Green = new(0.45490196F, 0.79607844F, 0.32156864F);
        static readonly Color LightGreen = new(0.7294118F, 0.8980392F, 0.65882355F);

        public FileSizeDetail()
        {
            Name = "File Size";
            Alignment = TextAlignment.Right;
            ColumnWidth = 65;
        }

        public override DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder)
        {
            var size = GetSize(assetPath, isFolder);
            return new DetailContent(EditorUtility.FormatBytes(size), GetColor(size), ToolTip);
        }

        Color GetColor(long size)
        {
            return size switch
            {
                > 64000000 => Red,
                > 32000000 => Orange,
                > 16000000 => Yellow,
                > 8000000 => Green,
                > 1000000 => LightGreen,
                _ => Color.white
            };
        }

        long GetSize(string path, bool isFolder)
        {
            if (!isFolder) return GetFileSize(path);

            if (!DirectorySizeCache.TryGetValue(path, out var value))
            {
                value = GetDirectorySize(path);
                DirectorySizeCache.Add(path, value);
            }

            return value;
        }

        static long GetFileSize(string assetPath)
        {
            var path = Path.Combine(ProjectPath, assetPath);
            return File.Exists(path) ? new FileInfo(path).Length : 0;
        }

        static long GetDirectorySize(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            long size = files.Sum(x => new FileInfo(x).Length);
            foreach (string s in subdirectories)
                size += GetDirectorySize(s);

            return size;
        }
    }
}
