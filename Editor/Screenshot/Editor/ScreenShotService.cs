using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ArtKit.Resolutions;
using Plugins.AAA.Editor.Editor.Resolutions;
using UnityEditor;
using UnityEngine;
using static System.Environment;

namespace Plugins.AAA.Editor.Editor.Screenshot.Editor
{
    public static class ScreenShotService
    {
        public static string ScreenShotFolderPath => $"{GetFolderPath(SpecialFolder.MyPictures)}/Screenshots/{PlayerSettings.productName}/";
        static string GetFileName(GameResolutionInfo resolutionInfo, string suffix = "")
            => string.Format("{0}{1}/{2}{8} {3:yyyy-MM-dd} at {4:HH.mm.ss} {5}({6}x{7}).png",
                ScreenShotFolderPath,
                resolutionInfo.Platform,
                PlayerSettings.productName,
                DateTime.Now, DateTime.Now,
                resolutionInfo.name,
                resolutionInfo.Resolution.x,
                resolutionInfo.Resolution.y,
                suffix);

        static ScreenShotService()
        {
            foreach (var platform in Enum.GetNames(typeof(Platform)))
            {
                var path = ScreenShotFolderPath + platform;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
        public static async Task TakeScreenShots(IEnumerable<GameResolutionInfo> requiredResolutions, bool copyResultToClipBoard = false)
        {
            var selectedIndex = GameViewUtils.GetSelectedIndex();
            Time.timeScale = 0f;

            foreach (var resolutionInfo in requiredResolutions)
                await TakeScreenShot(resolutionInfo, false);
            if(copyResultToClipBoard)
                CopyImageService.CopyToClipboard(ScreenCapture.CaptureScreenshotAsTexture());

            Time.timeScale = 1f;
            GameViewUtils.SetSize(selectedIndex);
        }

        static async Task TakeScreenShot(GameResolutionInfo resolutionInfo, bool copyResultToClipBoard = false)
        {
            GameViewUtils.SetOrAddSize(resolutionInfo);

            await WaitForFrame(2);

            ScreenCapture.CaptureScreenshot(GetFileName(resolutionInfo));
            if(copyResultToClipBoard)
                CopyImageService.CopyToClipboard(ScreenCapture.CaptureScreenshotAsTexture());
        }
        
        //TODO: Move this into it's own helper class
        public static async Task WaitForFrame(int frameCount = 1)
        {
            //Yield awaited during playerloop returns at the end of frame, skip first one for the next frame.
            await Task.Yield();
            for (var i = 0; i < frameCount; i++)
            {
                await Task.Yield();
            }
        }
    }
}
