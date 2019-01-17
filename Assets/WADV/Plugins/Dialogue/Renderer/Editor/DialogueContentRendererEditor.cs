using UnityEditor;
using WADV.Plugins.Dialogue.TextGenerator;

namespace WADV.Plugins.Dialogue.Renderer.Editor {
    [CustomEditor(typeof(DialogueContentRenderer))]
    public class DialogueContentRendererEditor : UnityEditor.Editor {
        private SerializedProperty _timeSpan;
        private DialogueTextGeneratorType _generatorType = DialogueTextGeneratorType.None;
        private DialogueContentRenderer _target;
        
        private void OnEnable() {
            _timeSpan = serializedObject.FindProperty("timeSpan");
            _target = (DialogueContentRenderer) target;
            _generatorType = _target.textGenerator;
        }

        public override void OnInspectorGUI() {
            var generator = (DialogueTextGeneratorType) EditorGUILayout.EnumPopup("Text Generator", _generatorType);
            if (generator != _generatorType) {
                _target.ResetGenerator(generator);
                _generatorType = generator;
            }
            EditorGUILayout.PropertyField(_timeSpan);
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(TextMeshDialogueContentRenderer))]
    public class TextMeshDialogueContentRendererEditor : DialogueContentRendererEditor { }
}