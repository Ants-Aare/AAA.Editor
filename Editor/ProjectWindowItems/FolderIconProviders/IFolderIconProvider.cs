using UnityEngine;

namespace Plugins.AAA.Editor.Editor.ProjectWindowItems.FolderIconProviders
{
    public interface IFolderIconProvider
    {
        Texture2D TryGetFolderIcon(string path);
    }
}
