using System;
using System.Collections.Generic;

namespace Core {
    /// <summary>
    /// Message service
    /// </summary>
    public static class MessageService {
        /// <summary>
        /// Message receivers
        /// </summary>
        public static readonly List<IMessenger> Receivers = new List<IMessenger>();
        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="message"></param>
        public static Message Process(Message message) {
            UnityEngine.Debug.Log(DateTime.Now.ToString($"HH:mm:ss,fff[{message.Mask}]: {message.Tag}"));
            foreach (var receiver in Receivers) {
                if (!receiver.Awaking || (receiver.Mask & message.Mask) == 0) {
                    continue;
                }
                message = receiver.Receive(message);
            }
            return message;
        }
    }
}
