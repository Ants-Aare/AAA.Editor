using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Screenshot
{
    public class ScreenshotEditorWindow : EditorWindow
    {
        int _selectedTab;

        static List<IScreenShotTabProvider> _screenshotTabDrawers;
        static string[] _toolBarTexts;
        string[] toolBarTexts => _toolBarTexts ??= screenshotTabDrawers.Select(x => x.TabName).ToArray();

        List<IScreenShotTabProvider> screenshotTabDrawers
        {
            get
            {
                if (_screenshotTabDrawers == null || _screenshotTabDrawers.Count == 0)
                {
                    _screenshotTabDrawers =
                        new List<IScreenShotTabProvider>(
                            AppDomain.CurrentDomain.GetInstancesOf<IScreenShotTabProvider>());
                    _screenshotTabDrawers.Sort((x, y) => x.Order.CompareTo(y.Order));
                }

                return _screenshotTabDrawers;
            }
        }

        [MenuItem("AAA/Screenshots")]
        static void ShowScreenshotsWindow()
            => GetWindow(typeof(ScreenshotEditorWindow), false, "Screenshots", true).Show();

        void OnGUI()
        {
            GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            if (screenshotTabDrawers.Count == 0)
            {
                EditorGUILayout.LabelField("No ScreenShotTabDrawers found.");
                return;
            }

            _selectedTab = GUILayout.Toolbar(_selectedTab, toolBarTexts);
            screenshotTabDrawers[_selectedTab].Draw();
        }
    }
}