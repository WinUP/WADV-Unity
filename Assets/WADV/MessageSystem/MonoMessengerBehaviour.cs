using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Thread;

namespace WADV.MessageSystem {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 带有自动注册消息循环监听器的Unity组件
    /// </summary>
    public abstract class MonoMessengerBehaviour : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public abstract int Mask { get; }
        
        /// <inheritdoc />
        public abstract bool IsStandaloneMessage { get; }

        private int _messageQuickCacheIndex = -1;
        private int _placeholderCacheIndex = -1;
        
        private static readonly Dictionary<int, Message> MessageCache = new Dictionary<int, Message>();
        private static readonly Dictionary<int, MainThreadPlaceholder> PlaceholderCache = new Dictionary<int, MainThreadPlaceholder>();
        private static int _cacheIndex = -1;

        /// <summary>
        /// 缓存消息
        /// </summary>
        /// <param name="target">目标消息</param>
        /// <returns></returns>
        public static int CacheMessage(Message target) {
            ++_cacheIndex;
            MessageCache.Add(_cacheIndex, target);
            return _cacheIndex;
        }

        /// <summary>
        /// 取出缓存的消息
        /// </summary>
        /// <param name="id">缓存ID</param>
        /// <returns></returns>
        [CanBeNull]
        public static Message PopMessage(int id) {
            if (!MessageCache.ContainsKey(id)) return null;
            var result = MessageCache[id];
            MessageCache.Remove(id);
            return result;
        }

        /// <summary>
        /// 缓存消息到快速缓存区（同一个组件一次只能缓存一个，后缓存的会替换先缓存的）
        /// </summary>
        /// <param name="target">目标消息</param>
        protected void QuickCacheMessage(Message target) {
            PopQuickCacheMessage();
            _messageQuickCacheIndex = CacheMessage(target);
        }

        /// <summary>
        /// 确定快速缓存区是否有消息数据
        /// </summary>
        /// <returns></returns>
        protected bool HasQuickCacheMessage() {
            return _messageQuickCacheIndex > -1;
        }

        /// <summary>
        /// 取出快速缓存区的消息并清空缓存
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        protected Message PopQuickCacheMessage() {
            if (_messageQuickCacheIndex < 0) return null;
            var result = PopMessage(_messageQuickCacheIndex);
            _messageQuickCacheIndex = -1;
            return result;
        }

        /// <summary>
        /// 取出快速缓存区的消息并尝试转换为指定格式，之后清空缓存
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        protected T PopQuickCacheMessage<T>() where T : Message {
            var result = PopQuickCacheMessage();
            if (result == null) return null;
            return typeof(T) == result.GetType() ? (T) result : null;
        }

        /// <summary>
        /// 读取快速缓存区的消息并尝试转换为指定格式，之后清空缓存
        /// </summary>
        /// <returns></returns>
        protected T PeekQuickCacheMessage<T>() where T : Message {
            var result = PopQuickCacheMessage();
            if (result == null) return null;
            QuickCacheMessage(result);
            return typeof(T) == result.GetType() ? (T) result : null;
        }

        /// <summary>
        /// 缓存一个主线程占位符
        /// </summary>
        /// <param name="placeholder">目标占位符</param>
        /// <returns></returns>
        public static int CachePlaceholder(MainThreadPlaceholder placeholder) {
            ++_cacheIndex;
            PlaceholderCache.Add(_cacheIndex, placeholder);
            return _cacheIndex;
        }

        /// <summary>
        /// 标记缓存的主线程占位符为完成并从缓存区移除
        /// </summary>
        /// <param name="id">缓存ID</param>
        public static void CompletePlaceholder(int id) {
            if (!PlaceholderCache.ContainsKey(id)) return;
            PlaceholderCache[id].Complete();
            PlaceholderCache.Remove(id);
        }

        /// <summary>
        /// 等待缓存的主线程占位符完成
        /// </summary>
        /// <param name="id">缓存ID</param>
        /// <returns></returns>
        public static async Task WaitPlaceholder(int id) {
            if (!PlaceholderCache.ContainsKey(id)) return;
            await PlaceholderCache[id];
        }

        /// <summary>
        /// 缓存一个主线程占位符到快速缓存区（同一个组件一次只能缓存一个，后缓存的会替换先缓存的）
        /// </summary>
        /// <param name="placeholder">目标占位符</param>
        protected void QuickCachePlaceholder(MainThreadPlaceholder placeholder) {
            _placeholderCacheIndex = CachePlaceholder(placeholder);
        }

        /// <summary>
        /// 确定快速缓存区是否有主线程占位符数据
        /// </summary>
        /// <returns></returns>
        protected bool HasQuickCachePlaceholder() {
            return _placeholderCacheIndex > -1;
        }

        /// <summary>
        /// 标记缓存的主线程占位符为完成（如果有）
        /// </summary>
        protected void CompleteCachedPlaceholder() {
            if (_placeholderCacheIndex < 0) return;
            CompletePlaceholder(_placeholderCacheIndex);
            _placeholderCacheIndex = -1;
        }

        /// <summary>
        /// 等待缓存的主线程占位符完成（如果有）
        /// </summary>
        /// <returns></returns>
        protected Task WaitCachedPlaceholder() {
            return _placeholderCacheIndex < 0 ? Task.CompletedTask : WaitPlaceholder(_placeholderCacheIndex);
        }
        
        /// <inheritdoc />
        public abstract Task<Message> Receive(Message message);

        protected virtual void OnEnable() {
            MessageService.Receivers.CreateChild(this);
        }

        protected virtual void OnDisable() {
            MessageService.Receivers.RemoveChild(this);
        }
    }
}