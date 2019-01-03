using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Core.VisualNovel.Compiler {
    /// <summary>
    /// 扩展二进制编写器
    /// </summary>
    public class ExtendedBinaryWriter : BinaryWriter {
        protected ExtendedBinaryWriter() { }
        public ExtendedBinaryWriter([NotNull] Stream output) : base(output) { }
        public ExtendedBinaryWriter([NotNull] Stream output, [NotNull] Encoding encoding) : base(output, encoding) { }
        public ExtendedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen) { }
        
        /// <summary>
        /// 写入七位压缩整数
        /// </summary>
        /// <param name="value">要写入的值</param>
        /// <returns></returns>
        public new void Write7BitEncodedInt(int value) {
            base.Write7BitEncodedInt(value);
        }
    }
}