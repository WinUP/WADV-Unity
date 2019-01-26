using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;

namespace WADV.Plugins.Dialogue.Renderer {
    public abstract class DialogueShowHideRenderer : MonoMessengerBehaviour {
        public override int Mask { get; } = DialoguePlugin.MessageIntegration.Mask;
        public override bool IsStandaloneMessage { get; } = false;
        
        private bool _hidden;
        
        public static LinkedTreeNode<IMessenger> RootMessenger { get; }

        static DialogueShowHideRenderer() {
            RootMessenger = MessageService.Receivers.CreateChild(new EmptyMessenger());
        }
        
        protected override void OnEnable() {
            RootMessenger.CreateChild(this);
        }

        protected override void OnDisable() {
            RootMessenger.RemoveChild(this);
        }
        
        protected virtual void PrepareStartShow(float totalTime) { }
        
        protected virtual void PrepareStartHide(float totalTime) { }
        
        protected virtual void AfterShow() { }
        
        protected virtual void AfterHide() { }

        protected abstract void OnShowFrame(float progress);

        protected abstract void OnHideFrame(float progress);

        public override Task<Message> Receive(Message message) {
            float totalTime;
            if (message.HasTag(DialoguePlugin.MessageIntegration.ShowDialogueBox)) {
                if (!_hidden) return Task.FromResult(message);
                _hidden = false;
                totalTime = message is Message<float> floatMessage ? floatMessage.Content : 0.0F;
                PrepareStartShow(totalTime);
            } else if (message.HasTag(DialoguePlugin.MessageIntegration.HideDialogueBox)) {
                if (_hidden) return Task.FromResult(message);
                _hidden = true;
                totalTime = message is Message<float> floatMessage ? floatMessage.Content : 0.0F;
                PrepareStartHide(totalTime);
            } else {
                return Task.FromResult(message);
            }
            if (HasQuickCachePlaceholder()) {
                CompleteCachedPlaceholder();
            }
            QuickCachePlaceholder(message.CreatePlaceholder());
            StartCoroutine(RunShowHide(totalTime, message.Tag == DialoguePlugin.MessageIntegration.ShowDialogueBox).AsIEnumerator());
            return Task.FromResult(message);
        }

        private async Task RunShowHide(float totalTime, bool isShow) {
            var time = 0.0F;
            while (time <= totalTime) {
                time += Time.deltaTime;
                if (isShow) {
                    OnShowFrame(Mathf.Clamp01(time / totalTime));
                } else {
                    OnHideFrame(Mathf.Clamp01(time / totalTime));
                }
                await Dispatcher.NextUpdate();
            }
            if (isShow) {
                AfterShow();
            } else {
                AfterHide();
            }
            CompleteCachedPlaceholder();
        }
    }
}