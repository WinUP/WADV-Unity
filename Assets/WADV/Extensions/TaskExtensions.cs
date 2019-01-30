using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace WADV.Extensions {
    public static class TaskExtensions {
        public static IEnumerator AsIEnumerator(this Task task) {
            while (!task.IsCompleted) {
                yield return null;
            }
            if (task.IsFaulted) {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
        }

        public static IEnumerator<T> AsIEnumerator<T>(this Task<T> task) {
            while (!task.IsCompleted) {
                yield return default(T);
            }
            if (task.IsFaulted) {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            yield return task.Result;
        }

        /// <summary>
        /// 等待任务执行结束并返回执行结果
        /// </summary>
        /// <param name="task">目标任务</param>
        /// <returns></returns>
        public static T GetResultAfterFinished<T>(this Task<T> task) {
            task.Wait();
            return task.Result;
        }
    }
}