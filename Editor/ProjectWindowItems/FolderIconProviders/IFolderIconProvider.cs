using UnityEngine;

namespace AAA.Editor.Editor.ProjectWindowItems.FolderIconProviders
{
    public interface IFolderIconProvider
    {
        Texture2D TryGetFolderIcon(string path);
    }
}
