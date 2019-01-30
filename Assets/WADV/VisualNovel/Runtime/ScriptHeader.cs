using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.VisualNovel.Provider;
using WADV.VisualNovel.ScriptStatus;

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
        /// 将脚本可执行内容载入缓存（如果该脚本不在缓存中）
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static async Task<(ScriptHeader Header, byte[] CodeSegment)> Load([NotNull] string id) {
            if (!LoadedScripts.ContainsKey(id)) return await Reload(id);
            var (scriptHeader, codeSegment) = LoadedScripts[id];
            var header = scriptHeader;
            var source = codeSegment;
            return (header, source);
        }

        /// <summary>
        /// 将脚本可执行内容载入缓存（如果该脚本不在缓存中）
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static (ScriptHeader Header, byte[] CodeSegment) LoadSync([NotNull] string id) {
            if (!LoadedScripts.ContainsKey(id)) return Reload(id).GetResultAfterFinished();
            var (scriptHeader, codeSegment) = LoadedScripts[id];
            var header = scriptHeader;
            var source = codeSegment;
            return (header, source);
        }
        
        /// <summary>
        /// 将脚本可执行内容载入缓存
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static async Task<(ScriptHeader Header, byte[] CodeSegment)> Reload([NotNull] string id) {
            if (!CompileConfiguration.Content.Scripts.ContainsKey(id))
                throw new KeyNotFoundException($"Unable to load script {id}: missing script information");
            var info = CompileConfiguration.Content.Scripts[id];
            if (!info.Binary.HasValue || string.IsNullOrEmpty(info.Binary.Value.Runtime))
                throw new MissingMemberException($"Unable to load script {id}: missing runtime binary resource");
            var code = await ResourceProviderManager.Load(info.GetBinaryRuntime());
            var source = code is ScriptAsset scriptAsset ? scriptAsset.content : ((BinaryData) code).Data;
            var result = ParseBinary(id, source);
            if (LoadedScripts.ContainsKey(id)) {
                LoadedScripts.Remove(id);
            }
            LoadedScripts.Add(id, result);
            return result;
        }

        /// <summary>
        /// 解析并缓存VNB二进制数据
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="source">要解析的数据</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static (ScriptHeader Header, byte[] Code) ParseBinary(string id, byte[] source) {
            ScriptHeader header;
            var reader = new ExtendedBinaryReader(new MemoryStream(source));
            switch (reader.ReadUInt32()) {
                case 0x963EFE4A:
                    header = LoadScriptVersion1(id, reader);
                    break;
                default:
                    throw new FormatException($"Unable to load script {id}: resource is not any acceptable type of Visual Novel Binary");
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
        public async Task<ScriptTranslation> LoadTranslation(string language) {
            if (Translations.ContainsKey(language)) return Translations[language];
            var content = await ResourceProviderManager.Load<string>(CompileConfiguration.Content.Scripts[Id].GetLanguageRuntime(language));
            if (string.IsNullOrEmpty(content)) return null;
            var translation = new ScriptTranslation(content);
            Translations.Add(language, translation);
            return translation;
        }

        /// <summary>
        /// 读取默认翻译
        /// </summary>
        /// <returns></returns>
        public ScriptTranslation LoadDefaultTranslation() {
            return Translations[TranslationManager.DefaultLanguage];
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