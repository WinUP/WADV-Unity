using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.VisualNovel.Translation;

// ReSharper disable InconsistentNaming

namespace WADV.VisualNovel.ScriptStatus {
    /// <summary>
    /// 脚本编译配置
    /// </summary>
    [Serializable]
    public class CompileConfiguration {
        public static readonly string RecordFilePath = Application.streamingAssetsPath + "/VisualNovelScriptDefaultCompileConfiguration.bin";
        public static CompileConfiguration Content { get; }
        
        /// <summary>
        /// 脚本信息列表
        /// </summary>
        public readonly Dictionary<string, ScriptInformation> Scripts = new Dictionary<string, ScriptInformation>();
        
        /// <summary>
        /// 默认编译路径
        /// </summary>
        public string DefaultCompilePath = "Assets/{provider}/Binary"; // +/{id}.vnb

        /// <summary>
        /// 默认二进制文件运行时路径
        /// </summary>
        public string DefaultBinaryRuntime = "{provider}://Binary/{id}";
        
        /// <summary>
        /// 默认语言文件输出路径
        /// </summary>
        public string DefaultLanguagePath = "Assets/{provider}/Translations/{language}/{id}"; // +.txt

        /// <summary>
        /// 默认语言文件运行时路径
        /// </summary>
        public string DefaultLanguageRuntime = "{provider}://Translations/{language}/{id}";

        /// <summary>
        /// 默认资源提供器名
        /// </summary>
        public string DefaultProvider = "Resources";

        static CompileConfiguration() {
            if (!File.Exists(RecordFilePath)) {
                File.CreateText(RecordFilePath).Close();
                Content = new CompileConfiguration();
                return;
            }
            var file = new FileStream(RecordFilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            Content = formatter.Deserialize(file) as CompileConfiguration;
            file.Close();
        }

        /// <summary>
        /// 解析模板字符串
        /// </summary>
        /// <param name="path">目标字符串</param>
        /// <param name="parts">模板替换项</param>
        /// <returns></returns>
        public static string ParseTemplate(string path, IEnumerable<KeyValuePair<string, string>> parts) {
            var list = parts.ToDictionary(e => e.Key, e => e.Value);
            if (!list.ContainsKey(TemplateItems.Language)) {
                list.Add(TemplateItems.Language, TranslationManager.DefaultLanguage);
            }
            if (!list.ContainsKey(TemplateItems.Provider)) {
                list.Add(TemplateItems.Provider, Content.DefaultProvider);
            }
            return path.ParseTemplate(list);
        }
        
        /// <summary>
        /// 保存脚本编译配置
        /// </summary>
        public static void Save() {
            var file = new FileStream(RecordFilePath, FileMode.Truncate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(file, Content);
            file.Close();
            MessageService.Process(new Message {Mask = CoreConstant.Mask, Tag = CoreConstant.RepaintCompileOptionEditor});
        }
        
        /// <summary>
        /// 清空所有脚本编译配置
        /// </summary>
        public static void Clear() {
            Content.Scripts.Clear();
            Save();
        }
        
        /// <summary>
        /// 模板项名
        /// </summary>
        public static class TemplateItems {
            /// <summary>
            /// 脚本ID
            /// </summary>
            public const string Id = "id";
            /// <summary>
            /// 语言
            /// </summary>
            public const string Language = "language";
            /// <summary>
            /// 提供器名
            /// </summary>
            public const string Provider = "provider";
        }
    }
}