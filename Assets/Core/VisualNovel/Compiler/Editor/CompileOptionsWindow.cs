using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;
using Core.MessageSystem;
using Core.VisualNovel.Runtime;
using Core.VisualNovel.Translation;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Compiler.Editor {
    public class CompileOptionsWindow : EditorWindow, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        private readonly Dictionary<string, ScriptEditorStatus> _isEditorOpened = new Dictionary<string, ScriptEditorStatus>();
        private Vector2 _scriptsScrollPosition = Vector2.zero;
        private LinkedTreeNode<IMessenger> _node;
        private string _newGlobalLanguage = "";

        public CompileOptionsWindow() {
            titleContent = new GUIContent("VNS Compile Options");
        }
        
        [MenuItem("Window/Visual Novel/Compile Options Viewer")]
        public static void ShowWindow() {
            GetWindowWithRect<CompileOptionsWindow>(new Rect(150, 50, 800, 450));
        }
        
        [MenuItem("Window/Visual Novel/Reload All Compile Options")]
        public static void Reload() {
            CompileOptions.Clear();
            ReloadDirectory("Assets/Resources");
            EditorUtility.DisplayDialog("Reload finished", "Successfully reloaded all script compile options", "Close");
        }

        private static void ReloadDirectory(string root) {
            foreach (var directory in Directory.GetDirectories(root)) {
                ReloadDirectory(directory);
            }
            foreach (var file in Directory.GetFiles(root).Where(e => e.EndsWith(".vns"))) {
                var target = CodeCompiler.CreatePathFromAsset(file);
                if (target == null) continue;
                CompileOptions.CreateOrUpdateScript(target);
            }
        }

        public Task<Message> Receive(Message message) {
            if (message.Tag == CoreConstant.RepaintCompileOptionEditor) {
                Repaint();
            }
            return Task.FromResult(message);
        }

        private void OnEnable() {
            titleContent.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/CompileOptionsWindow Icon.png") as Texture2D;
            _node = MessageService.Receivers.CreateChild(this);
        }
        
        public void OnDisable() {
            MessageService.Receivers.RemoveChild(_node);
        }


        private void OnGUI() {
            GUILayout.BeginHorizontal();
            // 左栏
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scripts", EditorStyles.boldLabel);
            // 文件列表
            _scriptsScrollPosition = GUILayout.BeginScrollView(_scriptsScrollPosition);
            GUILayout.BeginVertical();
            foreach (var (key, option) in CompileOptions.Options) {
                var target = CodeCompiler.CreatePathFromId(key);
                var content = new GUIContent(option.ExtraTranslationLanguages.Any()
                    ? $"  {target.SourceResource} (default, {string.Join(", ", option.ExtraTranslationLanguages)})"
                    : $"  {target.SourceResource} (default)");
                if (option.BinaryHash.HasValue) {
                    if (option.BinaryHash == option.SourceHash) {
                        content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/Compiled.png") as Texture2D;
                    } else {
                        content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/Outdated.png") as Texture2D;
                    }
                } else {
                    content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/NotCompile.png") as Texture2D;
                }
                if (!_isEditorOpened.ContainsKey(target.SourceResource)) {
                    _isEditorOpened.Add(target.SourceResource, new ScriptEditorStatus());
                }
                var scriptEditorStatus = _isEditorOpened[target.SourceResource];
                if (GUILayout.Button(content, new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft}, GUILayout.Height(25))) {
                    scriptEditorStatus.IsOpened = !scriptEditorStatus.IsOpened;
                }
                if (!scriptEditorStatus.IsOpened) continue;
                ++EditorGUI.indentLevel;
                scriptEditorStatus.LanguageName = ScriptImporterEditor.DrawLanguageGui(option, target, scriptEditorStatus.LanguageName, true);
                GUILayout.Space(5);
                --EditorGUI.indentLevel;
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            // 间隔
            GUILayout.Space(10);
            // 右栏
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Translation Coverage", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            var translations = CompileOptions.Options.Select(e => e.Value.ExtraTranslationLanguages).ToList();
            var translationCount = (float) translations.Count;
            EditorGUILayout.LabelField("default", "100.00%");
            foreach (var (key, rate) in translations.SelectMany(e => e).GroupBy(e => e).Select(e => (Key: e.Key, Rate: e.Count() / translationCount))) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key, rate.ToString("P2"));
                if (rate < 1.0 && GUILayout.Button("+", EditorStyles.miniButtonLeft)) {
                    if (EditorUtility.DisplayDialog("Create missing translation files", $"Would you want to create all missing files for translation \"{key}\"? This will precompile all script files.", "Continue", "Cancel")) {
                        PrecompileAll(false);
                        CreateGlobalTranslation(key);
                        GUILayout.BeginVertical(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
                        GUILayout.BeginHorizontal(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
                    }
                }
                if (GUILayout.Button("-", rate < 1.0 ? EditorStyles.miniButtonRight : EditorStyles.miniButton)) {
                    if (EditorUtility.DisplayDialog("Remove translation", $"Would you want to remove all files for translation \"{key}\"? This action cannot be reversed.", "Continue", "Cancel")) {
                        RemoveGlobalTranslation(key);
                        GUILayout.BeginVertical(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
                        GUILayout.BeginHorizontal(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            _newGlobalLanguage = EditorGUILayout.TextField(_newGlobalLanguage);
            if (GUILayout.Button("+", EditorStyles.miniButton)) {
                _newGlobalLanguage = CreateGlobalTranslation(_newGlobalLanguage);
            }
            GUILayout.EndHorizontal();
            --EditorGUI.indentLevel;
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
            if (GUILayout.Button("Precompile All")) {
                PrecompileAll();
            }
            if (GUILayout.Button("Recompile All (force)")) {
                if (EditorUtility.DisplayDialog("Force recompile all scripts?", "This action cannot be reversed.", "Continue", "Cancel")) {
                    PrecompileAll(true, true);
                    GUILayout.BeginHorizontal(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
                }
            }
            if (GUILayout.Button("Remove unavailable translation items")) {
                RemoveUnavailableTranslationItems();
                EditorGUILayout.BeginHorizontal(); // ? 不知为何不加这一句Unity会找不到水平布局，似乎前面什么代码有副作用把布局清了
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void RemoveGlobalTranslation(string languageName) {
            var totalCount = CompileOptions.Options.Count;
            var changedFiles = new List<string>();
            foreach (var ((key, _), i) in CompileOptions.Options.WithIndex()) {
                EditorUtility.DisplayProgressBar($"Removing translation \"{languageName}\"", $"{i}/{totalCount}", (float) i / totalCount);
                var script = CodeCompiler.CreatePathFromId(key);
                var targetFile = CodeCompiler.CreateLanguageAssetPathFromId(script.SourceResource, languageName);
                if (!File.Exists(targetFile)) continue;
                File.Delete(targetFile);
                changedFiles.Add(targetFile);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Translation remove finished",
                changedFiles.Any()
                    ? $"File changed:\n{string.Join("\n", changedFiles)}"
                    : "No missing translation file detected, skip creating",
                "Close");
        }

        private static string CreateGlobalTranslation(string languageName) {
            if (string.IsNullOrEmpty(languageName)) {
                EditorUtility.DisplayDialog("Invalid language", "Language name cannot be empty", "Close");
                return languageName;
            }
            var totalCount = CompileOptions.Options.Count;
            var changedFiles = new List<string>();
            foreach (var ((key, _), i) in CompileOptions.Options.WithIndex()) {
                EditorUtility.DisplayProgressBar($"Creating translation \"{languageName}\"", $"{i}/{totalCount}", (float) i / totalCount);
                var script = CodeCompiler.CreatePathFromId(key);
                var targetFile = CodeCompiler.CreateLanguageAssetPathFromId(script.SourceResource, languageName);
                if (File.Exists(targetFile)) continue;
                ScriptTranslation defaultTranslationContent;
                try {
                    var (header, _) = ScriptHeader.LoadAsset(script.SourceResource);
                    defaultTranslationContent = header.Translations[TranslationManager.DefaultLanguage];
                } catch (Exception) {
                    defaultTranslationContent = new ScriptTranslation("");
                }
                File.WriteAllText(targetFile, defaultTranslationContent.Pack(), Encoding.UTF8);
                changedFiles.Add(targetFile);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Translation create finished",
                changedFiles.Any()
                    ? $"File changed:\n{string.Join("\n", changedFiles)}"
                    : "No missing translation file detected, skip creating",
                "Close");
            return "";
        }

        private static void PrecompileAll(bool notice = true, bool force = false) {
            var totalCount = CompileOptions.Options.Count;
            var changedFiles = new List<string>();
            foreach (var ((key, option), i) in CompileOptions.Options.WithIndex()) {
                EditorUtility.DisplayProgressBar("Compiling", $"{i}/{totalCount}", (float) i / totalCount);
                var script = CodeCompiler.CreatePathFromId(key);
                var compileResult = CodeCompiler.CompileAsset(script.Source, option, force).ToList();
                if (compileResult.Count > 0) {
                    changedFiles.AddRange(compileResult);
                    CompileOptions.CreateOrUpdateScript(script);
                }
            }
            EditorUtility.ClearProgressBar();
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
            if (!EditorUtility.DisplayDialog("Remove all unavailable translation items?", "This will precompile all script files, and action cannot be reversed.", "Continue", "Cancel")) return;
            PrecompileAll(false);
            var totalCount = CompileOptions.Options.Count;
            var changedFiles = new List<string>();
            foreach (var ((key, option), i) in CompileOptions.Options.WithIndex()) {
                EditorUtility.DisplayProgressBar("Removing", $"{i}/{totalCount}", (float) i / totalCount);
                var script = CodeCompiler.CreatePathFromId(key);
                var translation = ScriptHeader.LoadAsset(script.SourceResource).Header.Translations[TranslationManager.DefaultLanguage];
                foreach (var language in option.ExtraTranslationLanguages) {
                    var languageFilePath = CodeCompiler.CreateLanguageAssetPathFromId(script.SourceResource, language);
                    if (!File.Exists(languageFilePath)) continue;
                    var existedTranslation = new ScriptTranslation(File.ReadAllText(languageFilePath));
                    if (!existedTranslation.RemoveUnavailableTranslations(translation)) continue;
                    File.WriteAllText(languageFilePath, existedTranslation.Pack(), Encoding.UTF8);
                    changedFiles.Add(languageFilePath);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Remove finished",
                changedFiles.Any()
                    ? $"File changed:\n{string.Join("\n", changedFiles)}"
                    : "All translation items are activated, skip removing",
                "Close");
        }

        private class ScriptEditorStatus {
            public bool IsOpened { get; set; }
            public string LanguageName { get; set; } = "";
        }
    }
}