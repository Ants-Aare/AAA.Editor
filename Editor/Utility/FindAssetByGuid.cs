using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AAA.Editor.Editor.Utility
{
    /// <summary>
    /// Unity Editor tool to find an Asset with a specific GUID.
    /// </summary>
    public class FindAssetByGuid : EditorWindow
    {
        [MenuItem("AAA/Find/Asset by GUID", false, 4)]
        static void Init()
        {
            FindAssetByGuid window = (FindAssetByGuid)GetWindow(typeof(FindAssetByGuid));
            window.Show();
        }

        void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            root.style.paddingTop = 10;
            root.style.paddingLeft = 5;
            root.style.paddingRight = 5;
            root.style.paddingBottom = 10;

            var descriptionText = new TextElement();
            descriptionText.text = "Search for assets by GUID";
            descriptionText.style.paddingBottom = 10;
            root.Add(descriptionText);

            var guidTextField = new TextField("Enter a GUID");
            guidTextField.style.paddingBottom = 5;
            root.Add(guidTextField);

            var searchButton = new Button(() => { OnSearch(guidTextField.value); });

            searchButton.contentContainer.Add(new Label("Search"));
            root.Add(searchButton);
        }

        void OnSearch(string guidToSearchFor)
        {
            if (guidToSearchFor.Length != 32)
            {
                EditorUtility.DisplayDialog("Invalid GUID!", "The requested GUID seems to have a wrong number of digits (should be 32)", "Ok");
                return;
            }

            Debug.Log($"Requested search for: {guidToSearchFor}");

            var assetPath = AssetDatabase.GUIDToAssetPath(guidToSearchFor);

            if (string.IsNullOrEmpty(assetPath))
            {
                EditorUtility.DisplayDialog("No results!", "No assets with this GUID were found.", "Ok");
                return;
            }

            var assetInProject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            Selection.activeObject = assetInProject;
            EditorGUIUtility.PingObject(assetInProject);
        }
    }
}
