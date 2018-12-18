using System.Threading.Tasks;
using Core.Extensions;
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

        public static async Task<Message> ProcessAsync(Message message) {
            foreach (var receiver in Receivers) {
                if ((receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                message = await receiver.Receive(message);
            }
            return message;
        }

        public static Message Process(Message message) {
            foreach (var receiver in Receivers) {
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
    }
}
