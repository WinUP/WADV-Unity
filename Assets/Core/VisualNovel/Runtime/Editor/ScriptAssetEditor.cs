using System;
using System.Linq;
using System.Text;
using Core.VisualNovel.Compiler;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Runtime.Editor {
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
                assemblyContent.AppendLine("; VisualNovelScript Version 1, assembly format");
                assemblyContent.AppendLine($"; ID {scriptAsset.id}");
                assemblyContent.AppendLine("");
                var script = ScriptHeader.ReloadAsset(scriptAsset.id, scriptAsset.content).Header.CreateRuntimeFile();
                OperationCode? code;
                do {
                    var label = script.Header.Labels.Where(e => e.Value == script.CurrentPosition).ToList();
                    if (label.Any()) {
                        foreach (var pointer in label) {
                            assemblyContent.Content.AppendLine($".label {pointer.Key}");
                        }
                    }
                    code = script.ReadOperationCode();
                    if (code != null) {
                        DrawOperationCode(code.Value, script, assemblyContent);
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

        private void DrawOperationCode(OperationCode code, ScriptFile file, AssemblyContent assemblyContent) {
            switch (code) {
                case OperationCode.LDC_I4_0:
                    assemblyContent.AppendLine("ldc.i4.0");
                    break;
                case OperationCode.LDC_I4_1:
                    assemblyContent.AppendLine("ldc.i4.1");
                    break;
                case OperationCode.LDC_I4_2:
                    assemblyContent.AppendLine("ldc.i4.2");
                    break;
                case OperationCode.LDC_I4_3:
                    assemblyContent.AppendLine("ldc.i4.3");
                    break;
                case OperationCode.LDC_I4_4:
                    assemblyContent.AppendLine("ldc.i4.4");
                    break;
                case OperationCode.LDC_I4_5:
                    assemblyContent.AppendLine("ldc.i4.5");
                    break;
                case OperationCode.LDC_I4_6:
                    assemblyContent.AppendLine("ldc.i4.6");
                    break;
                case OperationCode.LDC_I4_7:
                    assemblyContent.AppendLine("ldc.i4.7");
                    break;
                case OperationCode.LDC_I4_8:
                    assemblyContent.AppendLine("ldc.i4.8");
                    break;
                case OperationCode.LDC_I4:
                    assemblyContent.AppendLine($"ldc.i4 {file.ReadInteger()}");
                    break;
                case OperationCode.LDC_R4_0:
                    assemblyContent.AppendLine("ldc.r4.0");
                    break;
                case OperationCode.LDC_R4_025:
                    assemblyContent.AppendLine("ldc.r4.025");
                    break;
                case OperationCode.LDC_R4_05:
                    assemblyContent.AppendLine("ldc.r4.05");
                    break;
                case OperationCode.LDC_R4_075:
                    assemblyContent.AppendLine("ldc.r4.075");
                    break;
                case OperationCode.LDC_R4_1:
                    assemblyContent.AppendLine("ldc.r4.1");
                    break;
                case OperationCode.LDC_R4_125:
                    assemblyContent.AppendLine("ldc.r4.125");
                    break;
                case OperationCode.LDC_R4_15:
                    assemblyContent.AppendLine("ldc.r4.15");
                    break;
                case OperationCode.LDC_R4_175:
                    assemblyContent.AppendLine("ldc.r4.175");
                    break;
                case OperationCode.LDC_R4_2:
                    assemblyContent.AppendLine("ldc.r4.2");
                    break;
                case OperationCode.LDC_R4_225:
                    assemblyContent.AppendLine("ldc.r4.225");
                    break;
                case OperationCode.LDC_R4_25:
                    assemblyContent.AppendLine("ldc.r4.25");
                    break;
                case OperationCode.LDC_R4_275:
                    assemblyContent.AppendLine("ldc.r4.275");
                    break;
                case OperationCode.LDC_R4_3:
                    assemblyContent.AppendLine("ldc.r4.3");
                    break;
                case OperationCode.LDC_R4_325:
                    assemblyContent.AppendLine("ldc.r4.325");
                    break;
                case OperationCode.LDC_R4_35:
                    assemblyContent.AppendLine("ldc.r4.35");
                    break;
                case OperationCode.LDC_R4_375:
                    assemblyContent.AppendLine("ldc.r4.375");
                    break;
                case OperationCode.LDC_R4_4:
                    assemblyContent.AppendLine("ldc.r4.4");
                    break;
                case OperationCode.LDC_R4_425:
                    assemblyContent.AppendLine("ldc.r4.425");
                    break;
                case OperationCode.LDC_R4_45:
                    assemblyContent.AppendLine("ldc.r4.45");
                    break;
                case OperationCode.LDC_R4_475:
                    assemblyContent.AppendLine("ldc.r4.475");
                    break;
                case OperationCode.LDC_R4_5:
                    assemblyContent.AppendLine("ldc.r4.5");
                    break;
                case OperationCode.LDC_R4_525:
                    assemblyContent.AppendLine("ldc.r4.525");
                    break;
                case OperationCode.LDC_R4_55:
                    assemblyContent.AppendLine("ldc.r4.55");
                    break;
                case OperationCode.LDC_R4_575:
                    assemblyContent.AppendLine("ldc.r4.575");
                    break;
                case OperationCode.LDC_R4:
                    assemblyContent.AppendLine($"ldc.r4 {file.ReadFloat()}");
                    break;
                case OperationCode.LDSTR:
                    var stringIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"ldstr {file.Header.Strings[stringIndex]}");
                    break;
                case OperationCode.LDENTRY:
                    var addressLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"ldentry {addressLabelIndex}");
                    break;
                case OperationCode.LDSTT:
                    var translatedStringId = file.ReadUInt32();
                    assemblyContent.AppendLine($"ldstt {Convert.ToString(translatedStringId, 16).PadLeft(8, '0')} ({file.ActiveTranslation.GetTranslation(translatedStringId)})");
                    break;
                case OperationCode.LDNUL:
                    assemblyContent.AppendLine("ldnul");
                    break;
                case OperationCode.LDLOC:
                    assemblyContent.AppendLine("ldloc");
                    break;
                case OperationCode.LDCON:
                    assemblyContent.AppendLine("ldcon");
                    break;
                case OperationCode.LDT:
                    assemblyContent.AppendLine("ldt");
                    break;
                case OperationCode.LDF:
                    assemblyContent.AppendLine("ldf");
                    break;
                case OperationCode.CALL:
                    assemblyContent.AppendLine("call");
                    break;
                case OperationCode.POP:
                    assemblyContent.AppendLine("pop");
                    break;
                case OperationCode.DIALOGUE:
                    assemblyContent.AppendLine("dialogue");
                    break;
                case OperationCode.BVAL:
                    assemblyContent.AppendLine("bval");
                    break;
                case OperationCode.ADD:
                    assemblyContent.AppendLine("add");
                    break;
                case OperationCode.SUB:
                    assemblyContent.AppendLine("sub");
                    break;
                case OperationCode.MUL:
                    assemblyContent.AppendLine("mul");
                    break;
                case OperationCode.DIV:
                    assemblyContent.AppendLine("div");
                    break;
                case OperationCode.NOT:
                    assemblyContent.AppendLine("not");
                    break;
                case OperationCode.EQL:
                    assemblyContent.AppendLine("eql");
                    break;
                case OperationCode.CGE:
                    assemblyContent.AppendLine("cge");
                    break;
                case OperationCode.CGT:
                    assemblyContent.AppendLine("cgt");
                    break;
                case OperationCode.CLE:
                    assemblyContent.AppendLine("cle");
                    break;
                case OperationCode.CLT:
                    assemblyContent.AppendLine("clt");
                    break;
                case OperationCode.STLOC:
                    assemblyContent.AppendLine("stloc");
                    break;
                case OperationCode.PICK:
                    assemblyContent.AppendLine("pick");
                    break;
                case OperationCode.SCOPE:
                    assemblyContent.AppendLine("scope");
                    assemblyContent.Indent += 4;
                    break;
                case OperationCode.LEAVE:
                    assemblyContent.AppendLine("leave");
                    assemblyContent.Indent -= 4;
                    break;
                case OperationCode.RET:
                    assemblyContent.AppendLine("ret");
                    break;
                case OperationCode.FUNC:
                    assemblyContent.AppendLine("func");
                    break;
                case OperationCode.BF_S:
                    var jumpIfFalseLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"bf.s {jumpIfFalseLabelIndex}");
                    break;
                case OperationCode.BR_S:
                    var jumpLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"br.s {jumpLabelIndex}");
                    break;
                case OperationCode.BF:
                    var navigateIfFalseLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"bf.s {navigateIfFalseLabelIndex}");
                    break;
                case OperationCode.BR:
                    var navigateLabelIndex = file.Read7BitEncodedInt();
                    assemblyContent.AppendLine($"br.s {navigateLabelIndex}");
                    break;
                case OperationCode.LOAD:
                    assemblyContent.AppendLine("load");
                    break;
                case OperationCode.EXP:
                    assemblyContent.AppendLine("exp");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
        
        private class AssemblyContent {
            public StringBuilder Content { get; } = new StringBuilder();
            public int Indent { get; set; }

            public void AppendLine(string content) {
                Content.AppendLine($"{"".PadLeft(Indent, ' ')} {content}");
            }

            public override string ToString() {
                return Content.ToString();
            }
        }
    }
}