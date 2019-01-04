using System;

namespace Core.VisualNovel.Runtime {
    [Serializable]
    public struct CallStack {
        public string ScriptId { get; set; }
        public long Offset { get; set; }
    }
}