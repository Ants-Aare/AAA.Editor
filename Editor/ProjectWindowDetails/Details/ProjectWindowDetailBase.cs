using UnityEditor;
using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public abstract class ProjectWindowDetailBase
    {
        const string ShowPrefsKey = "ProjectWindowDetails.Show.";
        public int ColumnWidth = 100;
        public string Name = "Base";
        public TextAlignment Alignment = TextAlignment.Left;
        protected virtual bool EnabledByDefault => false;
        public virtual int Order => 0;

        public bool Visible
        {
            get => EditorPrefs.GetBool(string.Concat(ShowPrefsKey, Name), EnabledByDefault);

            set => EditorPrefs.SetBool(string.Concat(ShowPrefsKey, Name), value);
        }

        public abstract DetailContent GetLabel(string guid, string assetPath, Object asset, bool isFolder);

        public virtual void OnClicked(string guid)
        {
        }
    }
}
