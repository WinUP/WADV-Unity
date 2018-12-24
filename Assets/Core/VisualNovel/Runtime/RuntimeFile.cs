using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Extensions;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Translation;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.VisualNovel.Runtime {
    public partial class ScriptRuntime {
        /// <summary>
        /// 表示一个运行时脚本文件
        /// </summary>
        public class RuntimeFile {
            /// <summary>
            /// 脚本ID
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// 字符串常量列表
            /// </summary>
            public List<string> Strings { get; } = new List<string>();

            /// <summary>
            /// 跳转标签列表
            /// </summary>
            public Dictionary<int, long> Labels { get; } = new Dictionary<int, long>();

            /// <summary>
            /// 指令源文件位置对应表
            /// </summary>
            public Dictionary<long, SourcePosition> DebugPositions { get; } = new Dictionary<long, SourcePosition>();

            /// <summary>
            /// 当前代码段偏移地址
            /// </summary>
            public long CurrentPosition => _reader.BaseStream.Position - _codeSegmentPosition;

            /// <summary>
            /// 获取当前使用的翻译文件
            /// </summary>
            public ScriptTranslation ActiveTranslation { get; private set; }

            /// <summary>
            /// 获取脚本文件的默认翻译
            /// </summary>
            public ScriptTranslation DefaultTranslation { get; private set; }

            private readonly Dictionary<string, ScriptTranslation> _translations = new Dictionary<string, ScriptTranslation>();
            private readonly ScriptRuntime _runtime;
            private long _codeSegmentPosition;
            private Reader _reader;

            /// <summary>
            /// 创建一个运行时脚本文件
            /// </summary>
            /// <param name="id">脚本ID</param>
            /// <param name="runtime">脚本运行环境</param>
            public RuntimeFile(string id, ScriptRuntime runtime) {
                Id = id;
                _runtime = runtime;
                Reload();
            }

            ~RuntimeFile() {
                _reader.Close();
            }

            /// <summary>
            /// 设置使用的翻译
            /// </summary>
            /// <param name="name">语言名称，默认为default</param>
            public void UseTranslation(string name = TranslationManager.DefaultLanguage) {
                if (name == TranslationManager.DefaultLanguage) {
                    ActiveTranslation = DefaultTranslation;
                }
                else {
                    if (_translations.ContainsKey(name)) {
                        ActiveTranslation = _translations[name];
                    }
                    else {
                        var languageFilePath = CodeCompiler.CreateLanguageResourcePathFromId(Id, name);
                        var content = Resources.Load<TextAsset>(languageFilePath)?.text;
                        ActiveTranslation = string.IsNullOrEmpty(content) ? DefaultTranslation : new ScriptTranslation(content);
                    }
                }
            }

            /// <summary>
            /// 重新读取脚本内容并重置翻译为默认翻译
            /// </summary>
            public void Reload() {
                if (!CompileOptions.Has(Id)) {
                    throw new FileNotFoundException($"Cannot find script resource {Id}'s compile option");
                }
                var option = CompileOptions.Get(Id);
                byte[] source;
                if (option.BinaryHash.HasValue && option.BinaryHash.Value == option.SourceHash) {
                    var binaryFile = CodeCompiler.CreatePathFromId(Id).BinaryResource;
                    source = Resources.Load<TextAsset>(binaryFile)?.bytes;
                    if (source == null) {
                        throw new FileNotFoundException($"Cannot find binary file for compiled script {Id}");
                    }
                }
                else {
                    var (code, translations) = CodeCompiler.CompileResource(Id, option);
                    source = code;
                    foreach (var (name, content) in translations) {
                        _translations.Add(name, content);
                    }
                }
                _reader = new Reader(new MemoryStream(source));
                if (_reader.ReadUInt32() != 0x963EFE4A) {
                    throw new FormatException($"Resource {Id} is not Visual Novel Script");
                }
                _reader.ReadUInt32(); // 跳过哈希值
                var translationItems = new Dictionary<uint, string>();
                var translationCount = _reader.ReadInt32();
                for (var i = -1; ++i < translationCount;) {
                    translationItems.Add(_reader.ReadUInt32(), _reader.ReadString());
                }
                DefaultTranslation = new ScriptTranslation(translationItems);
                Strings.Clear();
                var stringCount = _reader.ReadInt32();
                for (var i = -1; ++i < stringCount;) {
                    Strings.Add(_reader.ReadString());
                }
                Labels.Clear();
                var labelCount = _reader.ReadInt32();
                for (var i = -1; ++i < labelCount;) {
                    Labels.Add(_reader.Read7BitEncodedInt(), _reader.ReadInt64());
                }
                DebugPositions.Clear();
                var positionCount = _reader.ReadInt32();
                var currentOffset = (long) 0;
                for (var i = -1; ++i < positionCount;) {
                    var offset = _reader.ReadByte();
                    currentOffset += offset;
                    DebugPositions.Add(currentOffset, SourcePosition.Create(_reader.Read7BitEncodedInt(), _reader.Read7BitEncodedInt()));
                }
                _codeSegmentPosition = _reader.BaseStream.Position;
                UseTranslation();
            }

            /// <summary>
            /// 移动到代码段指定偏移处
            /// </summary>
            /// <param name="offset">目标偏移</param>
            public void MoveTo(long offset) {
                _reader.BaseStream.Position = _codeSegmentPosition + offset;
            }

            /// <summary>
            /// 跳转到指定标签处
            /// </summary>
            /// <param name="labelId">标签ID</param>
            public void JumpTo(int labelId) {
                MoveTo(Labels[labelId]);
            }

            public OperationCode? ReadOperationCode() {
                if (_reader.BaseStream.Position > _reader.BaseStream.Length - 8) {
                    return null;
                }
                var value = _reader.ReadByte();
                if (value <= 0x45) {
                    return (OperationCode) value;
                }
                throw new RuntimeException(_runtime._callStack, $"Unknown operation code {Convert.ToString(value, 16)}");
            }

            public int ReadInteger() {
                return _reader.ReadInt32();
            }

            public float ReadFloat() {
                return _reader.ReadSingle();
            }

            public string ReadString() {
                var stringId = _reader.Read7BitEncodedInt();
                return stringId < Strings.Count ? Strings[stringId] : throw new RuntimeException(_runtime._callStack, $"Unable to find string constant #{stringId}");
            }

            public long ReadLabelOffset() {
                var labelId = _reader.Read7BitEncodedInt();
                return labelId < Labels.Count ? Labels[labelId] : throw new RuntimeException(_runtime._callStack, $"Unable to find label #{labelId}");
            }

            public uint ReadUInt32() {
                return _reader.ReadUInt32();
            }

            /// <summary>
            /// 用于访问BinaryReader内部函数的特定结构
            /// </summary>
            private class Reader : BinaryReader {
                public Reader([NotNull] Stream input) : base(input) {}

                public new int Read7BitEncodedInt() {
                    return base.Read7BitEncodedInt();
                }
            }
        }
    }
}