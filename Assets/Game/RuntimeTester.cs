using UnityEngine;
using WADV.MessageSystem;
using WADV.VisualNovelPlugins.Dialogue;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        private bool _hidden = false;
        public void TestDialogueShow() {
            MessageService.ProcessAsync(new Message<float>(0.2F) {Mask = DialoguePlugin.MessageMask, Tag = _hidden ? DialoguePlugin.ShowDialogueBoxMessageTag : DialoguePlugin.HideDialogueBoxMessageTag});
            _hidden = !_hidden;
        }
    }
}