using System.IO;
using System.Text;
using Core.VisualNovel.Runtime;

namespace Core.VisualNovel.Interoperation {
    public class LiteMemoryStreamReader : ExtendedBinaryReader {
        public MemoryStream Stream => (MemoryStream) BaseStream;
        
        public LiteMemoryStreamReader(byte[] source) : base(new MemoryStream(source), Encoding.UTF8) { }
    }
}