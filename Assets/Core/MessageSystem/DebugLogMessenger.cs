using System;
using System.Collections;
using UnityEngine;

namespace Core.MessageSystem {
    public class DebugLogMessenger : IMessenger {
        public int Mask { get; set; }
        
        public DebugLogMessenger(int mask = int.MaxValue) {
            Mask = mask;
        }
            
        public IEnumerator Receive(Message message) {
            if (Application.isEditor) {
                Debug.Log($"HH:mm:ss,fff[{message.Mask}]: Message {message.Tag}[{Convert.ToString(message.Mask, 2)}]");
            }
            yield break;
        }
    }
}