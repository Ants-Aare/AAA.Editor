using UnityEngine;

namespace AAA.Editor.Editor.Resolutions
{
    [CreateAssetMenu(menuName = "AAA/Screenshot Resolution Data", fileName = "ScreenshotResolutionData", order = 0)]
    public class GameResolutionInfo : ScriptableObject
    {
        public Vector2Int Resolution;
        public Platform Platform;
    }

    public enum Platform
    {
        Android,
        iOS,
    }
}
