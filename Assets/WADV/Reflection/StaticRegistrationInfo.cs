using System;
using JetBrains.Annotations;

namespace WADV.Reflection {
    /// <summary>
    /// 为类型提供自动注册支持
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StaticRegistrationInfo : Attribute {
        /// <summary>
        /// 类型注册名
        /// </summary>
        [CanBeNull]
        public readonly string Name;

        /// <summary>
        /// 注册优先级
        /// </summary>
        public readonly int Priority;
        
        /// <summary>
        /// 创建一个空的自动注册描述
        /// </summary>
        public StaticRegistrationInfo() { }

        /// <summary>
        /// 创建一个自动注册描述
        /// </summary>
        /// <param name="name">注册名</param>
        public StaticRegistrationInfo([NotNull] string name) {
            if (!string.IsNullOrEmpty(name)) {
                Name = name;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个自动注册描述
        /// </summary>
        /// <param name="name">注册名</param>
        /// <param name="priority">注册优先级</param>
        public StaticRegistrationInfo([NotNull] string name, int priority) : this(name) {
            Priority = priority;
        }
    }
}