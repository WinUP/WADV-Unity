using UnityEngine;
using WADV.MessageSystem;
using WADV.VisualNovelPlugins.Dialogue;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public void TestDialogueShow() {
            MessageService.ProcessAsync(new Message<float>(0.5F) {Mask = DialoguePlugin.MessageMask, Tag = DialoguePlugin.HideDialogueBoxMessageTag});
        }
    }
}