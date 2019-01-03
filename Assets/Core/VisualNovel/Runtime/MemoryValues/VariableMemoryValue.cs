using System;
using Core.VisualNovel.Interoperation;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime.MemoryValues {
    public class VariableMemoryValue : ISerializableValue {
        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        [NotNull]
        public ISerializableValue Value { get => _value;
            set {
                if (value == _value) return;
                if (IsConstant) throw new NotSupportedException("Cannot assign value to constant variable");
                _value = value ?? throw new NullReferenceException("Cannot assign null to not-null variable");
            }
        }
        
        /// <summary>
        /// 获取或设置变量是否为常量
        /// </summary>
        public bool IsConstant { get; set; }

        private ISerializableValue _value;
        
        /// <inheritdoc />
        public byte[] Serialize() {
            
        }

        /// <inheritdoc />
        public void Deserialize(byte[] source) {
            
        }

        /// <inheritdoc />
        public ISerializableValue Duplicate() {
            return new VariableMemoryValue {IsConstant = IsConstant, Value = Value.Duplicate()};
        }
    }
}