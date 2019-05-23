using System;
using UnityEngine;

namespace WADV.Extensions {
    public static class SwizzleExtensions {
        private static float PickItem(ref Color target, SwizzleItem item) {
            switch (item) {
                case SwizzleItem.X:
                    return target.r;
                case SwizzleItem.Y:
                    return target.g;
                case SwizzleItem.Z:
                    return target.b;
                case SwizzleItem.W:
                    return target.a;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, $"Cannot pick swizzle item from {target}: unrecognized item name");
            }
        }
        
        private static float PickItem(ref Vector2 target, SwizzleItem item) {
            switch (item) {
                case SwizzleItem.X:
                    return target.x;
                case SwizzleItem.Y:
                    return target.y;
                case SwizzleItem.Z:
                    return 0;
                case SwizzleItem.W:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, $"Cannot pick swizzle item from {target}: unrecognized item name");
            }
        }
        
        private static float PickItem(ref Vector3 target, SwizzleItem item) {
            switch (item) {
                case SwizzleItem.X:
                    return target.x;
                case SwizzleItem.Y:
                    return target.y;
                case SwizzleItem.Z:
                    return target.z;
                case SwizzleItem.W:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, $"Cannot pick swizzle item from {target}: unrecognized item name");
            }
        }
        
        private static float PickItem(ref Vector4 target, SwizzleItem item) {
            switch (item) {
                case SwizzleItem.X:
                    return target.x;
                case SwizzleItem.Y:
                    return target.y;
                case SwizzleItem.Z:
                    return target.z;
                case SwizzleItem.W:
                    return target.w;
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, $"Cannot pick swizzle item from {target}: unrecognized item name");
            }
        }
        
        /// <summary>
        /// 获取颜色除透明度外的分量
        /// </summary>
        /// <param name="value">目标颜色</param>
        /// <returns></returns>
        public static Vector3 Rgb(this Color value) {
            return new Vector3(value.r, value.g, value.b);
        }
        
        /// <summary>
        /// 获取颜色分量
        /// </summary>
        /// <param name="value">目标颜色</param>
        /// <returns></returns>
        public static Vector4 Rgba(this Color value) {
            return new Vector4(value.r, value.g, value.b, value.a);
        }

        /// <summary>
        /// 重组颜色分量
        /// </summary>
        /// <param name="value">目标颜色</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <returns></returns>
        public static Vector2 Swizzle(this Color value, SwizzleItem item1, SwizzleItem item2) {
            return new Vector2(PickItem(ref value, item1), PickItem(ref value, item2));
        }

        /// <summary>
        /// 重组颜色分量
        /// </summary>
        /// <param name="value">目标颜色</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <returns></returns>
        public static Vector3 Swizzle(this Color value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3) {
            return new Vector3(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3));
        }
        
        /// <summary>
        /// 重组颜色分量
        /// </summary>
        /// <param name="value">目标颜色</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <param name="item4">分量4</param>
        /// <returns></returns>
        public static Vector4 Swizzle(this Color value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3, SwizzleItem item4) {
            return new Vector4(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3), PickItem(ref value, item4));
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <returns></returns>
        public static Vector2 Swizzle(this Vector2 value, SwizzleItem item1, SwizzleItem item2) {
            return new Vector2(PickItem(ref value, item1), PickItem(ref value, item2));
        }

        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <returns></returns>
        public static Vector3 Swizzle(this Vector2 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3) {
            return new Vector3(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3));
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <param name="item4">分量4</param>
        /// <returns></returns>
        public static Vector4 Swizzle(this Vector2 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3, SwizzleItem item4) {
            return new Vector4(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3), PickItem(ref value, item4));
        }

        /// <summary>
        /// 转换为二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2 Xy(this Vector3 value) {
            return value;
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <returns></returns>
        public static Vector2 Swizzle(this Vector3 value, SwizzleItem item1, SwizzleItem item2) {
            return new Vector2(PickItem(ref value, item1), PickItem(ref value, item2));
        }

        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <returns></returns>
        public static Vector3 Swizzle(this Vector3 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3) {
            return new Vector3(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3));
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <param name="item4">分量4</param>
        /// <returns></returns>
        public static Vector4 Swizzle(this Vector3 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3, SwizzleItem item4) {
            return new Vector4(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3), PickItem(ref value, item4));
        }
        
        /// <summary>
        /// 转换为二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2 Xy(this Vector4 value) {
            return new Vector2(value.x, value.y);
        }
        
        /// <summary>
        /// 转换为三维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector3 Xyz(this Vector4 value) {
            return new Vector3(value.x, value.y, value.z);
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <returns></returns>
        public static Vector2 Swizzle(this Vector4 value, SwizzleItem item1, SwizzleItem item2) {
            return new Vector2(PickItem(ref value, item1), PickItem(ref value, item2));
        }

        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <returns></returns>
        public static Vector3 Swizzle(this Vector4 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3) {
            return new Vector3(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3));
        }
        
        /// <summary>
        /// 重组向量分量
        /// </summary>
        /// <param name="value">目标向量</param>
        /// <param name="item1">分量1</param>
        /// <param name="item2">分量2</param>
        /// <param name="item3">分量3</param>
        /// <param name="item4">分量4</param>
        /// <returns></returns>
        public static Vector4 Swizzle(this Vector4 value, SwizzleItem item1, SwizzleItem item2, SwizzleItem item3, SwizzleItem item4) {
            return new Vector4(PickItem(ref value, item1), PickItem(ref value, item2), PickItem(ref value, item3), PickItem(ref value, item4));
        }
    }
}