using System;
using System.Collections.Generic;
using System.Linq;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using JetBrains.Annotations;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <summary>
    /// <para>表示一个作用域内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>互操作支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ScopeValue : SerializableValue, IStringConverter {
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
        public ScopeValue ParentScope { get; set; }
        
        /// <summary>
        /// 获取局部变量列表
        /// </summary>
        public Dictionary<string, ReferenceValue> LocalVariables { get; private set; } = new Dictionary<string, ReferenceValue>();
        
        /// <inheritdoc />
        public override SerializableValue Duplicate() {
            return new ScopeValue {Entrance = Entrance, ScriptId = ScriptId, ParentScope = ParentScope, LocalVariables = LocalVariables.Duplicate()};
        }

        /// <inheritdoc />
        public string ConvertToString() {
            return $"ScopeValue {{ScriptId = {ScriptId}, Entrance = {Entrance}}}";
        }
        
        /// <inheritdoc />
        public string ConvertToString(string language) {
            return ConvertToString();
        }

        public override string ToString() {
            return ConvertToString();
        }

        /// <summary>
        /// 按照名称查找变量及其所在的作用域
        /// </summary>
        /// <param name="name">目标变量名</param>
        /// <param name="includeParent">是否递归向上查找父作用域（如果有）</param>
        /// <param name="mode">搜索模式</param>
        /// <returns></returns>
        [CanBeNull]
        public (ReferenceValue Target, ScopeValue Scope)? FindVariableAndScope(string name, bool includeParent, VariableSearchMode mode) {
            if (string.IsNullOrEmpty(name)) return null;
            IEnumerable<KeyValuePair<string, ReferenceValue>> items;
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
            if (result.Any()) return (result.First().Value, this);
            return includeParent ? ParentScope?.FindVariableAndScope(name, true, mode) : null;
        }

        /// <summary>
        /// 按照名称查找变量
        /// </summary>
        /// <param name="name">目标变量名</param>
        /// <param name="includeParent">是否递归向上查找父作用域（如果有）</param>
        /// <param name="mode">搜索模式</param>
        /// <returns></returns>
        [CanBeNull]
        public ReferenceValue FindVariable(string name, bool includeParent, VariableSearchMode mode) {
            return FindVariableAndScope(name, includeParent, mode)?.Target;
        }

        /// <summary>
        /// 按照名称查找具有指定类型值的变量并返回其值
        /// </summary>
        /// <param name="name">目标变量名</param>
        /// <param name="includeParent">是否递归向上查找父作用域（如果有）</param>
        /// <param name="mode">搜索模式</param>
        /// <returns></returns>
        [CanBeNull]
        public T FindVariableValue<T>(string name, bool includeParent, VariableSearchMode mode) where T : SerializableValue {
            var variable = FindVariable(name, includeParent, mode);
            if (variable == null || variable.Value.GetType() != typeof(T)) return null;
            return (T) variable.Value;
        }
    }
}