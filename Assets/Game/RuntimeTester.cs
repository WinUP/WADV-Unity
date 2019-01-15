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
            var context = PluginExecuteContext.Create(runtime);
            var character = new CharacterValue {Name = new StringValue {Value = "<color=#80d6ff>一之濑翼</color>"}, Avatar = new NullValue()};
            await MessageService.ProcessAsync(new Message<DialogueDescription>(
                                                  new DialogueDescription(context) {RawCharacter = character, RawContent = new StringValue {Value = "诶？啊……可、可以。[NoWait]"}},
                                                  DialoguePlugin.MessageMask,
                                                  DialoguePlugin.NewDialogueMessageTag));
        }
    }
}