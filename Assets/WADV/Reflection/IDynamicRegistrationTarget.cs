namespace WADV.Reflection {
    /// <summary>
    /// 为类型提供手动的动态注册支持
    /// </summary>
    public interface IDynamicRegistrationTarget {
        /// <summary>
        /// 类型注册名
        /// </summary>
        string RegistrationName { get; }
        
        /// <summary>
        /// 注册优先级
        /// </summary>
        int RegistrationPriority { get; }
    }
}