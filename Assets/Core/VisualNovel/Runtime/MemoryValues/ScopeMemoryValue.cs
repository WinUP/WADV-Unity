using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Interoperation;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime.MemoryValues {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// 表示一个作用域内存堆栈值
    /// </summary>
    [Serializable]
    public class ScopeMemoryValue : SerializableValue, IStringConverter {
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
        
        /// <summary>
        /// 获取局部变量列表
        /// </summary>
        public Dictionary<string, VariableMemoryValue> LocalVariables { get; private set; } = new Dictionary<string, VariableMemoryValue>();
        
        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new ScopeMemoryValue {Entrance = Entrance, ScriptId = ScriptId, ParentScope = ParentScope, LocalVariables = LocalVariables.Duplicate()};
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return $"OffsetMemoryValue {{ScriptId = {ScriptId}, Entrance = {Entrance}}}";
        }

        /// <summary>
        /// 按照名称查找变量
        /// </summary>
        /// <param name="name">目标变量名</param>
        /// <param name="includeParent">是否递归向上查找父作用域（如果有）</param>
        /// <param name="mode">搜索模式</param>
        /// <returns></returns>
        [CanBeNull]
        public VariableMemoryValue FindVariable(string name, bool includeParent, VariableSearchMode mode) {
            if (string.IsNullOrEmpty(name)) return null;
            IEnumerable<KeyValuePair<string, VariableMemoryValue>> items;
            switch (mode) {
                case VariableSearchMode.All:
                    items = LocalVariables;
                    break;
                case VariableSearchMode.OnlyConstant:
                    items = LocalVariables.Where(e => e.Value.IsConstant);
                    break;
                case VariableSearchMode.OnlyNonConstant:
                    items = LocalVariables.Where(e => !e.Value.IsConstant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, $"Unknown VariableSearchMode {mode}");
            }
            var result = items.Where(e => e.Key == name).ToList();
            if (result.Any()) return result.First().Value;
            return includeParent ? ParentScope?.FindVariable(name, true, mode) : null;
        }
    }
}