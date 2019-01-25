using System.Threading.Tasks;
using WADV.MessageSystem;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.Plugins.Dialogue.Renderer {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框角色组件
    /// </summary>
    public abstract class CharacterContentRenderer : MonoMessengerBehaviour {
        /// <inheritdoc />
        public override int Mask { get; } = DialoguePlugin.MessageIntegration.Mask | CoreConstant.Mask;
        
        /// <inheritdoc />
        public override bool IsStandaloneMessage { get; } = true;
        
        /// <summary>
        /// 获取当前显示的文本
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="text">目标文本</param>
        /// <returns></returns>
        public abstract Task ShowText(string text);

        /// <summary>
        /// 立即替换文本
        /// </summary>
        /// <param name="text">目标文本</param>
        public abstract void ReplaceText(string text);

        /// <inheritdoc />
        public override async Task<Message> Receive(Message message) {
            if (message.HasTag(DialoguePlugin.MessageIntegration.NewDialogue) && message is ContextMessage<DialoguePlugin.MessageIntegration.Content> dialogueMessage) {
                QuickCacheMessage(message);
                var dialogue = dialogueMessage.Content;
                var text = CreateCharacterText(dialogue.Character, dialogueMessage.Context.Runtime, dialogueMessage.Context.Runtime.ActiveLanguage);
                if (text == Text) return message;
                QuickCachePlaceholder(message.CreatePlaceholder());
                await ShowText(text);
                CompleteCachedPlaceholder();
            } else if (HasQuickCacheMessage() && message.HasTag(CoreConstant.LanguageChange) && message is Message<string> languageMessage) {
                if (HasQuickCachePlaceholder()) {
                    await WaitCachedPlaceholder();
                }
                dialogueMessage = PopQuickCacheMessage<ContextMessage<DialoguePlugin.MessageIntegration.Content>>();
                QuickCacheMessage(dialogueMessage);
                if (dialogueMessage == null) return message;
                var text = CreateCharacterText(dialogueMessage.Content.Character, dialogueMessage.Context.Runtime, dialogueMessage.Context.Language);
                if (text == Text) return message;
                ReplaceText(CreateCharacterText(dialogueMessage.Content.Character, dialogueMessage.Context.Runtime, languageMessage.Content));
            }
            return message;
        }

        private static string CreateCharacterText(SerializableValue raw, ScriptRuntime runtime, string language) {
            var character = DialoguePlugin.CreateCharacter(runtime, raw);
            return character == null ? "" : character.ConvertToString(language);
        }
    }
}