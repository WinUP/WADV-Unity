using System;
using System.Linq;

namespace WADV.Extensions {
    public static class TypeExtensions {
        public static bool HasInterface(this Type e, Type target) {
            return target.IsInterface && e.GetInterfaces().Contains(target);
        }
        
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