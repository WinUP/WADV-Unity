using System;
using System.Text;
using TMPro;
using UnityEngine;
using WADV.VisualNovelPlugins.Dialogue.Items;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    /// <inheritdoc cref="DialogueContentRenderer" />
    /// <summary>
    /// 表示一个使用TextMesh渲染的对话框插件
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class TextMeshDialogueContentRenderer : DialogueContentRenderer {
        private TextMeshProUGUI _textMesh;
        private string _styleStart = "";
        private string _styleEnd = "";

        private void Start() {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh == null) throw new NotSupportedException("Unable to create TextMeshDialogue: no TextMeshProUGUI component found in current object");
        }

        /// <inheritdoc />
        protected override string CurrentText => _textMesh.text;

        /// <inheritdoc />
        protected override void ClearText() {
            _textMesh.text = "";
        }

        /// <inheritdoc />
        protected override void PrepareStyle(TextDialogueItem currentText) {
            var startPart = new StringBuilder();
            var endPart = new StringBuilder();
            if (currentText.Bold == true) {
                startPart.Append("<b>");
                endPart.Append("</b>");
            }
            if (!string.IsNullOrEmpty(currentText.Color)) {
                startPart.Append(currentText.Color.StartsWith("#") ? $"<color={currentText.Color}>" : $"<color=\"{currentText.Color}\">");
                endPart.Append("</color>");
            }
            if (currentText.Italic == true) {
                startPart.Append("<i>");
                endPart.Append("</i>");
            }
            if (currentText.Underline == true) {
                startPart.Append("<u>");
                endPart.Append("</u>");
            }
            if (currentText.Strikethrough == true) {
                startPart.Append("<s>");
                endPart.Append("</s>");
            }
            if (currentText.FontSize.HasValue) {
                if (currentText.RelativeSize == true) {
                    startPart.Append(currentText.FontSize >= 0 ? $"<size=+{currentText.FontSize}>" : $"<size=-{currentText.FontSize}>");
                    endPart.Append("</size>");
                } else {
                    startPart.Append($"<size={currentText.FontSize}>");
                    endPart.Append("</size>");
                }
            }
            _styleStart = startPart.ToString();
            _styleEnd = endPart.ToString();
        }

        /// <inheritdoc />
        protected override void ShowText(StringBuilder previousPart, StringBuilder text) {
            _textMesh.text = $"{previousPart}{_styleStart}{text}{_styleEnd}";
        }
    }
}