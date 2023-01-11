using System.Linq;
using UnityEditor;

namespace AAA.Editor.Editor.Resolutions
{
    public static class GameResolutionEditorService
    {
        public static void DrawResolutionSelection()
        {
            var gameResolutionInfos = GameResolutionUtility.GetGameResolutionInfos();
            var splitList = gameResolutionInfos.GroupBy(x => x.Platform);
            var labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.LabelField("Resolutions:");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            foreach (var platform in splitList)
            {
                var prefsKey = "ScreenPlatform" + platform.Key;
                var isFoldedOut = EditorPrefs.GetBool(prefsKey, true);
                EditorGUIUtility.labelWidth = 70;
                isFoldedOut = EditorGUILayout.Toggle(platform.Key.ToString(), isFoldedOut);
                EditorPrefs.SetBool(prefsKey, isFoldedOut);
                if (isFoldedOut)
                {
                    // EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    foreach (var gameResolutionInfo in platform)
                    {
                        var key = $"EnableScreenshot{gameResolutionInfo.name}";
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorPrefs.SetBool(key, EditorGUILayout.Toggle(EditorPrefs.GetBool(key, true)));
                        EditorGUIUtility.labelWidth = 70;
                        EditorGUILayout.LabelField(gameResolutionInfo.name);
                        gameResolutionInfo.Resolution.x = EditorGUILayout.IntField("width", gameResolutionInfo.Resolution.x);
                        gameResolutionInfo.Resolution.y = EditorGUILayout.IntField("height", gameResolutionInfo.Resolution.y);
                        EditorGUI.EndChangeCheck();
                        EditorGUILayout.EndHorizontal();
                    }

                    // EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(10);
                }
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.Space(20);
        }
    }
}
