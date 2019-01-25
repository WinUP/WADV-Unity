using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace WADV.Editor {
    [CustomEditor(typeof(GraphicColorFade))]
    public class FadeImageGroupEditor : UnityEditor.Editor {
        private ReorderableList _list;
        
        private void OnEnable() {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("targets")) {
                drawHeaderCallback = rect => GUI.Label(rect, "Targets"),
                drawElementCallback = (rect, index, active, focused) => {
                    var item = _list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, item, GUIContent.none);
                }
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}