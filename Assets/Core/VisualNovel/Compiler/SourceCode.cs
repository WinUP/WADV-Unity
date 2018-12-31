using System;
using System.Linq;

namespace Core.VisualNovel.Compiler {
    /// <summary>
    /// 表示一个VNS脚本文件
    /// <para>VNS脚本辅助类提供大多数与字符串类相似的操作，不过均以当前偏移值为0索引处理</para>
    /// </summary>
    public class SourceCode {
        /// <summary>
        /// 脚本内容字符串
        /// </summary>
        public string Content { get; }
        /// <summary>
        /// 当前据脚本开头的偏移值
        /// </summary>
        public int Offset {
            get => _offset;
            set {
                if (Length == 0) {
                    return;
                }
                value = value > Length - 1 ? Length - 1 : value;
                value = value < 0 ? 0 : value;
                if (value == Offset) return;
                _offset = value;
                RecalculateCharacters();
            }
        }
        /// <summary>
        /// 脚本内容字符串总长度
        /// </summary>
        public int Length { get; }
        /// <summary>
        /// 当前操作的字符
        /// </summary>
        public char Current { get; private set; }
        /// <summary>
        /// 即将操作的字符
        /// </summary>
        public char Next { get; private set; }
        /// <summary>
        /// 最近操作过的字符
        /// </summary>
        public char Previous { get; private set; }
        /// <summary>
        /// 确定游标是否还未抵达脚本内容末尾
        /// </summary>
        public bool HasNext => Next != '\0';

        private int _offset = -1;

        /// <summary>
        /// 声明脚本内容
        /// </summary>
        /// <param name="content">脚本原始内容</param>
        public SourceCode(string content) {
            if (content.Contains('\t')) {
                throw new ArgumentException("Unable to create code content: WADV VNS string/file cannot includes \\t");
            }
            content = content.Replace("\r\n", "\n").Replace('\r', '\n');
            Content = content.Last() == '\n' ? content : content + '\n';
            Length = Content.Length;
            Offset = 0;
        }

        /// <summary>
        /// 获取指定偏移值处的字符
        /// </summary>
        /// <param name="i">目标偏移值</param>
        public char this[int i] {
            get {
                i += Offset;
                if (i > Length - 1 || i < 0) {
                    return '\0';
                }
                return Content[i];
            }
        }
        
        /// <summary>
        /// 复制脚本内容的一部分
        /// </summary>
        /// <param name="start">复制起点</param>
        /// <param name="stop">复制终点（终点不会被包括在结果中）</param>
        /// <returns></returns>
        public string CopyContent(int start, int stop) {
            return Content.Substring(start + Offset, stop - start);
        }

        /// <summary>
        /// 从当前字符开始复制脚本内容的一部分
        /// </summary>
        /// <param name="stop">复制终点（除终点不会被包括在结果中）</param>
        /// <returns></returns>
        public string CopyContent(int stop) {
            return CopyContent(0, stop);
        }

        /// <summary>
        /// 移动到下一个字符处
        /// </summary>
        /// <returns></returns>
        public bool MoveToNext() {
            if (!HasNext) {
                return false;
            }
            ++Offset;
            return true;
        }

        /// <summary>
        /// 移动到下一个换行符处
        /// </summary>
        /// <returns></returns>
        public bool MoveToNextLineBreak() {
            var index = Content.IndexOf('\n', Offset + 1);
            if (index < Offset) {
                return false;
            }
            Offset = index;
            return true;
        }

        /// <summary>
        /// 移动指定偏移值
        /// </summary>
        /// <param name="offset">要移动的偏移值</param>
        public void Move(int offset) {
            Offset += offset;
        }

        /// <summary>
        /// 获取指定字符距当前字符的偏移值
        /// </summary>
        /// <param name="values">目标字符集合</param>
        /// <returns></returns>
        public int IndexOf(params char[] values) {
            return IndexOf((contentChar, valueChar, contentIndex, valueIndex) => contentChar == valueChar, values);
        }

        /// <summary>
        /// 获取指定字符串距当前字符的偏移值
        /// </summary>
        /// <param name="values">目标字符串集合</param>
        /// <returns></returns>
        public int IndexOf(params string[] values) {
            return IndexOf((contentChar, valueChar, contentIndex, valueIndex) => contentChar == valueChar, values);
        }
        
        /// <summary>
        /// 获取指定字符距当前字符的偏移值并跳过被表示转义的字符
        /// </summary>
        /// <param name="values">目标字符集合</param>
        /// <returns></returns>
        public int IndexOfWithEscapeRecognize(params char[] values) {
            return IndexOf((contentChar, valueChar, contentIndex, valueIndex) => contentChar == valueChar && (contentIndex == 0 || Content[contentIndex - 1] != '\\'), values);
        }

        /// <summary>
        /// 获取指定字符串距当前字符的偏移值并跳过串中被表示转义的字符
        /// </summary>
        /// <param name="values">目标字符串集合</param>
        /// <returns></returns>
        public int IndexOfWithEscapeRecognize(params string[] values) {
            return IndexOf((contentChar, valueChar, contentIndex, valueIndex) => contentChar == valueChar && (contentIndex == 0 || Content[contentIndex - 1] != '\\'), values);
        }

        /// <summary>
        /// 查找下一个不属于指定集合内字符据当前字符的偏移值
        /// </summary>
        /// <param name="values">目标集合</param>
        /// <returns></returns>
        public int IndexOfUntilNot(params char[] values) {
            if (Next == '\0') {
                return values.Any(e => e == Current) ? -1 : 0;
            }
            var index = Offset;
            while (index < Length && values.Any(e => e == Content[index])) {
                ++index;
            }
            return index == Length ? -1 : index - Offset;
        }

        /// <summary>
        /// 判断当前操作的字符及后续字符是否以给定字符串开始
        /// </summary>
        /// <param name="values">所有可能的开始字符串</param>
        /// <returns></returns>
        public bool StartsWith(params string[] values) {
            foreach (var e in values) {
                if (e.Length == 0 || e.Length == 1 && e[0] == Current || e.Length == 2 && e[0] == Current && e[1] == Next) {
                    return true;
                }
                if (e.Length <= 2) continue;
                var suitable = true;
                for (var i = -1; ++i < e.Length;) {
                    if (i + Offset < Content.Length && Content[i + Offset] == e[i]) continue;
                    suitable = false;
                    break;
                }
                if (suitable) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 重置偏移值
        /// </summary>
        public void Reset() {
            Offset = -1;
            RecalculateCharacters();
        }
        
        private int IndexOf(Func<char, char, int, int, bool> comparator, params char[] values) {
            if (Next == '\0') {
                return values.Any(e => e == Current) ? 0 : -1;
            }
            for (var i = Offset - 1; ++i < Length;) {
                for (var j = -1; ++j < values.Length;) {
                    if (comparator(Content[i], values[j], i, j)) {
                        return i - Offset;
                    }
                }
            }
            return -1;
        }
        
        private int IndexOf(Func<char, char, int, int, bool> comparator, params string[] values) {
            if (Next == '\0') {
                return values.Any(e => e.Length == 1 && e[0] == Current) ? 0 : -1;
            }
            if (values.Any(e => e == "")) {
                return 0;
            }
            if (!values.All(e => e.Length <= Length)) {
                return -1;
            }
            // 以下部分基于Sunday字符串检索（的一种逐步推进型复合搜素方案），解释起来很麻烦，有需要请自行上网查
            // 目前的词法分析用不到这么精细的算法，不过无所谓
            var valuesOffset = new int[values.Length];
            var sourceOffset = Enumerable.Repeat(Offset, values.Length).ToArray();
            var selectedValueIndex = 0;
            while (sourceOffset[selectedValueIndex] < Length && valuesOffset[selectedValueIndex] < values[selectedValueIndex].Length) {
                var value = values[selectedValueIndex];
                if (comparator(Content[sourceOffset[selectedValueIndex]], value[valuesOffset[selectedValueIndex]], sourceOffset[selectedValueIndex], valuesOffset[selectedValueIndex])) {
                    ++valuesOffset[selectedValueIndex];
                    ++sourceOffset[selectedValueIndex];
                    if (valuesOffset[selectedValueIndex] == value.Length) {
                        return sourceOffset[selectedValueIndex] - value.Length - Offset;
                    }
                } else {
                    var sourceNext = sourceOffset[selectedValueIndex] + value.Length;
                    if (sourceNext >= Length) {
                        sourceOffset[selectedValueIndex] = sourceNext;
                    } else {
                        var valueNext = values[selectedValueIndex].LastIndexOf(Content[sourceNext]);
                        if (valueNext < 0) {
                            valueNext = 0;
                        }
                        sourceOffset[selectedValueIndex] += value.Length - valueNext;
                        valuesOffset[selectedValueIndex] = 0;
                    }
                }
                for (var i = -1; ++i < sourceOffset.Length;) {
                    if (sourceOffset[i] < sourceOffset[selectedValueIndex]) {
                        selectedValueIndex = i;
                    }
                }
            }
            return -1;
        }

        private void RecalculateCharacters() {
            Current = Offset < 0 || Offset >= Length - 1 ? '\0' : Content[Offset];
            Next = Offset >= Length - 1 ? '\0' : Content[Offset + 1];
            Previous = Offset <= 0 ? '\0' : Content[Offset - 1];
        }
    }
}