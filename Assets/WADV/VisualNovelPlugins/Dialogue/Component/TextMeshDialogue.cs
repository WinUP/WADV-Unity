using System;
using System.Text;
using System.Threading.Tasks;
using WADV.MessageSystem;
using TMPro;
using UnityEngine;
using WADV.Extensions;
using WADV.Thread;
using WADV.VisualNovelPlugins.Dialogue.DialogueItems;

namespace WADV.VisualNovelPlugins.Dialogue.Component {
    /// <inheritdoc cref="IMessenger" />
    /// <summary>
    /// 表示一个使用TextMesh渲染的对话框插件
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshDialogue : DialogueContent {
        private TextMeshProUGUI _textMesh;

        private void Start() {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh == null) throw new NotSupportedException("Unable to create TextMeshDialogue: no TextMeshProUGUI component found in current object");
        }

        protected override string CurrentText => _textMesh.text;

        protected override void ClearText() {
            _textMesh.text = "";
        }

        protected override Task ShowText(string historyText, TextDialogueItem currentText) {
            var (startPart, endPart) = CreateStyle(currentText);
            for (var i = -1; ++i < currentText.Text.Length;) {
                _textMesh.text = $"{history}{startPart}{generator.Current}{endPart}";
                for (var i = -1; ++i < frameSpan;) {
                    await Dispatcher.NextUpdate();
                }
            }
            while (generator.MoveNext()) {
                _textMesh.text = $"{history}{startPart}{generator.Current}{endPart}";
                for (var i = -1; ++i < frameSpan;) {
                    await Dispatcher.NextUpdate();
                }
            }
            history.Append(_textMesh.text);
        }

        private static (string StartPart, string EndPart) CreateStyle(TextDialogueItem source) {
            var startPart = new StringBuilder();
            var endPart = new StringBuilder();
            if (source.Bold == true) {
                startPart.Append("<b>");
                endPart.Append("</b>");
            }
            if (!string.IsNullOrEmpty(source.Color)) {
                startPart.Append(source.Color.StartsWith("#") ? $"<color={source.Color}>" : $"<color=\"{source.Color}\">");
                endPart.Append("</color>");
            }
            if (source.Italic == true) {
                startPart.Append("<i>");
                endPart.Append("</i>");
            }
            if (source.Underline == true) {
                startPart.Append("<u>");
                endPart.Append("</u>");
            }
            if (source.Strikethrough == true) {
                startPart.Append("<s>");
                endPart.Append("</s>");
            }
            if (source.FontSize.HasValue) {
                if (source.RelativeSize == true) {
                    startPart.Append(source.FontSize >= 0 ? $"<size=+{source.FontSize}>" : $"<size=-{source.FontSize}>");
                    endPart.Append("</size>");
                } else {
                    startPart.Append($"<size={source.FontSize}>");
                    endPart.Append("</size>");
                }
            }
            return (startPart.ToString(), endPart.ToString());
        }
    }
}