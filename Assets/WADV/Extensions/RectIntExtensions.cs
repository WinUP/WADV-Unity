using UnityEngine;

namespace WADV.Extensions {
    public static class RectIntExtensions {
        public static Rect ToRect(this RectInt value) {
            return new Rect(value.x, value.y, value.width, value.height);
        }
    }
}