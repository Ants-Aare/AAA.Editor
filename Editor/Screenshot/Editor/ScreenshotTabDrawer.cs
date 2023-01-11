using System;
using System.IO;
using AAA.Editor.Editor.Resolutions;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public abstract class ScreenshotTabDrawer : IScreenShotTabProvider
    {
        public virtual int Order => 0;
        public virtual string TabName => "";

        public abstract void Draw();

        protected virtual void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var platform in Enum.GetNames(typeof(Platform)))
                if (GUILayout.Button(new GUIContent($"{platform} Folder", $"Open {platform} Screenshots Folder")))
                    OpenFolder(ScreenShotService.ScreenShotFolderPath + platform);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(new GUIContent($"Screenshots Folder", $"Open Screenshots Folder"), GUILayout.MinHeight(30)))
                OpenFolder(ScreenShotService.ScreenShotFolderPath);

            EditorPrefs.SetBool(ScreenShotService.CopyScreenShotToClipboardKey, EditorGUILayout.Toggle("Copy Result to Clipboard", EditorPrefs.GetBool(ScreenShotService.CopyScreenShotToClipboardKey, false)));
        }

        static void OpenFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            //Both work, but there might be issues with the first on some systems and the second doesn't open the folder, so I'll leave both here for now
            Application.OpenURL($"file://{folderPath}");
            // EditorUtility.RevealInFinder(ScreenShotFolderPath);
        }
    }

    public interface IScreenShotTabProvider
    {
        int Order { get; }
        string TabName { get; }

        void Draw();
    }
}
