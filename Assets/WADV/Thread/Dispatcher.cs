// All dispatcher related codes are based on http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;

namespace WADV.Thread {
    /// <summary>
    /// 线程任务分发器
    /// </summary>
    public static class Dispatcher {
        /// <summary>
        /// 跳转到主线程执行
        /// </summary>
        /// <returns></returns>
        public static WaitForUpdate UseMainThread() => WaitForUpdate.Instance;

        /// <summary>
        /// 等待下一个更新循环
        /// </summary>
        /// <returns></returns>
        public static WaitForUpdate NextUpdate() => WaitForUpdate.Instance;

        /// <summary>
        /// 跳转到后台线程执行
        /// <para>后台线程往往拥有更出色的性能，但其不能访问任何Unity界面和游戏场景元素</para>
        /// </summary>
        /// <returns></returns>
        public static WaitForBackgroundThread UseBackgroundThread() => WaitForBackgroundThread.Instance;

        /// <summary>
        /// 等待一定时间
        /// </summary>
        /// <param name="seconds">总共等待秒数</param>
        /// <returns></returns>
        public static WaitForSeconds WaitForSeconds(float seconds) => new WaitForSeconds(seconds);

        /// <summary>
        /// 等待一定时间
        /// </summary>
        /// <param name="timespan">总共等待时间</param>
        /// <returns></returns>
        public static WaitForSeconds WaitForSeconds(TimeSpan timespan) => new WaitForSeconds((float) timespan.TotalSeconds);

        /// <summary>
        /// 生成一个新的主线程占位符
        /// </summary>
        /// <returns></returns>
        public static MainThreadPlaceholder CreatePlaceholder() => new MainThreadPlaceholder();

        /// <summary>
        /// 等待所有任务完成
        /// </summary>
        /// <param name="tasks">目标任务组</param>
        /// <returns></returns>
        public static async Task WaitAll(params Task[] tasks) {
            var target = tasks.Select(e => e.GetAwaiter()).ToList();
            while (target.Any(e => !e.IsCompleted)) {
                await NextUpdate();
            }
        }
        
        /// <summary>
        /// 等待所有任务完成
        /// </summary>
        /// <param name="tasks">目标任务组</param>
        /// <returns></returns>
        public static async Task WaitAll(IEnumerable<Task> tasks) {
            var target = tasks.Select(e => e.GetAwaiter()).ToList();
            while (target.Any(e => !e.IsCompleted)) {
                await NextUpdate();
            }
        }

        /// <summary>
        /// 等待任意一个任务完成
        /// </summary>
        /// <param name="tasks">目标任务组</param>
        /// <returns></returns>
        public static async Task WaitAny(params Task[] tasks) {
            var target = tasks.Select(e => e.GetAwaiter()).ToList();
            while (!target.Any(e => e.IsCompleted)) {
                await NextUpdate();
            }
        }
        
        /// <summary>
        /// 等待任意一个任务完成
        /// </summary>
        /// <param name="tasks">目标任务组</param>
        /// <returns></returns>
        public static async Task WaitAny(IEnumerable<Task> tasks) {
            var target = tasks.Select(e => e.GetAwaiter()).ToList();
            while (!target.Any(e => e.IsCompleted)) {
                await NextUpdate();
            }
        }
    }
}