using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions {
    public static class ListExtensions {
        /// <summary>
        /// 确定列表是否一指定元素结束
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="value">目标元素</param>
        /// <returns></returns>
        public static bool EndWith<T>(this List<T> e, T value) {
            return e.Last().Equals(value);
        }

        /// <summary>
        /// 将列表转换为内容-索引组
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <returns></returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> e) {
            return e.Select((item, index) => (item, index));
        }

        /// <summary>
        /// 倒序批量删除最后一个符合的元素
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="values">要删除的元素组</param>
        public static void Pop<T>(this List<T> e, params T[] values) {
            switch (values.Length) {
                case 0:
                    e.RemoveAt(e.Count - 1);
                    break;
                case 1:
                    e.RemoveAt(e.LastIndexOf(values[0]));
                    break;
                default:
                    foreach (var i in values.Reverse()) {
                        e.RemoveAt(e.LastIndexOf(i));
                    }
                    break;
            }
        }

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="values">要添加的元素组</param>
        public static void Push<T>(this List<T> e, params T[] values) {
            switch (values.Length) {
                case 0:
                    break;
                case 1:
                    e.Add(values[0]);
                    break;
                default:
                    e.AddRange(values.Reverse());
                    break;
            }
        }

        /// <summary>
        /// 确定列表尾部是否有不重复的唯一序列编组
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="values">序列编组</param>
        /// <returns></returns>
        public static bool Contains<T>(this List<T> e, params T[] values) {
            if (values.Length == 0) {
                return e.Count > 0;
            }
            var index = -1;
            foreach (var v in values) {
                var subIndex = e.LastIndexOf(v);
                if (subIndex < index) {
                    return false;
                }
                index = subIndex;
            }
            return true;
        }

        /// <summary>
        /// 确定序列编组是否都不存在与列表中
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="values">序列编组</param>
        /// <returns></returns>
        public static bool NotContains<T>(this List<T> e, params T[] values) {
            return values.All(r => !e.Contains(r));
        }
    }
}
