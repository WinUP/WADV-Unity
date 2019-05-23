using System;
using System.Linq;

namespace WADV.Extensions {
    public static class ReflectionExtensions {
        /// <summary>
        /// 检查类型是否实现了目标接口
        /// </summary>
        /// <param name="e">目标类型</param>
        /// <param name="target">要检查的接口</param>
        /// <returns></returns>
        public static bool HasInterface(this Type e, Type target) {
            return target.IsInterface && e.GetInterfaces().Contains(target);
        }
        
        /// <summary>
        /// 检查类型继承关系上是否包含指定类型
        /// </summary>
        /// <param name="e">目标类型</param>
        /// <param name="target">要检查的类型</param>
        /// <returns></returns>
        public static bool HasBase(this Type e, Type target) {
            var baseType = e;
            do {
                baseType = baseType.BaseType;
                if (baseType != null && baseType == target) return true;
            } while (baseType != null);
            return false;
        }
    }
}