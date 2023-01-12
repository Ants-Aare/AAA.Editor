using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Plugins.AAA.Editor.Editor.Utility
{
    public class ResetPrefsUtility
    {
        [MenuItem("AAA/Utility/Reset Player Prefs", false, 15)]
        public static void ResetUserData()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("AAA/Utility/Reset Editor Prefs", false, 16)]
        public static void ResetEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
        
        [MenuItem("AAA/Utility/Show Data Path", false, 13)]
        public static void ShowDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}