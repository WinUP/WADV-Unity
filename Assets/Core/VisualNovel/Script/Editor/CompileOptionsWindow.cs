using System.IO;
using System.Linq;
using Core.MessageSystem;
using Core.VisualNovel.Script.Compiler;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    public class CompileOptionsWindow : EditorWindow {
        private Vector2 _scriptsScrollPosition = Vector2.zero;
        
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

        private void OnEnable() {
            titleContent.image = EditorGUIUtility.Load("Assets/Gizmos/Core/VisualNovel/Script/Editor/CompileOptionsWindow Icon.png") as Texture2D;
        }

        private void OnGUI() {
            // 左栏
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Global Options", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("test")) {
                Debug.Log(CompileOptions.Collection.Count);
            }
            GUILayout.Button("test");
            GUILayout.EndHorizontal();
            GUILayout.Button("test");
            GUILayout.EndVertical();
            // 间隔
            GUILayout.Space(20);
            // 右栏
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            // 标题
            GUILayout.BeginHorizontal();
            GUILayout.Button("ID", EditorStyles.miniButtonLeft);
            GUILayout.Button("Precompiled", EditorStyles.miniButtonMid);
            GUILayout.Button("Clear useless translations", EditorStyles.miniButtonMid);
            GUILayout.Button("Translations", EditorStyles.miniButtonRight);
            GUILayout.EndHorizontal();
            // 文件列表
            _scriptsScrollPosition = GUILayout.BeginScrollView(_scriptsScrollPosition);
            foreach (var file in CompileOptions.Collection) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(file.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}