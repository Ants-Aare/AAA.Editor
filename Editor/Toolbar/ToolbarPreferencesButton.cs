using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Toolbar
{
    public class ToolbarPreferencesButton : IToolbarContentGuiLeft
    {
        public string Category => ExtendedToolbarHandler.AllGroups;
        public int Priority => 901;
        public bool Visible => true;
        public bool Enabled => true;

        GUIContent _preferencesContent;

        public void Initialize()
        {
            // Texture names found here: https://github.com/halak/unity-editor-icons
            var icon = EditorGUIUtility.IconContent(@"Preset.Context").image;
            _preferencesContent = new GUIContent(null, icon, "Preferences");
        }

        public void OnGUI()
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button(_preferencesContent, ExtendedToolbarHandler.DefaultButtonStyle))
                EditorApplication.ExecuteMenuItem("Unity/Preferences");
        
            GUI.backgroundColor = color;
        }
    }
}
