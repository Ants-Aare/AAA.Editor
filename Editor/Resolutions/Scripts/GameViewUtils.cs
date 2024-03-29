using System;
using System.Reflection;
using UnityEditor;

namespace AAA.Editor.Editor.Resolutions
{
    public static class GameViewUtils
    {
        static object gameViewSizesInstance;
        static MethodInfo getGroup;
        static readonly Type GameViewWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");

        static readonly PropertyInfo SelectedSizeIndexProperty = GameViewWindowType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic);

        static GameViewUtils()
        {
            // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
            var sizesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }

        public enum GameViewSizeType
        {
            AspectRatio,
            FixedResolution
        }

        public static int GetSelectedIndex()
        {
            var gameViewWindow = EditorWindow.GetWindow(GameViewWindowType);
            return (int)SelectedSizeIndexProperty.GetValue(gameViewWindow);
        }

        public static void SetSize(int index)
        {
            var gvWnd = EditorWindow.GetWindow(GameViewWindowType);
            SelectedSizeIndexProperty.SetValue(gvWnd, index, null);
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
            // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

            var group = GetGroup(sizeGroupType);
            var addCustomSizeMethod = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var gameViewSize = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var gameViewSizeType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var ctor = gameViewSize.GetConstructor(new Type[] { gameViewSizeType, typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSizeMethod.Invoke(group, new object[] { newSize });
        }

        public static void SetOrAddSize(string text, int width, int height)
        {
            var groupType = GetCurrentGroupType();

            var idx = FindSize(groupType, text);
            if (idx == -1)
            {
                AddCustomSize(GameViewSizeType.FixedResolution, groupType, width, height, text);
                idx = FindSize(groupType, text);
            }

            SetSize(idx);
        }

        public static void SetOrAddSize(GameResolutionInfo gameResolutionInfo)
        {
            var sizeName = $"{gameResolutionInfo.Resolution.x}x{gameResolutionInfo.Resolution.y} {gameResolutionInfo.name}";
            SetOrAddSize(sizeName, gameResolutionInfo.Resolution.x, gameResolutionInfo.Resolution.y);
        }

        public static void RemoveSize(int index)
        {
            var group = GetGroup(GetCurrentGroupType());
            var removeCustomSizeMethod = getGroup.ReturnType.GetMethod("RemoveCustomSize");
            removeCustomSizeMethod.Invoke(group, new object[] { index });
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
        {
            return FindSize(sizeGroupType, text) != -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // string[] texts = group.GetDisplayTexts();
            // for loop...

            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];
                // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
                // so if we're querying a custom size text we substring to only get the name
                // You could see the outputs by just logging
                // Debug.Log(display);
                int pren = display.IndexOf('(');
                if (pren != -1)
                    display = display.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
                if (display == text)
                    return i;
            }

            return -1;
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            // goal:
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }

            return -1;
        }

        static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }

        public static GameViewSizeGroupType GetCurrentGroupType()
        {
            var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
            return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
        }
    }
}
