using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Translation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="IMemoryValue" />
    /// <summary>
    /// 表示一个可翻译内存堆栈值
    /// </summary>
    public class TranslatableMemoryValue : IMemoryValue, IStringConverter {
        /// <summary>
        /// 获取或设置翻译所在的脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取说设置翻译ID
        /// </summary>
        public uint TranslationId { get; set; }

        /// <inheritdoc />
        public IMemoryValue Duplicate() {
            return new TranslatableMemoryValue {ScriptId = ScriptId, TranslationId = TranslationId};
        }

        public string ConvertToString() {
            var defaultTranslation = ScriptHeader.LoadAsset(ScriptId).Header.GetTranslation(TranslationManager.DefaultLanguage, TranslationId);
            return $"TranslatableMemoryValue {{ScriptId = {ScriptId}, TranslationId = {TranslationId}, Default = {defaultTranslation}}}";
        }
    }
}