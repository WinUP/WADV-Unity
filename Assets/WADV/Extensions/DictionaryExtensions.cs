using System.Collections.Generic;

namespace WADV.Extensions {
    public static class DictionaryExtensions {
        public static Dictionary<TKey, TValue> Duplicate<TKey, TValue>(this Dictionary<TKey, TValue> e) {
            var result = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in e) {
                result.Add(key, value);
            }
            return result;
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> e, TKey target) {
            if (!e.ContainsKey(target)) return false;
            e.Remove(target);
            return true;
        }
    }
}