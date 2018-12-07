using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.VisualNovel.Script.Compiler;
using UnityEngine;

namespace Core.VisualNovel.Script {
    public static class ModuleCompiler {
        public static void CompileModule(string entranceFile, CompileOption option) {
            foreach (var file in CompileFile(entranceFile, option)) {
                CompileModule(file, option);
            }
        }

        public static IEnumerable<string> CompileFile(string path, CompileOption option) {
            var source = Resources.Load<TextAsset>(path)?.text;
            if (string.IsNullOrEmpty(source)) {
                return new string[] { };
            }
            var identifier = new CodeIdentifier {Name = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var existedHash = ReadBinaryHash($"{path}_bin");
            if (!existedHash.HasValue || existedHash.Value != identifier.Hash) {
                var file = new Assembler(new Parser(new Lexer(source, identifier).Lex(), identifier).Parse(), identifier).Assemble();
                File.WriteAllBytes($"Assets/Resources/{path}.bin.bytes", file.Content);
                File.WriteAllText($"Assets/Resources/{path}.tr.default.txt", file.Translations, Encoding.UTF8);
                // TODO: import语句
                return new string[] { };
            } else {
                // TODO: import语句
                return new string[] { };
            }
        }

        private static uint? ReadBinaryHash(string path) {
            var binaryContent = Resources.Load<TextAsset>(path)?.bytes;
            if (binaryContent == null || binaryContent.Length == 0) {
                return null;
            }
            var reader = new BinaryReader(new MemoryStream(binaryContent));
            if (reader.ReadUInt32() != 0x963EFE4A) {
                return null;
            }
            var hash = reader.ReadUInt32();
            reader.Dispose();
            return hash;
        }
    }
}