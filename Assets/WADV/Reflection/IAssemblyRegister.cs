using System;
using JetBrains.Annotations;

namespace WADV.Reflection {
    /// <summary>
    /// 自动注册处理器
    /// </summary>
    public interface IAssemblyRegister {
        /// <summary>
        /// 处理一个类型
        /// </summary>
        /// <param name="target">目标类型</param>
        /// <param name="info">类型的静态注册信息</param>
        void RegisterType(Type target, [CanBeNull] StaticRegistrationInfoAttribute info);
    }
}