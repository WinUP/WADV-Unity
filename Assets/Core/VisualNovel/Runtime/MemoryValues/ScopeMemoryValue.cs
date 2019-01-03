using System.Collections.Generic;
using Core.VisualNovel.Interoperation;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="ISerializableValue" />
    /// <summary>
    /// 表示一个作用域内存堆栈值
    /// </summary>
    public class ScopeMemoryValue : ISerializableValue, IStringConverter {
        /// <summary>
        /// 获取或设置偏移量
        /// </summary>
        public long Entrance { get; set; }
        
        /// <summary>
        /// 获取或设置目标脚本ID
        /// </summary>
        public string ScriptId { get; set; }
        
        /// <summary>
        /// 获取或设置父函数
        /// </summary>
        [CanBeNull]
        public ScopeMemoryValue ParentScope { get; set; }
        
        private readonly Dictionary<string, VariableMemoryValue> _variables = new Dictionary<string, VariableMemoryValue>();

        public byte[] Serialize() {
            
        }

        public void Deserialize(byte[] source) {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ISerializableValue Duplicate() {
            return new ScopeMemoryValue {Entrance = Entrance, ScriptId = ScriptId, ParentScope = ParentScope?.Duplicate() as ScopeMemoryValue};
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return $"OffsetMemoryValue {{ScriptId = {ScriptId}, Entrance = {Entrance}}}";
        }
    }
}