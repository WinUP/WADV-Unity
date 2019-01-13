using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using WADV.Extensions;
using WADV.Thread;

namespace WADV.VisualNovelPlugins.Dialogue.Renderer {
    /// <inheritdoc cref="CharacterContentRenderer" />
    /// <summary>
    /// 表示一个使用TextMesh渲染的对话框角色插件
    /// </summary>
    public class TextMeshCharacterContentRenderer : CharacterContentRenderer {
        private TextMeshProUGUI _textMesh;
        
        private void Start() {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh == null) throw new NotSupportedException("Unable to create TextMeshCharacter: no TextMeshProUGUI component found in current object");
        }
        
        protected override async Task ShowText(CharacterValue text, string language) {
            var color = _textMesh.color;
            var time = 0.0F;
            while (time < 0.2F) {
                time += Time.deltaTime;
                _textMesh.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0.0F, time / 0.2F));
                await Dispatcher.NextUpdate();
            }
            _textMesh.text = text.ConvertToString(language);
        }
    }
}