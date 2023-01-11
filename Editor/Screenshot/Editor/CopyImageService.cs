using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace AAA.Editor.Editor.Screenshot
{
    public static class CopyImageService
    {
        public static void CopyToClipboard(Texture2D texture)
        {
#if UNITY_EDITOR_OSX
            var path = $"{Application.dataPath}/../Library/Clipboard.jpg";
            var encodedResult = texture.EncodeToJPG();
            File.WriteAllBytes(path, encodedResult);

            var startInfo = new ProcessStartInfo
            {
                FileName = "osascript",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = $" -e 'set the clipboard to (read (POSIX file \"{path}\") as JPEG picture)'"
            };

            var myProcess = new Process { StartInfo = startInfo };

            myProcess.Start();
            myProcess.WaitForExit();

            File.Delete(path);
#else
            Debug.LogError("Copying Images to Clipboard is not implemented for this Operating system");
#endif
        }
    }
}