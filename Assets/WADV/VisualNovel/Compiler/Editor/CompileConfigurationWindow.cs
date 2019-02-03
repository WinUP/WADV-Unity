using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.VisualNovel.ScriptStatus;
using UnityEditor;
using UnityEngine;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Compiler.Editor {
    public class CompileConfigurationWindow : EditorWindow, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;
        public bool IsStandaloneMessage { get; } = false;
        private LinkedTreeNode<IMessenger> _node;
        private Vector2 _scriptsScrollPosition = Vector2.zero;
        private ConfigurationCache _configurationCache;
        private GUIStyle _centerLabel;
        private string _newLanguage;
        
        private void OnEnable() {
            titleContent.image = EditorGUIUtility.Load("Script Editor/CompileOptionsWindow Icon.png") as Texture2D;
            _node = MessageService.Receivers.CreateChild(this);
            ResetCache();
        }
        
        private void OnDisable() {
            MessageService.Receivers.RemoveChild(_node);
        }
        
        public Task<Message> Receive(Message message) {
            if (message.Tag == CoreConstant.RepaintCompileOptionEditor) {
                Repaint();
            }
            return Task.FromResult(message);
        }

        [MenuItem("Window/Visual Novel/Compile Configuration")]
        public static void ShowWindow() {
            GetWindowWithRect<CompileConfigurationWindow>(new Rect(150, 50, 800, 450));
        }

        [MenuItem("Window/Visual Novel/Rescan Project")]
        public static void Rescan() {
            CompileConfiguration.ClearScripts();
            ReloadDirectory("Assets/Resources");
            CompileConfiguration.Save();
            var content = "Found scripts:\n\n" + string.Join("\n", CompileConfiguration.Content.Scripts.Keys.Take(20));
            if (CompileConfiguration.Content.Scripts.Count > 20) {
                content += "\n...more";
            }
            EditorUtility.DisplayDialog("Rescan finished", content, "Close");
        }

        /// <summary>
        /// 重新扫描项目并更新目标脚本的信息
        /// </summary>
        /// <param name="option">目标脚本信息</param>
        /// <param name="save">是否扫描完成后自动保存</param>
        public static void RescanScriptInformation(ScriptInformation option, bool save = true) {
            var source = option.SourceAssetPath();
            if (!string.IsNullOrEmpty(source)) {
                if (File.Exists(source)) {
                    option.Hash = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(source, Encoding.UTF8).UnifyLineBreak()));
                } else {
                    option.Hash = null;
                }
            }
            var binary = option.BinaryAssetPath();
            if (!string.IsNullOrEmpty(binary)) {
                if (File.Exists(source)) {
                    var stream = new FileStream(binary, FileMode.Open);
                    var hash = ScriptInformation.ReadBinaryHash(stream);
                    stream.Close();
                    option.RecordedHash = hash;
                } else {
                    option.RecordedHash = null;
                }
            }
            var detectedLanguage = new List<string>();
            foreach (var language in Directory.GetDirectories($"Assets/{CompileConfiguration.Content.TranslationFolder}").Select(Path.GetFileName)) {
                if (File.Exists($"Assets/{CompileConfiguration.Content.TranslationFolder}/{language}/{option.Id}.txt")) {
                    if (!option.Translations.ContainsKey(language)) {
                        option.Translations.Add(language, null);
                    }
                    detectedLanguage.Add(language);
                } else {
                    if (option.Translations.ContainsKey(language)) {
                        option.Translations.Remove(language);
                    }
                }
            }
            var needRemove = new List<string>();
            foreach (var (key, _) in option.Translations) {
                if (!detectedLanguage.Contains(key)) {
                    needRemove.Add(key);
                }
            }
            foreach (var key in needRemove) {
                option.Translations.Remove(key);
            }
            if (!option.HasSource() && !option.HasBinary()) {
                CompileConfiguration.Content.Scripts.Remove(option.Id);
            }
            if (save) {
                CompileConfiguration.Save();
            }
        }
        
        public CompileConfigurationWindow() {
            titleContent = new GUIContent("VNS/VNB Compile Configuration");
        }

        private void OnGUI() {
            CreateStyle();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            DrawScriptTable();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            DrawDefaultCompileConfiguration();
            EditorGUILayout.Space();
            DrawTranslationTool();
            EditorGUILayout.Space();
            DrawCompilerSetting();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void CreateStyle() {
            if (_centerLabel == null) {
                _centerLabel = new GUIStyle(GUI.skin.GetStyle("Label")) {alignment = TextAnchor.MiddleCenter};
            }
        }

        private void DrawScriptTable() {
             _scriptsScrollPosition = GUILayout.BeginScrollView(_scriptsScrollPosition);
            GUILayout.BeginHorizontal();
            // ID
            GUILayout.BeginVertical();
            GUILayout.Button("ID", EditorStyles.miniButtonLeft);
            foreach (var (key, _) in CompileConfiguration.Content.Scripts) {
                GUILayout.Label(key, _centerLabel);
            }
            GUILayout.EndVertical();
            // Source
            GUILayout.BeginVertical();
            GUILayout.Button("Source", EditorStyles.miniButtonMid);
            foreach (var (_, info) in CompileConfiguration.Content.Scripts) {
                GUILayout.Label(info.HasSource() ? "Available" : "Not Found", _centerLabel);
            }
            GUILayout.EndVertical();
            // Binary
            GUILayout.BeginVertical();
            GUILayout.Button("Binary", EditorStyles.miniButtonMid);
            foreach (var (_, info) in CompileConfiguration.Content.Scripts) {
                if (!info.HasSource()) {
                    GUILayout.Label("Standalone Library", _centerLabel);
                } else if (info.HasBinary()) {
                    GUILayout.Label(info.RecordedHash == info.Hash ? "Compiled" : "Need Recompile", _centerLabel);
                } else {
                    GUILayout.Label("Not Found", _centerLabel);
                }
            }
            GUILayout.EndVertical();
            // Translations
            GUILayout.BeginVertical();
            GUILayout.Button("Translations", EditorStyles.miniButtonRight);
            foreach (var (_, info) in CompileConfiguration.Content.Scripts) {
                GUILayout.Label(info.Translations.Count > 4
                                    ? $"Totally {info.Translations.Count}"
                                    : string.Join(" ", info.Translations.Keys)
                                , _centerLabel);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private void DrawDefaultCompileConfiguration() {
            EditorGUILayout.LabelField("Project Configuration", EditorStyles.boldLabel);
            if (_configurationCache.InEdit) {
                _configurationCache.SourceFolder = EditorGUILayout.TextField("Source Folder", _configurationCache.SourceFolder);
                _configurationCache.DistributionFolder = EditorGUILayout.TextField("Distribution Folder", _configurationCache.DistributionFolder);
                _configurationCache.TranslationFolder = EditorGUILayout.TextField("Translation Folder", _configurationCache.TranslationFolder);
                _configurationCache.RuntimeDistributionUri = EditorGUILayout.TextField("Runtime Distribution URI", _configurationCache.RuntimeDistributionUri);
                _configurationCache.RuntimeTranslationUri = EditorGUILayout.TextField("Runtime Translation URI", _configurationCache.RuntimeTranslationUri);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save & Rescan")) {
                    if (ApplyCache()) {
                        _configurationCache.InEdit = false;
                    }
                }
                if (GUILayout.Button("Cancel")) {
                    ResetCache();
                    _configurationCache.InEdit = false;
                }
                EditorGUILayout.EndHorizontal();
            } else {
                EditorGUILayout.LabelField("Source Folder", _configurationCache.SourceFolder);
                EditorGUILayout.LabelField("Distribution Folder", _configurationCache.DistributionFolder);
                EditorGUILayout.LabelField("Translation Folder", _configurationCache.TranslationFolder);
                EditorGUILayout.LabelField("Default Distribution URI", _configurationCache.RuntimeDistributionUri);
                EditorGUILayout.LabelField("Default Translation URI", _configurationCache.RuntimeTranslationUri);
                if (GUILayout.Button("Edit")) {
                    ResetCache();
                    _configurationCache.InEdit = true;
                }
            }
        }
        
        private void DrawTranslationTool() {
            EditorGUILayout.LabelField("Translation Coverage", EditorStyles.boldLabel);
            var translations = CompileConfiguration.Content.Scripts.Select(e => e.Value.Translations).ToList();
            var existedLanguageName = new List<string>();
            foreach (var (key, count, rate) in translations.SelectMany(e => e)
                                                 .GroupBy(e => e.Key)
                                                 .Select(e => (Key: e.Key, Count: e.Count(), Rate: e.Count() / (float) translations.Count))) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(key, "ProgressBarBack", _centerLabel);
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false,  EditorGUIUtility.singleLineHeight), rate, $"{count} / {translations.Count}");
                if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.MaxWidth(75))) {
                    RemoveTranslation(key);
                }
                EditorGUILayout.EndHorizontal();
                existedLanguageName.Add(key);
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("New Translation", "TextField",  _centerLabel);
            _newLanguage = EditorGUILayout.TextField(_newLanguage);
            if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.MaxWidth(100))) {
                CreateTranslation(existedLanguageName);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Remove Unavailable Translation Items")) {
                RemoveUnavailableTranslationItems();
            }
        }

        private void CreateTranslation(ICollection<string> existedLanguageName) {
            if (!TranslationManager.CheckLanguageName(_newLanguage)) {
                EditorUtility.DisplayDialog("Unable to add translation", $"Translation name can only contains A-Z, a-z, 0-9, _", "Close");
            } else if (existedLanguageName.Contains(_newLanguage)) {
                EditorUtility.DisplayDialog("Unable to add translation", $"Translation {_newLanguage} already existed", "Close");
            } else {
                foreach (var (_, info) in CompileConfiguration.Content.Scripts) {
                    info.Translations.Add(_newLanguage, null);
                    info.CreateTranslationFile(_newLanguage);
                }
                _newLanguage = "";
                CompileConfiguration.Save();
                AssetDatabase.Refresh();
            }
        }

        private static void DrawCompilerSetting() {
            EditorGUILayout.LabelField("Compiler", EditorStyles.boldLabel);
            CompileConfiguration.Content.AutoCompile = EditorGUILayout.Toggle("Compile When Import", CompileConfiguration.Content.AutoCompile);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Compile All")) {
                PrecompileAll();
            }
            if (GUILayout.Button("Recompile All")) {
                PrecompileAll(force: true);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private static void RemoveTranslation(string language) {
            if (!EditorUtility.DisplayDialog($"Delete translation {language}", "This action cannot be reversed", "Remove", "Cancel")) return;
            foreach (var (_, info) in CompileConfiguration.Content.Scripts.Where(e => e.Value.Translations.ContainsKey(language))) {
                info.Translations.Remove(language);
                info.RemoveTranslationFile(language);
            }
            CompileConfiguration.Save();
            AssetDatabase.Refresh();
        }
        
        private static void ReloadDirectory(string root) {
            foreach (var directory in Directory.GetDirectories(root)) {
                ReloadDirectory(directory);
            }
            foreach (var file in Directory.GetFiles(root).Where(e => e.EndsWith(".vns") || e.EndsWith(".vnb"))) {
                RescanScriptInformation(ScriptInformation.CreateInformationFromAsset(file), false);
            }
        }

        private static void PrecompileAll(bool notice = true, bool force = false) {
            var changedFiles = new List<string>();
            foreach (var (_, info) in CompileConfiguration.Content.Scripts.Where(e => e.Value.HasSource())) {
                var compileResult = CodeCompiler.CompileAsset(info.SourceAssetPath(), force).ToList();
                if (compileResult.Count > 0) {
                    changedFiles.AddRange(compileResult);
                }
            }
            AssetDatabase.Refresh();
            if (!notice) return;
            EditorUtility.DisplayDialog(
                "Compile finished",
                changedFiles.Any()
                    ? $"File changed:\n{string.Join("\n", changedFiles)}"
                    : "No change detected, skip compilation",
                "Close");
        }
        
        private static void RemoveUnavailableTranslationItems() {
            if (!EditorUtility.DisplayDialog("Remove all unavailable translation items", "This will launch WADV resource system and precompile all script files, which cannot be reversed.\n\nPlease make sure all scripts can be load properly before continue.", "Continue", "Cancel")) return;
            PrecompileAll(false);
            var changedFiles = new List<string>();
            foreach (var (key, info) in CompileConfiguration.Content.Scripts) {
                var translation = ScriptHeader.LoadSync(key).Header.LoadDefaultTranslation();
                foreach (var (language, _) in info.Translations) {
                    var languageFilePath = info.LanguageAssetPath(language);
                    if (!File.Exists(languageFilePath)) continue;
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(languageFilePath));
                    if (!existedTranslation.RemoveUnavailableTranslations(translation)) continue;
                    existedTranslation.SaveToAsset(languageFilePath);
                    changedFiles.Add(languageFilePath);
                }
            }
            EditorUtility.DisplayDialog(
                "Remove finished",
                changedFiles.Any()
                    ? $"File changed:\n{string.Join("\n", changedFiles)}"
                    : "All translation items are activated, skip removing",
                "Close");
            AssetDatabase.Refresh();
        }

        private bool ApplyCache() {
            bool IsDifferent(string a, string b) => !string.IsNullOrEmpty(a) && a != b;
            
            if (IsDifferent(_configurationCache.SourceFolder, CompileConfiguration.Content.SourceFolder)) {
                if (!Directory.Exists($"Assets/{_configurationCache.SourceFolder}")) {
                    EditorUtility.DisplayDialog("Set source folder failed", $"Folder {_configurationCache.SourceFolder} not existed", "Continue");
                    return false;
                }
                CompileConfiguration.Content.SourceFolder = _configurationCache.SourceFolder;
            }
            if (IsDifferent(_configurationCache.DistributionFolder, CompileConfiguration.Content.DistributionFolder)) {
                if (!Directory.Exists($"Assets/{_configurationCache.DistributionFolder}")) {
                    EditorUtility.DisplayDialog("Set distribution folder failed", $"Folder {_configurationCache.DistributionFolder} not existed", "Continue");
                    return false;
                }
                CompileConfiguration.Content.DistributionFolder = _configurationCache.DistributionFolder;
            }
            if (IsDifferent(_configurationCache.TranslationFolder, CompileConfiguration.Content.TranslationFolder)) {
                if (!Directory.Exists($"Assets/{_configurationCache.TranslationFolder}")) {
                    EditorUtility.DisplayDialog("Set translation folder failed", $"Folder {_configurationCache.TranslationFolder} not existed", "Continue");
                    return false;
                }
                CompileConfiguration.Content.TranslationFolder = _configurationCache.TranslationFolder;
            }
            if (IsDifferent(_configurationCache.RuntimeDistributionUri, CompileConfiguration.Content.DefaultRuntimeDistributionUri)) {
                CompileConfiguration.Content.DefaultRuntimeDistributionUri = _configurationCache.RuntimeDistributionUri;
            }
            if (IsDifferent(_configurationCache.RuntimeTranslationUri, CompileConfiguration.Content.DefaultRuntimeTranslationUri)) {
                CompileConfiguration.Content.DefaultRuntimeTranslationUri = _configurationCache.RuntimeTranslationUri;
            }
            ResetCache();
            Rescan();
            return true;
        }

        private void ResetCache() {
            _configurationCache.SourceFolder = CompileConfiguration.Content.SourceFolder;
            _configurationCache.DistributionFolder = CompileConfiguration.Content.DistributionFolder;
            _configurationCache.TranslationFolder = CompileConfiguration.Content.TranslationFolder;
            _configurationCache.RuntimeTranslationUri = CompileConfiguration.Content.DefaultRuntimeTranslationUri;
            _configurationCache.RuntimeDistributionUri = CompileConfiguration.Content.DefaultRuntimeDistributionUri;
        }

        private struct ConfigurationCache {
            public bool InEdit;
            public string SourceFolder;
            public string DistributionFolder;
            public string TranslationFolder;
            public string RuntimeDistributionUri;
            public string RuntimeTranslationUri;
        }
    }
}