namespace Core.VisualNovel.Runtime.MemoryValues {
    public class TranslatableMemoryValue : IMemoryValue {
        public string ScriptId { get; set; }
        public uint TranslationId { get; set; }


        public IMemoryValue Duplicate() {
            return new TranslatableMemoryValue {ScriptId = ScriptId, TranslationId = TranslationId};
        }
    }
}