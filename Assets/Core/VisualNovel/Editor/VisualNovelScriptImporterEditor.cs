using System;
using System.IO;
using System.Linq;
using System.Text;
using Core.VisualNovel.Script;
using Core.VisualNovel.Script.Compiler;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Core.VisualNovel.Editor {
    [CustomEditor(typeof(VisualNovelScriptImporter))]
    public class VisualNovelScriptImporterEditor : ScriptedImporterEditor {
        public override void OnInspectorGUI() {
            var importer = target as VisualNovelScriptImporter;
            if (importer == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            // 获取编译选项
            CompileOption option;
            if (!VisualNovelScriptImporter.ScriptCompileOptions.ContainsKey(importer.assetPath)) {
                option = new CompileOption();
                VisualNovelScriptImporter.ScriptCompileOptions.Add(importer.assetPath, option);
            } else {
                option = VisualNovelScriptImporter.ScriptCompileOptions[importer.assetPath];
            }
            // 显示ID
            var basePath = PathUtilities.DropExtension(importer.assetPath);
            var binaryPath = PathUtilities.Combine(basePath, PathUtilities.BinaryFile);
            var hash = ModuleCompiler.ReadBinaryHash(binaryPath);
            EditorGUILayout.LabelField("ID", basePath.Substring(17).Replace("\\", "/"));
            if (hash.HasValue) {
                var current = Hasher.Crc32(Encoding.UTF8.GetBytes(File.ReadAllText(importer.assetPath)));
                EditorGUILayout.LabelField("Precompiled", current == hash.Value ? "Yes" : "Outdated");
            } else {
                EditorGUILayout.LabelField("Precompiled", "No");
            }
            EditorGUILayout.LabelField("Compile Configuration", EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            option.RemoveUselessTranslations = EditorGUILayout.Toggle("Remove useless translation", option.RemoveUselessTranslations);
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
            --EditorGUI.indentLevel;
        }
    }
}