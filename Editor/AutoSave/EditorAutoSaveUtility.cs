using UnityEditor;

namespace AAA.Editor
{
    [InitializeOnLoad]
    public class EditorAutoSaveUtility
    {
        const string PrefsKey = "AAA.AutoSaveAssets";
        const string MenuItem = "AAA/Editor/Auto Save Assets";

        static bool AutoSaveEnabled
        {
            get => EditorPrefs.GetBool(PrefsKey, true);
            set => EditorPrefs.SetBool(PrefsKey, value);
        }

        static EditorAutoSaveUtility()
        {
            Menu.SetChecked(MenuItem, AutoSaveEnabled);

            EditorWindowFocusUtility.UnityEditorFocusChanged += (focus) =>
            {
                if (!AutoSaveEnabled)
                    return;

                if (!focus)
                    AssetDatabase.SaveAssets();
            };
        }

        [MenuItem(MenuItem)]
        public static void ToggleAutoSaveAssets() => Menu.SetChecked(MenuItem, AutoSaveEnabled = !AutoSaveEnabled);
    }
}
