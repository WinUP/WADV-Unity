using System;
using WADV.VisualNovel.Interoperation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个空内存值</para>
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
    ///     <item><description>真值比较互操作器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class NullValue : SerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator,
                             IEqualOperator {
        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new NullValue();
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return false;
        }

        /// <inheritdoc />
        public bool ConvertToBoolean(string language) {
            return ConvertToBoolean();
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return 0.0F;
        }

        /// <inheritdoc />
        public float ConvertToFloat(string language) {
            return ConvertToFloat();
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return 0;
        }

        /// <inheritdoc />
        public int ConvertToInteger(string language) {
            return ConvertToInteger();
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return "";
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ConvertToString();
        }

        /// <inheritdoc />
        public override string ToString() {
            return ConvertToString();
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target) {
            return target is NullValue;
        }

        /// <inheritdoc />
        public bool EqualsWith(SerializableValue target, string language) {
            return EqualsWith(target);
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target) {
            return target.Duplicate();
        }

        /// <inheritdoc />
        public SerializableValue AddWith(SerializableValue target, string language) {
            return AddWith(target);
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target) {
            return target is NullValue ? new NullValue() : throw new NotSupportedException("Unable to subtract null with any other value except null");
        }

        /// <inheritdoc />
        public SerializableValue SubtractWith(SerializableValue target, string language) {
            return SubtractWith(target);
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target) {
            return new NullValue();
        }

        /// <inheritdoc />
        public SerializableValue MultiplyWith(SerializableValue target, string language) {
            return MultiplyWith(target);
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target) {
            return target is NullValue ? new NullValue() : throw new NotSupportedException("Unable to divide null with any other value except null");
        }

        /// <inheritdoc />
        public SerializableValue DivideWith(SerializableValue target, string language) {
            return DivideWith(target);
        }
    }
}