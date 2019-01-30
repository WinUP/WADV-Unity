//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WADV.MessageSystem;
//using WADV.VisualNovel.Runtime;
//using WADV.VisualNovel.Translation;
//using UnityEditor;
//using UnityEditor.Experimental.AssetImporters;
//using UnityEngine;
//using WADV.VisualNovel.ScriptStatus;
//
//namespace WADV.VisualNovel.Compiler.Editor {
//    [CustomEditor(typeof(ScriptImporter))]
//    public class ScriptImporterEditor : ScriptedImporterEditor, IMessenger {
//        public int Mask { get; } = CoreConstant.Mask;
//        public bool IsStandaloneMessage { get; } = false;
//        
//        private string _newLanguage;
//        private LinkedTreeNode<IMessenger> _node;
//        private static readonly LinkedTreeNode<IMessenger> ImporterEditorRoot;
//
//        static ScriptImporterEditor() {
//            ImporterEditorRoot = MessageService.Receivers.CreateChild(new EmptyMessenger());
//        }
//        
//        public Task<Message> Receive(Message message) {
//            if (message.Tag == CoreConstant.RepaintCompileOptionEditor) {
//                Repaint();
//            }
//            return Task.FromResult(message);
//        }
//
//        public override void OnEnable() {
//            _node = ImporterEditorRoot.CreateChild(this);
//            base.OnEnable();
//        }
//
//        public override void OnDisable() {
//            ImporterEditorRoot.RemoveChild(_node);
//            base.OnDisable();
//        }
//
//        public override void OnInspectorGUI() {
//            var importer = target as ScriptImporter;
//            if (importer == null) {
//                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
//            }
//            // 显示ID
//            var assetPaths = CodeCompiler.CreatePathFromAsset(importer.assetPath);
//            if (assetPaths == null) {
//                EditorGUILayout.LabelField("Error: Unable to parse source file");
//                return;
//            }
//            var option = CompileOptions.Get(assetPaths.SourceResource);
//            EditorGUILayout.LabelField("Resource ID", assetPaths.SourceResource);
//            // 显示编译状态
//            if (option.BinaryHash.HasValue) {
//                EditorGUILayout.LabelField("Precompiled", option.BinaryHash == option.SourceHash ? "Yes" : "Outdated");
//            } else {
//                EditorGUILayout.LabelField("Precompiled", "No");
//            }
//            // 语言配置
//            _newLanguage = DrawLanguageGui(option, assetPaths, _newLanguage);
//            GUILayout.Space(5);
//            // 工具按钮
//            EditorGUILayout.BeginHorizontal();
//            if (GUILayout.Button("Global Options")) {
//                EditorWindow.GetWindow(typeof(CompileOptionsWindow));
//            }
//            if (GUILayout.Button("Precompile")) {
//                try {
//                    var changedFiles = CodeCompiler.CompileAsset(importer.assetPath, option).ToArray();
//                    EditorUtility.DisplayDialog(
//                        "Compile finished",
//                        changedFiles.Any()
//                            ? $"File changed:\n{string.Join("\n", changedFiles)}"
//                            : "No change detected, skip compilation",
//                        "Close");
//                    CompileOptions.CreateOrUpdateScript(assetPaths);
//                } catch (CompileException compileException) {
//                    EditorUtility.DisplayDialog("Script has error", compileException.Message, "Close");
//                } catch (Exception exception) {
//                    EditorUtility.DisplayDialog("Unknown exception", exception.Message, "Close");
//                }
//                AssetDatabase.Refresh();
//            }
//            EditorGUILayout.EndHorizontal();
//        }
//
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
//    }
//}