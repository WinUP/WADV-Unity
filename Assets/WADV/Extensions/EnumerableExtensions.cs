using System.Collections.Generic;
using System.Linq;

namespace WADV.Extensions {
    public static class EnumerableExtensions {
        /// <summary>
        /// 将列表转换为内容-索引组
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <returns></returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> e) {
            return e.Select((item, index) => (item, index));
        }
    }
}