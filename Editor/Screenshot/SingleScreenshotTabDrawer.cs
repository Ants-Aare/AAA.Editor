using AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public class SingleScreenshotTabDrawer : ScreenshotTabDrawer
    {
        public override int Order => 0;
        public override string TabName => "Single";

        bool _isOverrideResolution = true;
        bool _isTransparentScreenShot = false;
        Vector2Int _overrideTargetResolution = new Vector2Int(1080, 1920);

        public override void Draw()
        {
            EditorGUILayout.LabelField("Resolution:");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            var labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 70;
            _isOverrideResolution = EditorGUILayout.Toggle("Override", _isOverrideResolution);

            if (_isOverrideResolution)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var width = EditorGUILayout.IntField("Width", _overrideTargetResolution.x);
                var height = EditorGUILayout.IntField("Height", _overrideTargetResolution.y);
                EditorGUI.EndChangeCheck();
                _overrideTargetResolution = new Vector2Int(width, height);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            if (GUILayout.Button(new GUIContent("Take Single Screenshot", "Take Screenshot using the current Resolution or the override Resolution"), GUILayout.MinHeight(40)))
            {
                var copyResult = EditorPrefs.GetBool(ScreenShotService.CopyScreenShotToClipboardKey);

                if (_isOverrideResolution)
                {
                    ScreenShotService.TakeScreenShot("ScreenShot", copyResult, _isTransparentScreenShot)
                        .FireAndForget();
                }
                else
                {
                    ScreenShotService.TakeScreenShotWithResolution("ScreenShot", _overrideTargetResolution, copyResult, _isTransparentScreenShot)
                        .FireAndForget();
                }
            }

            EditorGUILayout.Space(20);
            EditorGUIUtility.labelWidth = labelWidth;

            DrawFooter();
            _isTransparentScreenShot = EditorApplication.isPlaying && EditorGUILayout.Toggle("Transparent", _isTransparentScreenShot);
        }
    }
}
