using System.Collections.Generic;

namespace Core.Extensions {
    public static class StackExtensions {
        public static Stack<T> Duplicate<T>(this Stack<T> e) {
            return new Stack<T>(new Stack<T>(e));
        }
    }
}