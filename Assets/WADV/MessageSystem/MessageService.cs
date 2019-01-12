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
        
        public static async Task<Message> ProcessAsync(Message message) {
            VerifyWaitingTasks(message);
            foreach (var (_, receiver) in Receivers) {
                if ((receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                if (receiver.NoWaiting) {
                    TaskDelegator.Instance.StartCoroutine(receiver.Receive(message).AsIEnumerator());
                } else {
                    message = await receiver.Receive(message);
                }
            }
            return message;
        }

        public static Message Process(Message message) {
            VerifyWaitingTasks(message);
            foreach (var (_, receiver) in Receivers) {
                if ((receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                // ReSharper disable once AccessToModifiedClosure
                var task = receiver.Receive(message).WrapErrors();
                task.Wait();
                message = task.GetAwaiter().GetResult();
            }
            return message;
        }

        public static async Task WaitUntil(Func<Message, bool> prediction) {
            var awaiter = new MessageAwaiter();
            WaitingTasks.Add(prediction, awaiter);
            await awaiter.Wait();
        }

        public static async Task WaitUntil(int mask, string tag = null) {
            await WaitUntil(message => (message.Mask & mask) != 0 && (string.IsNullOrEmpty(tag) || message.Tag == tag));
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
