using System;
using System.Collections.Generic;
using AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace AAA.Editor.Editor.Toolbar
{
    [InitializeOnLoad]
    public class ExtendedToolbarHandler
    {
        public static GUIStyle DefaultButtonStyle;
        public static GUIStyle DefaultDropDownStyle;
        public static void Space() => GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.85f);
        static List<IToolbarContentGuiLeft> _contentGroupsLeft;
        static List<IToolbarContentGuiRight> _contentGroupsRight;
        static int _selectedUserGroup = 0;
        const int SpaceThreshold = 50;
        const string UserGroupPref = "ExtendedToolbar.UserGroup";

        public const string AllGroups = null;
        public const string Game = "Game";
        public const string Dev = "Dev";
        public const string LevelDesigner = "Level Designer";
        public const string Artist = "Artist";
        public const string Qa = "QA";

        static readonly string[] UserGroups = {
            Game, Dev, LevelDesigner,  Artist, Qa,
        };

        static ExtendedToolbarHandler()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);

            _contentGroupsLeft = new List<IToolbarContentGuiLeft>(AppDomain.CurrentDomain.GetInstancesOf<IToolbarContentGuiLeft>());
            if (_contentGroupsLeft.Count > 0)
                _contentGroupsLeft.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            _contentGroupsRight = new List<IToolbarContentGuiRight>(AppDomain.CurrentDomain.GetInstancesOf<IToolbarContentGuiRight>());
            if (_contentGroupsRight.Count > 0)
                _contentGroupsRight.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            foreach (var content in _contentGroupsLeft)
                content.Initialize();
            foreach (var content in _contentGroupsRight)
                content.Initialize();

            var userGroup = EditorPrefs.GetString(UserGroupPref, Dev);
            _selectedUserGroup = ArrayUtility.FindIndex(UserGroups, x => x == userGroup);
        }

        static void OnLeftToolbarGUI()
        {
            SetDefaultStyles();

            // decoration to fit exactly with Unity spacing
            GUILayout.Space(2f);
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                _selectedUserGroup = EditorGUILayout.Popup(Mathf.Clamp(_selectedUserGroup, 0, UserGroups.Length), UserGroups, DefaultDropDownStyle);
                if (change.changed)
                {
                    EditorPrefs.SetString(UserGroupPref, UserGroups[_selectedUserGroup]);
                }
            }

            if (_contentGroupsLeft.Count == 0)
                return;

            GUILayout.FlexibleSpace();
            IToolbarContentGui previousVisibleGroup = null;
            foreach (var group in _contentGroupsLeft)
            {
                if (group.Visible && (group.Category == AllGroups || group.Category == UserGroups[_selectedUserGroup]))
                {
                    if (previousVisibleGroup != null && previousVisibleGroup.Priority + SpaceThreshold <= group.Priority)
                    {
                        Space();
                    }
                    using (new EditorGUI.DisabledScope(!group.Enabled))
                    {
                        group.OnGUI();
                    }
                    previousVisibleGroup = group;
                }
            }
        }

        static void OnRightToolbarGUI()
        {
            if (_contentGroupsRight.Count == 0)
                return;

            SetDefaultStyles();

            IToolbarContentGui previousVisibleGroup = null;
            foreach (var group in _contentGroupsRight)
            {
                if (group.Visible && (group.Category == null || group.Category == UserGroups[_selectedUserGroup]))
                {
                    if (previousVisibleGroup != null && previousVisibleGroup.Priority + SpaceThreshold <= group.Priority)
                    {
                        Space();
                    }
                    using (new EditorGUI.DisabledScope(!group.Enabled))
                    {
                        group.OnGUI();
                    }
                    previousVisibleGroup = group;
                }
            }
        }

        static void SetDefaultStyles()
        {
            DefaultButtonStyle ??= new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedHeight = 18,
                fixedWidth = 33,
                margin = new RectOffset(0, 0, 1, 0),
                padding = new RectOffset(0, 0, -1, 0),
            };

            DefaultDropDownStyle ??= new GUIStyle(EditorStyles.toolbarPopup)
            {
                fixedHeight = 18,
                margin = new RectOffset(0, 0, 0, -1),
            };
        }
    }

    public interface IToolbarContentGui
    {
        // Use
        public string Category { get; }
        public int Priority { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        public void Initialize();
        public void OnGUI();
    }

    public interface IToolbarContentGuiLeft : IToolbarContentGui
    {
    }

    public interface IToolbarContentGuiRight : IToolbarContentGui
    {
    }
}