using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Plugins.AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.AAA.Editor.Editor.EditorAssemblyReferenceHelpers
{
    /// <summary>
    /// A helper script to automate the creation of Assembly Definition References (.asmref) in folders named "Editor".
    /// <remarks>Normally, "Editor" folders are "magically" added to a Unity auto-generated Editor assembly. But when
    /// we have custom Assemblies, we have to create explicit Assembly References to make our custom Editor assembly reference them.</remarks>
    /// </summary>
    public class EditorAssemblyReferencesCreator : EditorWindow
    {
        const string EditorAssemblyReferenceTemplate = "{{\"reference\": \"GUID:{0}\"}}";

        static IEnumerable<string> _directoriesNamedEditorWithoutAsmdefOrAsmrefFile;

        public static string GuidOfDefaultEditorAssembly => "653876658ca5c465f8c44bec51bfea98";

        #region GUI

        /// <summary>
        /// Creates a custom Editor GUI to manually initialize the creation of .asmrefs. Could be needed for already existing
        /// folders named "Editor" that can't go through the automated Post-Processor flow in <see cref="EditorFolderAsmrefCreationPostprocessor"/>.
        /// </summary>
        [MenuItem("GameKit/Editor Assemblies/Create Editor Folder Assembly References", false, 101)]
        public static void Init()
        {
            EditorAssemblyReferencesCreator wnd = GetWindow<EditorAssemblyReferencesCreator>();
            wnd.titleContent = new GUIContent("Create Editor Assembly References");
            wnd.ShowModal();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var titleLabel = new Label("Please give the GUID of the (Editor) Assembly that you want to be referenced by all Editor folders (that have no .asmref)");
            titleLabel.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
            titleLabel.style.paddingTop = 10f;
            titleLabel.style.paddingBottom = 10f;
            root.Add(titleLabel);

            var inputTextField = new TextField("GUID of Editor Assembly");
            inputTextField.value = GuidOfDefaultEditorAssembly;;
            inputTextField.layout.AddPadding(10);
            root.Add(inputTextField);

            var acceptButton = new Button();
            acceptButton.text = "Create asmrefs";
            acceptButton.clicked += () =>
            {
                var directoriesThatNeedAsmrefs = GetDirectoriesThatNeedAsmrefs();
                var numberOfDirectoriesThatNeedAsmrefs = directoriesThatNeedAsmrefs.Count();
                var guidOfEditorAssemblyReference = inputTextField.text;

                if (!IsGuidAValidAsmdef(guidOfEditorAssemblyReference))
                {
                    EditorUtility.DisplayDialog("Creation of asmrefs inconclusive", "The entered Editor Assembly Definition GUID is wrong or empty!", "Ok");
                }
                else if (numberOfDirectoriesThatNeedAsmrefs == 0)
                {
                    EditorUtility.DisplayDialog("Creation of asmrefs inconclusive", "No \"Editor\" folders without asmrefs were found!", "Ok");
                }
                else
                {
                    try
                    {
                        CreateAsmrefInPaths(directoriesThatNeedAsmrefs, guidOfEditorAssemblyReference);
                        EditorUtility.DisplayDialog("Creation of asmrefs complete!", $"Asmrefs pointing to {guidOfEditorAssemblyReference} were created for {numberOfDirectoriesThatNeedAsmrefs} folders", "Ok");
                    }
                    catch
                    {
                        EditorUtility.DisplayDialog("Creation of asmrefs failed!", "Some exception was thrown! You might have to check your Git changes - and maybe revert!", "Ok");
                    }
                }

                GetWindow<EditorAssemblyReferencesCreator>().Close();
            };

            var cancelButton = new Button();
            cancelButton.text = "Cancel";
            cancelButton.clicked += () =>
            {
                Debug.Log("Cancelled");
                GetWindow<EditorAssemblyReferencesCreator>().Close();
            };

            root.Add(acceptButton);
            root.Add(cancelButton);
        }

        #endregion

        public static bool DoesDirectoryNeedEditorAsmref(string directoryPath) => Path.GetFileName(directoryPath) == "Editor" && DirectoryDoesNotContainAsmdefOrAsmref(directoryPath);

        public static void CreateEditorAsmrefsInPaths(string directoryPath) => CreateEditorAsmrefsInPaths(new List<string> {directoryPath});

        public static void CreateEditorAsmrefsInPaths(IEnumerable<string> directoryPaths) => CreateAsmrefInPaths(directoryPaths, GuidOfDefaultEditorAssembly);

        static void CreateAsmrefInPaths(IEnumerable<string> directoryPaths, string guidOfEditorAssemblyReference)
        {
            var contentOfGeneratedAsmref = String.Format(EditorAssemblyReferenceTemplate, guidOfEditorAssemblyReference);

            foreach (var directoryPath in directoryPaths)
            {
                File.WriteAllText(Path.Combine(directoryPath, "Editor.asmref"), contentOfGeneratedAsmref);
            }
        }

        static bool DirectoryDoesNotContainAsmdefOrAsmref(string directoryPath)
        {
            return (Directory.GetFiles(directoryPath, "*.asmdef", SearchOption.TopDirectoryOnly).Length == 0)
                   && (Directory.GetFiles(directoryPath, "*.asmref", SearchOption.TopDirectoryOnly).Length == 0);
        }

        IEnumerable<string> GetDirectoriesThatNeedAsmrefs()
        {
            var directoriesUnderAssets = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories);

            var directoriesThatNeedAsmrefs = from directoryPath in directoriesUnderAssets
                where DoesDirectoryNeedEditorAsmref(directoryPath)
                select directoryPath;

            return directoriesThatNeedAsmrefs;
        }

        bool IsGuidAValidAsmdef(string guidToSearchFor)
        {
            if (guidToSearchFor.Length != 32)
                return false;

            var assetPath = AssetDatabase.GUIDToAssetPath(guidToSearchFor);
            var assetExtension = Path.GetExtension(assetPath);

            return assetExtension == ".asmdef";
        }
    }
}
