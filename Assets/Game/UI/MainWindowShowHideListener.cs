using System;
using UnityEngine;
using UnityEngine.UI;
using WADV;
using WADV.VisualNovelPlugins.Dialogue.Renderer;

namespace Game.UI {
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class MainWindowShowHideListener : DialogueShowHideRenderer {
        public Color visibleColor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        public Color hiddenColor = new Color(1.0F, 1.0F, 1.0F, 0.0F);

        private Image _image;
        private RectTransform _rectTransform;
        
        private void Start() {
            _image = GetComponent<Image>();
            if (_image == null) throw new NotSupportedException($"Unable to create {nameof(MainWindowShowHideListener)}: no Image component found in current object");
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null) throw new NotSupportedException($"Unable to create {nameof(MainWindowShowHideListener)}: no RectTransform component found in current object");
        }

        protected override void OnShowFrame(float progress) {
            progress = Easing.QuartInOut(progress);
            ApplyColor(ref hiddenColor, ref visibleColor, progress);
            ApplyPosition(-243.7F, 0.0F, progress);
        }

        protected override void OnHideFrame(float progress) {
            progress = Easing.QuartInOut(progress);
            ApplyColor(ref visibleColor, ref hiddenColor, progress);
            ApplyPosition(0.0F, -243.7F, progress);
        }

        private void ApplyColor(ref Color from, ref Color to, float progress) {
            _image.color = new Color(
                Mathf.Lerp(from.r, to.r, progress),
                Mathf.Lerp(from.g, to.g, progress),
                Mathf.Lerp(from.b, to.b, progress),
                Mathf.Lerp(from.a, to.a, progress));
        }

        private void ApplyPosition(float from, float to, float progress) {
            _rectTransform.offsetMin = new Vector2(0, Mathf.Lerp(from, to, progress));
        }
    }
}