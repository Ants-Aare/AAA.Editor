using Plugins.AAA.Editor.Editor.Toolbar;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public class ToolbarScreenShotButton : IToolbarContentGuiRight
    {
        public string Category => ExtendedToolbarHandler.AllGroups;
        public int Priority => 800;
        public bool Visible => true;
        public bool Enabled => true;

        GUIContent _screenshotContent;

        public void Initialize()
        {
            // Texture names found here: https://github.com/halak/unity-editor-icons
            var icon = EditorGUIUtility.IconContent(@"FrameCapture").image;
            _screenshotContent = new GUIContent(null, icon, "Screenshot");
        }

        public void OnGUI()
        {
            if (GUILayout.Button(_screenshotContent, ExtendedToolbarHandler.DefaultButtonStyle))
            {
                EditorWindow.GetWindow(typeof(ScreenshotEditorWindow), false, "Screenshots", true).Show();
            }
        }
    }
}