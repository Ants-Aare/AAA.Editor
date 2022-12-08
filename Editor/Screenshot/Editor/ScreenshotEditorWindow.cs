using System;
using System.IO;
using System.Linq;
using AAA.Editor.Editor.Resolutions;
using Plugins.AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot.Editor
{
    public class ScreenshotEditorWindow : EditorWindow
    {
        bool _copyResult = true;
        int _selectedTab;
        // List of Default resolutions 👍
        // Way to add resolutions 👍
        // take screenshots on main menu
        // take screenshots in level (with delay)
        // take screenshots in all levels (with delay)
        // take screenshots with transparency
        // take screenshots for different platforms ios/android 👍
        // take single screenshot
        // take single screenshot with current resolution
        // Button to open screenshot folder 👍
        // copy result to clipboard👍

        [MenuItem("AAA/Screenshot/Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = EditorWindow.GetWindow(typeof(ScreenshotEditorWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            _selectedTab = GUILayout.Toolbar(_selectedTab, new[] { "Multi", "Single", "Levels", });
            switch (_selectedTab)
            {
                case 0:
                    DrawMultiScreenshotWindow();
                    break;
                case 1:
                    DrawSingleScreenshotWindow();
                    break;
                case 2:
                    DrawLevelsScreenshotWindow();
                    break;
                default:
                    break;
            }

        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var platform in Enum.GetNames(typeof(Platform)))
                if (GUILayout.Button($"Open {platform} Screenshots Folder"))
                    OpenFolder(ScreenShotService.ScreenShotFolderPath + platform);
            EditorGUILayout.EndHorizontal();
            _copyResult = EditorGUILayout.Toggle("Copy Result to Clipboard", _copyResult);
        }

        static void OpenFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            //Both work, but there might be issues with the first on some systems and the second doesn't open the folder, so I'll leave both here for now
            Application.OpenURL($"file://{folderPath}");
            // EditorUtility.RevealInFinder(ScreenShotFolderPath);
        }

        void DrawMultiScreenshotWindow()
        {
            GameResolutionEditorService.DrawResolutionSelection();

            if(!EditorApplication.isPlaying)
                EditorGUI.BeginDisabledGroup(true);

            if (GUILayout.Button("Take Screenshots now (Runtime)", GUILayout.MinHeight(40)))
            {
                ScreenShotService.TakeScreenShots(GetRequiredResolutions(), _copyResult).FireAndForget();
            }

            if(!EditorApplication.isPlaying)
                EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Take Test Screenshots", GUILayout.MinHeight(40)))
            {
                //Open Game

                //Wait for loading screen to finish

                //Take main menu screenshot

                //Take screenshot of provided level

                // CoroutineRunner.Run(ScreenShotService.TakeScreenShots(GetRequiredResolutions()));
            }
            DrawHeader();
        }

        static GameResolutionInfo[] GetRequiredResolutions()
        {
            return GameResolutionUtility.GetGameResolutionInfos()
                .Where(x => EditorPrefs.GetBool($"EnableScreenshot{x.name}")).ToArray();
        }

        void DrawLevelsScreenshotWindow()
        {
        }

        void DrawSingleScreenshotWindow()
        {
        }


        void TakeScreenShots(GameResolutionInfo[] requiredResolutions, int[] levels)
        {

        }
    }
}
