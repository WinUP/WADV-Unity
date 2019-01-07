using System;
using Core.VisualNovel.Interoperation;
using UnityEngine;

namespace Core.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个32位整数内存值</para>
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
    ///     <item><description>比较互操作器</description></item>
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>子元素/特性支持</description></listheader>
    ///     <item><description>ToBoolean</description></item>
    ///     <item><description>ToNegative</description></item>
    ///     <item><description>ToFloat</description></item>
    ///     <item><description>ToString</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class IntegerValue : SerializableValue, IBooleanConverter, IIntegerConverter, IFloatConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                                      ICompareOperator, IEqualOperator, IPickChildOperator {
        /// <summary>
        /// 获取或设置内存堆栈值
        /// </summary>
        public int Value { get; set; }
        
        /// <summary>
        /// 尝试将可序列化值解析为32为整数值
        /// </summary>
        /// <param name="value">目标内存值</param>
        /// <returns></returns>
        public static int TryParse(SerializableValue value) {
            switch (value) {
                case IIntegerConverter intTarget:
                    return intTarget.ConvertToInteger();
                case IFloatConverter floatTarget:
                    return Mathf.RoundToInt(floatTarget.ConvertToFloat());
                case IStringConverter stringTarget:
                    var stringValue = stringTarget.ConvertToString();
                    if (int.TryParse(stringValue, out var intValue)) return intValue;
                    if (float.TryParse(stringValue, out var floatValue)) return Mathf.RoundToInt(floatValue);
                    throw new NotSupportedException($"Unable to convert {stringValue} to integer: unsupported string format");
                case IBooleanConverter boolTarget:
                    return boolTarget.ConvertToBoolean() ? 1 : 0;
                default:
                    throw new NotSupportedException($"Unable to convert {value} to integer: unsupported format");
            }
        }

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new IntegerValue {Value = Value};
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return Value != 0;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return Value;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return Value;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return Value.ToString();
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ConvertToString();
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"IntegerValue {{Value = {ConvertToString()}}}";
        }

        /// <inheritdoc />
        public SerializableValue PickChild(SerializableValue name) {
            if (!(name is IStringConverter stringConverter))
                throw new NotSupportedException($"Unable to get feature in integer value with feature id {name}: only string feature name is accepted");
            var target = stringConverter.ConvertToString();
            switch (target) {
                case "ToBoolean":
                    return new BooleanValue {Value = ConvertToBoolean()};
                case "ToNegative":
                    return new IntegerValue {Value = -Value};
                case "ToFloat":
                    return new FloatValue {Value = ConvertToFloat()};
                case "ToString":
                    return new StringValue {Value = ConvertToString()};
                default:
                    throw new NotSupportedException($"Unable to get feature in integer value: unsupported feature {target}");
            }
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            try {
                return Value == TryParse(target);
            } catch {
                return false;
            }
        }

        /// <inheritdoc />
        public int CompareWith(SerializableValue target) {
            var value = Value - FloatValue.TryParse(target);
            return value.Equals(0.0F) ? 0 : value < 0 ? -1 : 1;
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return new IntegerValue {Value = Value + TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return new IntegerValue {Value = Value - TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new IntegerValue {Value = Value * TryParse(target)};
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return new IntegerValue {Value = Value / TryParse(target)};
        }
    }
}