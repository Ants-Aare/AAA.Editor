using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details;
using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails
{
    [InitializeOnLoad]
    public static class ProjectWindowDetailsDrawer
    {
        const string DrawProjectWindowDetailsKey = "DrawProjectWindowDetails";
        const int SpaceBetweenColumns = 10;
        const int MenuIconWidth = 20;

        static bool HasClicked => Event.current.type == EventType.MouseDown && Event.current.button == 0;

        static readonly List<ProjectWindowDetailBase> Details = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.BaseType == typeof(ProjectWindowDetailBase))
            .Select(type => (ProjectWindowDetailBase)Activator.CreateInstance(type))
            .ToList();

        [MenuItem("AAA/Editor/Toggle Asset Details")]
        public static void ToggleProjectWindowDetails()
        {
            var enabled = !EditorPrefs.GetBool(DrawProjectWindowDetailsKey, true);
            EditorPrefs.SetBool(DrawProjectWindowDetailsKey, enabled);
            if (enabled)
            {
                EditorApplication.projectWindowItemOnGUI -= DrawProjectWindowDetails;
                EditorApplication.projectWindowItemOnGUI += DrawProjectWindowDetails;
            }
            else
            {
                EditorApplication.projectWindowItemOnGUI -= DrawProjectWindowDetails;
            }
        }

        static ProjectWindowDetailsDrawer()
        {
            Details.Sort((detail1, detail2) => detail1.Order.CompareTo(detail2.Order));
            if (EditorPrefs.GetBool(DrawProjectWindowDetailsKey, true))
            {
                EditorApplication.projectWindowItemOnGUI -= DrawProjectWindowDetails;
                EditorApplication.projectWindowItemOnGUI += DrawProjectWindowDetails;
            }
        }

        static void DrawProjectWindowDetails(string guid, Rect rect)
        {
            if (Application.isPlaying || IsProjectListAsset(rect)) return;
            if (!EditorPrefs.GetBool(DrawProjectWindowDetailsKey, true)) return;

            var isSelected = Array.IndexOf(Selection.assetGUIDs, guid) >= 0;

            rect.x += rect.width;
            rect.x -= MenuIconWidth;
            rect.width = MenuIconWidth;

            if (isSelected)
                ApplyInput(rect, guid);

            if (Event.current.type != EventType.Repaint) return;
            if (isSelected)
                EditorGUI.LabelField(rect, EditorGUIUtility.IconContent("_Menu"));

            DrawDetails(guid, rect);
        }

        static void ApplyInput(Rect rect, string guid)
        {
            if (!HasClicked) return;
            if (rect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                ShowContextMenu(rect);
            }
            else
            {
                for (var i = Details.Count - 1; i >= 0; i--)
                {
                    var detail = Details[i];
                    if (!detail.Visible)
                        continue;

                    rect.width = detail.ColumnWidth;
                    rect.x -= detail.ColumnWidth + SpaceBetweenColumns;
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        detail.OnClicked(guid);
                    }
                }
            }
        }

        static void DrawDetails(string guid, Rect rect)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            var isValidFolder = AssetDatabase.IsValidFolder(assetPath);

            var contentColor = GUI.contentColor;
            for (var i = Details.Count - 1; i >= 0; i--)
            {
                var detail = Details[i];
                if (!detail.Visible)
                    continue;

                rect.width = detail.ColumnWidth;
                rect.x -= detail.ColumnWidth + SpaceBetweenColumns;
                var detailContent = detail.GetLabel(guid, assetPath, asset, isValidFolder);
                GUI.contentColor = detailContent.DetailColor;
                GUI.Label(rect, new GUIContent(detailContent.DetailText, detailContent.DetailTooltip), GetStyle(detail.Alignment));
            }

            GUI.contentColor = contentColor;
        }

        static GUIStyle GetStyle(TextAlignment alignment)
            => alignment == TextAlignment.Left ? EditorStyles.boldLabel : new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleRight };

        static void ShowContextMenu(Rect rect)
        {
            var rectXMax = rect.xMax;
            rect.xMin = rectXMax - 150;
            var menu = new GenericMenu();
            foreach (var detail in Details)
            {
                menu.AddItem(new GUIContent(detail.Name), detail.Visible, ToggleMenu, detail);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Deselect All"), false, HideAllDetails);
            menu.AddItem(new GUIContent("Disable"), false, ToggleProjectWindowDetails);
            // menu.AddItem(new GUIContent("Help"), false, () => Application.OpenURL(""));
            menu.DropDown(rect);
        }

        static void HideAllDetails()
        {
            foreach (var detail in Details)
                detail.Visible = false;
        }

        static void ToggleMenu(object data)
        {
            var detail = (ProjectWindowDetailBase)data;
            detail.Visible = !detail.Visible;
        }

        // Don't draw details if project view shows large preview icons or it's a sub asset
        static bool IsProjectListAsset(Rect rect)
            => rect.height > 20 || rect.width < 400;
    }
}
