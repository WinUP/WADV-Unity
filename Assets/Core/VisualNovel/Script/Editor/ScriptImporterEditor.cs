using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.VisualNovel.Script.Compiler;
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
            // 获取编译选项
            var option = CompileOptions.Get(importer.assetPath);
            // 显示ID
            var basePath = PathUtilities.DropExtension(importer.assetPath);
            var binaryPath = PathUtilities.Combine(basePath, PathUtilities.BinaryFile);
            var hash = ModuleCompiler.ReadBinaryHash(binaryPath);
            EditorGUILayout.LabelField("ID", basePath.Substring(17).Replace("\\", "/"));
            // 检查编译状态
            if (hash.HasValue) {
                var current = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(importer.assetPath)));
                EditorGUILayout.LabelField("Precompiled", current == hash.Value ? "Yes" : "Outdated");
            } else {
                EditorGUILayout.LabelField("Precompiled", "No");
            }
            // 编译选项配置
            var removeUselessTranslations = EditorGUILayout.Toggle("Clear unused translation", option.RemoveUselessTranslations);
            if (option.RemoveUselessTranslations != removeUselessTranslations) {
                option.RemoveUselessTranslations = removeUselessTranslations;
                CompileOptions.Save();
            }
            EditorGUILayout.LabelField("Translation Generator", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            EditorGUILayout.LabelField("default", "cannot be removed");
            foreach (var language in option.ExtraTranslationLanguages.ToArray()) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(language);
                if (GUILayout.Button("-", EditorStyles.miniButton)) {
                    option.ExtraTranslationLanguages.Remove(language);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            _newLanguage = EditorGUILayout.TextField(_newLanguage);
            if (GUILayout.Button("+", EditorStyles.miniButton)) {
                if (option.ExtraTranslationLanguages.Contains(_newLanguage)) {
                    EditorUtility.DisplayDialog("Language name conflict", $"Language {_newLanguage} is already existed", "Close");
                } else {
                    option.ExtraTranslationLanguages.Add(_newLanguage);
                    _newLanguage = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            --EditorGUI.indentLevel;
            EditorGUILayout.HelpBox("New translation file will be created on next compilation", MessageType.Info);
            // 重编译按钮
            if (GUILayout.Button("Precompile")) {
                try {
                    var changedFiles = ModuleCompiler.CompileFile(importer.assetPath, option).ToArray();
                    EditorUtility.DisplayDialog(
                        "Compile finished",
                        changedFiles.Any()
                            ? $"File changed:\n{string.Join("\n", changedFiles)}"
                            : "No change detected, skip compile",
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