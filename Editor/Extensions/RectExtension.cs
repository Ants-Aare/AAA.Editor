using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Extensions
{
    public static class RectExtension
    {
        public static Rect AddPadding(this Rect rect, float padding)
        {
            return rect.AddPadding(new Vector2(padding, padding));
        }

        public static Rect AddPadding(this Rect rect, Vector2 padding)
        {
            var halfPadding = padding / 2f;
            rect.max += halfPadding;
            rect.min -= halfPadding;
            return rect;
        }
    }
}