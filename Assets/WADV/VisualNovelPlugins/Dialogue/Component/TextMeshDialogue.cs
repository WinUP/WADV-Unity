using System;
using System.Threading.Tasks;
using WADV.MessageSystem;
using TMPro;
using UnityEngine;

namespace WADV.VisualNovelPlugins.Dialogue.Component {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 表示一个使用TextMesh渲染的对话框插件
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshDialogue : MonoBehaviour, IMessenger {
        /// <inheritdoc />
        public int Mask { get; } = DialoguePlugin.MessageMask;

        private TextMeshProUGUI _textMesh;

        /// <summary>
        /// TextMesh对话框插件根消息监听器节点
        /// </summary>
        public static LinkedTreeNode<IMessenger> RootListenerNode { get; }

        static TextMeshDialogue() {
            RootListenerNode = MessageService.Receivers.CreateChild(new EmptyMessenger {Mask = DialoguePlugin.MessageMask});
        }

        private void Start() {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh == null) throw new NotSupportedException("Unable to create TextMeshDialogue: no TextMeshProUGUI component found in current object");
        }

        private void OnDisable() {
            RootListenerNode.CreateChild(this);
        }

        private void OnEnable() {
            RootListenerNode.RemoveChild(this);
        }

        /// <inheritdoc />
        public async Task<Message> Receive(Message message) {
            if (message.Tag != DialoguePlugin.NewDialogueMessageTag || !(message is Message<DialogueDescription> dialogueMessage)) return message;
            var dialogue = dialogueMessage.Content;
            
            return message;
        }
    }
}