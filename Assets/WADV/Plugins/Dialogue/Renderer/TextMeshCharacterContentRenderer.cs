using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using WADV.Extensions;
using WADV.Thread;

namespace WADV.Plugins.Dialogue.Renderer {
    /// <inheritdoc cref="CharacterContentRenderer" />
    /// <summary>
    /// 表示一个使用TextMesh渲染的对话框角色插件
    /// </summary>
    public class TextMeshCharacterContentRenderer : CharacterContentRenderer {
        /// <summary>
        /// 淡入淡出时间
        /// </summary>
        [Range(0.0F, 0.5F)]
        public float fadeTime = 0.075F;
        
        /// <inheritdoc />
        public override string Text => _textMesh.text;
        
        private TextMeshProUGUI _textMesh;
        
        private void Start() {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh == null) throw new NotSupportedException("Unable to create TextMeshCharacter: no TextMeshProUGUI component found in current object");
        }

        /// <inheritdoc />
        public override async Task ShowText(string text) {
            var color = _textMesh.color;
            var time = 0.0F;
            while (time < fadeTime) {
                _textMesh.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0.0F, time / 0.1F));
                await Dispatcher.NextUpdate();
                time += Time.deltaTime;
            }
            _textMesh.color = new Color(color.r, color.g, color.b, 0.0F);
            _textMesh.text = text;
            time = 0.0F;
            while (time < fadeTime) {
                _textMesh.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0.0F, color.a, time / 0.1F));
                await Dispatcher.NextUpdate();
                time += Time.deltaTime;
            }
            _textMesh.color = color;
        }

        /// <inheritdoc />
        public override void ReplaceText(string text) {
            _textMesh.text = text;
        }
    }
}