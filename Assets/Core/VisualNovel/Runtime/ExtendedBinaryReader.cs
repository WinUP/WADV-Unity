using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 扩展二进制读取器
    /// </summary>
    public class ExtendedBinaryReader : BinaryReader {
        public ExtendedBinaryReader([NotNull] Stream input) : base(input) { }
        public ExtendedBinaryReader([NotNull] Stream input, [NotNull] Encoding encoding) : base(input, encoding) { }
        public ExtendedBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

        /// <summary>
        /// 读取七位压缩整数
        /// </summary>
        /// <returns></returns>
        public new int Read7BitEncodedInt() {
            return base.Read7BitEncodedInt();
        }
    }
}