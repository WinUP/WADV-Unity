using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.UI.Editor {
    [CustomEditor(typeof(FadeImageGroup))]
    public class FadeImageGroupEditor : UnityEditor.Editor {
        private ReorderableList _list;
        
        private void OnEnable() {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("images")) {
                drawHeaderCallback = rect => GUI.Label(rect, "Images"),
                drawElementCallback = (rect, index, active, focused) => {
                    var item = _list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, item, GUIContent.none);
                }
            };
        }

        public override void OnInspectorGUI() {
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}