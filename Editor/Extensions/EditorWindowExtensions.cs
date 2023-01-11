using System;
using System.Linq;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Extensions
{
    public static class EditorWindowExtensions
    {
        public static Rect GetEditorMainWindowPos()
        {
            var containerWindowType = AppDomain.CurrentDomain
                .GetAllDerivedTypes(typeof(ScriptableObject))
                .FirstOrDefault(t => t.Name == "ContainerWindow");

            var showModeField = containerWindowType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWindowType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            var windows = Resources.FindObjectsOfTypeAll(containerWindowType);
            foreach (var window in windows)
            {
                var showmode = (int)showModeField.GetValue(window);
                if (showmode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(window, null);
                    return pos;
                }
            }
            throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWindow(this UnityEditor.EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            var w = (main.width - pos.width) * 0.5f;
            var h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }
    }
}