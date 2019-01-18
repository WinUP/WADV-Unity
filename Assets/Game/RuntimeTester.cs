using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue;
using WADV.Thread;
using WADV.VisualNovel.Runtime;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public async void TestScript() {
            var runtime = new ScriptRuntime("Logic/!Entrance");
            await runtime.ExecuteScript();
        }

        public async void NextDialogue() {
            await MessageService.ProcessAsync(new Message(DialoguePlugin.MessageMask, DialoguePlugin.FinishContentWaiting));
        }
    }
}