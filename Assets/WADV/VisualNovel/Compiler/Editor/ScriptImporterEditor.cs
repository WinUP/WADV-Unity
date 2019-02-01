using System;
using System.Linq;
using System.Threading.Tasks;
using WADV.MessageSystem;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.ScriptStatus;

namespace WADV.VisualNovel.Compiler.Editor {
    [CustomEditor(typeof(ScriptImporter))]
    public class ScriptImporterEditor : ScriptedImporterEditor, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;
        public bool IsStandaloneMessage { get; } = false;
        private LinkedTreeNode<IMessenger> _node;
        private static readonly LinkedTreeNode<IMessenger> ImporterEditorRoot;
        
        private ScriptInformation _option;
        private bool _editMode;
        private string _customizedDistribution;

        static ScriptImporterEditor() {
            ImporterEditorRoot = MessageService.Receivers.CreateChild(new EmptyMessenger());
        }
        
        public Task<Message> Receive(Message message) {
            if (message.Tag == CoreConstant.RepaintCompileOptionEditor) {
                Repaint();
            }
            return Task.FromResult(message);
        }

        public override void OnEnable() {
            _node = ImporterEditorRoot.CreateChild(this);
            base.OnEnable();
            var importer = target as ScriptImporter;
            if (importer == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            _option = ScriptInformation.CreateInformationFromAsset(importer.assetPath);
            if (_option == null) return;
            _customizedDistribution = _option.DistributionTargetTemplate();
        }

        public override void OnDisable() {
            ImporterEditorRoot.RemoveChild(_node);
            base.OnDisable();
        }

        public override void OnInspectorGUI() {
            // ID
            if (_option == null) {
                EditorGUILayout.LabelField("Source Folder", $"Assets/{CompileConfiguration.Content.SourceFolder}");
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Put in source folder to enable script and editor ui", MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Global Options")) {
                    CreateInstance<CompileOptionsWindow>();
                }
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", _option.Id);
            if (GUILayout.Button("Refresh", EditorStyles.miniButton)) {
                CompileOptionsWindow.RescanScriptInformation(_option);
            }
            EditorGUILayout.EndHorizontal();
            // 源状态
            EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            EditorGUILayout.LabelField("Status", _option.HasSource() ? "Available" : "Not Found");
            if (_option.HasSource()) {
                EditorGUILayout.LabelField("Target", _option.SourceAssetPath());
            }
            --EditorGUI.indentLevel;
            // 编译状态
            EditorGUILayout.LabelField("Binary", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            if (!_option.HasSource()) {
                EditorGUILayout.LabelField("Status", "Standalone Library");
            } else if (_option.HasBinary()) {
                EditorGUILayout.LabelField("Status",_option.RecordedHash == _option.Hash ? "Compiled" : "Need Recompile");
            } else {
                EditorGUILayout.LabelField("Status", "Not Found");
            }
            if (_option.HasBinary()) {
                EditorGUILayout.LabelField("Target", _option.BinaryAssetPath());
                if (_editMode) {
                    _customizedDistribution = EditorGUILayout.TextField("Runtime URI", _customizedDistribution);
                } else {
                    EditorGUILayout.LabelField("Runtime URI", _option.DistributionTargetUri());
                }
            }
            --EditorGUI.indentLevel;
            // 翻译
            EditorGUILayout.LabelField("Translations", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            if (_option.Translations.Any()) {
                if (_editMode) {
                
                } else {
                    foreach (var (key, _) in _option.Translations) {
                        EditorGUILayout.LabelField(key, _option.LanguageUri(key));
                    }
                }
            } else {
                EditorGUILayout.LabelField("No Translation Detected");
            }
            --EditorGUI.indentLevel;
            // 工具按钮
            EditorGUILayout.BeginHorizontal();
            if (_editMode) {
                if (GUILayout.Button("Save")) {
                    if (string.IsNullOrEmpty(_customizedDistribution) || _customizedDistribution == CompileConfiguration.Content.DefaultRuntimeDistributionUri) {
                        _option.DistributionTarget = null;
                    } else {
                        _option.DistributionTarget = _customizedDistribution;
                    }
                    CompileConfiguration.Save();
                    _editMode = false;
                }
            } else {
                if (GUILayout.Button("Edit")) {
                    if (string.IsNullOrEmpty(_customizedDistribution)) {
                        _customizedDistribution = _option.DistributionTargetTemplate();
                    }
                    _editMode = true;
                }
            }
            if (GUILayout.Button("Compile")) {
                try {
                    var changedFiles = CodeCompiler.CompileAsset(_option.SourceAssetPath()).ToArray();
                    EditorUtility.DisplayDialog(
                        "Compile finished",
                        changedFiles.Any()
                            ? $"File changed:\n{string.Join("\n", changedFiles)}"
                            : "No change detected, skip compilation",
                        "Close");
                } catch (CompileException compileException) {
                    EditorUtility.DisplayDialog("Script has error", compileException.Message, "Close");
                } catch (Exception exception) {
                    EditorUtility.DisplayDialog("Unknown exception", exception.Message, "Close");
                }
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }

//        public static string DrawLanguageGui(ScriptCompileOption option, CodeCompiler.ScriptPaths script, string languageName, bool showOpenButton = false) {
//            EditorGUILayout.BeginVertical();
//            if (showOpenButton) {
//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField("Available Translations", EditorStyles.boldLabel);
//                if (GUILayout.Button("Open", EditorStyles.miniButton)) {
//                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(script.Source, 0);
//                }
//                EditorGUILayout.EndHorizontal();
//            } else {
//                EditorGUILayout.LabelField("Available Translations", EditorStyles.boldLabel);
//            }
//            ++EditorGUI.indentLevel;
//            EditorGUILayout.LabelField("default", "build-in resource");
//            foreach (var language in option.ExtraTranslationLanguages.ToArray()) {
//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField(language, "text file");
//                if (GUILayout.Button("-", EditorStyles.miniButton)) {
//                    if (EditorUtility.DisplayDialog($"Remove \"{language}\" translation from {script.SourceResource}?", "This action cannot be reversed", "Continue", "Cancel")) {
//                        AssetDatabase.DeleteAsset(CodeCompiler.CreateLanguageAssetPathFromId(script.SourceResource, language));
//                    }
//                }
//                EditorGUILayout.EndHorizontal();
//            }
//            EditorGUILayout.BeginHorizontal();
//            languageName = EditorGUILayout.TextField(languageName);
//            if (GUILayout.Button("+", EditorStyles.miniButton)) {
//                if (string.IsNullOrEmpty(languageName)) {
//                    EditorUtility.DisplayDialog("Invalid language", "Language name cannot be empty", "Close");
//                } else if (option.ExtraTranslationLanguages.Contains(languageName)) {
//                    EditorUtility.DisplayDialog("Language name conflict", $"Language {languageName} is already existed", "Close");
//                } else {
//                    ScriptTranslation defaultTranslationContent;
//                    try {
//                        var (header, _) = ScriptHeader.Load(script.SourceResource);
//                        defaultTranslationContent = header.Translations[TranslationManager.DefaultLanguage];
//                    } catch (Exception e) {
//                        Debug.LogError(e);
//                        defaultTranslationContent = new ScriptTranslation("");
//                    }
//                    File.WriteAllText(CodeCompiler.CreateLanguageAssetPathFromId(script.SourceResource, languageName), defaultTranslationContent.Pack(), Encoding.UTF8);
//                    AssetDatabase.Refresh();
//                    languageName = "";
//                }
//            }
//            EditorGUILayout.EndHorizontal();
//            --EditorGUI.indentLevel;
//            EditorGUILayout.EndVertical();
//            return languageName;
//        }
    }
}