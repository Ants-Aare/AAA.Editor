using System;
using AAA.Editor.Editor.Extensions;
using UnityEditor;

namespace AAA.Editor.AssetPostProcessors
{
    /// <summary>
    /// Ensures that source generator DLLs have the necessary settings to work in Unity.
    /// See: https://docs.unity3d.com/Manual/roslyn-analyzers.html
    /// </summary>
    internal class SourceGeneratorImporter : AssetPostprocessor
    {
        private void OnPreprocessAsset()
        {
            switch (assetImporter)
            {
                case PluginImporter pluginImporter
                    when assetImporter.importSettingsMissing && IsSourceGeneratorPath(assetPath):
                    pluginImporter.SetCompatibleWithAnyPlatform(false);
                    assetImporter.EnsureLabel("RoslynAnalyzer");
                    assetImporter.EnsureLabel("SourceGenerator");
                    assetImporter.EnsureLabel("RunOnlyOnAssembliesWithReference");
                    pluginImporter.SaveAndReimport();
                    break;
                default:
                    return;
            }
        }

        private static bool IsSourceGeneratorPath(string path)
            => path.EndsWith(".Generator.dll", StringComparison.OrdinalIgnoreCase);

        
    }
}