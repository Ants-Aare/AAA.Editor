using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ArtKit.Resolutions
{
    public static class GameResolutionUtility
    {
        static List<GameResolutionInfo> _gameResolutionInfos;

        public static List<GameResolutionInfo> GameResolutionInfos =>
            _gameResolutionInfos ??= GetGameResolutionInfos();
        public static List<GameResolutionInfo> GetGameResolutionInfos() =>
            AssetDatabase.FindAssets("t:Object", new[]{"Assets/ArtTools/Resolutions/Data"})
                .Select(x=> AssetDatabase.LoadAssetAtPath<GameResolutionInfo>(AssetDatabase.GUIDToAssetPath(x)))
                .ToList();
    }
}
