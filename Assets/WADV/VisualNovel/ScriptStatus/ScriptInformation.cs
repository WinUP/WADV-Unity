using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.VisualNovel.Compiler;

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
        /// 运行时二进制加载URI
        /// </summary>
        [CanBeNull]
        public string DistributionTarget;
        
        /// <summary>
        /// 运行时翻译加载URI列表
        /// </summary>
        public readonly Dictionary<string, string> Translations = new Dictionary<string, string>();

        public bool HasSource => Hash.HasValue;

        public bool HasBinary => RecordedHash.HasValue;

        [CanBeNull]
        public static string CreateIdFromAsset([NotNull] string path) {
            path = path.StartsWith("Assets") ? path.Substring(7) : path;
            path = path.StartsWith("/") ? path.Substring(1) : path;
            if (path.StartsWith(CompileConfiguration.Content.DistributionFolder))
                return path.EndsWith(".vnb") ? path.RemoveStarts($"{CompileConfiguration.Content.DistributionFolder}/").RemoveEnds(".vnb") : null;
            if (path.StartsWith(CompileConfiguration.Content.SourceFolder))
                return path.EndsWith(".vns") ? path.RemoveStarts($"{CompileConfiguration.Content.SourceFolder}/").RemoveEnds(".vns") : null;
            if (path.StartsWith(CompileConfiguration.Content.LanguageFolder)) {
                if (!path.EndsWith(".txt")) return null;
                var target = path.RemoveStarts($"{CompileConfiguration.Content.LanguageFolder}/").RemoveEnds(".txt");
                return target.Substring(target.IndexOf("/", StringComparison.Ordinal) + 1);
            }
            return null;
        }

        public static string CreateSourceAssetFromId([NotNull] string id) {
            return $"Assets/{CompileConfiguration.Content.SourceFolder}/{id}.vns";
        }

        public static string CreateBinaryAssetFromId([NotNull] string id) {
            return $"Assets/{CompileConfiguration.Content.DistributionFolder}/{id}.vnb";
        }
        
        public static string CreateLanguageAssetFromId([NotNull] string id, [NotNull] string language) {
            return $"Assets/{CompileConfiguration.Content.LanguageFolder}/{language}/{id}.txt";
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
                result.Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(path, Encoding.UTF8).UnifyLineBreak()));
            } else if (path.EndsWith(".vnb")) {
                result.RecordedHash = ReadBinaryHash(new FileStream(path, FileMode.Open));
            } else {
                path = path.RemoveStarts($"{CompileConfiguration.Content.LanguageFolder}/");
                var language = path.Substring(0, path.IndexOf("/", StringComparison.Ordinal));
                if (!result.Translations.ContainsKey(language)) {
                    result.Translations.Add(language, null);
                }
            }
            CompileConfiguration.Save();
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
            data.Close();
            return hash;
        }

        public bool HasLanguage(string language) => Translations.ContainsKey(language);
        
        public string CreateSourceAsset() {
            return CreateSourceAssetFromId(Id);
        }

        public string CreateBinaryAsset() {
            return CreateBinaryAssetFromId(Id);
        }
        
        public string CreateLanguageAsset([NotNull] string language) {
            return CreateLanguageAssetFromId(Id, language);
        }
    }
}