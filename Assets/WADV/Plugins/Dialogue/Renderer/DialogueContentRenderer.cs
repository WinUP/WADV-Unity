using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue.Items;
using WADV.Plugins.Dialogue.TextGenerator;
using WADV.Thread;

namespace WADV.Plugins.Dialogue.Renderer {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框内容组件
    /// </summary>
    public abstract class DialogueContentRenderer : MonoMessengerBehaviour {
        /// <inheritdoc />
        public override int Mask { get; } = DialoguePlugin.MessageIntegration.Mask | CoreConstant.Mask;
        public override bool IsStandaloneMessage { get; } = true;

        /// <summary>
        /// 获取渲染状态
        /// </summary>
        public RenderState State { get; private set; } = RenderState.Idle;

        /// <summary>
        /// 文本生成器
        /// </summary>
        public DialogueTextGeneratorType textGenerator = DialogueTextGeneratorType.Simple;
        
        /// <summary>
        /// 每两次生成的时间间隔
        /// </summary>
        [Range(0.0F, 0.5F)]
        public float timeSpan;
        
        /// <summary>
        /// 获取当前显示的文本（用于多段生成时缓存之前文本段的内容）
        /// </summary>
        protected abstract string CurrentText { get; }

        private DialogueTextGenerator _generator;
        private bool _quickMode;

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
        protected abstract void ShowText([CanBeNull] string previousPart, StringBuilder text);

        public void ResetGenerator(DialogueTextGeneratorType type) {
            textGenerator = type;
            _generator = DialogueTextGenerator.Create(textGenerator);
        }

        /// <inheritdoc />
        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(DialoguePlugin.MessageIntegration.NewDialogue) && message is ContextMessage<DialoguePlugin.MessageIntegration.Content> dialogueMessage) {
                if (HasQuickCachePlaceholder()) {
                    await WaitCachedPlaceholder();
                }
                QuickCacheMessage(message);
                QuickCachePlaceholder(message.CreatePlaceholder());
                await ProcessText(dialogueMessage.Context.Runtime.ActiveLanguage);
                PopQuickCacheMessage();
                CompleteCachedPlaceholder();
            } else if (HasQuickCacheMessage() && message.HasTag(CoreConstant.LanguageChange) && message is Message<string> languageMessage) {
                if (HasQuickCachePlaceholder()) {
                    await WaitCachedPlaceholder();
                }
                QuickCachePlaceholder(message.CreatePlaceholder());
                await ProcessText(languageMessage.Content);
                CompleteCachedPlaceholder();
            } else if (State == RenderState.Rendering && message.HasTag(DialoguePlugin.MessageIntegration.FinishContentWaiting)) {
                _quickMode = true;
            }
            return message;
        }

        private async Task ProcessText(string language) {
            var dialogue = PopQuickCacheMessage<ContextMessage<DialoguePlugin.MessageIntegration.Content>>();
            QuickCacheMessage(dialogue);
            if (dialogue == null) return;
            var (content, noWait, noClear) = DialoguePlugin.CreateDialogueContent(dialogue.Context.Runtime, dialogue.Content.Text, language);
            var history = noClear ? CurrentText : null;
            if (_generator == null) {
                ResetGenerator(textGenerator);
            }
            if (content.Any()) {
                foreach (var item in content) {
                    switch (item) {
                        case ClearDialogueItem _:
                            ClearText();
                            history = null;
                            break;
                        case PauseDialogueItem pauseDialogueItem:
                            State = RenderState.Waiting;
                            if (_quickMode) {
                            } else if (pauseDialogueItem.Time.HasValue) {
                                await Dispatcher.WaitForSeconds(pauseDialogueItem.Time.Value);
                            } else {
                                await MessageService.WaitUntil(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.FinishContentWaiting);
                            }
                            State = RenderState.Idle;
                            break;
                        case TextDialogueItem textDialogueItem:
                            State = RenderState.Rendering;
                            PrepareStyle(textDialogueItem);
                            _generator.Text = textDialogueItem.Text;
                            _generator.Reset();
                            while (_generator.MoveNext()) {
                                ShowText(history, _generator.Current);
                                if (timeSpan <= 0.0F) continue;
                                var time = 0.0F;
                                while (time < timeSpan) {
                                    time += Time.deltaTime;
                                    if (_quickMode) break;
                                    await Dispatcher.NextUpdate();
                                }
                            }
                            State = RenderState.Idle;
                            history = CurrentText;
                            break;
                    }
                }
            } else {
                ShowText(history, new StringBuilder());
            }
            _quickMode = false;
            if (!noWait) {
                await MessageService.WaitUntil(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.FinishContentWaiting);
            }
        }
    }
}