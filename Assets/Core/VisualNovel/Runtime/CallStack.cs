using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime {
    [Serializable]
    public class CallStack : IEnumerable<CallStack.StackItem> {
        private readonly LinkedList<StackItem> _callStack = new LinkedList<StackItem>();

        public int Count => _callStack.Count;

        public StackItem Last => _callStack.Last.Value;
        
        [CanBeNull]
        public StackItem Pop() {
            if (_callStack.Count > 0) {
                var last = _callStack.Last.Value;
                _callStack.RemoveLast();
                return last;
            } else {
                return null;
            }
        }

        public void Push(ScriptFile script) {
            _callStack.AddLast(new StackItem {ScriptId = script.Header.Id, Offset = script.CurrentPosition});
        }

        public void Push(IEnumerable<StackItem> items) {
            foreach (var item in items) {
                _callStack.AddLast(item);
            }
        }

        public void Clear() {
            _callStack.Clear();
        }

        [Serializable]
        public class StackItem {
            public string ScriptId { get; set; }
            public long Offset { get; set; }
        }

        public IEnumerator<StackItem> GetEnumerator() {
            return _callStack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}