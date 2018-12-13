using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Translation;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    [CustomEditor(typeof(ScriptImporter))]
    public class ScriptImporterEditor : ScriptedImporterEditor {
        private string _newLanguage;
        
        public override void OnInspectorGUI() {
            var importer = target as ScriptImporter;
            if (importer == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            // 显示ID
            var paths = CodeCompiler.CreatePathFromAsset(importer.assetPath);
            var hash = ModuleCompiler.ReadBinaryHash(paths.Binary);
            var id = paths.SourceResource.Replace("\\", "/");
            EditorGUILayout.LabelField("Resource ID", id);
            // 获取编译选项
            var option = CompileOptions.Get(id);
            // 检查编译状态
            if (hash.HasValue) {
                var currentHash = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(importer.assetPath)));
                EditorGUILayout.LabelField("Precompiled", currentHash == hash.Value ? "Yes" : "Outdated");
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
                    if (EditorUtility.DisplayDialog($"Remove \"{language}\" translation for {id}?", "This action cannot be reversed", "Continue", "Cancel")) {
                        AssetDatabase.DeleteAsset(CodeCompiler.CreateLanguageAssetPathFromId(id, language));
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
                        var runtimeFile = new RuntimeFile(id);
                        defaultTranslationContent = runtimeFile.DefaultTranslation;
                    } catch (Exception) {
                        defaultTranslationContent = new ScriptTranslation("");
                    }
                    File.WriteAllText(CodeCompiler.CreateLanguageAssetPathFromId(id, _newLanguage), defaultTranslationContent.Pack(), Encoding.UTF8);
                    AssetDatabase.Refresh();
                    _newLanguage = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            --EditorGUI.indentLevel;
            EditorGUILayout.HelpBox("You can control global settings under menu Window/VisualNovel", MessageType.Info);
            // 重编译按钮
            if (GUILayout.Button("Precompile")) {
                try {
                    var changedFiles = ModuleCompiler.CompileFile(importer.assetPath, option).ToArray();
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
}