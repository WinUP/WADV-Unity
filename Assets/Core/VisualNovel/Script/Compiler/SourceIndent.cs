using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 表示脚本缩进
    /// </summary>
    public class SourceIndent {
        /// <summary>
        /// 当前缩进总长度
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// 缩进列表
        /// </summary>
        private readonly Dictionary<int, int> _indents = new Dictionary<int, int>();

        /// <summary>
        /// 添加一个缩进
        /// </summary>
        /// <param name="length">缩进距离</param>
        public void Push(int length) {
            Length += length;
            _indents.Add(Length, length);
        }

        /// <summary>
        /// 找到距离目标数值最近的合法缩进值
        /// </summary>
        /// <param name="target">目标数值</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">目标数值超过当前缩进总长度，无法找到合适的缩进值</exception>
        public int FindApproachingIndent(int target) {
            var indents = _indents.Keys.ToList();
            for (var i = -1; ++i < indents.Count;) {
                if (indents[i] == target) {
                    return target;
                } else if (indents[i] > target) {
                    if (i == 0) {
                        return indents[0];
                    } else if (indents[i] - target > indents[i - 1] - target) {
                        return indents[i - 1];
                    } else {
                        return indents[i];
                    }
                }
            }
            throw new ArgumentOutOfRangeException(
                $"Unable to find approaching indent: value {target} is more than current maximum indent {indents.Last()}");
        }

        /// <summary>
        /// 将缩进减少到不超过目标数值
        /// </summary>
        /// <param name="target">目标数值</param>
        /// <returns>被去除的缩进长度列表</returns>
        public IEnumerable<int> ShrinkTo(int target) {
            var result = new List<int>();
            foreach (var e in from i in _indents where i.Key > target orderby i.Key select i) {
                _indents.Remove(e.Key);
                Length -= e.Value;
                result.Add(e.Value);
            }
            return result;
        }
    }
}