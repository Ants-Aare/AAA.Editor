using System;
using System.Collections.Generic;
using System.Linq;
using AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AAA.Editor.Editor.Utility
{
    /// <summary>
    /// An editor tool that locates all the textures in the project that are not power of two(PoT) and also not in an atlas.
    /// </summary>
    public class CheckPoTTexture : EditorWindow
    {
        ScrollView _results;

        [MenuItem("AAA/Find/Texture not power of two", false, 3)]
        static void Init()
        {
            var window = (CheckPoTTexture)GetWindow(typeof(CheckPoTTexture));
            window.Show();
        }

        void CreateGUI()
        {
            var root = rootVisualElement;

            root.style.paddingTop = 10;
            root.style.paddingLeft = 5;
            root.style.paddingRight = 5;
            root.style.paddingBottom = 10;

            var searchButton = new Button(OnSearch);
            searchButton.contentContainer.Add(new Label("Search"));
            root.Add(searchButton);
            _results = new ScrollView()
            {
                style =
                {
                    paddingTop = 10,
                    paddingBottom = 10
                }
            };
            root.Add(_results);
        }

        void OnSearch()
        {
            _results.Clear();

            var sprites = AssetDatabaseExtension.FindAssetsByType<Sprite>();
            var atlantes = AssetDatabaseExtension.FindAssetsByType<SpriteAtlas>();

            var problemSprites = sprites.Where(sprite => IsSpriteProblematic(sprite, atlantes)).ToList();

            problemSprites.Sort((x, y)=>IsDemoOrExample(x).CompareTo(IsDemoOrExample(y)));
            
            foreach (var sprite in problemSprites)
            {
                var buttonVisualElement = new Button(() => { Selection.activeObject = sprite; });
                
                var isDemoOrExample = IsDemoOrExample(sprite);
                if (!isDemoOrExample)
                    buttonVisualElement.style.backgroundColor = new StyleColor(Color.grey);

                buttonVisualElement.text = sprite.name;
                buttonVisualElement.tooltip = isDemoOrExample
                    ? "This is just a Texture that is being used in a Demo Scene. Feel free to ignore."
                    : "Select Sprite";

                _results.Add(buttonVisualElement);
            }
        }

        // Problematic means sprite is not a PoT and not in any of these atlantes.
        static bool IsSpriteProblematic(Sprite sprite, IEnumerable<SpriteAtlas> atlantes) =>
            !IsPowerOfTwoSprite(sprite) && !IsInsideAnyAtlas(sprite, atlantes);

        private static bool IsDemoOrExample(Sprite sprite)
        {
            var assetPath = AssetDatabase.GetAssetPath(sprite);
            return assetPath.Contains("/Examples/") || assetPath.Contains("/Demo/");
        }

        static bool IsInsideAnyAtlas(Sprite sprite, IEnumerable<SpriteAtlas> atlantes) =>
            atlantes.Any(atlas => atlas.CanBindTo(sprite));

        static bool IsPowerOfTwoSprite(Sprite sprite) =>
            IsPowerOfTwo((ulong)sprite.rect.width) && IsPowerOfTwo((ulong)sprite.rect.height);

        static bool IsPowerOfTwo(ulong x) => (x != 0) && ((x & (x - 1)) == 0);
    }
}