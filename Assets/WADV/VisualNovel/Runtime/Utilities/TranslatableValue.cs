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

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new TranslatableValue {ScriptId = ScriptId, TranslationId = TranslationId};
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return ConvertToString(TranslationManager.DefaultLanguage);
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ScriptHeader.LoadSync(ScriptId).Header.GetTranslation(language, TranslationId);
        }

        public override string ToString() {
            return ConvertToString();
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return AddWith(target, TranslationManager.DefaultLanguage);
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target, string language) {
            return new StringValue {Value = ConvertToString(language)}.AddWith(target);
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return SubtractWith(target, TranslationManager.DefaultLanguage);
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target, string language) {
            return new StringValue {Value = ConvertToString(language)}.SubtractWith(target);
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return MultiplyWith(target, TranslationManager.DefaultLanguage);
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target, string language) {
            return new StringValue {Value = ConvertToString(language)}.MultiplyWith(target);
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return DivideWith(target, TranslationManager.DefaultLanguage);
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target, string language) {
            return new StringValue {Value = ConvertToString(language)}.DivideWith(target);
        }
    }
}