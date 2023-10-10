using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.AAA.Editor.Editor.ProjectWindowDetails.Details
{
    public abstract class ProjectWindowDetailBase : IComparable<ProjectWindowDetailBase>
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

        public int CompareTo(ProjectWindowDetailBase other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Order.CompareTo(other.Order);
        }
    }
}
