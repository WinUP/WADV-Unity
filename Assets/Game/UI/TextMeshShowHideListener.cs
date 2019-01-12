using System;
using TMPro;
using UnityEngine;
using WADV;
using WADV.VisualNovelPlugins.Dialogue.Renderer;

namespace Game.UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class TextMeshShowHideListener : DialogueShowHideRenderer {
        public Color defaultVisibleColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        public Color defaultHiddenColor = new Color(1.0F, 1.0F, 1.0F, 0.0F);
        
        private Color? _initialColor;
        private TextMeshProUGUI _text;
        
        private void Start() {
            _text = GetComponent<TextMeshProUGUI>();
            if (_text == null) throw new NotSupportedException($"Unable to create {nameof(ImageShowHideListener)}: no Image component found in current object");
        }

        protected override void PrepareStartHide(float totalTime) {
            base.PrepareStartHide(totalTime);
            _initialColor = _text.color;
        }

        protected override void PrepareStartShow(float totalTime) {
            base.PrepareStartShow(totalTime);
            if (_initialColor == null) {
                _initialColor = defaultVisibleColor;
            }
        }

        protected override void OnShowFrame(float progress) {
            var color = _initialColor ?? defaultVisibleColor;
            _text.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0.0F, color.a, Easing.CubicOut(progress)));
        }

        protected override void OnHideFrame(float progress) {
            var color = _initialColor ?? defaultHiddenColor;
            _text.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0.0F, Easing.CubicOut(progress)));
        }
    }
}