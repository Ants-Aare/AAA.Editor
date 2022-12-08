using UnityEngine;

namespace ArtKit.Resolutions
{
    [CreateAssetMenu(menuName = "ArtKit/Screenshot Resolution Data", fileName = "ScreenshotResolutionData", order = 0)]
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
