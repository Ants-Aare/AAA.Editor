using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Toolbar
{
    public class ToolbarFrameDebuggerButton : IToolbarContentGuiRight
    {
        public string Category => ExtendedToolbarHandler.AllGroups;
        public int Priority => 901;
        public bool Visible => true;
        public bool Enabled => true;

        GUIContent _preferencesContent;

        public void Initialize()
        {
            // Texture names found here: https://github.com/halak/unity-editor-icons
            var icon = EditorGUIUtility.IconContent(@"d_Profiler.GPU").image;
            _preferencesContent = new GUIContent(null, icon, "Profiler");
        }

        public void OnGUI()
        {
            if (!EditorApplication.isPlaying) return;
            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button(_preferencesContent, ExtendedToolbarHandler.DefaultButtonStyle))
            {
                EditorApplication.ExecuteMenuItem("Window/Analysis/Frame Debugger");
                EditorApplication.isPaused = true;
            }

            GUI.backgroundColor = color;
        }
    }
}
