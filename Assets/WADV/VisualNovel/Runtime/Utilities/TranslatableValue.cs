using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个可翻译内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class TranslatableValue : SerializableValue, IStringConverter {
        /// <summary>
        /// 获取或设置翻译所在的脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取说设置翻译ID
        /// </summary>
        public uint TranslationId { get; set; }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new TranslatableValue {ScriptId = ScriptId, TranslationId = TranslationId};
        }

        /// <inheritdoc />
        public string ConvertToString() {
            var defaultTranslation = ScriptHeader.LoadAsset(ScriptId).Header.GetTranslation(TranslationManager.DefaultLanguage, TranslationId);
            return $"TranslatableValue {{ScriptId = {ScriptId}, TranslationId = {TranslationId}, Default = {defaultTranslation}}}";
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ScriptHeader.LoadAsset(ScriptId).Header.GetTranslation(language, TranslationId);
        }

        public override string ToString() {
            return ConvertToString();
        }
    }
}