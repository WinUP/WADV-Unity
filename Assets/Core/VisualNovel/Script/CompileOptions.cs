using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Core.VisualNovel.Script.Compiler;
using UnityEngine;

namespace Core.VisualNovel.Script {
    public static class CompileOptions {
        public static string SavedPath { get; } = Application.streamingAssetsPath + "/VisualNovelScriptDefaultCompileOptions.bytes";
        
        private static Dictionary<string, CompileOption> Options { get; } = new Dictionary<string, CompileOption>();

        static CompileOptions() {
            if (File.Exists(SavedPath)) {
                var file = new FileStream(SavedPath, FileMode.Open);
                var formatter = new BinaryFormatter();
                Options = formatter.Deserialize(file) as Dictionary<string, CompileOption>;
                file.Close();
            } else {
                var file = new FileStream(SavedPath, FileMode.Create);
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, Options);
                file.Close();
            }
        }

        public static bool Has(string id) {
            return Options.ContainsKey(id);
        }

        public static CompileOption Get(string id) {
            CompileOption option;
            if (!Options.ContainsKey(id)) {
                option = new CompileOption();
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
            var file = new FileStream(SavedPath, FileMode.Truncate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(file, Options);
            file.Close();
        }
    }
}