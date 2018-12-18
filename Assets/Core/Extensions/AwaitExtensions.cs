using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Core.Extensions {
    public static class AwaitExtensions {
        public static TaskAwaiter<int> GetAwaiter(this Process process) {
            var source = new TaskCompletionSource<int>();
            if (process.HasExited) {
                source.TrySetResult(process.ExitCode);
            } else {
                process.EnableRaisingEvents = true;
                process.Exited += (s, e) => source.TrySetResult(process.ExitCode);
            }
            return source.Task.GetAwaiter();
        }

        public static async void WrapErrors(this Task task) {
            await task;
        }
        
        public static async Task<T> WrapErrors<T>(this Task<T> task) {
            return await task;
        }
    }
}