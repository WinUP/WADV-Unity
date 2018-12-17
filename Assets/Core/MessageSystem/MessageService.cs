using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.MessageSystem {
    /// <summary>
    /// 消息服务组件
    /// </summary>
    public static class MessageService {
        /// <summary>
        /// 消息接收器链接树的根节点
        /// </summary>
        public static readonly LinkedTreeNode<IMessenger> Receivers = new LinkedTreeNode<IMessenger>(Application.isEditor ? (IMessenger) new DebugLogMessenger() : new EmptyMessenger());

        public static MessageListener ActiveListener { get; private set; }

        public static void UseListener(MessageListener listener) {
            if (Listeners.Contains(listener)) {
                Listeners.Remove(listener);
            }
            Listeners.Add(listener);
            ActiveListener = listener;
        }

        public static void CancelListener(MessageListener listener) {
            if (Listeners.Contains(listener)) {
                Listeners.Remove(listener);
            }
            ActiveListener = Listeners.Count > 0 ? Listeners.Last() : null;
        }
        
        private static readonly List<MessageListener> Listeners = new List<MessageListener>();
        
        /// <summary>
        /// 在协程中处理消息
        /// </summary>
        /// <param name="message">目标消息</param>
        public static void Process(Message message) {
            if (ActiveListener == null) {
                throw new NotSupportedException("No MessageListener detected, must have one MessageListener component activate in current scene");
            }
            ActiveListener.Process(message, Receivers);
        }
    }
}
