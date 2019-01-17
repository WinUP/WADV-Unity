using System.Text;
using JetBrains.Annotations;

namespace WADV.Plugins.Dialogue.TextGenerator {
    /// <inheritdoc />
    /// <summary>
    /// 提供逐字显示效果的对话文本生成器
    /// </summary>
    public class SimpleDialogueTextGenerator : DialogueTextGenerator {
        /// <inheritdoc />
        [NotNull]
        public override StringBuilder Current { get; } = new StringBuilder();

        /// <inheritdoc />
        public override string Text {
            get => _text;
            set {
                _text = value;
                _textLength = _text.Length;
            }
        }

        private string _text;
        private int _textLength;
        private int _pointer;
        
        /// <inheritdoc />
        public override bool MoveNext() {
            if (_pointer >= _textLength) return false;
            Current.Append(_text[_pointer]);
            ++_pointer;
            return true;
        }

        /// <inheritdoc />
        public override void Reset() {
            _pointer = 0;
            Current.Clear();
        }

        /// <inheritdoc />
        public override void Dispose() {
            Reset();
            _text = null;
        }
    }
}