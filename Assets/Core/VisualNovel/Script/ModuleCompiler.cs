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
        public static IEnumerable<string> CompileFile(string path, CompileOption option) {
            var source = File.ReadAllText(path, Encoding.UTF8);
            path = PathUtilities.DropExtension(path); // 与Unity资源格式统一
            var binPath = PathUtilities.Combine(path, PathUtilities.BinaryFile);
            var defaultTranslationPath = PathUtilities.Combine(path, PathUtilities.TranslationFileFormat, "default");
            // 编译文件
            var identifier = new CodeIdentifier {Name = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var existedHash = ReadBinaryHash(binPath);
            if (existedHash.HasValue && existedHash.Value == identifier.Hash) {
                return new string[] { }; // 如果源代码内容没有变化则直接跳过编译
            }
            var changedFiles = new List<string>();
            var file = new Assembler(new Parser(new Lexer(source, identifier).Lex(), identifier).Parse(), identifier).Assemble();
            File.WriteAllBytes(binPath, file.Content);
            changedFiles.Add(binPath);
            // 处理其他翻译
            foreach (var language in option.ExtraTranslationLanguages) {
                var extraPath = PathUtilities.Combine(path, PathUtilities.TranslationFileFormat, language);
                if (File.Exists(extraPath)) {
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(path));
                    if (!existedTranslation.MergeWith(file.Translations)) continue;
                    File.WriteAllText(extraPath, existedTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(extraPath);
                } else {
                    // 如果翻译不存在，以默认翻译为蓝本新建翻译文件
                    File.WriteAllText(extraPath, file.Translations.Pack(), Encoding.UTF8);
                    changedFiles.Add(extraPath);
                }
            }
            return changedFiles;
        }

        /// <summary>
        /// 运行时编译文件
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="option">编译选项</param>
        /// <returns></returns>
        public static (byte[] Code, IReadOnlyDictionary<string, ScriptTranslation> Translations) CompileFileRuntime(string path, CompileOption option) {
            var source = Resources.Load<TextAsset>(path)?.text ?? "";
            // 编译文件
            var identifier = new CodeIdentifier {Name = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var file = new Assembler(new Parser(new Lexer(source, identifier).Lex(), identifier).Parse(), identifier).Assemble();
            // 生成默认翻译
            var translations = new Dictionary<string, ScriptTranslation>();
            var defaultTranslation = MergeDefaultTranslationRuntime(path, file.Translations, option);
            translations.Add("default", defaultTranslation);
            // 处理其他翻译
            foreach (var language in option.ExtraTranslationLanguages) {
                var extraPath = PathUtilities.Combine(path, PathUtilities.TranslationFileFormat, language);
                var content = Resources.Load<TextAsset>(extraPath)?.text;
                if (string.IsNullOrEmpty(content)) {
                    translations.Add(language, defaultTranslation);
                } else {
                    var existedTranslation = new ScriptTranslation(content);
                    existedTranslation.MergeWith(defaultTranslation);
                    translations.Add(language, existedTranslation);
                }
            }
            return (file.Content, translations);
        }

        /// <summary>
        /// 读取预编译VNS文件哈希值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static uint? ReadBinaryHash(string path) {
            return File.Exists(path) ? ReadBinaryHash(File.ReadAllBytes(path)) : null;
        }
        
        /// <summary>
        /// 运行时读取预编译VNS文件哈希值
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns></returns>
        public static uint? ReadBinaryHashRuntime(string assetPath) {
            var binaryContent = Resources.Load<TextAsset>(assetPath)?.bytes;
            return binaryContent == null ? null : ReadBinaryHash(binaryContent);
        }
        
        private static (ScriptTranslation Translation, bool Changed) MergeDefaultTranslation(string basePath, ScriptTranslation newTranslation, CompileOption option) {
            var path = PathUtilities.Combine(basePath, PathUtilities.TranslationFileFormat, "default");
            if (!File.Exists(path)) return (newTranslation, true);
            var existedDefaultTranslation = new ScriptTranslation(File.ReadAllText(path));
            var changed = existedDefaultTranslation.MergeWith(newTranslation, option.RemoveUselessTranslations);
            return (existedDefaultTranslation, changed);
        }

        private static ScriptTranslation MergeDefaultTranslationRuntime(string basePath, ScriptTranslation newTranslation, CompileOption option) {
            var path = PathUtilities.Combine(basePath, PathUtilities.TranslationResourceFormat, "default");
            var content = Resources.Load<TextAsset>(path)?.text;
            if (content == null) {
                return newTranslation;
            }
            var existedDefaultTranslation = new ScriptTranslation(content);
            existedDefaultTranslation.MergeWith(newTranslation, option.RemoveUselessTranslations);
            return existedDefaultTranslation;
        }

        private static uint? ReadBinaryHash(byte[] data) {
            if (data.Length == 0) {
                return null;
            }
            var reader = new BinaryReader(new MemoryStream(data));
            if (reader.ReadUInt32() != 0x963EFE4A) {
                return null;
            }
            var hash = reader.ReadUInt32();
            reader.Dispose();
            return hash;
        }
    }
}