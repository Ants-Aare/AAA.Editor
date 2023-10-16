using AAA.Editor.Editor.Extensions;
using UnityEngine;

namespace AAA.Editor.Editor.ProjectWindowItems.FolderIconProviders
{
    public class DefaultFolderIconProvider : IFolderIconProvider
    {
        static Texture2D _cachedIcon;
        public Texture2D TryGetFolderIcon(string path) => _cachedIcon ??= AssetDatabaseExtension.LoadAssetFromGUID<Texture2D>("ff881797775d844758c904a143b7d33b");
    }
}
