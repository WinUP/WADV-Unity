using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Core.VisualNovel;
using Core.VisualNovel.Script.Compiler;
using JetBrains.Annotations;
using UnityEditor;

namespace Core.VisualNovel.Editor {
    [CustomEditor(typeof(VisualNetwork))]
    [UsedImplicitly]
    public class VisualNetworkEditor: UnityEditor.Editor {
        private readonly bool[] _isMessageDetailOpened = new bool[1];

        public override void OnInspectorGUI() {
            var network = target as VisualNetwork;
            if (network == null) {
                throw new NullReferenceException("Inspected Visual Network is null");
            }
            EditorGUILayout.LabelField("Message System", EditorStyles.boldLabel);
            network.Awaking = EditorGUILayout.Toggle("Awaking", network.Awaking);
            _isMessageDetailOpened[0] = EditorGUILayout.Foldout(_isMessageDetailOpened[0], "Mask: VisualNetwork");
            if (_isMessageDetailOpened[0]) {
                EditorGUILayout.LabelField("RunCommand", "<VisualCommand>");
                EditorGUILayout.LabelField("NextCommand", "<NULL>");
            }
            
            
            
            var lexer = Lexer.FromFile("Logic/!Entrance");
            var parser = new Parser(lexer.Lex(), lexer.Identifier);
            var assembler = new Assembler(parser.Parse(), parser.Identifier);
            var file = assembler.Assemble();
            var reader = new BinaryReader(new MemoryStream(file.Content), Encoding.UTF8);
            var data = new StringBuilder();
            File.WriteAllBytes("Assets/Resources/Logic/!Entrance_bin.bytes", file.Content);
            File.WriteAllText("Assets/Resources/Logic/!Entrance_tr_default.txt", file.Translations, Encoding.UTF8);
            if (reader.ReadInt32() != 0x564E5331) {
                throw new FormatException("Not VNS1 File");
            }
            var strings = new List<string>();
            var stringCount = reader.ReadInt32();
            for (var i = -1; ++i < stringCount;) {
                var item = reader.ReadString().Replace("\n", "\\n");
                data.AppendLine($".string {i} {item}");
                strings.Add(item);
            }
            var labels = new Dictionary<string, long>();
            var labelCount = reader.ReadInt32();
            for (var i = -1; ++i < labelCount;) {
                var offset = reader.ReadInt64();
                var item = reader.ReadString();
                data.AppendLine($".label {offset} {item}");
                labels.Add(item, offset);
            }
            var sceneCount = reader.ReadInt32();
            for (var i = -1; ++i < sceneCount;) {
                var sceneName = reader.ReadString();
                var label = reader.ReadString();
                data.AppendLine($".scene {sceneName} {label}");
            }
            data.AppendLine();
            var globalOffset = reader.BaseStream.Position;
            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                foreach (var label in labels.Where(e => e.Value == reader.BaseStream.Position - globalOffset)) {
                    data.Append($"[{label.Key}]");
                }
                var command = (OpCodeType) reader.ReadByte();
                switch (command) {
                    case OpCodeType.LDC_I4:
                        data.AppendLine($"{command} {reader.ReadInt32()}");
                        break;
                    case OpCodeType.LDSTR:
                        var strIndex = reader.ReadInt32();
                        data.AppendLine($"{command} {strIndex}({strings[strIndex].Replace("\n", "\\n")})");
                        break;
                    case OpCodeType.LDSTT:
                        var sttIndex = reader.ReadInt32();
                        data.AppendLine($"{command} {Convert.ToString(sttIndex, 16).PadLeft(8, '0')}");
                        break;
                    case OpCodeType.LDADDR:
                        data.AppendLine($"{command} -> {reader.ReadString()}");
                        break;
                    case OpCodeType.LDC_R4:
                        data.AppendLine($"{command} {reader.ReadSingle()}");
                        break;
                    case OpCodeType.BF_S:
                    case OpCodeType.BR_S:
                        data.AppendLine($"{command} {reader.ReadString()}");
                        break;
                    default:
                        data.AppendLine(command.ToString());
                        break;
                }
            }
            File.WriteAllText("Assets/Resources/Logic/!Entrance.debug.txt", data.ToString());
        }
    }
}

