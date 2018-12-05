using Assets.Core.VisualNovel;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.VisualNovel {
    public class VisualNetwork : MonoBehaviour, IMessenger {
        
        public int Mask { get; } = MessageMask.VisualNetwork;
        public bool Awaking { get; set; }

        public Message Receive(Message message) {
            return message;
        }

        [UsedImplicitly]
        public void Start() {
            MessageService.Receivers.Add(this);
        }

        [UsedImplicitly]
        public void OnEnable() {
            Awaking = true;
        }

        [UsedImplicitly]
        public void OnDisable() {
            Awaking = false;
        }

        [UsedImplicitly]
        public void OnDestroy() {
            MessageService.Receivers.Remove(this);
        }
    }
}
