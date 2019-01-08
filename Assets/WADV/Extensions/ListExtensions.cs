using System.Collections.Generic;

namespace WADV.Extensions {
    public static class ListExtensions {
        public static List<T> Duplicate<T>(this List<T> e) {
            var result = new List<T>();
            result.AddRange(e);
            return result;
        }
    }
}