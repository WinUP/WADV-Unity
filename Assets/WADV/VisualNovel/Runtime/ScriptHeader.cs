using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using WADV.Extensions;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;
using UnityEngine;

namespace WADV.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个VNB脚本文件头
    /// </summary>
    public class ScriptHeader {
        private static readonly Dictionary<string, (ScriptHeader Header, byte[] CodeSegment)> LoadedScripts = new Dictionary<string, (ScriptHeader Header, byte[] CodeSegment)>();
        
        /// <summary>
        /// 获取脚本ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 获取字符串常量列表
        /// </summary>
        public ReadOnlyCollection<string> Strings { get; }

        /// <summary>
        /// 获取跳转标签列表
        /// </summary>
        public ReadOnlyDictionary<int, long> Labels { get; }

        /// <summary>
        /// 获取指令源文件位置对应表
        /// </summary>
        public ReadOnlyDictionary<long, SourcePosition> Positions { get; }

        /// <summary>
        /// 获取脚本文件的已加载翻译组
        /// </summary>
        public Dictionary<string, ScriptTranslation> Translations { get; } = new Dictionary<string, ScriptTranslation>();

        /// <summary>
        /// 新建一个VNB脚本文件头
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="strings">字符串常量列表</param>
        /// <param name="labels">跳转标签列表</param>
        /// <param name="positions">指令源文件位置对应表</param>
        private ScriptHeader([NotNull] string id, [NotNull] IEnumerable<string> strings, [NotNull] IDictionary<int, long> labels, [NotNull] IDictionary<long, SourcePosition> positions) {
            Id = id;
            Strings = strings.ToList().AsReadOnly();
            Labels = new ReadOnlyDictionary<int, long>(labels);
            Positions = new ReadOnlyDictionary<long, SourcePosition>(positions);
        }
        
        /// <summary>
        /// 按照以下优先级将脚本可执行内容载入缓存
        /// <list type="bullet">
        ///     <item><description>若缓存中已存在则使用缓存</description></item>
        ///     <item><description>当第二个参数存在时使用该参数内容</description></item>
        ///     <item><description>当二进制文件存在时使用该文件内容（如果此时源文件存在且过期会得到警告）</description></item>
        ///     <item><description>使用源文件的即时编译结果</description></item>
        /// </list>
        /// <para>不论采用何种方式，所有可用的翻译文件都会被尽可能载入</para>
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="source">用于覆盖原有脚本的可执行内容（该参数仅覆盖内存实现，不会修改本地文件）</param>
        /// <returns></returns>
        public static (ScriptHeader Header, byte[] CodeSegment) LoadAsset([NotNull] string id, byte[] source = null) {
            ScriptHeader header;
            if (LoadedScripts.ContainsKey(id)) {
                var (scriptHeader, codeSegment) = LoadedScripts[id];
                header = scriptHeader;
                source = codeSegment;
            } else {
                (header, source) = ReloadAsset(id, source);
            }
            return (header, source);
        }
        
        /// <summary>
        /// 按照以下优先级将脚本可执行内容载入缓存
        /// <list type="bullet">
        ///     <item><description>当第二个参数存在时使用该参数内容</description></item>
        ///     <item><description>当二进制文件存在时使用该文件内容（如果此时源文件存在且过期会得到警告）</description></item>
        ///     <item><description>使用源文件的即时编译结果</description></item>
        /// </list>
        /// <para>不论采用何种方式，所有可用的翻译文件都会被尽可能载入</para>
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="source">用于覆盖原有脚本的可执行内容（该参数仅覆盖内存实现，不会修改本地文件）</param>
        /// <returns></returns>
        public static (ScriptHeader Header, byte[] CodeSegment) ReloadAsset([NotNull] string id, byte[] source = null) {
            var initialTranslations = new Dictionary<string, ScriptTranslation>();
            if (source == null) {
                source = CodeCompiler.LoadBinaryResourceFromId(id)?.content;
                var option = CompileOptions.Has(id) ? CompileOptions.Get(id) : null;
                if (source == null) {
                    if (option == null) throw new KeyNotFoundException($"Unable to load script {id}: Missing compile option");
                    var (code, translations) = CodeCompiler.CompileResource(id, option);
                    source = code;
                    foreach (var (name, content) in translations.Where(e => e.Key != TranslationManager.DefaultLanguage)) {
                        initialTranslations.Add(name, content);
                    }
                } else if (option != null && option.BinaryHash != option.SourceHash) {
                    Debug.LogWarning($"VNScript binary file outdated: {id}");
                }
            }
            ScriptHeader header;
            var reader = new ExtendedBinaryReader(new MemoryStream(source));
            switch (reader.ReadUInt32()) {
                case 0x963EFE4A:
                    header = LoadScriptVersion1(id, reader);
                    break;
                default:
                    throw new FormatException($"Resource {id} is not any acceptable type of Visual Novel Binary");
            }
            foreach (var (language, translation) in initialTranslations) {
                header.Translations.Add(language, translation);
            }
            var codeSegment = new MemoryStream();
            reader.BaseStream.CopyTo(codeSegment);
            reader.Close();
            var codes = codeSegment.ToArray();
            codeSegment.Close();
            if (LoadedScripts.ContainsKey(id)) {
                LoadedScripts.Remove(id);
            }
            LoadedScripts.Add(id, (header, codes));
            return (header, codes);
        }
        
        /// <summary>
        /// 从本地文件加载并缓存翻译
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        [CanBeNull]
        public ScriptTranslation LoadTranslation(string language) {
            if (Translations.ContainsKey(language)) return Translations[language];
            var content = Resources.Load<TextAsset>(CodeCompiler.CreateLanguageResourcePathFromId(Id, language))?.text;
            if (string.IsNullOrEmpty(content)) return null;
            var translation = new ScriptTranslation(content);
            Translations.Add(language, translation);
            return translation;
        }

        /// <summary>
        /// 新建一个此VNB脚本的可执行对象
        /// </summary>
        /// <returns></returns>
        public ScriptFile CreateRuntimeFile() {
            return new ScriptFile(this, LoadedScripts[Id].CodeSegment);
        }

        /// <summary>
        /// 获取翻译（如果目标语言不存在则使用默认翻译）
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="id">翻译ID</param>
        /// <returns></returns>
        public string GetTranslation(string language, uint id) {
            if (!Translations.ContainsKey(language)) {
                language = TranslationManager.DefaultLanguage;
            }
            return Translations[language].GetTranslation(id);
        }
        
        /// <summary>
        /// 判断目标语言和翻译是否都存在
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="id">翻译ID</param>
        /// <returns></returns>
        public bool HasTranslation(string language, uint id) {
            return Translations.ContainsKey(language) && Translations[language].HasTranslation(id);
        }

        private static ScriptHeader LoadScriptVersion1([NotNull] string id, [NotNull] ExtendedBinaryReader reader) {
            reader.ReadUInt32(); // 跳过哈希值
            // 默认翻译段
            var translationItems = new Dictionary<uint, string>();
            var translationCount = reader.ReadInt32();
            for (var i = -1; ++i < translationCount;) {
                translationItems.Add(reader.ReadUInt32(), reader.ReadString());
            }
            var defaultTranslation = new ScriptTranslation(translationItems);
            // 字符串常量段
            var stringCount = reader.ReadInt32();
            var strings = new List<string>();
            for (var i = -1; ++i < stringCount;) {
                strings.Add(reader.ReadString());
            }
            // 跳转标签段
            var labelCount = reader.ReadInt32();
            var labels = new Dictionary<int, long>();
            for (var i = -1; ++i < labelCount;) {
                labels.Add(reader.Read7BitEncodedInt(), reader.ReadInt64());
            }
            // 调试信息段
            var positionCount = reader.ReadInt32();
            var positions = new Dictionary<long, SourcePosition>();
            var currentOffset = (long) 0;
            for (var i = -1; ++i < positionCount;) {
                var offset = reader.ReadByte();
                currentOffset += offset;
                positions.Add(currentOffset, SourcePosition.Create(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt()));
            }
            // 生成文件头
            var header = new ScriptHeader(id, strings, labels, positions);
            header.Translations.Add(TranslationManager.DefaultLanguage, defaultTranslation);
            return header;
        }
    }
}