using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Core.MessageSystem;
using UnityEngine;

namespace Core.VisualNovel.Compiler {
    /// <summary>
    /// 此项目中所有VNS文件的编译选项
    /// </summary>
    public static class CompileOptions {
        private static readonly string RecordFilePath = Application.streamingAssetsPath + "/VisualNovelScriptDefaultCompileOptions.bin";
        
        public static readonly Dictionary<string, ScriptCompileOption> Options = new Dictionary<string, ScriptCompileOption>();

        static CompileOptions() {
            if (!File.Exists(RecordFilePath)) {
                File.CreateText(RecordFilePath).Close();
                return;
            }
            var file = new FileStream(RecordFilePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            Options = formatter.Deserialize(file) as Dictionary<string, ScriptCompileOption>;
            file.Close();
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

        public static void Rename(string from, string to) {
            if (!Has(from) || Has(to)) {
                throw new ArgumentException($"Cannot rename compile option {from} -> {to}: Source unavailable or Name existed");
            }
            Options.Add(to, Options[from]);
            Options.Remove(from);
            Save();
        }

        public static void Rename(CodeCompiler.ScriptPaths from, CodeCompiler.ScriptPaths to) {
            Rename(from.SourceResource, to.SourceResource);
        }

        public static void Remove(string id) {
            if (!Options.ContainsKey(id)) return;
            Options.Remove(id);
            Save();
        }

        public static void Remove(CodeCompiler.ScriptPaths target) {
            Remove(target.SourceResource);
        }
        
        public static void CreateOrUpdateScript(CodeCompiler.ScriptPaths target) {
            var option = Get(target.SourceResource);
            var language = (from e in CodeCompiler.FilterAssetFromId(Directory.GetFiles(target.Directory), target.SourceResource)
                where !string.IsNullOrEmpty(e.Language) && !option.ExtraTranslationLanguages.Contains(e.Language)
                select e.Language).ToList();
            if (language.Any()) {
                option.ExtraTranslationLanguages.AddRange(language);
            }
            option.SourceHash = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(target.Source, Encoding.UTF8)));
            UpdateBinaryHash(target);
        }
        
        public static void UpdateBinaryHash(CodeCompiler.ScriptPaths target) {
            if (!Has(target.SourceResource)) return;
            Get(target.SourceResource).BinaryHash = CodeCompiler.ReadBinaryHash(target.Binary);
            Save();
        }

        public static void ApplyLanguage(CodeCompiler.ScriptPaths target) {
            if (string.IsNullOrEmpty(target.Language) || !Has(target.SourceResource)) return;
            var option = Get(target.SourceResource);
            if (option.ExtraTranslationLanguages.Contains(target.Language)) return;
            option.ExtraTranslationLanguages.Add(target.Language);
            Save();
        }

        public static void RemoveLanguage(CodeCompiler.ScriptPaths target) {
            if (string.IsNullOrEmpty(target.Language) || !Has(target.SourceResource)) return;
            var option = Get(target.SourceResource);
            if (!option.ExtraTranslationLanguages.Contains(target.Language)) return;
            option.ExtraTranslationLanguages.Remove(target.Language);
            Save();
        }

        public static void Save() {
            var file = new FileStream(RecordFilePath, FileMode.Truncate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(file, Options);
            file.Close();
            MessageService.Process(new Message {Mask = CoreConstant.Mask, Tag = CoreConstant.RepaintCompileOptionEditor});
        }

        public static void Clear() {
            Options.Clear();
            Save();
        }
    }
}