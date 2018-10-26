using System.Collections.Generic;
using Assets.Core.System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Core.VisualNovel.Sound {
    public class SoundManager: MonoBehaviour, IMessenger {
        public Dictionary<string, SoundChannel> Channels { get; } = new Dictionary<string, SoundChannel>();

        public int Mask { get; } = MessageMask.Sound;
        public bool Awaking { get; set; }

        public Message Receive(Message message) {
            switch (message.Tag) {
                case MessageTag.AddChannel:
                    var newChannelName = ((Message<string>) message).Content;
                    if (!Channels.ContainsKey(newChannelName)) {
                        Channels.Add(newChannelName, new SoundChannel());
                    }
                    break;
                case MessageTag.RemoveChannel:
                    var removedChannelName = ((Message<string>)message).Content;
                    if (Channels.ContainsKey(removedChannelName)) {
                        StartCoroutine(Channels[removedChannelName].Stop());
                        Channels.Remove(removedChannelName);
                    }
                    break;
            }
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
