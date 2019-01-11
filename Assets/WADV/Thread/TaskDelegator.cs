using System;
using System.Threading;
using UnityEngine;

namespace WADV.Thread {
    /// <inheritdoc />
    /// <summary>
    /// Unity协程执行器
    /// </summary>
    public class TaskDelegator : MonoBehaviour {
        /// <summary>
        /// 获取Unity主线程ID
        /// </summary>
        public static int MainThreadId { get; private set; }
        /// <summary>
        /// 获取Unity主线程执行上下文
        /// </summary>
        public static SynchronizationContext MainThreadContext { get; private set; }
        
        private static TaskDelegator _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DetectUnityThreadContext() {
            MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            MainThreadContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// 获取Unity协程执行器实例
        /// </summary>
        public static TaskDelegator Instance {
            get {
                if (_instance == null) {
                    _instance = new GameObject($"CoroutineRunner [{Guid.NewGuid().ToString()}]").AddComponent<TaskDelegator>();
                }
                return _instance;
            }
        }

        private void Awake() {
            var target = gameObject;
            target.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(target);
        }
    }
}