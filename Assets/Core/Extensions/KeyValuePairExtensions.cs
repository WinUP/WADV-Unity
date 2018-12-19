using System.Collections.Generic;

namespace Core.Extensions {
    public static class KeyValuePairExtensions {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
            key = pair.Key;
            value = pair.Value;
        }
    }
}