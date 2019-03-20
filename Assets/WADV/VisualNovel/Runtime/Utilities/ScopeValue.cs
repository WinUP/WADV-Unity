using System;
using System.Collections.Generic;
using System.Linq;
using WADV.Extensions;
using WADV.VisualNovel.Interoperation;
using JetBrains.Annotations;
using WADV.Translation;

namespace WADV.VisualNovel.Runtime.Utilities {
    /// <inheritdoc cref="SerializableValue" />
    /// <inheritdoc cref="IStringConverter" />
    /// <summary>
    /// <para>表示一个作用域内存值</para>
    /// <list type="bullet">
    ///     <listheader><description>复制方式</description></listheader>
    ///     <item><description>对偏移值使用值复制</description></item>
    ///     <item><description>对其他部分使用引用复制</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>自有数据字节量</description></listheader>
    ///     <item><description>8 字节</description></item>
    ///     <item><description>1 字符串</description></item>
    ///     <item><description>2 ObjectId</description></item>
    ///     <item><description>1 引用关联的WADV.VisualNovel.Runtime.Utilities.ScopeValue</description></item>
    ///     <item><description>1 引用关联的System.Collections.Generic.Dictionary&lt;string, WADV.VisualNovel.Runtime.Utilities.ReferenceValue&gt;</description></item>
    /// </list>
    /// <list type="bullet">
    ///     <listheader><description>类型转换支持</description></listheader>
    ///     <item><description>字符串转换器</description></item>
    /// </list>
    /// </summary>
    [Serializable]
    public class ScopeValue : SerializableValue, IStringConverter {
        /// <summary>
        /// 获取或设置偏移量
        /// </summary>
        public long entrance;

        /// <summary>
        /// 获取或设置目标脚本ID
        /// </summary>
        public string scriptId;

        /// <summary>
        /// 获取或设置父函数
        /// </summary>
        [CanBeNull]
        public ScopeValue parentScope;
        
        /// <summary>
        /// 获取局部变量列表
        /// </summary>
        public Dictionary<string, ReferenceValue> LocalVariables { get; private set; } = new Dictionary<string, ReferenceValue>();
        
        public override SerializableValue Duplicate() {
            return new ScopeValue {entrance = entrance, scriptId = scriptId, parentScope = parentScope, LocalVariables = LocalVariables.Duplicate()};
        }
        
        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return $"ScopeValue {{ScriptId = {scriptId}, Entrance = {entrance}}}";
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
            return includeParent ? parentScope?.FindVariableAndScope(name, true, mode) : null;
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
            if (variable == null || variable.ReferenceTarget.GetType() != typeof(T)) return null;
            return (T) variable.ReferenceTarget;
        }
    }
}