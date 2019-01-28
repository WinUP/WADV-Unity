using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace WADV.Editor {
    [CustomEditor(typeof(ParallaxController))]
    public class ParallaxControllerEditor : UnityEditor.Editor {
        private ReorderableList _list;

        private void OnEnable() {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("targets")) {
                drawHeaderCallback = rect => GUI.Label(rect, "Targets and Scale levels"),
                drawElementCallback = (rect, index, active, focused) => {
                    var item = _list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
                        item.FindPropertyRelative("transform"), GUIContent.none);
                    EditorGUI.PropertyField(
                        new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight),
                        item.FindPropertyRelative("scale"), GUIContent.none);
                },
                onAddCallback = list => {
                    ++list.serializedProperty.arraySize;
                    list.index = list.serializedProperty.arraySize - 1;
                    var newItem = list.serializedProperty.GetArrayElementAtIndex(list.index);
                    newItem.FindPropertyRelative("transform").objectReferenceValue = null;
                    newItem.FindPropertyRelative("scale").intValue = 0;
                }
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _list.DoLayoutList();
            EditorGUILayout.HelpBox("It is possible to use component's Add/Remove function on runtime to change targets", MessageType.Info);
            serializedObject.ApplyModifiedProperties();
        }
    }
}