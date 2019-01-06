using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
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
    ///     <item><description>取子元素互操作器</description></item>
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
    public class BooleanMemoryValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, IMultiplyOperator,
                                      IPickChildOperator, IEqualOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// 尝试将可序列化值解析为布尔值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <returns></returns>
        public static bool TryParse(SerializableValue value) {
            switch (value) {
                case IBooleanConverter booleanConverter:
                    return booleanConverter.ConvertToBoolean();
                case IFloatConverter floatConverter:
                    return !floatConverter.ConvertToFloat().Equals(0.0F);
                case IIntegerConverter integerConverter:
                    return integerConverter.ConvertToInteger() != 0;
                case IStringConverter stringConverter:
                    var upperValue = stringConverter.ConvertToString().ToUpper();
                    if (upperValue == "F" || upperValue == "FALSE") return false;
                    if (int.TryParse(upperValue, out var intValue) && intValue == 0) return false;
                    return !(float.TryParse(upperValue, out var floatValue) && floatValue.Equals(0.0F));
                default:
                    return value != null;
            }
        }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new BooleanMemoryValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return Value;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value ? 1.0F : 0.0F;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Value ? 1 : 0;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value ? "True" : "False";
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"BooleanMemoryValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            var value = TryParse(target);
            return value == Value;
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new BooleanMemoryValue {Value = Value || TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new BooleanMemoryValue {Value = Value && TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in boolean value with feature id {name}: only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "Reverse":
                    return new BooleanMemoryValue {Value = !Value};
                case "ToNumber":
                    return new IntegerMemoryValue {Value = ConvertToInteger()};
                case "ToString":
                    return new StringMemoryValue {Value = ConvertToString()};
                default:
                    throw new NotSupportedException($"Unable to get feature in boolean value: unsupported feature {target}");
            }
        }
    }
}