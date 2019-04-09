using System;
using System.Collections.Generic;
using System.Linq;

namespace WADV.Extensions {
    public static class CollectionExtensions {
        /// <summary>
        /// 尝试删除目标元素
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <param name="value">要删除的元素</param>
        /// <returns></returns>
        public static bool TryRemove<T>(this ICollection<T> e, T value) {
            if (!e.Contains(value)) return false;
            e.Remove(value);
            return true;
        }
        
        /// <summary>
        /// 获得当前字典的浅拷贝副本
        /// </summary>
        /// <param name="e">目标字典</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Duplicate<TKey, TValue>(this Dictionary<TKey, TValue> e) {
            var result = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in e) {
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 尝试删除目标元素
        /// </summary>
        /// <param name="e">目标字典</param>
        /// <param name="target">要删除的元素</param>
        /// <returns></returns>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> e, TKey target) {
            if (!e.ContainsKey(target)) return false;
            e.Remove(target);
            return true;
        }

        /// <summary>
        /// 删除所有符合条件的元素
        /// </summary>
        /// <param name="e">目标字典</param>
        /// <param name="prediction">判断函数</param>
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> e, Func<KeyValuePair<TKey, TValue>, bool> prediction) {
            foreach (var key in from pair in e where prediction(pair) select pair.Key) {
                e.Remove(key);
            }
        }
        
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> e, IEnumerable<TKey> keys) {
            foreach (var key in keys) {
                e.Remove(key);
            }
        }
        
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> e, params TKey[] keys) {
            foreach (var key in keys) {
                e.Remove(key);
            }
        }
        
        /// <summary>
        /// 将列表转换为内容-索引组
        /// </summary>
        /// <param name="e">目标列表</param>
        /// <returns></returns>
        public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> e) {
            return e.Select((item, index) => (item, index));
        }
    }
}