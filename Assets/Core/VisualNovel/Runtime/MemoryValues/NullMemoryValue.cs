using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="ISerializableValue" />
    /// <summary>
    /// 表示一个空内存堆栈值
    /// </summary>
    public class NullMemoryValue : ISerializableValue, IBooleanConverter, IFloatConverter, IIntegerConverter, IStringConverter, IAddOperator, ISubtractOperator, IMultiplyOperator, IDivideOperator {
        /// <inheritdoc />
        public byte[] Serialize() {
            return new byte[0];
        }

        /// <inheritdoc />
        public void Deserialize(byte[] source) { }

        /// <inheritdoc />
        public ISerializableValue Duplicate() {
            return new NullMemoryValue();
        }

        /// <inheritdoc />
        public bool ConvertToBoolean() {
            return false;
        }

        /// <inheritdoc />
        public float ConvertToFloat() {
            return 0.0F;
        }

        /// <inheritdoc />
        public int ConvertToInteger() {
            return 0;
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return "";
        }

        /// <inheritdoc />
        public ISerializableValue AddWith(ISerializableValue target) {
            return target.Duplicate();
        }

        /// <inheritdoc />
        public ISerializableValue SubtractWith(ISerializableValue target) {
            return target is NullMemoryValue ? new NullMemoryValue() : throw new NotSupportedException("Unable to subtract null with any other value except null");
        }

        /// <inheritdoc />
        public ISerializableValue MultiplyWith(ISerializableValue target) {
            return new NullMemoryValue();
        }

        /// <inheritdoc />
        public ISerializableValue DivideWith(ISerializableValue target) {
            return target is NullMemoryValue ? new NullMemoryValue() : throw new NotSupportedException("Unable to divide null with any other value except null");
        }
    }
}