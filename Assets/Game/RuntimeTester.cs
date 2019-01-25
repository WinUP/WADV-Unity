using UnityEngine;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue;
using WADV.Plugins.Input;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public async void TestScript() {
            var runtime = new ScriptRuntime("Logic/!Entrance");
            await runtime.ExecuteScript();
        }

        public async void NextDialogue() {
            await MessageService.ProcessAsync(new Message(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.FinishContentWaiting));
        }

        public async void UseInput() {
            var title = new StringValue {Value = "输入姓名"};
            var defaultText = new StringValue {Value = "诹访部 翔平"};
            var confirmText = new StringValue {Value = "继续"};
            var context = PluginExecuteContext.Create(new ScriptRuntime("Logic/Utilities"));
            var message = await MessageService.ProcessAsync(ContextMessage<InputPlugin.MessageIntegration.Content>.Create(context,
                                                                new InputPlugin.MessageIntegration.Content {Title = title, Default = defaultText, ButtonText = confirmText},
                                                                InputPlugin.MessageIntegration.Mask,
                                                                InputPlugin.MessageIntegration.CreateInput));
            if (message is Message<string> stringMessage) {
                Debug.Log(stringMessage.Content);
            }
        }
    }
}