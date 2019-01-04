using System;
using Core.VisualNovel.Interoperation;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个变量内存堆栈值
    /// </summary>
    /// <remarks>将变量作为SerializableValue确实解决了作用域的存档问题，不过也产生了一个额外结果：汇编层面上将一个变量的值设为另一个变量（通俗意义上的取址操作）似乎变得可行了</remarks>
    [Serializable]
    public class VariableMemoryValue : SerializableValue, IStringConverter {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        public SerializableValue Value { get => _value;
            set {
                if (value == _value) return;
                if (IsConstant) throw new NotSupportedException("Cannot assign value to constant variable");
                _value = value ?? new NullMemoryValue();
            }
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private SerializableValue _value;

        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new VariableMemoryValue {IsConstant = IsConstant, Value = Value.Duplicate()};
        }

        public string ConvertToString() {
            return Value is IStringConverter stringConverter ? stringConverter.ConvertToString() : $"VariableMemoryValue {{ Value = {Value} }}";
        }
    }
}