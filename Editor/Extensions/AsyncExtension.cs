using System;
using System.Threading.Tasks;

namespace Plugins.AAA.Editor.Editor.Extensions
{
    public static class AsyncExtension
    {
        public static async Task WaitForFrame(int frameCount = 1)
        {
            //Yield awaited during player loop returns at the end of frame, skip first one for the next frame.
            await Task.Yield();
            for (var i = 0; i < frameCount; i++)
            {
                await Task.Yield();
            }
        }

        public static async void FireAndForget(this Task task, Action<Exception> onException = null,
            bool rethrowException = true)
        {
            bool OnError(Exception ex)
            {
                onException?.Invoke(ex);
                return !rethrowException;
            }

            try
            {
                await task;
            }
            catch (Exception ex) when (OnError(ex))
            {
            }
        }
    }
}