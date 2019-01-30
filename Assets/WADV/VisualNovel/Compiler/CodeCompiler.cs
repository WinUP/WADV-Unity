using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WADV.Extensions;
using WADV.VisualNovel.ScriptStatus;
using WADV.VisualNovel.Translation;
using UnityEngine;

namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 编译器快捷入口
    /// </summary>
    public static class CodeCompiler {
        /// <summary>
        /// 文件格式"Visual Novel Binary, Version 1"的标志
        /// </summary>
        public const uint Vnb1FileHeader = 0x963EFE4A;
        
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
        /// <param name="option">脚本信息</param>
        /// <param name="forceCompile">是否强制重新编译</param>
        /// <returns>发生变化的文件列表</returns>
        public static IEnumerable<string> CompileAsset(ScriptInformation option, bool forceCompile = false) {
            if (!Application.isEditor)
                throw new NotSupportedException($"Cannot compile {option.Source}: static file compiler can only run in editor mode");
            if (string.IsNullOrEmpty(option.Source?.Asset) || !option.Hash.HasValue) return new string[] { };
            var changedFiles = new List<string>();
            if (!option.Source.Value.Asset.EndsWith(".vns"))
                throw new NotSupportedException($"Cannot compile {option.Source}: File name extension must be vns");
            var source = File.ReadAllText(option.Source.Value.Asset, Encoding.UTF8).UnifyLineBreak();
            var identifier = new CodeIdentifier {Id = option.Id, Hash = option.Hash.Value};
            // 编译文件
            if (!forceCompile) {
                if (option.RecordedHash.HasValue && option.RecordedHash.Value == identifier.Hash) {
                    return new string[] { }; // 如果源代码内容没有变化则直接跳过编译
                }
            }
            var (content, defaultTranslation) = CompileCode(source, identifier);
            var binaryFile = option.GetBinaryAsset();
            File.WriteAllBytes(binaryFile, content);
            option.Binary = new RelativePath {Asset = binaryFile, Runtime = option.Binary?.Runtime};
            option.RecordedHash = option.Hash = identifier.Hash;
            changedFiles.Add(option.Binary.Value.Asset);
            // 处理其他翻译
            foreach (var (language, _) in option.Translations) {
                var languageFile = option.GetLanguageAsset(language);
                if (File.Exists(languageFile)) {
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(languageFile));
                    if (!existedTranslation.MergeWith(defaultTranslation)) continue;
                    File.WriteAllText(languageFile, existedTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(languageFile);
                } else {
                    // 如果翻译不存在，以默认翻译为蓝本新建翻译文件
                    File.WriteAllText(languageFile, defaultTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(languageFile);
                }
            }
            CompileConfiguration.Save();
            return changedFiles;
        }
    }
}