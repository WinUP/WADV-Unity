using System;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个布尔内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>布尔转换器</description></item>
    ///     <item><description>浮点转换器</description></item>
    ///     <item><description>整数转换器</description></item>
    ///     <item><description>字符串转换器</description></item>
    ///     <item><description>加法互操作器</description></item>
    ///     <item><description>乘法互操作器</description></item>
    ///     <item><description>取反互操作器</description></item>
    ///     <item><description>比较互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素/特性支持</description></listheader>
    ///     <item><description>Reverse</description></item>
    ///     <item><description>ToNumber</description></item>
    ///     <item><description>ToString</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class BooleanValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator,
                                IPickChildOperator, IEqualOperator, INegativeOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// 尝试将可序列化值解析为布尔值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static bool TryParse(SerializableValue value, string language = TranslationManager.DefaultLanguage) {
            switch (value) {
                case IBooleanConverter booleanConverter:
                    return booleanConverter.ConvertToBoolean(language);
                case IFloatConverter floatConverter:
                    return !floatConverter.ConvertToFloat(language).Equals(0.0F);
                case IIntegerConverter integerConverter:
                    return integerConverter.ConvertToInteger(language) != 0;
                case IStringConverter stringConverter:
                    var upperValue = stringConverter.ConvertToString(language).ToUpper();
                    if (upperValue == "F" || upperValue == "FALSE") return false;
                    if (int.TryParse(upperValue, out var intValue) && intValue == 0) return false;
                    return !(float.TryParse(upperValue, out var floatValue) && floatValue.Equals(0.0F));
                default:
                    return value != null;
            }
        }

        public override SerializableValue Duplicate() {
            return new BooleanValue {Value = Value};
        }

        public bool ConvertToBoolean(string language = TranslationManager.DefaultLanguage) {
            return Value;
        }

        public float ConvertToFloat(string language = TranslationManager.DefaultLanguage) {
            return Value ? 1.0F : 0.0F;
        }

        public int ConvertToInteger(string language = TranslationManager.DefaultLanguage) {
            return Value ? 1 : 0;
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return Value ? "True" : "False";
        }

        public override string ToString() {
            return ConvertToString();
        }

        public SerializableValue ToNegative(string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {Value = !Value};
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            var value = TryParse(target, language);
            return value == Value;
        }

        public SerializableValue AddWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {Value = Value || TryParse(target, language)};
        }

        public SerializableValue MultiplyWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return new BooleanValue {Value = Value && TryParse(target, language)};
        }

        public SerializableValue PickChild(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            if (!(target is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in boolean value with feature id {target}: only string feature name is accepted");
            var name = stringConverter.ConvertToString(language);
            switch (name) {
                case "Reverse":
                    return new BooleanValue {Value = !Value};
                case "ToNumber":
                    return new IntegerValue {Value = ConvertToInteger(language)};
                case "ToString":
                    return new StringValue {Value = ConvertToString(language)};
                default:
                    throw new NotSupportedException($"Unable to get feature in boolean value: unsupported feature {name}");
            }
        }
    }
}