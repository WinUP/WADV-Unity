using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Translation;
using UnityEngine;

namespace Core.VisualNovel.Script {
    /// <summary>
    /// 模块编译工具
    /// </summary>
    public static class ModuleCompiler {
        /// <summary>
        /// 编译文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="option">编译选项</param>
        /// <returns>发生变化的文件列表</returns>
        public static IEnumerable<string> CompileFile(string path, ScriptCompileOption option) {
            if (!path.EndsWith(".vns")) {
                throw new NotSupportedException($"Cannot compile {path}: File name extension must be vns");
            }
            var source = File.ReadAllText(path, Encoding.UTF8);
            var paths = CodeCompiler.CreatePathFromAsset(path);
            // 编译文件
            var identifier = new CodeIdentifier {Id = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var existedHash = ReadBinaryHash(paths.Binary);
            if (existedHash.HasValue && existedHash.Value == identifier.Hash) {
                return new string[] { }; // 如果源代码内容没有变化则直接跳过编译
            }
            var changedFiles = new List<string>();
            var file = CodeCompiler.Compile(source, identifier);
            File.WriteAllBytes(paths.Binary, file.Content);
            changedFiles.Add(paths.Binary);
            // 处理其他翻译
            foreach (var language in option.ExtraTranslationLanguages) {
                var languageFilePath = CodeCompiler.CreateLanguageAssetPathFromId(paths.SourceResource, language);
                if (File.Exists(languageFilePath)) {
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(languageFilePath));
                    if (!existedTranslation.MergeWith(file.DefaultTranslation, option.RemoveUselessTranslations)) continue;
                    File.WriteAllText(languageFilePath, existedTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(languageFilePath);
                } else {
                    // 如果翻译不存在，以默认翻译为蓝本新建翻译文件
                    File.WriteAllText(languageFilePath, file.DefaultTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(languageFilePath);
                }
            }
            return changedFiles;
        }

        /// <summary>
        /// 运行时编译文件
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <param name="option">编译选项</param>
        /// <returns></returns>
        public static (byte[] Code, IReadOnlyDictionary<string, ScriptTranslation> Translations) CompileFileRuntime(string id, ScriptCompileOption option) {
            var source = Resources.Load<TextAsset>(id)?.text ?? "";
            // 编译文件
            var identifier = new CodeIdentifier {Id = id, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var file = CodeCompiler.Compile(source, identifier);
            // 生成翻译
            var translations = new Dictionary<string, ScriptTranslation>();
            foreach (var language in option.ExtraTranslationLanguages) {
                var languageFilePath = CodeCompiler.CreateLanguageResourcePathFromId(id, language);
                var content = Resources.Load<TextAsset>(languageFilePath)?.text;
                if (string.IsNullOrEmpty(content)) continue;
                var existedTranslation = new ScriptTranslation(content);
                existedTranslation.MergeWith(file.DefaultTranslation, option.RemoveUselessTranslations);
                translations.Add(language, existedTranslation);
            }
            return (file.Content, translations);
        }

        /// <summary>
        /// 读取预编译VNS文件哈希值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static uint? ReadBinaryHash(string path) {
            return File.Exists(path) ? ReadBinaryHash(new FileStream(path, FileMode.Open)) : null;
        }
        
        /// <summary>
        /// 运行时读取预编译VNS文件哈希值
        /// </summary>
        /// <param name="resource">资源路径</param>
        /// <returns></returns>
        public static uint? ReadBinaryHashRuntime(string resource) {
            var binaryContent = Resources.Load<TextAsset>(resource)?.bytes;
            return binaryContent == null ? null : ReadBinaryHash(new MemoryStream(binaryContent));
        }

        private static uint? ReadBinaryHash(Stream data) {
            if (data.Length == 0) {
                return null;
            }
            var reader = new BinaryReader(data);
            if (reader.ReadUInt32() != 0x963EFE4A) {
                return null;
            }
            var hash = reader.ReadUInt32();
            reader.Dispose();
            return hash;
        }
    }
}