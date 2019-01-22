using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using WADV.Plugins.Input;
using WADV.VisualNovel.Plugin;

namespace Game.UI {
    public class InputCanvas : InputRenderer {
        public TextMeshProUGUI titleText;
        
        public TMP_InputField inputField;

        public Button confirmButton;

        public override string Text => inputField.text;

        public override Task Show(PluginExecuteContext context, InputPlugin.MessageIntegration.Content content) {
            throw new NotSupportedException();
        }

        public override Task Hide() {
            throw new NotSupportedException();
        }
    }
}