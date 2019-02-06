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
    public class TranslatableValue : SerializableValue, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
        /// <summary>
        /// 获取或设置翻译所在的脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取说设置翻译ID
        /// </summary>
        public uint TranslationId { get; set; }

        public override SerializableValue Duplicate() {
            return new TranslatableValue {ScriptId = ScriptId, TranslationId = TranslationId};
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return ScriptHeader.LoadSync(ScriptId).Header.GetTranslation(language, TranslationId);
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.AddWith(target, language);
        }

        public SerializableValue SubtractWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.SubtractWith(target, language);
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.MultiplyWith(target, language);
        }

        public SerializableValue DivideWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.DivideWith(target, language);
        }
    }
}