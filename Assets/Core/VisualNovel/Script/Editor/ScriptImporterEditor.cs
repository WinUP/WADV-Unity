#pragma warning disable 1998

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;
using Core.MessageSystem;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Translation;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    [CustomEditor(typeof(ScriptImporter))]
    public class ScriptImporterEditor : ScriptedImporterEditor, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;
        
        private string _newLanguage;
        private LinkedTreeNode<IMessenger> _node;
        private static readonly LinkedTreeNode<IMessenger> ImporterEditorRoot;

        static ScriptImporterEditor() {
            ImporterEditorRoot = MessageService.Receivers.CreateChild(new EmptyMessenger());
        }
        
        public async Task<Message> Receive(Message message) {
            if (message.Tag == CoreConstant.ReloadAllCompileOptionsTag) {
                Repaint();
            }
            return message;
        }

        public override void OnEnable() {
            _node = ImporterEditorRoot.CreateChild(this);
            base.OnEnable();
        }

        public override void OnDisable() {
            ImporterEditorRoot.RemoveChild(_node);
            base.OnDisable();
        }

        public override void OnInspectorGUI() {
            var importer = target as ScriptImporter;
            if (importer == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            // 显示ID
            var assetPaths = CodeCompiler.CreatePathFromAsset(importer.assetPath);
            if (assetPaths == null) {
                EditorGUILayout.LabelField("Error: Unable to parse source file");
                return;
            }
            var option = CompileOptions.Get(assetPaths.SourceResource);
            EditorGUILayout.LabelField("Resource ID", assetPaths.SourceResource);
            // 检查编译状态
            if (option.BinaryHash.HasValue) {
                EditorGUILayout.LabelField("Precompiled", option.BinaryHash == option.SourceHash ? "Yes" : "Outdated");
            } else {
                EditorGUILayout.LabelField("Precompiled", "No");
            }
            // 编译选项配置
            var removeUselessTranslations = EditorGUILayout.Toggle("Clear unused translation", option.RemoveUselessTranslations);
            if (option.RemoveUselessTranslations != removeUselessTranslations) {
                option.RemoveUselessTranslations = removeUselessTranslations;
                CompileOptions.Save();
            }
            EditorGUILayout.LabelField("Available Translations", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            EditorGUILayout.LabelField("default", "build-in resource");
            foreach (var language in option.ExtraTranslationLanguages.ToArray()) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(language, "text file");
                if (GUILayout.Button("-", EditorStyles.miniButton)) {
                    if (EditorUtility.DisplayDialog($"Remove \"{language}\" translation for {assetPaths.SourceResource}?", "This action cannot be reversed", "Continue", "Cancel")) {
                        AssetDatabase.DeleteAsset(CodeCompiler.CreateLanguageAssetPathFromId(assetPaths.SourceResource, language));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            _newLanguage = EditorGUILayout.TextField(_newLanguage);
            if (GUILayout.Button("+", EditorStyles.miniButton)) {
                if (string.IsNullOrEmpty(_newLanguage)) {
                    EditorUtility.DisplayDialog("Invalid language", "Language name cannot be empty", "Close");
                } else if (option.ExtraTranslationLanguages.Contains(_newLanguage)) {
                    EditorUtility.DisplayDialog("Language name conflict", $"Language {_newLanguage} is already existed", "Close");
                } else {
                    ScriptTranslation defaultTranslationContent;
                    try {
                        var runtimeFile = new RuntimeFile(assetPaths.SourceResource);
                        defaultTranslationContent = runtimeFile.DefaultTranslation;
                    } catch (Exception) {
                        defaultTranslationContent = new ScriptTranslation("");
                    }
                    File.WriteAllText(CodeCompiler.CreateLanguageAssetPathFromId(assetPaths.SourceResource, _newLanguage), defaultTranslationContent.Pack(), Encoding.UTF8);
                    AssetDatabase.Refresh();
                    _newLanguage = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            --EditorGUI.indentLevel;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Global Options")) {
                EditorWindow.GetWindow(typeof(CompileOptionsWindow));
            }
            // 重编译按钮
            if (GUILayout.Button("Precompile")) {
                try {
                    var changedFiles = CodeCompiler.CompileAsset(importer.assetPath, option).ToArray();
                    EditorUtility.DisplayDialog(
                        "Compile finished",
                        changedFiles.Any()
                            ? $"File changed:\n{string.Join("\n", changedFiles)}"
                            : "No change detected, skip compilation",
                        "Close");
                    CompileOptions.CreateOrUpdateScript(assetPaths);
                } catch (CompileException compileException) {
                    EditorUtility.DisplayDialog("Script has error", compileException.Message, "Close");
                } catch (Exception exception) {
                    EditorUtility.DisplayDialog("Unknown exception", exception.Message, "Close");
                }
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}