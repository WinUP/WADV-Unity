using System;
using System.Threading;
using UnityEngine;

namespace Core.Dispatcher {
    /// <inheritdoc />
    /// <summary>
    /// Unity协程执行器
    /// </summary>
    public class CoroutineRunner : MonoBehaviour {
        /// <summary>
        /// 获取Unity主线程ID
        /// </summary>
        public static int MainThreadId { get; private set; }
        /// <summary>
        /// 获取Unity主线程执行上下文
        /// </summary>
        public static SynchronizationContext MainThreadContext { get; private set; }
        
        private static CoroutineRunner _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DetectUnityThreadContext() {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
            MainThreadContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// 获取Unity协程执行器实例
        /// </summary>
        public static CoroutineRunner Instance {
            get {
                if (_instance == null) {
                    _instance = new GameObject($"CoroutineRunner [{Guid.NewGuid().ToString()}]").AddComponent<CoroutineRunner>();
                }
                return _instance;
            }
        }

        private void Awake() {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }
    }
}