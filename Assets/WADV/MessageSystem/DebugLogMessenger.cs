using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WADV.MessageSystem {
    public class DebugLogMessenger : IMessenger {
        public int Mask { get; set; }
        
        public DebugLogMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
            
        public Task<Message> Receive(Message message) {
            if (Application.isEditor) {
                Debug.Log($"{DateTime.Now:HH:mm:ss,fff}: {message.Tag}[{Convert.ToString(message.Mask, 2)}]");
            }
            return Task.FromResult(message);
        }
    }
}