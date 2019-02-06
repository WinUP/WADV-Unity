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
        /// <param name="path">脚本ID</param>
        /// <param name="forceCompile">是否强制重新编译</param>
        /// <returns>发生变化的文件列表</returns>
        public static IEnumerable<string> CompileAsset(string path, bool forceCompile = false) {
            if (!Application.isEditor)
                throw new NotSupportedException($"Cannot compile {path}: static file compiler can only run in editor mode");
            if (!path.EndsWith(".vns"))
                throw new NotSupportedException($"Cannot compile {path}: file name extension must be .vns");
            var option = ScriptInformation.CreateInformationFromAsset(path);
            if (option == null)
                throw new NullReferenceException($"Cannot compile {path}: target outside of source folder or target is not acceptable script/binary");
            var changedFiles = new List<string>();
            var source = File.ReadAllText(path, Encoding.UTF8).UnifyLineBreak();
            if (!option.hash.HasValue) {
                option.hash = Hasher.Crc32(Encoding.UTF8.GetBytes(source));
            }
            var identifier = new CodeIdentifier {Id = option.id, Hash = option.hash.Value};
            // 编译文件
            if (!forceCompile) {
                if (option.recordedHash.HasValue && option.recordedHash.Value == identifier.Hash) {
                    return new string[] { }; // 如果源代码内容没有变化则直接跳过编译
                }
            }
            var (content, defaultTranslation) = CompileCode(source, identifier);
            var binaryFile = option.BinaryAssetPath();
            File.WriteAllBytes(binaryFile, content);
            option.recordedHash = option.hash = identifier.Hash;
            changedFiles.Add(binaryFile);
            // 处理其他翻译
            foreach (var (language, _) in option.Translations) {
                var languageFile = option.LanguageAssetPath(language);
                if (File.Exists(languageFile)) {
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(languageFile));
                    if (!existedTranslation.MergeWith(defaultTranslation)) continue;
                    existedTranslation.SaveToAsset(languageFile);
                    changedFiles.Add(languageFile);
                } else {
                    // 如果翻译不存在，以默认翻译为蓝本新建翻译文件
                    defaultTranslation.SaveToAsset(languageFile);
                    changedFiles.Add(languageFile);
                }
            }
            CompileConfiguration.Save();
            return changedFiles;
        }
    }
}