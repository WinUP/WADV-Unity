using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WADV.Extensions {
    public static class ValuesExtensions {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
            key = pair.Key;
            value = pair.Value;
        }
        
        /// <summary>
        /// 确定数值是否在指定范围内
        /// </summary>
        /// <param name="value">目标数值</param>
        /// <param name="min">范围最小值</param>
        /// <param name="max">范围最大值</param>
        /// <param name="compareType">范围区间类型</param>
        /// <returns></returns>
        public static bool InRange(this int value, int min, int max, IntervalType compareType = IntervalType.ClosedOpen) {
            if (value < min || value > max) return false;
            if (((byte) compareType & 0b0100) != 0 && value == min) return false;
            if (((byte) compareType & 0b0001) != 0 && value == max) return false;
            return true;
        }

        /// <summary>
        /// 确定数值是否在指定范围内
        /// </summary>
        /// <param name="value">目标数值</param>
        /// <param name="min">范围最小值</param>
        /// <param name="max">范围最大值</param>
        /// <param name="compareType">范围区间类型</param>
        /// <returns></returns>
        public static bool InRange(this float value, float min, float max, IntervalType compareType = IntervalType.ClosedOpen) {
            if (value < min || value > max) return false;
            if (((byte) compareType & 0b0100) != 0 && value.Equals(min)) return false;
            if (((byte) compareType & 0b0001) != 0 && value.Equals(max)) return false;
            return true;
        }

        /// <summary>
        /// 获取变换矩阵的平移分量
        /// </summary>
        /// <param name="value">当前矩阵</param>
        /// <returns></returns>
        public static Vector3 GetTranslation(this Matrix4x4 value) {
            return new Vector3(value.m03, value.m13, value.m23);
        }

        /// <summary>
        /// 设置变换矩阵的平移分量
        /// </summary>
        /// <param name="value">当前矩阵</param>
        /// <param name="translation">目标平移分量</param>
        public static void SetTranslation(this ref Matrix4x4 value, Vector3 translation) {
            value.m03 = translation.x;
            value.m13 = translation.y;
            value.m23 = translation.z;
        }

        /// <summary>
        /// 将目标矩形与矩阵相乘以获取包含结果区域的最小矩形
        /// </summary>
        /// <param name="value">当前矩阵</param>
        /// <param name="target">目标矩形</param>
        /// <returns></returns>
        public static Rect MultiplyRect(this Matrix4x4 value, Rect target) {
            var topLeft = value.MultiplyPoint(new Vector3(target.x, target.y, 0));
            var topRight = value.MultiplyPoint(new Vector3(target.xMax, target.y, 0));
            var bottomLeft = value.MultiplyPoint(new Vector3(target.x, target.yMax, 0));
            var bottomRight = value.MultiplyPoint(new Vector3(target.xMax, target.yMax, 0));
            var xMin = Mathf.Min(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x);
            var xMax = Mathf.Max(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x);
            var yMin = Mathf.Min(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y);
            var yMax = Mathf.Max(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// 转换为普通二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector2Int value) {
            return new Vector2(value.x, value.y);
        }

        /// <summary>
        /// 进位转换为普通二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2Int CeilToVector2Int(this Vector2 value) {
            return new Vector2Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));
        }
        
        /// <summary>
        /// 四舍五入转换为普通二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2Int RoundToVector2Int(this Vector2 value) {
            return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
        }
        
        /// <summary>
        /// 去尾转换为普通二维向量
        /// </summary>
        /// <param name="value">当前向量</param>
        /// <returns></returns>
        public static Vector2Int FloorToVector2Int(this Vector2 value) {
            return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
        }
        
        /// <summary>
        /// 转换为普通矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <returns></returns>
        public static Rect ToRect(this RectInt value) {
            return new Rect(value.x, value.y, value.width, value.height);
        }

        public static RectInt MaximizeToRectInt(this Rect value) {
            var x = value.x < 0 ? Mathf.FloorToInt(value.x) : Mathf.CeilToInt(value.x);
            var y = value.y < 0 ? Mathf.FloorToInt(value.y) : Mathf.CeilToInt(value.y);
            return new RectInt(x, y, Mathf.CeilToInt(value.width), Mathf.CeilToInt(value.height));
        }
        
        /// <summary>
        /// 进位转换为整坐标矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <returns></returns>
        public static RectInt CeilToRectInt(this Rect value) {
            return new RectInt(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y), Mathf.CeilToInt(value.width), Mathf.CeilToInt(value.height));
        }
        
        /// <summary>
        /// 四舍五入转换为整坐标矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <returns></returns>
        public static RectInt RoundToRectInt(this Rect value) {
            return new RectInt(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y), Mathf.RoundToInt(value.width), Mathf.RoundToInt(value.height));
        }
        
        /// <summary>
        /// 去尾转换为整坐标矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <returns></returns>
        public static RectInt FloorToRectInt(this Rect value) {
            return new RectInt(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y), Mathf.FloorToInt(value.width), Mathf.FloorToInt(value.height));
        }

        /// <summary>
        /// 移动矩形位置
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <param name="distance">移动距离</param>
        /// <returns></returns>
        public static Rect Move(this Rect value, Vector2 distance) {
            return new Rect(value.x + distance.x, value.y + distance.y, value.width, value.height);
        }

        /// <summary>
        /// 获取同时覆盖当前矩形和目标矩形的最小矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <param name="target">目标矩形</param>
        /// <returns></returns>
        public static RectInt MergeWith(this RectInt value, RectInt target) {
            var xMin = Mathf.Min(value.xMin, target.xMin);
            var yMin = Mathf.Min(value.yMin, target.yMin);
            var xMax = Mathf.Max(value.xMax, target.xMax);
            var yMax = Mathf.Max(value.yMax, target.yMax);
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }
        
        /// <summary>
        /// 获取同时覆盖当前矩形和目标矩形的最小矩形
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <param name="target">目标矩形</param>
        /// <returns></returns>
        public static Rect MergeWith(this Rect value, Rect target) {
            var xMin = Mathf.Min(value.xMin, target.xMin);
            var yMin = Mathf.Min(value.yMin, target.yMin);
            var xMax = Mathf.Max(value.xMax, target.xMax);
            var yMax = Mathf.Max(value.yMax, target.yMax);
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
        
        /// <summary>
        /// 获取当前矩形与目标矩形的重叠区域
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <param name="target">目标矩形</param>
        /// <returns></returns>
        public static RectInt OverlapWith(this RectInt value, RectInt target) {
            var left = value.xMin > target.xMin ? value.xMin : target.xMin;
            var right = value.xMax < target.xMax ? value.xMax : target.xMax;
            var top = value.yMax < target.yMax ? value.yMax : target.yMax;
            var bottom = value.yMin > target.yMin ? value.yMin : target.yMin;
            if (top < bottom || right < left) return new RectInt(0, 0, 0, 0);
            return new RectInt(left, bottom, right - left, top - bottom);
        }

        /// <summary>
        /// 获取当前矩形与目标矩形的重叠区域
        /// </summary>
        /// <param name="value">当前矩形</param>
        /// <param name="target">目标矩形</param>
        /// <returns></returns>
        public static Rect OverlapWith(this Rect value, Rect target) {
            var left = value.xMin > target.xMin ? value.xMin : target.xMin;
            var right = value.xMax < target.xMax ? value.xMax : target.xMax;
            var top = value.yMax < target.yMax ? value.yMax : target.yMax;
            var bottom = value.yMin > target.yMin ? value.yMin : target.yMin;
            if (top > bottom || right > left) return Rect.zero;
            return Rect.MinMaxRect(left, bottom, right, top);
        }
        
        /// <summary>
        /// 如果字符串以目标子串开头则删除该子串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="part">要删除的子串</param>
        /// <returns></returns>
        public static string RemoveStarts(this string value, string part) {
            return value == part
                ? ""
                : value.StartsWith(part)
                    ? value.Substring(part.Length)
                    : value;
        }
        
        /// <summary>
        /// 如果字符串以目标子串结尾则删除该子串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="part">要删除的子串</param>
        /// <returns></returns>
        public static string RemoveEnds(this string value, string part) {
            return value == part
                ? ""
                : value.EndsWith(part)
                    ? value.Substring(0, value.Length - part.Length)
                    : value;
        }

        /// <summary>
        /// 重复字符串若干次
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="times">重复次数</param>
        /// <returns></returns>
        public static string Repeat(this string value, int times) {
            if (times < 0) throw new NotSupportedException($"Unable to repeat string {value} with times {times}: times must not be negative number");
            if (times == 0) return "";
            if (times == 1) return value;
            var result = new StringBuilder(value, value.Length * times);
            for (var i = 0; ++i < times;) {
                result.Append(value);
            }
            return result.ToString();
        }

        /// <summary>
        /// 统一所有斜线至左斜线
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <returns></returns>
        public static string UnifySlash(this string value) {
            return value.Replace('\\', '/');
        }

        /// <summary>
        /// 统一所有换行符至\n
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <returns></returns>
        public static string UnifyLineBreak(this string value) {
            return value.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        /// <summary>
        /// 解析双大括号模板字符串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="parts">模板替换项</param>
        /// <returns></returns>
        public static string ParseTemplate(this string value, IEnumerable<KeyValuePair<string, string>> parts) {
            foreach (var (pattern, content) in parts) {
                value = value.Replace($"{{{pattern}}}", content);
            }
            return value;
        }
        
        /// <summary>
        /// 反向转义字符串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <returns></returns>
        public static string PackEscapeCharacters(this string value) {
            var result = new StringBuilder();
            var length = value.Length;
            for (var i = -1; ++i < length;) {
                switch (value[i]) {
                    case '\n':
                        result.Append(@"\n");
                        continue;
                    case '\t':
                        result.Append(@"\t");
                        continue;
                    default:
                        result.Append(value[i]);
                        continue;
                }
            }
            return result.ToString();
        }
        
        /// <summary>
        /// 解析字符串中所有转义字符
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <returns></returns>
        public static string ExecuteEscapeCharacters(this string value) {
            var result = new StringBuilder();
            var length = value.Length;
            for (var i = -1; ++i < length;) {
                if (value[i] == '\\') {
                    if (i == length - 1) {
                        result.Append('\\');
                    } else {
                        switch (value[i + 1]) {
                            case 'n':
                                result.Append('\n');
                                break;
                            case 't':
                                result.Append('\t');
                                break;
                            case 's':
                                result.Append(' ');
                                break;
                            default:
                                result.Append(value[i + 1]);
                                break;;
                        }
                        ++i;
                    }
                } else {
                    result.Append(value[i]);
                }
            }
            return result.ToString();
        }
    }
}