using System;
using System.Collections;
using System.Collections.Generic;
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

        private static readonly Dictionary<Func<Message, bool>, MessageAwaiter> WaitingTasks = new Dictionary<Func<Message, bool>, MessageAwaiter>();
        
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
        public static async Task WaitUntil(Func<Message, bool> prediction) {
            var awaiter = new MessageAwaiter();
            WaitingTasks.Add(prediction, awaiter);
            await awaiter.Wait();
        }

        /// <summary>
        /// 等待直到满足条件的消息出现
        /// </summary>
        /// <param name="mask">目标消息的掩码</param>
        /// <param name="tag">目标消息的标记（不提供或值为null则不作为判断依据）</param>
        /// <returns></returns>
        public static Task WaitUntil(int mask, string tag = null) {
            return WaitUntil(message => (message.Mask & mask) != 0 && (string.IsNullOrEmpty(tag) || message.Tag == tag));
        }

        private static void VerifyWaitingTasks(Message message) {
            var needRemove = new List<Func<Message, bool>>();
            foreach (var (prediction, awaiter) in WaitingTasks) {
                if (!prediction.Invoke(message)) continue;
                needRemove.Add(prediction);
                awaiter.Completed = true;
            }
            foreach (var item in needRemove) {
                WaitingTasks.Remove(item);
            }
        }

        private class MessageAwaiter {
            public bool Completed { get; set; }

            public IEnumerator Wait() {
                while (true) {
                    if (Completed) yield break;
                }
            }
        }
    }
}
