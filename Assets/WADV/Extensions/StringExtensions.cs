using System;
using System.Collections.Generic;
using System.Text;

namespace WADV.Extensions {
    public static class StringExtensions {
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
            if (times < 0) throw new NotSupportedException("Unable to ");
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
        /// 统一所有换行符
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <returns></returns>
        public static string UnifyLineBreak(this string value) {
            return value.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        /// <summary>
        /// 解析模板字符串
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