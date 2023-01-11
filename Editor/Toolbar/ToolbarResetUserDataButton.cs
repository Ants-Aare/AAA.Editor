using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.Toolbar
{
    public class ToolbarResetUserDataButton : IToolbarContentGuiLeft
    {
        public string Category => ExtendedToolbarHandler.AllGroups;
        public int Priority => 800;
        public bool Visible => true;
        public bool Enabled => true;

        GUIContent _resetUserDataContent;

        public void Initialize()
        {
            var icon = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _resetUserDataContent = new GUIContent(null, icon, "Reset User Data");
        }

        public void OnGUI()
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(_resetUserDataContent, ExtendedToolbarHandler.DefaultButtonStyle))
                PlayerPrefs.DeleteAll();
            GUI.backgroundColor = color;
        }
    }
}
