using System;
using System.Linq;
using System.Text;
using WADV.VisualNovel.Compiler;
using UnityEditor;
using UnityEngine;

namespace WADV.VisualNovel.Runtime.Editor {
    [CustomEditor(typeof(ScriptAsset))]
    public class ScriptAssetEditor : UnityEditor.Editor {
        private GUIStyle _scriptStyle;
        private string _assemblyCode;
        
        public override void OnInspectorGUI() {
            if (_scriptStyle == null) {
                _scriptStyle = new GUIStyle("ScriptText");
            }
            var scriptAsset = target as ScriptAsset;
            if (scriptAsset == null) {
                throw new TypeLoadException("Inspected type is not VisualNovelScriptImporter");
            }
            if (scriptAsset.content?.Length == 0) {
                EditorGUILayout.LabelField("This is an empty file");
                return;
            }
            if (string.IsNullOrEmpty(_assemblyCode)) {
                var assemblyContent = new AssemblyContent();
                var (header, bytes) = ScriptHeader.ParseBinary("", scriptAsset.content);
                var script = new ScriptFile(header, bytes);
                OperationCode? code;
                do {
                    var label = script.Header.Labels.Where(e => e.Value == script.CurrentPosition).ToList();
                    if (label.Any()) {
                        foreach (var pointer in label) {
                            assemblyContent.Content.AppendLine($".label {pointer.Key}");
                        }
                    }
                    SourcePosition? position;
                    try {
                        position = script.Header.Positions[script.CurrentPosition];
                    } catch {
                        position = null;
                    }
                    code = script.ReadOperationCode();
                    if (code != null) {
                        DrawOperationCode(position, code.Value, script, assemblyContent);
                    }
                } while (code != null);
                _assemblyCode = assemblyContent.ToString();
            }
            var rect = GUILayoutUtility.GetRect(new GUIContent(_assemblyCode), _scriptStyle);
            rect.x = 0f;
            rect.y -= 3f;
            rect.width = EditorGUIUtility.currentViewWidth + 1f;
            GUI.Box(rect, _assemblyCode, _scriptStyle);
        }

        private void DrawOperationCode(SourcePosition? position, OperationCode code, ScriptFile file, AssemblyContent assemblyContent) {
            switch (code) {
                case OperationCode.LDC_I4_0:
                    assemblyContent.AppendLine(position, "ldc.i4.0");
                    break;
                case OperationCode.LDC_I4_1:
                    assemblyContent.AppendLine(position, "ldc.i4.1");
                    break;
                case OperationCode.LDC_I4_2:
                    assemblyContent.AppendLine(position, "ldc.i4.2");
                    break;
                case OperationCode.LDC_I4_3:
                    assemblyContent.AppendLine(position, "ldc.i4.3");
                    break;
                case OperationCode.LDC_I4_4:
                    assemblyContent.AppendLine(position, "ldc.i4.4");
                    break;
                case OperationCode.LDC_I4_5:
                    assemblyContent.AppendLine(position, "ldc.i4.5");
                    break;
                case OperationCode.LDC_I4_6:
                    assemblyContent.AppendLine(position, "ldc.i4.6");
                    break;
                case OperationCode.LDC_I4_7:
                    assemblyContent.AppendLine(position, "ldc.i4.7");
                    break;
                case OperationCode.LDC_I4_8:
                    assemblyContent.AppendLine(position, "ldc.i4.8");
                    break;
                case OperationCode.LDC_I4:
                    assemblyContent.AppendLine(position, $"ldc.i4 {file.ReadInteger()}");
                    break;
                case OperationCode.LDC_R4_0:
                    assemblyContent.AppendLine(position, "ldc.r4.0");
                    break;
                case OperationCode.LDC_R4_025:
                    assemblyContent.AppendLine(position, "ldc.r4.025");
                    break;
                case OperationCode.LDC_R4_05:
                    assemblyContent.AppendLine(position, "ldc.r4.05");
                    break;
                case OperationCode.LDC_R4_075:
                    assemblyContent.AppendLine(position, "ldc.r4.075");
                    break;
                case OperationCode.LDC_R4_1:
                    assemblyContent.AppendLine(position, "ldc.r4.1");
                    break;
                case OperationCode.LDC_R4_125:
                    assemblyContent.AppendLine(position, "ldc.r4.125");
                    break;
                case OperationCode.LDC_R4_15:
                    assemblyContent.AppendLine(position, "ldc.r4.15");
                    break;
                case OperationCode.LDC_R4_175:
                    assemblyContent.AppendLine(position, "ldc.r4.175");
                    break;
                case OperationCode.LDC_R4_2:
                    assemblyContent.AppendLine(position, "ldc.r4.2");
                    break;
                case OperationCode.LDC_R4_225:
                    assemblyContent.AppendLine(position, "ldc.r4.225");
                    break;
                case OperationCode.LDC_R4_25:
                    assemblyContent.AppendLine(position, "ldc.r4.25");
                    break;
                case OperationCode.LDC_R4_275:
                    assemblyContent.AppendLine(position, "ldc.r4.275");
                    break;
                case OperationCode.LDC_R4_3:
                    assemblyContent.AppendLine(position, "ldc.r4.3");
                    break;
                case OperationCode.LDC_R4_325:
                    assemblyContent.AppendLine(position, "ldc.r4.325");
                    break;
                case OperationCode.LDC_R4_35:
                    assemblyContent.AppendLine(position, "ldc.r4.35");
                    break;
                case OperationCode.LDC_R4_375:
                    assemblyContent.AppendLine(position, "ldc.r4.375");
                    break;
                case OperationCode.LDC_R4_4:
                    assemblyContent.AppendLine(position, "ldc.r4.4");
                    break;
                case OperationCode.LDC_R4_425:
                    assemblyContent.AppendLine(position, "ldc.r4.425");
                    break;
                case OperationCode.LDC_R4_45:
                    assemblyContent.AppendLine(position, "ldc.r4.45");
                    break;
                case OperationCode.LDC_R4_475:
                    assemblyContent.AppendLine(position, "ldc.r4.475");
                    break;
                case OperationCode.LDC_R4_5:
                    assemblyContent.AppendLine(position, "ldc.r4.5");
                    break;
                case OperationCode.LDC_R4_525:
                    assemblyContent.AppendLine(position, "ldc.r4.525");
                    break;
                case OperationCode.LDC_R4_55:
                    assemblyContent.AppendLine(position, "ldc.r4.55");
                    break;
                case OperationCode.LDC_R4_575:
                    assemblyContent.AppendLine(position, "ldc.r4.575");
                    break;
                case OperationCode.LDC_R4:
                    assemblyContent.AppendLine(position, $"ldc.r4 {file.ReadFloat()}");
                    break;
                case OperationCode.LDSTR:
                    var stringIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine(position, $"ldstr {file.Header.Strings[stringIndex]}");
                    break;
                case OperationCode.LDENTRY:
                    var addressLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine(position, $"ldentry {addressLabelIndex}");
                    break;
                case OperationCode.LDSTT:
                    var translatedStringId = file.ReadUInt32();
                    assemblyContent.AppendLine(position, $"ldstt {Convert.ToString(translatedStringId, 16).PadLeft(8, '0')} ({file.ActiveTranslation.GetTranslation(translatedStringId)})");
                    break;
                case OperationCode.LDNUL:
                    assemblyContent.AppendLine(position, "ldnul");
                    break;
                case OperationCode.LDLOC:
                    assemblyContent.AppendLine(position, "ldloc");
                    break;
                case OperationCode.LDCON:
                    assemblyContent.AppendLine(position, "ldcon");
                    break;
                case OperationCode.LDT:
                    assemblyContent.AppendLine(position, "ldt");
                    break;
                case OperationCode.LDF:
                    assemblyContent.AppendLine(position, "ldf");
                    break;
                case OperationCode.CALL:
                    assemblyContent.AppendLine(position, "call");
                    break;
                case OperationCode.POP:
                    assemblyContent.AppendLine(position, "pop");
                    break;
                case OperationCode.DIALOGUE:
                    assemblyContent.AppendLine(position, "dialogue");
                    break;
                case OperationCode.BVAL:
                    assemblyContent.AppendLine(position, "bval");
                    break;
                case OperationCode.ADD:
                    assemblyContent.AppendLine(position, "add");
                    break;
                case OperationCode.SUB:
                    assemblyContent.AppendLine(position, "sub");
                    break;
                case OperationCode.MUL:
                    assemblyContent.AppendLine(position, "mul");
                    break;
                case OperationCode.DIV:
                    assemblyContent.AppendLine(position, "div");
                    break;
                case OperationCode.NOT:
                    assemblyContent.AppendLine(position, "not");
                    break;
                case OperationCode.EQL:
                    assemblyContent.AppendLine(position, "eql");
                    break;
                case OperationCode.CGE:
                    assemblyContent.AppendLine(position, "cge");
                    break;
                case OperationCode.CGT:
                    assemblyContent.AppendLine(position, "cgt");
                    break;
                case OperationCode.CLE:
                    assemblyContent.AppendLine(position, "cle");
                    break;
                case OperationCode.CLT:
                    assemblyContent.AppendLine(position, "clt");
                    break;
                case OperationCode.STLOC:
                    assemblyContent.AppendLine(position, "stloc");
                    break;
                case OperationCode.STCON:
                    assemblyContent.AppendLine(position, "stcon");
                    break;
                case OperationCode.STMEM:
                    assemblyContent.AppendLine(position, "stmem");
                    break;
                case OperationCode.PICK:
                    assemblyContent.AppendLine(position, "pick");
                    break;
                case OperationCode.SCOPE:
                    assemblyContent.AppendLine(position, "scope");
                    assemblyContent.Indent += 4;
                    break;
                case OperationCode.LEAVE:
                    assemblyContent.AppendLine(position, "leave");
                    assemblyContent.Indent -= 4;
                    break;
                case OperationCode.RET:
                    assemblyContent.AppendLine(position, "ret");
                    break;
                case OperationCode.FUNC:
                    assemblyContent.AppendLine(position, "func");
                    break;
                case OperationCode.BF:
                    var jumpIfFalseLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine(position, $"bf.s {jumpIfFalseLabelIndex}");
                    break;
                case OperationCode.BR:
                    var jumpLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine(position, $"br.s {jumpLabelIndex}");
                    break;
                case OperationCode.LOAD:
                    assemblyContent.AppendLine(position, "load");
                    break;
                case OperationCode.EXP:
                    assemblyContent.AppendLine(position, "exp");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
        
        private class AssemblyContent {
            public StringBuilder Content { get; } = new StringBuilder();
            public int Indent { get; set; }

            public void AppendLine(SourcePosition? position, string content) {
                var positionString = position.HasValue ? $"({position.Value.Line + 1}, {position.Value.Column + 1}) " : "";
                Content.AppendLine($"{positionString}\t{"".PadLeft(Indent, ' ')} {content}");
            }

            public override string ToString() {
                return Content.ToString();
            }
        }
    }
}