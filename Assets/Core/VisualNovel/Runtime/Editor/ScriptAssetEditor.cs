using System;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Compiler;
using UnityEditor;

namespace Core.VisualNovel.Runtime.Editor {
    [CustomEditor(typeof(ScriptAsset))]
    public class ScriptAssetEditor : UnityEditor.Editor {
        private bool _isDataSegmentOpened;
        private bool _isCodeSegmentOpened;
        
        public override void OnInspectorGUI() {
            var scriptAsset = target as ScriptAsset;
            if (scriptAsset == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            if (scriptAsset.content?.Length == 0) {
                EditorGUILayout.LabelField("This is an empty file");
                return;
            }
            var script = ScriptHeader.Reload(scriptAsset.id, scriptAsset.content).Header.CreateRuntimeFile();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("VisualNovelScript Version 1, assembly format", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Source", scriptAsset.id);
            EditorGUILayout.Space();
            _isDataSegmentOpened = EditorGUILayout.Foldout(_isDataSegmentOpened, "Data Segment");
            if (_isDataSegmentOpened) {
                ++EditorGUI.indentLevel;
                foreach (var (stringConstant, i) in script.Header.Strings.WithIndex()) {
                    EditorGUILayout.LabelField($".string {i} {stringConstant}");
                }
                foreach (var (id, content) in script.ActiveTranslation) {
                    EditorGUILayout.LabelField($".translate {Convert.ToString(id, 16).PadLeft(8, '0')} {content}");
                }
                --EditorGUI.indentLevel;
            }
            _isCodeSegmentOpened = EditorGUILayout.Foldout(_isCodeSegmentOpened, "Code Segment");
            if (_isCodeSegmentOpened) {
                ++EditorGUI.indentLevel;
                OperationCode? code;
                do {
                    var label = script.Header.Labels.Where(e => e.Value == script.CurrentPosition).ToList();
                    if (label.Any()) {
                        var initialIndent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        foreach (var pointer in label) {
                            EditorGUILayout.LabelField($".label {pointer.Key}");
                        }
                        EditorGUI.indentLevel = initialIndent;
                    }
                    code = script.ReadOperationCode();
                    if (code != null) {
                        DrawOperationCode(code.Value, script);
                    }
                } while (code != null);
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawOperationCode(OperationCode code, ScriptFile file) {
            switch (code) {
                case OperationCode.LDC_I4_0:
                    EditorGUILayout.LabelField("ldc.i4.0");
                    break;
                case OperationCode.LDC_I4_1:
                    EditorGUILayout.LabelField("ldc.i4.1");
                    break;
                case OperationCode.LDC_I4_2:
                    EditorGUILayout.LabelField("ldc.i4.2");
                    break;
                case OperationCode.LDC_I4_3:
                    EditorGUILayout.LabelField("ldc.i4.3");
                    break;
                case OperationCode.LDC_I4_4:
                    EditorGUILayout.LabelField("ldc.i4.4");
                    break;
                case OperationCode.LDC_I4_5:
                    EditorGUILayout.LabelField("ldc.i4.5");
                    break;
                case OperationCode.LDC_I4_6:
                    EditorGUILayout.LabelField("ldc.i4.6");
                    break;
                case OperationCode.LDC_I4_7:
                    EditorGUILayout.LabelField("ldc.i4.7");
                    break;
                case OperationCode.LDC_I4_8:
                    EditorGUILayout.LabelField("ldc.i4.8");
                    break;
                case OperationCode.LDC_I4:
                    EditorGUILayout.LabelField($"ldc.i4 {file.ReadInteger()}");
                    break;
                case OperationCode.LDC_R4_0:
                    EditorGUILayout.LabelField("ldc.r4.0");
                    break;
                case OperationCode.LDC_R4_025:
                    EditorGUILayout.LabelField("ldc.r4.025");
                    break;
                case OperationCode.LDC_R4_05:
                    EditorGUILayout.LabelField("ldc.r4.05");
                    break;
                case OperationCode.LDC_R4_075:
                    EditorGUILayout.LabelField("ldc.r4.075");
                    break;
                case OperationCode.LDC_R4_1:
                    EditorGUILayout.LabelField("ldc.r4.1");
                    break;
                case OperationCode.LDC_R4_125:
                    EditorGUILayout.LabelField("ldc.r4.125");
                    break;
                case OperationCode.LDC_R4_15:
                    EditorGUILayout.LabelField("ldc.r4.15");
                    break;
                case OperationCode.LDC_R4_175:
                    EditorGUILayout.LabelField("ldc.r4.175");
                    break;
                case OperationCode.LDC_R4_2:
                    EditorGUILayout.LabelField("ldc.r4.2");
                    break;
                case OperationCode.LDC_R4_225:
                    EditorGUILayout.LabelField("ldc.r4.225");
                    break;
                case OperationCode.LDC_R4_25:
                    EditorGUILayout.LabelField("ldc.r4.25");
                    break;
                case OperationCode.LDC_R4_275:
                    EditorGUILayout.LabelField("ldc.r4.275");
                    break;
                case OperationCode.LDC_R4_3:
                    EditorGUILayout.LabelField("ldc.r4.3");
                    break;
                case OperationCode.LDC_R4_325:
                    EditorGUILayout.LabelField("ldc.r4.325");
                    break;
                case OperationCode.LDC_R4_35:
                    EditorGUILayout.LabelField("ldc.r4.35");
                    break;
                case OperationCode.LDC_R4_375:
                    EditorGUILayout.LabelField("ldc.r4.375");
                    break;
                case OperationCode.LDC_R4_4:
                    EditorGUILayout.LabelField("ldc.r4.4");
                    break;
                case OperationCode.LDC_R4_425:
                    EditorGUILayout.LabelField("ldc.r4.425");
                    break;
                case OperationCode.LDC_R4_45:
                    EditorGUILayout.LabelField("ldc.r4.45");
                    break;
                case OperationCode.LDC_R4_475:
                    EditorGUILayout.LabelField("ldc.r4.475");
                    break;
                case OperationCode.LDC_R4_5:
                    EditorGUILayout.LabelField("ldc.r4.5");
                    break;
                case OperationCode.LDC_R4_525:
                    EditorGUILayout.LabelField("ldc.r4.525");
                    break;
                case OperationCode.LDC_R4_55:
                    EditorGUILayout.LabelField("ldc.r4.55");
                    break;
                case OperationCode.LDC_R4_575:
                    EditorGUILayout.LabelField("ldc.r4.575");
                    break;
                case OperationCode.LDC_R4:
                    EditorGUILayout.LabelField($"ldc.r4 {file.ReadFloat()}");
                    break;
                case OperationCode.LDSTR:
                    var stringIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"ldstr {stringIndex} ({file.Header.Strings[stringIndex]})");
                    break;
                case OperationCode.LDENTRY:
                    var addressLabelIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"ldentry {addressLabelIndex}");
                    break;
                case OperationCode.LDSTT:
                    var translatedStringId = file.ReadUInt32();
                    EditorGUILayout.LabelField($"ldstt {Convert.ToString(translatedStringId, 16).PadLeft(8, '0')} ({file.ActiveTranslation.GetTranslation(translatedStringId)})");
                    break;
                case OperationCode.LDNUL:
                    EditorGUILayout.LabelField("ldnul");
                    break;
                case OperationCode.LDLOC:
                    EditorGUILayout.LabelField("ldloc");
                    break;
                case OperationCode.LDCON:
                    EditorGUILayout.LabelField("ldcon");
                    break;
                case OperationCode.LDT:
                    EditorGUILayout.LabelField("ldt");
                    break;
                case OperationCode.LDF:
                    EditorGUILayout.LabelField("ldf");
                    break;
                case OperationCode.CALL:
                    EditorGUILayout.LabelField("call");
                    break;
                case OperationCode.POP:
                    EditorGUILayout.LabelField("pop");
                    break;
                case OperationCode.DIALOGUE:
                    EditorGUILayout.LabelField("dialogue");
                    break;
                case OperationCode.BVAL:
                    EditorGUILayout.LabelField("bval");
                    break;
                case OperationCode.ADD:
                    EditorGUILayout.LabelField("add");
                    break;
                case OperationCode.SUB:
                    EditorGUILayout.LabelField("sub");
                    break;
                case OperationCode.MUL:
                    EditorGUILayout.LabelField("mul");
                    break;
                case OperationCode.DIV:
                    EditorGUILayout.LabelField("div");
                    break;
                case OperationCode.NOT:
                    EditorGUILayout.LabelField("not");
                    break;
                case OperationCode.EQL:
                    EditorGUILayout.LabelField("eql");
                    break;
                case OperationCode.CGE:
                    EditorGUILayout.LabelField("cge");
                    break;
                case OperationCode.CGT:
                    EditorGUILayout.LabelField("cgt");
                    break;
                case OperationCode.CLE:
                    EditorGUILayout.LabelField("cle");
                    break;
                case OperationCode.CLT:
                    EditorGUILayout.LabelField("clt");
                    break;
                case OperationCode.STLOC:
                    EditorGUILayout.LabelField("stloc");
                    break;
                case OperationCode.PICK:
                    EditorGUILayout.LabelField("pic");
                    break;
                case OperationCode.SCOPE:
                    EditorGUILayout.LabelField("scope");
                    ++EditorGUI.indentLevel;
                    break;
                case OperationCode.LEAVE:
                    EditorGUILayout.LabelField("leave");
                    --EditorGUI.indentLevel;
                    break;
                case OperationCode.LANG:
                    EditorGUILayout.LabelField($"lang {file.ReadString()}");
                    break;
                case OperationCode.RET:
                    EditorGUILayout.LabelField("ret");
                    break;
                case OperationCode.FUNC:
                    EditorGUILayout.LabelField("func");
                    break;
                case OperationCode.BF_S:
                    var jumpIfFalseLabelIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"bf.s {jumpIfFalseLabelIndex}");
                    break;
                case OperationCode.BR_S:
                    var jumpLabelIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"br.s {jumpLabelIndex}");
                    break;
                case OperationCode.BF:
                    var navigateIfFalseLabelIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"bf.s {navigateIfFalseLabelIndex}");
                    break;
                case OperationCode.BR:
                    var navigateLabelIndex = file.Read7BitEncodedInt();
                    EditorGUILayout.LabelField($"br.s {navigateLabelIndex}");
                    break;
                case OperationCode.LOAD:
                    EditorGUILayout.LabelField("load");
                    break;
                case OperationCode.EXP:
                    EditorGUILayout.LabelField("exp");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }
}