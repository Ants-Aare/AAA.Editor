using System.IO;
using Plugins.AAA.Editor.Editor.EditorAssemblyReferenceHelpers;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An Asset Postprocessor that runs automatically on any Asset update/addition/change, intercepts any new folders named "Editor" that would need an assembly
/// reference, and asks <see cref="EditorAssemblyReferencesCreator"/> to create it.
/// </summary>
public class EditorFolderAsmrefCreationPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool shouldRefreshAssetDatabase = false;

        foreach (string importedAssetPath in importedAssets)
        {
            if (Directory.Exists(importedAssetPath))
            {
                if (EditorAssemblyReferencesCreator.DoesDirectoryNeedEditorAsmref(importedAssetPath))
                {
                    Debug.Log($"Asmrefs Creator: Found a folder named \"Editor\" at {importedAssetPath}. Will try to create assembly reference to the predefined GameKit Editor Assembly.");

                    shouldRefreshAssetDatabase = true;

                    EditorAssemblyReferencesCreator.CreateEditorAsmrefsInPaths(importedAssetPath);
                }
            }
        }

        if (shouldRefreshAssetDatabase)
            AssetDatabase.Refresh();
    }
}
