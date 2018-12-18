// All dispatcher related codes are based on http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/

using System;
using UnityEngine;

namespace Core.Thread {
    /// <summary>
    /// 线程任务分发器
    /// </summary>
    public static class Dispatcher {
        /// <summary>
        /// 跳转到主线程执行
        /// </summary>
        /// <returns></returns>
        public static WaitForUpdate UseMainThread() {
            return WaitForUpdate();
        }

        /// <summary>
        /// 等待下一个更新循环
        /// </summary>
        /// <returns></returns>
        public static WaitForUpdate WaitForUpdate() {
            return new WaitForUpdate();
        }

        /// <summary>
        /// 跳转到后台线程执行
        /// <para>后台线程往往拥有更出色的性能，但其不能访问任何Unity界面和游戏场景元素</para>
        /// </summary>
        /// <returns></returns>
        public static WaitForBackgroundThread UseBackgroundThread() {
            return new WaitForBackgroundThread();
        }

        /// <summary>
        /// 等待一定时间
        /// </summary>
        /// <param name="seconds">总共等待秒数</param>
        /// <returns></returns>
        public static WaitForSeconds WaitForSeconds(float seconds) {
            return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// 等待一定时间
        /// </summary>
        /// <param name="timespan">总共等待时间</param>
        /// <returns></returns>
        public static WaitForSeconds WaitForSeconds(TimeSpan timespan) {
            return new WaitForSeconds((float) timespan.TotalSeconds);
        }
    }
}