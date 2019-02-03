using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Translation;

// ReSharper disable InconsistentNaming

namespace WADV.VisualNovel.ScriptStatus {
    /// <summary>
    /// 脚本信息
    /// </summary>
    [Serializable]
    public class ScriptInformation {
        /// <summary>
        /// 脚本ID（即相对DistributionFolder的不含扩展名的文件路径）
        /// </summary>
        public string Id;
        /// <summary>
        /// 源文件当前哈希值
        /// </summary>
        public uint? Hash;
        
        /// <summary>
        /// 二进制文件中记录的编译时源文件哈希值
        /// </summary>
        public uint? RecordedHash;

        /// <summary>
        /// 自定义的运行时二进制加载URI
        /// </summary>
        [CanBeNull]
        public string DistributionTarget;
        
        /// <summary>
        /// 支持的翻译和自定义运行时翻译加载URI列表
        /// </summary>
        public readonly Dictionary<string, string> Translations = new Dictionary<string, string>();
        
        /// <summary>
        /// 根据Asset路径创建脚本ID
        /// </summary>
        /// <param name="path">目标文件路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static string CreateIdFromAsset([NotNull] string path) {
            path = path.UnifySlash();
            path = path.StartsWith("Assets") ? path.Substring(7) : path;
            path = path.StartsWith("/") ? path.Substring(1) : path;
            if (path.StartsWith(CompileConfiguration.Content.DistributionFolder))
                return path.EndsWith(".vnb") ? path.RemoveStarts($"{CompileConfiguration.Content.DistributionFolder}/").RemoveEnds(".vnb") : null;
            if (path.StartsWith(CompileConfiguration.Content.SourceFolder))
                return path.EndsWith(".vns") ? path.RemoveStarts($"{CompileConfiguration.Content.SourceFolder}/").RemoveEnds(".vns") : null;
            if (path.StartsWith(CompileConfiguration.Content.TranslationFolder)) {
                if (!path.EndsWith(".txt")) return null;
                var target = path.RemoveStarts($"{CompileConfiguration.Content.TranslationFolder}/").RemoveEnds(".txt");
                return target.Substring(target.IndexOf("/", StringComparison.Ordinal) + 1);
            }
            return null;
        }

        /// <summary>
        /// 根据脚本ID创建源文件路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static string CreateSourceAssetPathFromId([NotNull] string id) {
            return $"Assets/{CompileConfiguration.Content.SourceFolder}/{id}.vns";
        }

        /// <summary>
        /// 根据脚本ID创建二进制文件路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static string CreateBinaryAssetPathFromId([NotNull] string id) {
            return $"Assets/{CompileConfiguration.Content.DistributionFolder}/{id}.vnb";
        }

        /// <summary>
        /// 根据脚本ID创建翻译文件路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="language">目标翻译的语言名</param>
        /// <returns></returns>
        public static string CreateLanguageAssetPathFromId([NotNull] string id, [NotNull] string language) {
            return $"Assets/{CompileConfiguration.Content.TranslationFolder}/{language}/{id}.txt";
        }

        /// <summary>
        /// 从Asset路径新建或更新脚本信息并保存到编译配置中
        /// </summary>
        /// <param name="path">目标文件路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static ScriptInformation CreateInformationFromAsset([NotNull] string path) {
            var id = CreateIdFromAsset(path);
            if (string.IsNullOrEmpty(id)) return null;
            ScriptInformation result;
            if (CompileConfiguration.Content.Scripts.ContainsKey(id)) {
                result = CompileConfiguration.Content.Scripts[id];
            } else {
                result = new ScriptInformation {Id = id};
                CompileConfiguration.Content.Scripts.Add(id, result);
            }
            if (path.EndsWith(".vns")) {
                var hash = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(path, Encoding.UTF8).UnifyLineBreak()));
                if (result.Hash == hash) return result;
                result.Hash = hash;
                CompileConfiguration.Save();
            } else if (path.EndsWith(".vnb")) {
                var stream = new FileStream(path, FileMode.Open);
                var hash = ReadBinaryHash(stream);
                stream.Close();
                if (result.RecordedHash == hash) return result;
                result.RecordedHash = hash;
                CompileConfiguration.Save();
            } else {
                path = path.RemoveStarts($"Assets/{CompileConfiguration.Content.TranslationFolder}/");
                var language = path.Substring(0, path.IndexOf("/", StringComparison.Ordinal));
                if (result.Translations.ContainsKey(language)) return result;
                result.Translations.Add(language, null);
                CompileConfiguration.Save();
            }
            return result;
        }

        /// <summary>
        /// 从VNB二进制数据中读取记录的编译时源文件哈希值
        /// </summary>
        /// <param name="data">目标数据流</param>
        /// <returns></returns>
        public static uint? ReadBinaryHash(byte[] data) {
            var stream = new MemoryStream(data);
            var result = ReadBinaryHash(stream);
            stream.Close();
            return result;
        }
        
        /// <summary>
        /// 从VNB数据流中读取记录的编译时源文件哈希值
        /// </summary>
        /// <param name="data">目标数据流</param>
        /// <returns></returns>
        public static uint? ReadBinaryHash(Stream data) {
            if (data.Length == 0) {
                return null;
            }
            var reader = new BinaryReader(data);
            if (reader.ReadUInt32() != CodeCompiler.Vnb1FileHeader) {
                return null;
            }
            var hash = reader.ReadUInt32();
            reader.Dispose();
            return hash;
        }
        
        /// <summary>
        /// 创建翻译文件
        /// </summary>
        /// <param name="language">目标语言</param>
        public void CreateTranslationFile(string language) {
            if (!Application.isEditor)
                throw new NotSupportedException($"Cannot create translation file for {Id}: static file compiler can only run in editor mode");
            var target = $"Assets/{CompileConfiguration.Content.TranslationFolder}/{language}";
            if (!Directory.Exists(target)) {
                Directory.CreateDirectory(target);
            }
            target = CreateLanguageAssetPathFromId(Id, language);
            if (File.Exists(target)) {
                var translation = new ScriptTranslation(File.ReadAllText(target, Encoding.UTF8));
                if (translation.MergeWith(ScriptHeader.LoadSync(Id).Header.LoadDefaultTranslation())) {
                    translation.SaveToAsset(target);
                }
            } else {
                ScriptHeader.LoadSync(Id).Header.LoadDefaultTranslation().SaveToAsset(target);
            }
        }

        /// <summary>
        /// 删除翻译文件
        /// </summary>
        /// <param name="language">目标语言</param>
        public void RemoveTranslationFile(string language) {
            var target = LanguageAssetPath(language);
            if (!File.Exists(target)) return;
            File.Delete(target);
            if (Translations.ContainsKey(language)) {
                Translations.Remove(language);
            }
        }
        
        /// <summary>
        /// 确定脚本是否有源文件
        /// </summary>
        public bool HasSource() => Hash.HasValue;

        /// <summary>
        /// 确定脚本是否有二进制文件
        /// </summary>
        public bool HasBinary() => RecordedHash.HasValue;

        /// <summary>
        /// 确定脚本是否支持指定翻译
        /// </summary>
        /// <param name="language">目标翻译的语言名</param>
        /// <returns></returns>
        public bool HasLanguage(string language) => Translations.ContainsKey(language);
        
        /// <summary>
        /// 获取脚本的源文件路径
        /// </summary>
        public string SourceAssetPath() => CreateSourceAssetPathFromId(Id);
        
        /// <summary>
        /// 获取脚本的二进制文件路径
        /// </summary>
        public string BinaryAssetPath() => CreateBinaryAssetPathFromId(Id);
        
        /// <summary>
        /// 获取脚本的运行时二进制加载URI模板
        /// </summary>
        public string DistributionTargetTemplate() => string.IsNullOrEmpty(DistributionTarget) ? CompileConfiguration.Content.DefaultRuntimeDistributionUri : DistributionTarget;

        /// <summary>
        /// 获取脚本的运行时二进制加载URI
        /// </summary>
        public string DistributionTargetUri() => DistributionTargetTemplate().ParseTemplate(new Dictionary<string, string> {{"id", Id}, {"language", "default"}});
        
        /// <summary>
        /// 获取脚本的翻译文件路径
        /// </summary>
        /// <param name="language">目标翻译的语言名</param>
        /// <returns></returns>
        public string LanguageAssetPath([NotNull] string language) => CreateLanguageAssetPathFromId(Id, language);

        /// <summary>
        /// 获取脚本的运行时翻译加载URI模板
        /// </summary>
        /// <param name="language">目标翻译的语言名</param>
        /// <returns></returns>
        [CanBeNull]
        public string LanguageTemplate(string language) {
            if (!Translations.ContainsKey(language)) return null;
            var address = Translations[language];
            return string.IsNullOrEmpty(address) ? CompileConfiguration.Content.DefaultRuntimeTranslationUri : address;
        }

        /// <summary>
        /// 获取脚本的运行时翻译加载URI
        /// </summary>
        /// <param name="language">目标翻译的语言名</param>
        /// <returns></returns>
        [CanBeNull]
        public string LanguageUri(string language) => LanguageTemplate(language)?.ParseTemplate(new Dictionary<string, string> {{"id", Id}, {"language", language}});
    }
}