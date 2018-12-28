using System.IO;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 扩展二进制读取器
    /// </summary>
    public class ExtendedBinaryReader : BinaryReader {
        public ExtendedBinaryReader([NotNull] Stream input) : base(input) {}

        /// <summary>
        /// 读取七位压缩整数
        /// </summary>
        /// <returns></returns>
        public new int Read7BitEncodedInt() {
            return base.Read7BitEncodedInt();
        }
    }
}