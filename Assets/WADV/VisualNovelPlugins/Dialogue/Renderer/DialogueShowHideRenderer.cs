using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    public abstract class DialogueShowHideRenderer : MonoBehaviour, IMessenger {
        public int Mask { get; } = DialoguePlugin.MessageMask;
        public bool IsStandaloneMessage { get; } = true;

        private float _initialAlpha;
        private bool _hidden;
        
        public static LinkedTreeNode<IMessenger> RootMessenger { get; }

        static DialogueShowHideRenderer() {
            RootMessenger = MessageService.Receivers.CreateChild(new EmptyMessenger());
        }
        
        private void OnEnable() {
            RootMessenger.CreateChild(this);
        }

        private void OnDisable() {
            RootMessenger.RemoveChild(this);
        }
        
        protected virtual void PrepareStartShow(float totalTime) { }
        
        protected virtual void PrepareStartHide(float totalTime) { }

        protected abstract void OnShowFrame(float progress);

        protected abstract void OnHideFrame(float progress);

        public async Task<Message> Receive(Message message) {
            if (message.Tag != DialoguePlugin.HideDialogueBoxMessageTag && message.Tag != DialoguePlugin.ShowDialogueBoxMessageTag) return message;
            if (message.Tag == DialoguePlugin.ShowDialogueBoxMessageTag && !_hidden || message.Tag == DialoguePlugin.HideDialogueBoxMessageTag && _hidden) return message;
            _hidden = message.Tag == DialoguePlugin.HideDialogueBoxMessageTag;
            var fadeTime = message is Message<float> floatMessage ? floatMessage.Content : 0.0F;
            if (fadeTime.Equals(0.0F)) {
                if (message.Tag == DialoguePlugin.ShowDialogueBoxMessageTag) {
                    PrepareStartShow(fadeTime);
                    OnShowFrame(1.0F);
                } else {
                    PrepareStartHide(fadeTime);
                    OnHideFrame(1.0F);
                }
                return message;
            }
            if (message.Tag == DialoguePlugin.ShowDialogueBoxMessageTag) {
                PrepareStartShow(fadeTime);
            } else {
                PrepareStartHide(fadeTime);
            }
            var time = 0.0F;
            while (time <= fadeTime) {
                time += Time.deltaTime;
                if (message.Tag == DialoguePlugin.ShowDialogueBoxMessageTag) {
                    OnShowFrame(Mathf.Clamp01(time / fadeTime));
                } else {
                    OnHideFrame(Mathf.Clamp01(time / fadeTime));
                }
                await Dispatcher.NextUpdate();
            }
            return message;
        }
    }
}