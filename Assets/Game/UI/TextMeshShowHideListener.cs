using System;
using TMPro;
using UnityEngine;
using WADV;
using WADV.VisualNovelPlugins.Dialogue.Renderer;

namespace Game.UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class TextMeshShowHideListener : DialogueShowHideRenderer {
        public Color visibleColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        public Color hiddenColor = new Color(1.0F, 1.0F, 1.0F, 0.0F);

        private TextMeshProUGUI _text;
        
        private void Start() {
            _text = GetComponent<TextMeshProUGUI>();
            if (_text == null) throw new NotSupportedException($"Unable to create {nameof(MainWindowShowHideListener)}: no Image component found in current object");
        }

        protected override void OnShowFrame(float progress) {
            ApplyColor(ref hiddenColor, ref visibleColor, Easing.CubicOut(progress));
        }

        protected override void OnHideFrame(float progress) {
            ApplyColor(ref visibleColor, ref hiddenColor, Easing.CubicOut(progress));
        }

        private void ApplyColor(ref Color from, ref Color to, float progress) {
            _text.color = new Color(
                Mathf.Lerp(from.r, to.r, progress),
                Mathf.Lerp(from.g, to.g, progress),
                Mathf.Lerp(from.b, to.b, progress),
                Mathf.Lerp(from.a, to.a, progress));
        }
    }
}