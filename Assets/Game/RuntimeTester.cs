using UnityEngine;
using WADV.MessageSystem;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovelPlugins.Dialogue;

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