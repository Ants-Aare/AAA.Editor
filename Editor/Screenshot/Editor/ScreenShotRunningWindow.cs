using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public class ScreenShotRunningWindow : EditorWindow
    {
        void OnGUI()
        {
            GUILayout.Button("Taking Screenshots.\nPlease Wait.\nDo not close this Window", GUILayout.MinHeight(400));
        }
    }
}
