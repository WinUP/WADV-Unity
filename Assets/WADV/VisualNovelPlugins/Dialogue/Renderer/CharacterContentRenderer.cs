using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 基础对话框角色组件
    /// </summary>
    public abstract class CharacterContentRenderer : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public int Mask { get; } = DialoguePlugin.MessageMask | CoreConstant.Mask;
        
        /// <inheritdoc />
        public bool IsStandaloneMessage { get; } = true;

        [CanBeNull] private DialogueDescription _currentDescription;
        [CanBeNull] private MainThreadPlaceholder _currentPlaceholder;

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
        /// <returns></returns>
        protected abstract Task ShowText(string text);

        /// <summary>
        /// 立即替换文本
        /// </summary>
        /// <param name="text">目标文本</param>
        protected abstract void ReplaceText(string text);

        /// <inheritdoc />
        public async Task<Message> Receive(Message message) {
            if (message.Tag == DialoguePlugin.NewDialogueMessageTag && message is Message<DialogueDescription> dialogueMessage) {
                var dialogue = _currentDescription = dialogueMessage.Content;
                _currentPlaceholder = message.CreatePlaceholder();
                await ShowText(CreateCharacterText(dialogue.RawCharacter, dialogue.Context.Runtime, dialogue.Context.Runtime.ActiveLanguage));
                if (_currentPlaceholder == null) return message;
                _currentPlaceholder.Complete();
                _currentPlaceholder = null;
            } else if (_currentDescription != null && message.Tag == CoreConstant.LanguageChange && message is Message<string> languageMessage) {
                if (_currentPlaceholder != null) {
                    await _currentPlaceholder;
                }
                ReplaceText(CreateCharacterText(_currentDescription.RawCharacter, _currentDescription.Context.Runtime, languageMessage.Content));
            }
            return message;
        }

        private static string CreateCharacterText(SerializableValue raw, ScriptRuntime runtime, string language) {
            var character = DialoguePlugin.CreateCharacter(runtime, raw);
            return character == null ? "" : character.ConvertToString(language);
        }
    }
}