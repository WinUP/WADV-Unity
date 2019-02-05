using System;
using JetBrains.Annotations;

namespace WADV.Reflection {
    /// <summary>
    /// 为类型提供自动注册支持
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UseStaticRegistration : Attribute {
        /// <summary>
        /// 类型注册名
        /// </summary>
        [CanBeNull]
        public string Name { get; }
        
        /// <summary>
        /// 注册优先级
        /// </summary>
        public int Priority { get; }
        
        public UseStaticRegistration() { }

        public UseStaticRegistration([NotNull] string name) {
            if (!string.IsNullOrEmpty(name)) {
                Name = name;
            }
        }

        public UseStaticRegistration([NotNull] string name, int priority) : this(name) {
            Priority = priority;
        }
    }
}