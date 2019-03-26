using UnityEngine;

namespace WADV.Extensions {
    public static class RectIntExtensions {
        /// <summary>
        /// 转换为Rect
        /// </summary>
        /// <param name="value">目标RectInt</param>
        /// <returns></returns>
        public static Rect ToRect(this RectInt value) {
            return new Rect(value.x, value.y, value.width, value.height);
        }

        public static RectInt MergeWith(this RectInt value, RectInt target) {
            var xMin = Mathf.Min(value.xMin, target.xMin);
            var yMin = Mathf.Min(value.yMin, target.yMin);
            var xMax = Mathf.Max(value.xMax, target.xMax);
            var yMax = Mathf.Max(value.yMax, target.yMax);
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}