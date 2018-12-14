using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Extensions;
using Core.VisualNovel.Translation;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 编译器快捷入口
    /// </summary>
    public static class CodeCompiler {
        public class ScriptPaths {
            public string Source;
            public string SourceResource;
            public string Binary;
            public string BinaryResource;
            public string Directory;
            public string DirectoryResource;
            public string Language;
        }
        
        private static readonly Regex LanguagePathTester = new Regex(@".+\.tr\.(.+)\.txt$", RegexOptions.Compiled);
        
        /// <summary>
        /// 编译源代码
        /// </summary>
        /// <param name="source">源代码文本</param>
        /// <param name="identifier">源代码脚本ID</param>
        /// <returns></returns>
        public static (byte[] Content, ScriptTranslation DefaultTranslation) CompileCode(string source, CodeIdentifier identifier) {
            return ByteCodeGenerator.Generate(Parser.Parse(Lexer.Lex(source, identifier), identifier), identifier);
        }
        
        /// <summary>
        /// 编译文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="option">编译选项</param>
        /// <returns>发生变化的文件列表</returns>
        public static IEnumerable<string> CompileAsset(string path, ScriptCompileOption option) {
            if (!path.EndsWith(".vns")) {
                throw new NotSupportedException($"Cannot compile {path}: File name extension must be vns");
            }
            var source = File.ReadAllText(path, Encoding.UTF8);
            var paths = CreatePathFromAsset(path);
            if (paths == null) {
                throw new NotSupportedException($"File {path}'s path cannot be recognized as visual novel script");
            }
            // 编译文件
            var identifier = new CodeIdentifier {Id = path, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var existedHash = ReadBinaryHash(paths.Binary);
            if (existedHash.HasValue && existedHash.Value == identifier.Hash) {
                return new string[] { }; // 如果源代码内容没有变化则直接跳过编译
            }
            var changedFiles = new List<string>();
            var file = CompileCode(source, identifier);
            File.WriteAllBytes(paths.Binary, file.Content);
            changedFiles.Add(paths.Binary);
            // 处理其他翻译
            foreach (var language in option.ExtraTranslationLanguages) {
                var languageFilePath = CreateLanguageAssetPathFromId(paths.SourceResource, language);
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
        /// 编译资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <param name="option">编译选项</param>
        /// <returns></returns>
        public static (byte[] Code, IReadOnlyDictionary<string, ScriptTranslation> Translations) CompileResource(string id, ScriptCompileOption option) {
            var source = Resources.Load<TextAsset>(id)?.text ?? "";
            // 编译文件
            var identifier = new CodeIdentifier {Id = id, Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source))};
            var file = CompileCode(source, identifier);
            // 生成翻译
            var translations = new Dictionary<string, ScriptTranslation>();
            foreach (var language in option.ExtraTranslationLanguages) {
                var languageFilePath = CreateLanguageResourcePathFromId(id, language);
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
        public static uint? ReadBinaryResourceHash(string resource) {
            var binaryContent = Resources.Load<TextAsset>(resource)?.bytes;
            return binaryContent == null ? null : ReadBinaryHash(new MemoryStream(binaryContent));
        }
        
        /// <summary>
        /// 根据脚本ID生成可能的各种此脚本的同组资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static ScriptPaths CreatePathFromId(string id) {
            var directory = Path.GetDirectoryName(id) ?? id;
            return new ScriptPaths {
                Source = id + ".vns",
                SourceResource = id,
                Binary = id + ".bin.bytes",
                BinaryResource = id + ".bin",
                Directory = $"Assets/Resources/{directory}",
                DirectoryResource = directory
            };
        }
        
        /// <summary>
        /// 根据资源路径生成可能的各种此脚本的同组资源路径
        /// </summary>
        /// <param name="asset">资源路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static ScriptPaths CreatePathFromAsset(string asset) {
            var assetDirectory = Path.GetDirectoryName(asset) ?? asset;
            var idDirectory = assetDirectory.Remove("Assets/Resources/");
            if (asset.EndsWith(".vns")) {
                var assetWithoutExtension = asset.RemoveLast(".vns");
                var id = assetWithoutExtension.Remove("Assets/Resources/");
                return new ScriptPaths {
                    Source = asset,
                    SourceResource = id,
                    Binary = assetWithoutExtension + ".bin.bytes",
                    BinaryResource = id + ".bin",
                    Directory = assetDirectory,
                    DirectoryResource = idDirectory
                };
            }
            if (asset.EndsWith(".bin.bytes")) {
                var assetWithoutExtension = asset.RemoveLast(".bin.bytes");
                var id = assetWithoutExtension.Remove("Assets/Resources/");
                return new ScriptPaths {
                    Source = assetWithoutExtension + ".vns",
                    SourceResource = id,
                    Binary = asset,
                    BinaryResource = id + ".bin",
                    Directory = assetDirectory,
                    DirectoryResource = idDirectory
                };
            }
            if (asset.EndsWith(".txt")) {
                var language = GetLanguageNameFromPath(asset);
                if (string.IsNullOrEmpty(language)) {
                    return null;
                }
                var assetWithoutExtension = asset.RemoveLast($".tr.{language}.txt");
                var id = assetWithoutExtension.Remove("Assets/Resources/");
                return new ScriptPaths {
                    Source = assetWithoutExtension + ".vns",
                    SourceResource = id,
                    Binary = assetWithoutExtension + ".bin.bytes",
                    BinaryResource = id + ".bin",
                    Directory = assetDirectory,
                    DirectoryResource = idDirectory,
                    Language = language
                };
            }
            return null;
        }

        /// <summary>
        /// 从指定路径中提取翻译文件的目标语言
        /// </summary>
        /// <param name="path">目标路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static string GetLanguageNameFromPath(string path) {
            path = Path.GetFileName(path);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            var matches = LanguagePathTester.Match(path);
            if (!matches.Success || matches.Length == 0) {
                return null;
            }
            return matches.Groups[1].Value;
        }

        /// <summary>
        /// 从指定路径组中过滤出特定脚本ID对应的翻译文件路径
        /// </summary>
        /// <param name="assetPaths">目标路径组</param>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static IEnumerable<ScriptPaths> FilterAssetFromId(IEnumerable<string> assetPaths, string id) {
            return from path in assetPaths let scriptPaths = CreatePathFromAsset(path) where scriptPaths != null && scriptPaths.SourceResource == id select scriptPaths;
        }

        /// <summary>
        /// 根据脚本ID生成二进制资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static string CreateBinaryAssetPathFromId(string id) {
            return $"Assets/Resources/{CreateBinaryResourcePathFromId(id)}.bytes";
        }
        
        /// <summary>
        /// 根据脚本ID生成供Resources.Load使用的二进制资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static string CreateBinaryResourcePathFromId(string id) {
            return $"{id}.bin";
        }

        /// <summary>
        /// 根据脚本ID生成语言资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string CreateLanguageAssetPathFromId(string id, string language) {
            return $"Assets/Resources/{CreateLanguageResourcePathFromId(id, language)}.txt";
        }

        /// <summary>
        /// 根据脚本ID生成供Resources.Load使用的语言资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string CreateLanguageResourcePathFromId(string id, string language) {
            return $"{id}.tr.{language}";
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