using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Core.VisualNovel.Script.Compiler;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Script {
    /// <summary>
    /// 此项目中所有VNS文件的编译选项
    /// </summary>
    public static class CompileOptions {
        private static readonly string RecordFilePath = Application.streamingAssetsPath + "/VisualNovelScriptDefaultCompileOptions.bytes";
        
        private static Dictionary<string, ScriptCompileOption> Options { get; } = new Dictionary<string, ScriptCompileOption>();
        public static IReadOnlyDictionary<string, ScriptCompileOption> Collection { get; } = Options;

        static CompileOptions() {
            if (!File.Exists(RecordFilePath)) return;
            var file = new FileStream(RecordFilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            Options = formatter.Deserialize(file) as Dictionary<string, ScriptCompileOption>;
            file.Close();
        }

        [MenuItem("Window/Visual Novel/Reload All Compile Options")]
        public static void Reload() {
            
        }
        
        public static bool Has(string id) {
            return Options.ContainsKey(id);
        }

        public static ScriptCompileOption Get(string id) {
            ScriptCompileOption option;
            if (!Options.ContainsKey(id)) {
                option = new ScriptCompileOption();
                Options.Add(id, option);
                Save();
            } else {
                option = Options[id];
            }
            return option;
        }

        public static void Remove(string id) {
            if (Options.ContainsKey(id)) {
                Options.Remove(id);
            }
        }

        public static void Save() {
            var file = new FileStream(RecordFilePath, FileMode.Truncate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(file, Options);
            file.Close();
        }
    }
}