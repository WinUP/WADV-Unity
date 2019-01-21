using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.Thread;

namespace WADV.MessageSystem {
    /// <summary>
    /// 消息服务组件
    /// </summary>
    public static class MessageService {
        /// <summary>
        /// 消息接收器链接树的根节点
        /// </summary>
        public static readonly LinkedTreeNode<IMessenger> Receivers = new LinkedTreeNode<IMessenger>(Application.isEditor ? (IMessenger) new DebugLogMessenger() : new EmptyMessenger());

        private static readonly Dictionary<Func<Message, bool>, MainThreadPlaceholder<Message>> WaitingTasks = new Dictionary<Func<Message, bool>, MainThreadPlaceholder<Message>>();
        
        /// <summary>
        /// 异步处理消息
        /// </summary>
        /// <param name="message">要处理的消息</param>
        /// <returns></returns>
        public static async Task<Message> ProcessAsync(Message message) {
            VerifyWaitingTasks(message);
            foreach (var (_, receiver) in Receivers) {
                if ((receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                if (receiver.IsStandaloneMessage) {
                    TaskDelegator.Instance.StartCoroutine(receiver.Receive(message).AsIEnumerator());
                } else {
                    message = await receiver.Receive(message);
                }
            }
            while (message.Placeholders.Any(e => e.keepWaiting)) {
                await Dispatcher.NextUpdate();
            }
            return message;
        }

        /// <summary>
        /// 同步处理消息
        /// </summary>
        /// <param name="message">要处理的消息</param>
        /// <returns></returns>
        public static Message Process(Message message) {
            var task = ProcessAsync(message).WrapErrors();
            task.Wait();
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 等待直到满足条件的消息出现
        /// </summary>
        /// <param name="prediction">判断消息是否满足条件的函数</param>
        /// <returns></returns>
        public static async Task<Message> WaitUntil(Func<Message, bool> prediction) {
            var awaiter = new MainThreadPlaceholder<Message>();
            WaitingTasks.Add(prediction, awaiter);
            await awaiter;
            return awaiter.Value;
        }

        /// <summary>
        /// 等待直到满足条件的消息出现
        /// </summary>
        /// <param name="mask">目标消息的掩码</param>
        /// <param name="tag">目标消息的标记（不提供或值为null则不作为判断依据）</param>
        /// <returns></returns>
        public static Task<Message> WaitUntil(int mask, string tag = null) {
            return WaitUntil(message => (message.Mask & mask) != 0 && (string.IsNullOrEmpty(tag) || message.Tag == tag));
        }

        private static void VerifyWaitingTasks(Message message) {
            var needRemove = new List<Func<Message, bool>>();
            foreach (var (prediction, awaiter) in WaitingTasks) {
                if (!prediction.Invoke(message)) continue;
                needRemove.Add(prediction);
                awaiter.Complete(message);
            }
            foreach (var item in needRemove) {
                WaitingTasks.Remove(item);
            }
        }
    }
}
