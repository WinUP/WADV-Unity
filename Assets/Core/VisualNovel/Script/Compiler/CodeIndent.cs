using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.VisualNovel.Script.Compiler {
    public class CodeIndent {
        public int Length { get; private set; }
        
        private readonly Dictionary<int, int> _indents = new Dictionary<int, int>();

        public void Push(int length) {
            Length += length;
            _indents.Add(Length, length);
        }

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

        public IEnumerable<int> ShrinkTo(int target) {
            var focusedIndents = (from i in _indents where i.Key > target orderby i.Key select i).ToList();
            foreach (var e in focusedIndents) {
                _indents.Remove(e.Key);
                Length -= e.Value;
            }

            return focusedIndents.Select(e => e.Value);
        }
    }
}