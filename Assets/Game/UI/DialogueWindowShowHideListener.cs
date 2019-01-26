using UnityEngine;
using UnityEngine.UI;
using WADV;
using WADV.Plugins.Dialogue.Renderer;

namespace Game.UI {
    [AddComponentMenu("UI/Listeners/Dialogue Window Show Hide Listener")]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class DialogueWindowShowHideListener : DialogueShowHideRenderer {
        public GraphicColorFade fadeProvider;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        
        private void Start() {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
        }

        protected override void PrepareStartShow(float totalTime) {
            _canvas.enabled = true;
        }

        protected override void OnShowFrame(float progress) {
            fadeProvider.ReverseFrame(progress);
            ApplyPosition(-243.7F, 0.0F, Easing.GetEasingFunction(fadeProvider.easingType)(progress));
        }

        protected override void OnHideFrame(float progress) {
            fadeProvider.FadeFrame(progress);
            ApplyPosition(0.0F, -243.7F, Easing.GetEasingFunction(fadeProvider.easingType)(progress));
        }

        protected override void AfterHide() {
            _canvas.enabled = false;
        }

        private void ApplyPosition(float from, float to, float progress) {
            _rectTransform.offsetMin = new Vector2(0, Mathf.Lerp(from, to, progress));
        }
    }
}