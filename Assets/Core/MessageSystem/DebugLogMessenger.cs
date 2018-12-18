#pragma warning disable 1998

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.MessageSystem {
    public class DebugLogMessenger : IMessenger {
        public int Mask { get; set; }
        
        public DebugLogMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
            
        public async Task<Message> Receive(Message message) {
            if (Application.isEditor) {
                Debug.Log($"{DateTime.Now:HH:mm:ss,fff}: {message.Tag}[{Convert.ToString(message.Mask, 2)}]");
            }
            return message;
        }
    }
}