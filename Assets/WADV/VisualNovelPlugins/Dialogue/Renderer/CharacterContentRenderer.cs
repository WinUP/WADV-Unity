using System.Threading.Tasks;
using UnityEngine;
using WADV.MessageSystem;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框角色组件
    /// </summary>
    public abstract class CharacterContentRenderer : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public int Mask { get; } = DialoguePlugin.MessageMask;
        
        /// <inheritdoc />
        public bool IsStandaloneMessage { get; } = true;

        private void OnEnable() {
            MessageService.Receivers.CreateChild(this);
        }

        private void OnDisable() {
            MessageService.Receivers.RemoveChild(this);
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        /// <param name="text">目标文本</param>
        /// <param name="language">当前语言</param>
        /// <returns></returns>
        protected abstract Task ShowText(CharacterValue text, string language);

        /// <inheritdoc />
        public async Task<Message> Receive(Message message) {
            if (message.Tag != DialoguePlugin.NewDialogueMessageTag || !(message is Message<DialogueDescription> dialogueMessage)) return message;
            var placeholder = message.CreatePlaceholder();
            await ShowText(dialogueMessage.Content.Character, dialogueMessage.Content.Language);
            placeholder.Complete();
            return message;
        }
    }
}