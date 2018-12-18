#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.MessageSystem;
using Core.VisualNovel.Script.Compiler;
using Core.VisualNovel.Translation;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    public class CompileOptionsWindow : EditorWindow, IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        private readonly Dictionary<string, bool> _isEditorOpened = new Dictionary<string, bool>();
        private Vector2 _scriptsScrollPosition = Vector2.zero;
        private LinkedTreeNode<IMessenger> _node;

        public CompileOptionsWindow() {
            titleContent = new GUIContent("VNS Compile Options");
        }
        
        [MenuItem("Window/Visual Novel/Compile Options Viewer")]
        public static void ShowWindow() {
            GetWindow<CompileOptionsWindow>();
        }
        
        [MenuItem("Window/Visual Novel/Reload All Compile Options")]
        public static void Reload() {
            CompileOptions.Clear();
            ReloadDirectory("Assets/Resources");
            MessageService.Process(new Message {Mask = CoreConstant.Mask, Tag = CoreConstant.ReloadAllCompileOptionsTag});
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

        public async Task<Message> Receive(Message message) {
            if (message.Tag == CoreConstant.ReloadAllCompileOptionsTag) {
                Repaint();
            }
            return message;
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
            // ID
            GUILayout.BeginVertical();
            foreach (var file in CompileOptions.Options) {
                var target = CodeCompiler.CreatePathFromId(file.Key);
                var option = file.Value;
                GUILayout.BeginHorizontal();
                var content = new GUIContent(file.Value.ExtraTranslationLanguages.Any()
                    ? $"  {target.SourceResource} (default, {string.Join(", ", file.Value.ExtraTranslationLanguages)})"
                    : $"  {target.SourceResource} (default)");
                if (option.BinaryHash.HasValue) {
                    if (option.BinaryHash == file.Value.SourceHash) {
                        content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/Compiled.png") as Texture2D;
                    } else {
                        content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/Outdated.png") as Texture2D;
                    }
                } else {
                    content.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/NotCompile.png") as Texture2D;
                }
                if (GUILayout.Button(content, new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft}, GUILayout.Height(25))) {
                    if (!_isEditorOpened.ContainsKey(target.SourceResource)) {
                        _isEditorOpened.Add(target.SourceResource, false);
                    }
                    _isEditorOpened[target.SourceResource] = !_isEditorOpened[target.SourceResource];
                    if (_isEditorOpened[target.SourceResource]) {
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
                                if (EditorUtility.DisplayDialog($"Remove \"{language}\" translation for {target.SourceResource}?", "This action cannot be reversed", "Continue", "Cancel")) {
                                    AssetDatabase.DeleteAsset(CodeCompiler.CreateLanguageAssetPathFromId(target.SourceResource, language));
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        --EditorGUI.indentLevel;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            // 间隔
            GUILayout.Space(20);
            // 右栏
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Global Options", EditorStyles.boldLabel);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}