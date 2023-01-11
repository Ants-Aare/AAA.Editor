using AAA.Editor.Editor.Resolutions;
using Plugins.AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public class MultiResolutionScreenshotTabDrawer : ScreenshotTabDrawer
    {
        public override int Order => -100;
        public override string TabName => "Multi";

        public override void Draw()
        {
            GameResolutionEditorService.DrawResolutionSelection();

            if (GUILayout.Button(new GUIContent("Take Screenshots", "Take Multiresolution Screenshots using the selected Screenshots"), GUILayout.MinHeight(40)))
            {
                ScreenShotService.TakeScreenShots(EditorPrefs.GetBool(ScreenShotService.CopyScreenShotToClipboardKey))
                    .FireAndForget();
            }

            EditorGUILayout.Space(20);
            if (EditorApplication.isPlaying)
                EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button(new GUIContent("Take Gameplay Screenshots", "Enter PlayMode and Take Multiresolution Screenshots of the Main Menu and the Target Level"), GUILayout.MinHeight(40)))
                ScreenShotService.TakeGameplayScreenShots();
            if (EditorApplication.isPlaying)
                EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(20);

            DrawFooter();
        }
    }
}
