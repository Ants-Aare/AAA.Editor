using System;
using UnityEditor;

namespace AAA.Editor
{
    public static class EditorWindowFocusUtility
    {
        public static Action<bool> UnityEditorFocusChanged
        {
            get
            {
                var fieldInfo = typeof(EditorApplication).GetField("focusChanged",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                return (Action<bool>)fieldInfo.GetValue(null);
            }
            set
            {
                var fieldInfo = typeof(EditorApplication).GetField("focusChanged",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                fieldInfo.SetValue(null, value);
            }
        }
    }
}
