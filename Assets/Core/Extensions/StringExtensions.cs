using System.Text;

namespace Core.Extensions {
    public static class StringExtensions {
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