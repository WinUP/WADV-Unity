using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WADV.MessageSystem {
    /// <inheritdoc />
    /// <summary>
    /// 控制台日志消息接收器
    /// </summary>
    public class DebugLogMessenger : IMessenger {
        /// <inheritdoc />
        public int Mask { get; }
        
        /// <inheritdoc />
        public bool IsStandaloneMessage { get; } = false;

        /// <summary>
        /// 创建一个控制台日志消息接收器
        /// </summary>
        /// <param name="mask">可处理的消息的掩码（默认为2^31 - 1）</param>
        public DebugLogMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
        
        /// <inheritdoc />
        public Task<Message> Receive(Message message) {
            if (Application.isEditor) {
                Debug.Log($"{DateTime.Now:HH:mm:ss,fff}: {message.Tag}[{Convert.ToString(message.Mask, 2)}]");
            }
            return Task.FromResult(message);
        }
    }
}