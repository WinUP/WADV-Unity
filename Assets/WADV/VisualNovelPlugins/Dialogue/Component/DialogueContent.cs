using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovelPlugins.Dialogue.DialogueItems;

namespace WADV.VisualNovelPlugins.Dialogue.Component {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框内容组件
    /// </summary>
    public abstract class DialogueContent : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public int Mask { get; } = DialoguePlugin.MessageMask;

        protected abstract string CurrentText { get; }
        
        /// <summary>
        /// 每两次生成的帧间隔
        /// </summary>
        [Range(0, 60)]
        public int frameSpan;
        
        private void OnEnable() {
            MessageService.Receivers.CreateChild(this);
        }

        private void OnDisable() {
            MessageService.Receivers.RemoveChild(this);
        }

        protected abstract void ClearText();

        protected abstract Task ShowText(string historyText, TextDialogueItem currentText);

        /// <inheritdoc />
        public async Task<Message> Receive(Message message) {
            if (message.Tag != DialoguePlugin.NewDialogueMessageTag || !(message is Message<DialogueDescription> dialogueMessage)) return message;
            var dialogue = dialogueMessage.Content;
            var history = new StringBuilder();
            if (dialogue.NoClear) {
                history.Append(CurrentText);
            }
            foreach (var item in dialogue.Content) {
                switch (item) {
                    case ClearDialogueItem _:
                        ClearText();
                        history.Clear();
                        break;
                    case PauseDialogueItem pauseDialogueItem:
                        if (pauseDialogueItem.Time.HasValue) {
                            await Dispatcher.WaitForSeconds(pauseDialogueItem.Time.Value);
                        } else {
                            await MessageService.WaitUntil(CoreConstant.Mask, CoreConstant.ScreenClicked);
                        }
                        break;
                    case TextDialogueItem textDialogueItem:
                        await ShowText(history.ToString(), textDialogueItem);
                        history.Append(CurrentText);
                        break;
                }
            }
            if (!dialogue.NoWait) {
                await MessageService.WaitUntil(CoreConstant.Mask, CoreConstant.ScreenClicked);
            }
            return message;
        }
    }
}