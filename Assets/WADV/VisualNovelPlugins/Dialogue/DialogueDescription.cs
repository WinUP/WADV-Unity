using System.Collections.Generic;
using WADV.VisualNovelPlugins.Dialogue.DialogueItems;

namespace WADV.VisualNovelPlugins.Dialogue {
    public class DialogueDescription {
        public CharacterValue Character { get; set; }
        public List<IDialogueItem> Content { get; set; }
        public bool NoWait { get; set; }
        public bool NoClear { get; set; }
    }
}