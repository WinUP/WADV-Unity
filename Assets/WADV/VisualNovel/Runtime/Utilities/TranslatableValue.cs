using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个可翻译内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
    ///     <item><description>浮点转换器</description></item>
    ///     <item><description>整数转换器</description></item>
    ///     <item><description>字符串转换器</description></item>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>减法互操作器</description></item>
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>除法互操作器</description></item>
    ///     <item><description>取反互操作器</description></item>
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class TranslatableValue : SerializableValue, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                     INegativeOperator, IBooleanConverter, IIntegerConverter, IFloatConverter {
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

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.ConvertToFloat(language);
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.ConvertToInteger(language);
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.ConvertToBoolean(language);
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new StringValue {Value = ConvertToString(language)}.ToNegative(language);
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