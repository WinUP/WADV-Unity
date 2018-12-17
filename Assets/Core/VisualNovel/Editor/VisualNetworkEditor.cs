//using System;
//using System.IO;
//using System.Text;
//using Core.VisualNovel.Script;
//using Core.VisualNovel.Script.Compiler;
//using JetBrains.Annotations;
//using UnityEditor;
//
//namespace Core.VisualNovel.Editor {
//    [CustomEditor(typeof(VisualNetwork))]
//    [UsedImplicitly]
//    public class VisualNetworkEditor: UnityEditor.Editor {
//        private readonly bool[] _isMessageDetailOpened = new bool[1];
//
//        public override void OnInspectorGUI() {
//            var network = target as VisualNetwork;
//            if (network == null) {
//                throw new NullReferenceException("Inspected Visual Network is null");
//            }
//            EditorGUILayout.LabelField("Message System", EditorStyles.boldLabel);
//            ++EditorGUI.indentLevel;
//            network.Awaking = EditorGUILayout.Toggle("Awaking", network.Awaking);
//            _isMessageDetailOpened[0] = EditorGUILayout.Foldout(_isMessageDetailOpened[0], "Mask: VisualNetwork");
//            if (_isMessageDetailOpened[0]) {
//                EditorGUILayout.LabelField("RunCommand", "<VisualCommand>");
//                EditorGUILayout.LabelField("NextCommand", "<NULL>");
//            }
//            --EditorGUI.indentLevel;
//        }
//    }
//}
//
