using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;

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
        /// 源文件目录
        /// </summary>
        public string SourceFolder {
            get => _sourceFolder;
            set => _sourceFolder = NormalizePath(value);
        }
        
        /// <summary>
        /// 编译输出目录
        /// </summary>
        public string DistributionFolder {
            get => _distributionFolder;
            set => _distributionFolder = NormalizePath(value);
        }
        
        /// <summary>
        /// 翻译输出目录
        /// </summary>
        public string LanguageFolder {
            get => _languageFolder;
            set => _languageFolder = NormalizePath(value);
        }

        /// <summary>
        /// 默认运行时二进制加载URI
        /// </summary>
        public string DefaultRuntimeDistributionUri { get; set; } = "Resources://Logic/Distribution/{id}";
        
        /// <summary>
        /// 默认运行时翻译加载URI
        /// </summary>
        public string DefaultRuntimeLanguageUri { get; set; } = "Resources://Logic/Translations/{language}/{id}";

        /// <summary>
        /// 是否进入Play模式以及发布前时自动编译
        /// </summary>
        public bool AutoCompile { get; set; }

        private string _sourceFolder = "Resources/Logic/Source";
        private string _distributionFolder = "Resources/Logic/Distribution";
        private string _languageFolder = "Resources/Logic/Translation";

        static CompileConfiguration() {
            if (!File.Exists(RecordFilePath)) {
                File.CreateText(RecordFilePath).Close();
                Content = new CompileConfiguration();
                Save();
                return;
            }
            var file = new FileStream(RecordFilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            Content = formatter.Deserialize(file) as CompileConfiguration;
            file.Close();
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
        public static void ClearScripts() {
            Content.Scripts.Clear();
            Save();
        }

        private static string NormalizePath(string source) {
            if (string.IsNullOrEmpty(source)) return "Resources";
            source = source.UnifySlash();
            if (source.StartsWith("/")) {
                source = source.Substring(1);
            }
            if (source.EndsWith("/")) {
                source = source.Substring(0, source.Length - 1);
            }
            return string.IsNullOrEmpty(source) ? "Resources" : source;
        }
    }
}