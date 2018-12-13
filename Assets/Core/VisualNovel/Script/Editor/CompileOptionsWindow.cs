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

        private void OnGUI() {
            // 左栏
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Global Options", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Button("test");
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