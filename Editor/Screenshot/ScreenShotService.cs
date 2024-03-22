using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AAA.Editor.Editor.Resolutions;
using AAA.Editor.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using static System.Environment;
using static System.String;

namespace AAA.Editor.Editor.Screenshot
{
    public static class ScreenShotService
    {
        public const string CopyScreenShotToClipboardKey = "CopyScreenShotToClipboard";
        public const string TakeScreenshotsOnGameStartKey = "TakeScreenshotsOnGameStart";

        public static string ScreenShotFolderPath =>
            $"{GetFolderPath(SpecialFolder.MyPictures)}/Screenshots/{PlayerSettings.productName}/";

        static RenderTexture _renderTexture;

        static string GetFileName(GameResolutionInfo resolutionInfo, string suffix = "")
            => Format("{0}{1}/{2}{3} {4:yyyy-MM-dd} at {5:HH.mm.ss} {6}({7}x{8}).png"
                , ScreenShotFolderPath
                , resolutionInfo.Platform
                , PlayerSettings.productName
                , suffix
                , DateTime.Now
                , DateTime.Now
                , resolutionInfo.name
                , resolutionInfo.Resolution.x
                , resolutionInfo.Resolution.y);

        static string GetFileName(Vector2Int size, string suffix = "")
            =>
                $"{ScreenShotFolderPath}{PlayerSettings.productName}{suffix} {DateTime.Now:yyyy-MM-dd} at {DateTime.Now:HH.mm.ss} ({size.x}x{size.y}).png";

        static ScreenShotService()
        {
            foreach (var platform in Enum.GetNames(typeof(Platform)))
            {
                var path = ScreenShotFolderPath + platform;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static async Task TakeScreenShots(bool copyResultToClipBoard = false, string suffix = "",
            IEnumerable<GameResolutionInfo> requiredResolutions = default)
        {
            requiredResolutions ??= GetRequiredResolutions();

            var selectedIndex = GameViewUtils.GetSelectedIndex();
            Time.timeScale = 0f;

            foreach (var resolutionInfo in requiredResolutions)
                await TakeScreenShot(resolutionInfo, false, false, suffix);

            if (copyResultToClipBoard)
                CopyImageService.CopyToClipboard(ScreenCapture.CaptureScreenshotAsTexture());

            Time.timeScale = 1f;
            GameViewUtils.SetSize(selectedIndex);
        }

        static GameResolutionInfo[] GetRequiredResolutions()
        {
            return GameResolutionUtility.GetGameResolutionInfos()
                .Where(x => EditorPrefs.GetBool($"EnableScreenshot{x.name}")).ToArray();
        }

        public static void TakeGameplayScreenShots()
        {
            EditorPrefs.SetBool("TakeScreenshotsOnGameStart", true);
            if (!EditorApplication.isPlaying)
                EditorApplication.isPlaying = true;
        }

        [InitializeOnLoadMethod]
        static void OnGameLoaded()
        {
            if (EditorPrefs.GetBool(TakeScreenshotsOnGameStartKey, false))
            {
                EditorPrefs.SetBool(TakeScreenshotsOnGameStartKey, false);
                Debug.Log("Taking Gameplay Screenshots");
                TakeGameplayScreenShotsAsync().FireAndForget();
            }
        }

        static async Task TakeGameplayScreenShotsAsync()
        {
            await Task.Delay(1000);
            if (!EditorApplication.isPlaying)
            {
                Debug.Log("Can't Take Test Screenshots if UnityEditor is not in Playmode");
                return;
            }

            var editorWindow = EditorWindow.GetWindow(typeof(ScreenShotRunningWindow), true,
                "Taking ScreenShots. Please Wait.", true);
            editorWindow.minSize = new Vector2(600, 400);
            editorWindow.maxSize = new Vector2(600, 400);
            editorWindow.CenterOnMainWindow();
            editorWindow.ShowUtility();

            //Wait for loading screen to finish
            // while (!Contexts.Instance.isGameReady)
            // {
            //     if (!EditorApplication.isPlaying)
            //         return;
            //
            //     Debug.Log("Waiting until Game is initialized.");
            //     await Task.Delay(1000);
            // }

            await Task.Delay(1000);
            var resolutions = GetRequiredResolutions();
            await TakeScreenShots(false, " Main Menu", resolutions);
            Debug.Log("Successfully Took Main Menu Screenshots");

            EditorApplication.isPlaying = false;
            editorWindow.Close();
        }

        public static Task TakeScreenShot(GameResolutionInfo resolutionInfo, bool copyResultToClipBoard, bool isTransparent = false, string suffix = "")
        {
            GameViewUtils.SetOrAddSize(resolutionInfo);
            return CreateScreenshotAndSaveToFile(GetFileName(resolutionInfo, suffix), copyResultToClipBoard, isTransparent);
        }

        public static Task TakeScreenShot(string file, bool copyResultToClipBoard, bool isTransparent = false)
        {
            return CreateScreenshotAndSaveToFile(GetFileName(new Vector2Int(Screen.width, Screen.height), file), copyResultToClipBoard, isTransparent);
        }

        public static async Task TakeScreenShotWithResolution(string file, Vector2Int resolution, bool copyResultToClipBoard, bool isTransparent = false)
        {
            var index = GameViewUtils.GetSelectedIndex();
            GameViewUtils.SetOrAddSize("TemporaryWindowSize", resolution.x, resolution.y);

            await CreateScreenshotAndSaveToFile(GetFileName(new Vector2Int(Screen.width, Screen.height), file),
                copyResultToClipBoard, isTransparent);

            await AsyncExtension.WaitForFrame();
            GameViewUtils.RemoveSize(GameViewUtils.GetSelectedIndex());
            GameViewUtils.SetSize(index);
        }

        static async Task CreateScreenshotAndSaveToFile(string fileName, bool copyResultToClipBoard,
            bool isTransparent = false)
        {
            await AsyncExtension.WaitForFrame(2);

            Texture2D screenShotTexture = null;
            if (isTransparent)
            {
                screenShotTexture = GetTransparentScreenshotTexture();
                if (screenShotTexture == null)
                {
                    Debug.LogWarning("Could not take Transparent Screenshot");
                    return;
                }

                var bytes = screenShotTexture.EncodeToPNG();
                await File.WriteAllBytesAsync(fileName, bytes);
            }
            else
            {
                await AsyncExtension.WaitForFrame();
                ScreenCapture.CaptureScreenshot(fileName);
            }

            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("Can't copy to Clipboard when not in Playmode");
                return;
            }

            if (copyResultToClipBoard)
            {
                screenShotTexture ??= ScreenCapture.CaptureScreenshotAsTexture();
                CopyImageService.CopyToClipboard(screenShotTexture);
            }
        }

        static Texture2D GetTransparentScreenshotTexture()
        {
            if (!EditorApplication.isPlaying)
                return null;

            var camera = Camera.main;
            var resWidth = camera.pixelWidth;
            var resHeight = camera.pixelHeight;

            var oldProperties = camera.GetProperties();

            var renderTexture = GetRenderTexture(resWidth, resHeight);
            camera.SetProperties(new CameraProperties()
            {
                ClearFlags = CameraClearFlags.SolidColor,
                BackgroundColor = Color.clear,
                TargetTexture = renderTexture,
            });

            camera.Render();
            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0, false);
            screenShot.Apply();

            camera.SetProperties(oldProperties);
            RenderTexture.active = null;

            renderTexture.Release();
            return screenShot;
        }

        static RenderTexture GetRenderTexture(int width, int height)
        {
            if (_renderTexture is null)
            {
                return new RenderTexture(width, height, 32);
            }

            _renderTexture.Release();
            _renderTexture.width = width;
            _renderTexture.height = height;
            return _renderTexture;
        }

        struct CameraProperties
        {
            public CameraClearFlags ClearFlags;
            public Color BackgroundColor;
            public RenderTexture TargetTexture;
        }

        static CameraProperties GetProperties(this Camera camera)
        {
            return new CameraProperties()
            {
                ClearFlags = camera.clearFlags,
                BackgroundColor = camera.backgroundColor,
                TargetTexture = camera.targetTexture
            };
        }

        static void SetProperties(this Camera camera, CameraProperties newProperties)
        {
            camera.clearFlags = newProperties.ClearFlags;
            camera.backgroundColor = newProperties.BackgroundColor;
            camera.targetTexture = newProperties.TargetTexture;
        }
    }
}