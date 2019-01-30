using System;
using System.Collections.Generic;
using System.IO;
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
        /// 脚本ID
        /// </summary>
        public string Id;
        
        /// <summary>
        /// 源文件路径
        /// </summary>
        public RelativePath? Source;
        
        /// <summary>
        /// 二进制文件路径
        /// </summary>
        public RelativePath? Binary;
        
        /// <summary>
        /// 源文件当前哈希值
        /// </summary>
        public uint? Hash;
        
        /// <summary>
        /// 二进制文件中记录的编译时源文件哈希值
        /// </summary>
        public uint? RecordedHash;
        
        /// <summary>
        /// 翻译文件路径列表
        /// </summary>
        public readonly Dictionary<string, RelativePath?> Translations = new Dictionary<string, RelativePath?>();

        /// <summary>
        /// 从Asset路径新建脚本ID
        /// </summary>
        /// <param name="path">目标文件路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static string CreateIdFromAsset(string path) {
            path = path.UnifySlash();
            if (!path.StartsWith("Assets") || !path.EndsWith(".vns") && !path.EndsWith(".vnb")) return null;
            if (path.StartsWith("Assets/Resources"))
                return path.Substring(17, path.LastIndexOf(".", StringComparison.Ordinal) - 17);
            if (path.StartsWith("Assets/StreamingAssets"))
                return path.Substring(23, path.LastIndexOf(".", StringComparison.Ordinal) - 23);
            return path.Substring(7, path.LastIndexOf(".", StringComparison.Ordinal) - 7);
        }

        /// <summary>
        /// 从Asset路径新建或更新脚本信息并保存到编译配置中
        /// </summary>
        /// <param name="path">目标文件路径</param>
        /// <returns></returns>
        [CanBeNull]
        public static ScriptInformation CreateInformationFromAsset(string path) {
            var id = CreateIdFromAsset(path);
            if (id == null) return null;
            ScriptInformation result;
            if (CompileConfiguration.Content.Scripts.ContainsKey(id)) {
                result = CompileConfiguration.Content.Scripts[id];
            } else {
                result = new ScriptInformation {Id = id};
                CompileConfiguration.Content.Scripts.Add(id, result);
            }
            if (path.EndsWith(".vns")) {
                if (!string.IsNullOrEmpty(result.Source?.Asset) && result.Source?.Asset != path)
                    throw new NotSupportedException($"Unable to create script information for {id}: script with same id is already existed under Assets, Resources or StreamingAssets");
                result.Source = new RelativePath {Asset = path, Runtime = result.Source?.Runtime};
                result.Hash = Hasher.Crc32(File.ReadAllBytes(path));
            } else {
                if (!string.IsNullOrEmpty(result.Binary?.Asset) && result.Binary?.Asset != path)
                    throw new NotSupportedException($"Unable to create script information for {id}: binary with same id is already existed under Assets, Resources or StreamingAssets");
                result.Binary = new RelativePath {Asset = path, Runtime = result.Binary?.Runtime};
                result.RecordedHash = ReadBinaryHash(new FileStream(path, FileMode.Open));
            }
            CompileConfiguration.Save();
            return result;
        }

        /// <summary>
        /// 从脚本编译配置中删除与此脚本信息同ID的配置
        /// </summary>
        public void RemoveFromConfiguration() {
            if (CompileConfiguration.Content.Scripts.ContainsKey(Id)) {
                CompileConfiguration.Content.Scripts.Remove(Id);
            }
        }

        /// <summary>
        /// 获取二进制文件Asset路径
        /// </summary>
        /// <returns></returns>
        public string GetBinaryAsset() {
            return CompileConfiguration.ParseTemplate($"{Binary?.Asset ?? CompileConfiguration.Content.DefaultCompilePath}/{{id}}.vnb",
                                                      new Dictionary<string, string> {{CompileConfiguration.TemplateItems.Id, Id}});
        }

        /// <summary>
        /// 获取二进制文件运行时路径
        /// </summary>
        /// <returns></returns>
        public string GetBinaryRuntime() {
            return CompileConfiguration.ParseTemplate(Binary?.Runtime ?? CompileConfiguration.Content.DefaultBinaryRuntime,
                                                      new Dictionary<string, string> {{CompileConfiguration.TemplateItems.Id, Id}});
        }

        /// <summary>
        /// 获取语言文件Asset路径
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public string GetLanguageAsset(string language) {
            return CompileConfiguration.ParseTemplate($"{Translations[language]?.Asset ?? CompileConfiguration.Content.DefaultLanguagePath}.txt",
                                                      new Dictionary<string, string> {
                                                          {CompileConfiguration.TemplateItems.Id, Id},
                                                          {CompileConfiguration.TemplateItems.Language, language}
                                                      });
        }

        /// <summary>
        /// 获取翻译文件运行时路径
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public string GetLanguageRuntime(string language) {
            return CompileConfiguration.ParseTemplate($"{Translations[language]?.Runtime ?? CompileConfiguration.Content.DefaultLanguageRuntime}.txt",
                                                      new Dictionary<string, string> {
                                                          {CompileConfiguration.TemplateItems.Id, Id},
                                                          {CompileConfiguration.TemplateItems.Language, language}
                                                      });
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
    }
}