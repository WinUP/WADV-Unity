using UnityEngine;
using WADV.MessageSystem;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovelPlugins.Dialogue;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public async void TestDialogueShow() {
            var runtime = new ScriptRuntime("Logic/!Entrance");
            await runtime.ExecuteScript();
        }
    }
}