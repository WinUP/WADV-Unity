using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WADV;
using WADV.Plugins.Input;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;

namespace Game.UI {
    [RequireComponent(typeof(Canvas))]
    public class InputBoxRenderer : InputRenderer {
        public TextMeshProUGUI titleText;

        public TextMeshProUGUI confirmText;
        
        public TMP_InputField inputField;

        public Button confirmButton;

        public GraphicColorFade imageFader;
        
        public GraphicColorFade textFader;

        public override string Text => inputField.text;

        private Canvas _canvas;

        private void Start() {
            _canvas = GetComponent<Canvas>();
        }

        protected override void OnEnable() {
            base.OnEnable();
            confirmButton.onClick.AddListener(OnButtonClick);
        }

        protected override void OnDisable() {
            base.OnDisable();
            confirmButton.onClick.RemoveListener(OnButtonClick);
        }

        public override void SetText(PluginExecuteContext context, InputPlugin.MessageIntegration.Content content) {
            titleText.text = content.Title != null ? content.Title.ConvertToString(context.Language) : "";
            inputField.text = content.Default != null ? content.Default.ConvertToString(context.Language) : "";
            confirmText.text = content.ButtonText != null ? content.ButtonText.ConvertToString(context.Language) : "OK";
        }

        public override async Task Show() {
            _canvas.enabled = true;
            await Dispatcher.WaitAll(imageFader.Fade(), textFader.Fade());
        }

        public override async Task Wait() {
            QuickCachePlaceholder(Dispatcher.CreatePlaceholder());
            await WaitCachedPlaceholder();
        }

        public override async Task Hide() {
            await Dispatcher.WaitAll(imageFader.Reverse(), textFader.Reverse());
            _canvas.enabled = false;
        }

        private void OnButtonClick() {
            if (!string.IsNullOrEmpty(Text)) {
                CompleteCachedPlaceholder();
            }
        }
    }
}