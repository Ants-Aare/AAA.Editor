using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Toolbar
{
    public class ToolBarBuildSettingsButton : IToolbarContentGuiLeft
    {
        public string Category => ExtendedToolbarHandler.AllGroups;
        public int Priority => 902;
        public bool Visible => true;
        public bool Enabled => true;

        GUIContent _preferencesContent;

        public void Initialize()
        {
            // Texture names found here: https://github.com/halak/unity-editor-icons
            var icon = EditorGUIUtility.IconContent(@"d_UnityEditor.ConsoleWindow@2x").image;
            _preferencesContent = new GUIContent(null, icon, "Build Settings");
        }

        public void OnGUI()
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button(_preferencesContent, ExtendedToolbarHandler.DefaultButtonStyle))
                EditorWindow.GetWindow(typeof(BuildPlayerWindow), false, "Build Settings", true);

            GUI.backgroundColor = color;
        }
    }
}
