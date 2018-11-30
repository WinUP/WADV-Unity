using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.VisualNovel.Script.Compiler {
    public class AssembleFile {
        private readonly BinaryWriter _writer = new BinaryWriter(new MemoryStream());
        private readonly Dictionary<string, long> _labels = new Dictionary<string, long>();
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
        private uint _stringCount;

        public void WriteOpCode(OpCodeType code) {
            _writer.Write((byte) code);
        }

        public void WriteString(string content) {
            _writer.Write(content);
        }

        public void WriteInteger(int value) {
            _writer.Write(value);
        }

        public void WriteFloat(float value) {
            _writer.Write(value);
        }

        public void WriteLabel(string name) {
            if (_labels.ContainsKey(name)) {
                throw new ArgumentException($"Label {name} is already existed");
            }
            _labels.Add(name, _writer.BaseStream.Position);
        }

        public void WriteLoadTranslatableString(string content) {
            WriteOpCode(OpCodeType.LDSTT);
            _writer.Write(_stringCount);
            var hash = Hasher.Compute(Encoding.UTF8.GetBytes(content));
            _translations.Add(Convert.ToString((_stringCount << 16) + hash, 16).ToUpper().PadLeft(8, '0'), content);
            ++_stringCount;
        }

        public void WritePluginName(string language, string name) {
            
        }

        public void WriteUid(byte[] value) {
            if (value.Length != 4) {
                throw new ArgumentException("Identifier array's length must be 4");
            }
            WriteOpCode(OpCodeType.LDUID);
            foreach (var v in value) {
                _writer.Write(v);
            }
        }

        public (byte[] content, IReadOnlyDictionary<string, long> labels, IReadOnlyDictionary<string, string> translations) Create() {
            return ((_writer.BaseStream as MemoryStream)?.ToArray(), _labels, _translations);
        }
    }
}