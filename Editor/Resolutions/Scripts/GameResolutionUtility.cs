using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace AAA.Editor.Editor.Resolutions
{
    public static class GameResolutionUtility
    {
        static List<GameResolutionInfo> _gameResolutionInfos;

        public static List<GameResolutionInfo> GameResolutionInfos
            => _gameResolutionInfos = (_gameResolutionInfos != null && _gameResolutionInfos.Count != 0)
                ? _gameResolutionInfos
                : GetGameResolutionInfos();

        public static List<GameResolutionInfo> GetGameResolutionInfos() =>
            AssetDatabase.FindAssets("t:Object", new[] { DataPath })
                .Select(x => AssetDatabase.LoadAssetAtPath<GameResolutionInfo>(AssetDatabase.GUIDToAssetPath(x)))
                .ToList();

        // Assets/Plugins/AAA.Editor/Resolutions/Data
        private static readonly string DataPath = AssetDatabase.GUIDToAssetPath("6801dac9e14148ee9239fa1cbdb17d92");
    }
}