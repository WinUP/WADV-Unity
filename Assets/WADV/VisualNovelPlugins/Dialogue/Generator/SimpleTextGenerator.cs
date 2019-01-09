using System.Text;
using JetBrains.Annotations;

namespace WADV.VisualNovelPlugins.Dialogue.Generator {
    /// <inheritdoc />
    /// <summary>
    /// 提供逐字显示效果的对话文本生成器
    /// </summary>
    public class SimpleTextGenerator : TextGenerator {
        [NotNull]
        public override StringBuilder Current { get; } = new StringBuilder();

        /// <inheritdoc />
        public override string Text {
            get => _raw;
            set {
                _raw = value;
                Reset();
            }
        }

        private string _raw;
        private int _pointer;
        
        /// <inheritdoc />
        public override bool MoveNext() {
            if (_pointer >= _raw.Length) return false;
            Current.Append(_raw[_pointer]);
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
            Current.Clear();
            _pointer = 0;
            _raw = null;
        }
    }
}