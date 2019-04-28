using UnityEngine;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue;
using WADV.Plugins.Input;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public async void TestScript() {
            var runtime = new ScriptRuntime("Test");
            await runtime.ExecuteScript();
        }

        public async void NextDialogue() {
            await MessageService.ProcessAsync(Message.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.FinishContentWaiting));
        }

        public async void TestInput() {
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.HideDialogueBox, 0.3F));
            var title = new StringValue {value = "输入姓和名（空格隔开）"};
            var defaultText = new StringValue {value = "诹访部 翔平"};
            var confirmText = new StringValue {value = "继续"};
            var context = PluginExecuteContext.Create(new ScriptRuntime("Utilities"));
            var message = await MessageService.ProcessAsync(ContextMessage<InputPlugin.MessageIntegration.Content>.Create(
                                                                InputPlugin.MessageIntegration.Mask,
                                                                InputPlugin.MessageIntegration.CreateInput,
                                                                new InputPlugin.MessageIntegration.Content {Title = title, Default = defaultText, ButtonText = confirmText},
                                                                context));
            if (message is Message<string> stringMessage) {
                Debug.Log(stringMessage.Content);
            }
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.ShowDialogueBox, 0.3F));
        }
    }
}