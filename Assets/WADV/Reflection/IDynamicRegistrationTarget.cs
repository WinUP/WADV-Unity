namespace WADV.Reflection {
    /// <summary>
    /// 为类型提供手动注册支持
    /// </summary>
    public interface IDynamicRegistrationTarget {
        /// <summary>
        /// 类型注册名
        /// </summary>
        StaticRegistrationInfoAttribute[] RegistrationInfo { get; }
    }
}