using System;
using System.Text;

namespace Core.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// 删除字符串中第一次出现的子串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="part">要删除的子串</param>
        /// <returns></returns>
        public static string RemoveWhenStartsWith(this string value, string part) {
            var index = value.IndexOf(part, StringComparison.Ordinal);
            if (index < 0) return value;
            if (index + part.Length >= value.Length) {
                return value.Substring(0, index);
            }
            return index < 0 ? value : value.Substring(part.Length);
        }
        
        /// <summary>
        /// 删除字符串中最后一次出现的子串
        /// </summary>
        /// <param name="value">目标字符串</param>
        /// <param name="part">要删除的子串</param>
        /// <returns></returns>
        public static string RemoveWhenEndsWith(this string value, string part) {
            var index = value.LastIndexOf(part, StringComparison.Ordinal);
            return index < 0
                ? value
                : index + part.Length > value.Length
                    ? value.Remove(index, part.Length)
                    : value.Substring(0, index);
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
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UnifySlash(this string value) {
            return value.Replace('\\', '/');
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
                            case ';':
                                result.Append(';');
                                break;
                            case '"':
                                result.Append('"');
                                break;
                            case '\'':
                                result.Append('\'');
                                break;
                            case '0':
                                result.Append('0');
                                break;
                            case '1':
                                result.Append('1');
                                break;
                            case '2':
                                result.Append('2');
                                break;
                            case '3':
                                result.Append('3');
                                break;
                            case '4':
                                result.Append('4');
                                break;
                            case '5':
                                result.Append('5');
                                break;
                            case '6':
                                result.Append('6');
                                break;
                            case '7':
                                result.Append('7');
                                break;
                            case '8':
                                result.Append('8');
                                break;
                            case '9':
                                result.Append('9');
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