using System;
using System.Collections.Generic;
using System.IO;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Translation;
using UnityEngine;

namespace Core.VisualNovel.Script {
    public class RuntimeFile {
        public ScriptTranslation DefaultTranslation { get; }
        public List<string> Strings { get; } = new List<string>();
        public Dictionary<int, long> Labels { get; } = new Dictionary<int, long>();
        public List<SourcePosition> Positions { get; } = new List<SourcePosition>();
        private readonly BinaryReader _reader;
        
        public RuntimeFile(string id) {
            var source = Resources.Load<TextAsset>(CodeCompiler.CreatePathFromId(id).BinaryResource)?.bytes;
            if (source == null) {
                throw new FileNotFoundException($"Could not find resource {id}");
            }
            _reader = new BinaryReader(new MemoryStream(source));
            if (_reader.ReadUInt32() != 0x963EFE4A) {
                throw new FormatException($"Resource {id} is not Visual Novel Script");
            }
            _reader.ReadUInt32(); // 跳过哈希值
            // 读取默认翻译表
            var translations = new Dictionary<uint, string>();
            var translationCount = _reader.ReadInt32();
            for (var i = -1; ++i < translationCount;) {
                translations.Add(_reader.ReadUInt32(), _reader.ReadString());
            }
            DefaultTranslation = new ScriptTranslation(translations);
            // 读取常量表
            var stringCount = _reader.ReadInt32();
            for (var i = -1; ++i < stringCount;) {
                Strings.Add(_reader.ReadString());
            }
            // 读取跳转标签表
            var labelCount = _reader.ReadInt32();
            for (var i = -1; ++i < labelCount;) {
                Labels.Add(_reader.ReadInt32(), _reader.ReadInt64());
            }
            // 读取调试信息
            var positionCount = _reader.ReadInt32();
            for (var i = -1; ++i < positionCount;) {
                Positions.Add(SourcePosition.Create(_reader.ReadInt32(), _reader.ReadInt32()));
            }
        }

        public OperationCode ReadOperationCode() {
            var value = _reader.ReadByte();
            return OperationCode.LDNUL;
        }
    }
}