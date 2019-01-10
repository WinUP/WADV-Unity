using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovelPlugins.Dialogue;

namespace Game.UI {
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class ImageShowHideListener : MonoBehaviour, IMessenger {
        public int Mask { get; } = DialoguePlugin.MessageMask;

        private float _initialAlpha;
        private Image _image;
        
        public static LinkedTreeNode<IMessenger> RootMessenger { get; }

        static ImageShowHideListener() {
            RootMessenger = MessageService.Receivers.CreateChild(new EmptyMessenger());
        }
        
        private void OnEnable() {
            RootMessenger.CreateChild(this);
        }

        private void OnDisable() {
            RootMessenger.RemoveChild(this);
        }

        private void Start() {
            _image = GetComponent<Image>();
            if (_image == null) throw new NotSupportedException($"Unable to create {nameof(ImageShowHideListener)}: no Image component found in current object");
        }

        public async Task<Message> Receive(Message message) {
            if (message.Tag != DialoguePlugin.HideDialogueBoxMessageTag && message.Tag != DialoguePlugin.ShowDialogueBoxMessageTag) return message;
            var fadeTime = message is Message<float> floatMessage ? floatMessage.Content : 0.0F;
            var color = _image.color;
            if (fadeTime.Equals(0.0F)) {
                _image.color = new Color(color.r, color.g, color.b, message.Tag == DialoguePlugin.HideDialogueBoxMessageTag ? 0.0F : 1.0F);
                return message;
            }
            var start = 0.0F;
            var end = 0.0F;
            switch (message.Tag) {
                case DialoguePlugin.HideDialogueBoxMessageTag:
                    start = _initialAlpha = _image.color.a;
                    end = 0.0F;
                    break;
                case DialoguePlugin.ShowDialogueBoxMessageTag:
                    start = 0.0F;
                    end = _initialAlpha;
                    break;
            }
            var time = 0.0F;
            while (time < fadeTime) {
                time += Time.deltaTime;
                _image.color = new Color(color.r, color.g, color.b, MathfExtended.Sinerp(start, end, time / fadeTime));
                await Dispatcher.NextUpdate();
            }
            return message;
        }
    }
}