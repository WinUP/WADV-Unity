using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.VisualNovel.ScriptStatus;
using WADV.VisualNovel.Translation;

namespace WADV.VisualNovel.Compiler.Editor {
    [CustomEditor(typeof(ScriptImporter))]
    public class ScriptImporterEditor : ScriptImporterEditorBase { }
    
    [CustomEditor(typeof(ScriptBinaryImporter))]
    public class ScriptBinaryImporterEditor : ScriptImporterEditorBase { }
    
    public abstract class ScriptImporterEditorBase : ScriptedImporterEditor, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;
        public bool IsStandaloneMessage { get; } = false;
        private LinkedTreeNode<IMessenger> _node;
        private static readonly LinkedTreeNode<IMessenger> ImporterEditorRoot;
        
        private ScriptInformation _option;
        private bool _editMode;
        private string _customizedDistribution;
        private string _newLanguage;
        private readonly List<(string Key, string Value)> _customizedLanguage = new List<(string Key, string Value)>();

        static ScriptImporterEditorBase() {
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
            var script = target as ScriptImporter;
            var binary = target as ScriptBinaryImporter;
            var path = script != null ? script.assetPath : binary != null ? binary.assetPath : null;
            if (path == null) {
                throw new TypeLoadException("Inspected type is not ScriptImporter or ScriptBinaryImporter");
            }
            _option = ScriptInformation.CreateInformationFromAsset(path);
            if (_option == null) return;
            _customizedDistribution = _option.DistributionTargetTemplate();
            foreach (var (key, _) in _option.Translations) {
                _customizedLanguage.Add((key, _option.LanguageTemplate(key)));
            }
        }

        public override void OnDisable() {
            ImporterEditorRoot.RemoveChild(_node);
            base.OnDisable();
        }

        public override void OnInspectorGUI() {
            // ID
            if (_option == null) {
                EditorGUILayout.LabelField("Source Folder", $"Assets/{CompileConfiguration.Content.SourceFolder}");
                EditorGUILayout.LabelField("Distribution Folder", $"Assets/{CompileConfiguration.Content.DistributionFolder}");
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Put vns in source folder/vnb in distribution folder to enable script and editor ui", MessageType.Warning);
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Global Options")) {
                    CompileOptionsWindow.ShowWindow();
                }
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", _option.Id);
            if (!_editMode && GUILayout.Button("Refresh", EditorStyles.miniButton)) {
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
                    for (var i = -1; ++i < _customizedLanguage.Count;) {
                        var (key, value) = _customizedLanguage[i];
                        EditorGUILayout.BeginHorizontal();
                        _customizedLanguage[i] = (key, EditorGUILayout.TextField(key, value));
                        if (GUILayout.Button("-", EditorStyles.miniButton)) {
                            if (OnRemoveTranslationClicked(key)) {
                                --i;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.BeginHorizontal();
                    _newLanguage = EditorGUILayout.TextField(_newLanguage);
                    if (GUILayout.Button("Add")) {
                        OnAddTranslationClicked();
                    }
                    EditorGUILayout.EndHorizontal();
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
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (_editMode) {
                if (GUILayout.Button("Save")) {
                    OnSaveClicked();
                }
            } else {
                if (GUILayout.Button("Edit")) {
                    OnEditClicked();
                }
            }
            if (!_editMode && GUILayout.Button("Compile")) {
                OnCompileClicked();
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool OnRemoveTranslationClicked(string key) {
            if (!EditorUtility.DisplayDialog($"Delete translation {key}", "This action cannot be reversed", "Continue", "Cancel")) return false;
            _option.Translations.Remove(key);
            _customizedLanguage.RemoveAll(e => e.Key == key);
            _option.RemoveTranslationFile(key);
            CompileConfiguration.Save();
            AssetDatabase.Refresh();
            return true;
        }

        private void OnAddTranslationClicked() {
            if (!TranslationManager.CheckLanguageName(_newLanguage)) {
                EditorUtility.DisplayDialog("Unable to add translation", $"Translation name can only contains A-Z, a-z, 0-9, _", "Close");
            } else if (_option.Translations.ContainsKey(_newLanguage)) {
                EditorUtility.DisplayDialog("Unable to add translation", $"Translation {_newLanguage} already existed", "Close");
            } else {
                _option.Translations.Add(_newLanguage, null);
                _customizedLanguage.Add((_newLanguage, _option.LanguageTemplate(_newLanguage)));
                _option.CreateTranslationFile(_newLanguage);
                _newLanguage = "";
                CompileConfiguration.Save();
                AssetDatabase.Refresh();
            }
        }

        private void OnSaveClicked() {
            if (string.IsNullOrEmpty(_customizedDistribution) || _customizedDistribution == CompileConfiguration.Content.DefaultRuntimeDistributionUri) {
                _option.DistributionTarget = null;
            } else {
                _option.DistributionTarget = _customizedDistribution;
            }
            for (var i = -1; ++i < _customizedLanguage.Count;) {
                var (key, value) = _customizedLanguage[i];
                if (string.IsNullOrEmpty(value) || value == CompileConfiguration.Content.DefaultRuntimeLanguageUri) {
                    _option.Translations[key] = null;
                } else {
                    _option.Translations[key] = value;
                }
            }
            CompileConfiguration.Save();
            _editMode = false;
        }

        private void OnEditClicked() {
            if (string.IsNullOrEmpty(_customizedDistribution)) {
                _customizedDistribution = _option.DistributionTargetTemplate();
            }
            for (var i = -1; ++i < _customizedLanguage.Count;) {
                var (key, value) = _customizedLanguage[i];
                if (string.IsNullOrEmpty(value)) {
                    _customizedLanguage[i] = (key, _option.LanguageTemplate(key));
                }
            }
            _editMode = true;
        }

        private void OnCompileClicked() {
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
    }
}