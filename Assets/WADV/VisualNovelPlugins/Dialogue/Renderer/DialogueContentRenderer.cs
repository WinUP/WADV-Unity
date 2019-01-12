using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovelPlugins.Dialogue.Items;
using WADV.VisualNovelPlugins.Dialogue.TextGenerator;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框内容组件
    /// </summary>
    public abstract class DialogueContentRenderer : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public int Mask { get; } = DialoguePlugin.MessageMask;
        public bool NoWaiting { get; } = true;

        /// <summary>
        /// 文本生成器
        /// </summary>
        public DialogueTextGenerator generator;
        
        /// <summary>
        /// 每两次生成的帧间隔
        /// </summary>
        [Range(0, 60)]
        public int frameSpan;
        
        /// <summary>
        /// 获取当前显示的文本（用于多段生成时缓存之前文本段的内容）
        /// </summary>
        protected abstract string CurrentText { get; }
        
        private void OnEnable() {
            MessageService.Receivers.CreateChild(this);
        }

        private void OnDisable() {
            MessageService.Receivers.RemoveChild(this);
        }

        /// <summary>
        /// 清空文本
        /// </summary>
        protected abstract void ClearText();

        /// <summary>
        /// 准备文本段样式
        /// </summary>
        /// <param name="currentText">目标文本段</param>
        protected abstract void PrepareStyle(TextDialogueItem currentText);

        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="previousPart">之前所有文本段已实际显示（或格式化）的文本</param>
        /// <param name="text">当前段已经生成的文本</param>
        /// <returns></returns>
        protected abstract void ShowText(StringBuilder previousPart, StringBuilder text);

        /// <inheritdoc />
        public async Task<Message> Receive(Message message) {
            if (message.Tag == DialoguePlugin.NewDialogueMessageTag && message is Message<DialogueDescription> dialogueMessage) {
                await ProcessText(dialogueMessage);
            }
            return message;
        }

        private async Task ProcessText(Message<DialogueDescription> dialogueMessage) {
            var dialogue = dialogueMessage.Content;
            var history = dialogue.NoClear ? new StringBuilder(CurrentText) : new StringBuilder();
            if (generator == null) {
                generator = new EmptyDialogueTextGenerator();
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
                        PrepareStyle(textDialogueItem);
                        generator.Text = textDialogueItem.Text;
                        generator.Reset();
                        while (generator.MoveNext()) {
                            ShowText(history, generator.Current);
                            for (var j = -1; ++j < frameSpan;) {
                                await Dispatcher.NextUpdate();
                            }
                        }
                        history.Append(CurrentText);
                        break;
                }
            }
            if (!dialogue.NoWait) {
                await MessageService.WaitUntil(CoreConstant.Mask, CoreConstant.ScreenClicked);
            }
        }
    }
}