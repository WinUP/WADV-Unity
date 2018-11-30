using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.VisualNovel.Plugin;

namespace Core.VisualNovel.Script.Compiler {
    public class AssembleFile {
        private readonly BinaryWriter _writer = new BinaryWriter(new MemoryStream());
        private readonly Dictionary<string, long> _labels = new Dictionary<string, long>();
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
        private uint _stringCount;

        /// <summary>
        /// 编写指令
        /// </summary>
        /// <param name="code">指令类型</param>
        public void OpCode(OpCodeType code) {
            _writer.Write((byte) code);
        }

        /// <summary>
        /// 编写入栈普通字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        public void LoadString(string content) {
            OpCode(OpCodeType.LDSTR);
            _writer.Write(content);
        }
        
        /// <summary>
        /// 编写入栈可翻译字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        public void LoadTranslatableString(string content) {
            OpCode(OpCodeType.LDSTT);
            _writer.Write(_stringCount);
            var hash = Hasher.Compute(Encoding.UTF8.GetBytes(content));
            _translations.Add(Convert.ToString((_stringCount << 16) + hash, 16).ToUpper().PadLeft(8, '0'), content);
            ++_stringCount;
        }

        /// <summary>
        /// 编写入栈32位有符号整数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        public void LoadInteger(int value) {
            switch (value) {
                case 0:
                    OpCode(OpCodeType.LDC_I4_0);
                    break;
                case 1:
                    OpCode(OpCodeType.LDC_I4_1);
                    break;
                case 2:
                    OpCode(OpCodeType.LDC_I4_2);
                    break;
                case 3:
                    OpCode(OpCodeType.LDC_I4_3);
                    break;
                case 4:
                    OpCode(OpCodeType.LDC_I4_4);
                    break;
                case 5:
                    OpCode(OpCodeType.LDC_I4_5);
                    break;
                case 6:
                    OpCode(OpCodeType.LDC_I4_6);
                    break;
                case 7:
                    OpCode(OpCodeType.LDC_I4_7);
                    break;
                case 8:
                    OpCode(OpCodeType.LDC_I4_8);
                    break;
                default:
                    OpCode(OpCodeType.LDC_I4);
                    _writer.Write(value);
                    break;
            }
        }
        
        /// <summary>
        /// 编写入栈32位浮点数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        public void LoadFloat(float value) {
            switch (value) {
                case 0.0F:
                    OpCode(OpCodeType.LDC_R4_0);
                    break;
                case 0.25F:
                    OpCode(OpCodeType.LDC_R4_025);
                    break;
                case 0.5F:
                    OpCode(OpCodeType.LDC_R4_05);
                    break;
                case 0.75F:
                    OpCode(OpCodeType.LDC_R4_075);
                    break;
                case 1.0F:
                    OpCode(OpCodeType.LDC_R4_1);
                    break;
                case 1.25F:
                    OpCode(OpCodeType.LDC_R4_125);
                    break;
                case 1.5F:
                    OpCode(OpCodeType.LDC_R4_15);
                    break;
                case 1.75F:
                    OpCode(OpCodeType.LDC_R4_175);
                    break;
                case 2.0F:
                    OpCode(OpCodeType.LDC_R4_2);
                    break;
                case 2.25F:
                    OpCode(OpCodeType.LDC_R4_225);
                    break;
                case 2.5F:
                    OpCode(OpCodeType.LDC_R4_25);
                    break;
                case 2.75F:
                    OpCode(OpCodeType.LDC_R4_275);
                    break;
                case 3F:
                    OpCode(OpCodeType.LDC_R4_3);
                    break;
                case 3.25F:
                    OpCode(OpCodeType.LDC_R4_325);
                    break;
                case 3.5F:
                    OpCode(OpCodeType.LDC_R4_35);
                    break;
                case 3.75F:
                    OpCode(OpCodeType.LDC_R4_375);
                    break;
                case 4F:
                    OpCode(OpCodeType.LDC_R4_4);
                    break;
                case 4.25F:
                    OpCode(OpCodeType.LDC_R4_425);
                    break;
                case 4.5F:
                    OpCode(OpCodeType.LDC_R4_45);
                    break;
                case 4.75F:
                    OpCode(OpCodeType.LDC_R4_475);
                    break;
                case 5F:
                    OpCode(OpCodeType.LDC_R4_5);
                    break;
                case 5.25F:
                    OpCode(OpCodeType.LDC_R4_525);
                    break;
                case 5.5F:
                    OpCode(OpCodeType.LDC_R4_55);
                    break;
                case 5.75F:
                    OpCode(OpCodeType.LDC_R4_575);
                    break;
                default:
                    OpCode(OpCodeType.LDC_R4);
                    _writer.Write(value);
                    break;
            }
        }

        /// <summary>
        /// 编写入栈空值指令
        /// </summary>
        public void LoadNull() {
            OpCode(OpCodeType.LDNUL);
        }

        /// <summary>
        /// 编写入栈布尔常量指令
        /// </summary>
        /// <param name="value">目标布尔值</param>
        public void LoadBoolean(bool value) {
            OpCode(value ? OpCodeType.LDT : OpCodeType.LDF);
        }

        /// <summary>
        /// 编写入栈对话指令
        /// </summary>
        /// <param name="speaker">角色</param>
        /// <param name="content">对话内容</param>
        public void LoadDialogue(string speaker, string content) {
            LoadTranslatableString(content);
            if (speaker == null) {
                LoadNull();
            } else {
                LoadTranslatableString(speaker);
            }
            OpCode(OpCodeType.DIALOGUE);
        }
        
        /// <summary>
        /// 编写入栈插件名指令
        /// </summary>
        /// <param name="description">插件描述</param>
        /// <param name="name">描述不可用时的替代插件名字符串</param>
        public void LoadPluginName(PluginDescription? description, string name) {
            if (description.HasValue) {
                OpCode(OpCodeType.LDUID);
                _writer.Write(
                    ((uint) description.Value.Identifier.Part1 << 24) +
                    ((uint) description.Value.Identifier.Part2 << 16) +
                    ((uint) description.Value.Identifier.Part3 << 8) +
                    description.Value.Identifier.Part4);
            } else {
                LoadString(name);
            }
        }
        
        /// <summary>
        /// 编写入栈插件参数名指令
        /// </summary>
        /// <param name="description">插件描述</param>
        /// <param name="name">参数名</param>
        public void LoadPluginParameterName(PluginDescription? description, string name) {
            if (description.HasValue) {
                for (var i = -1; ++i < description.Value.Parameters.Length;) {
                    if (description.Value.Parameters[i] != name) continue;
                    LoadInteger(i);
                    return;
                }
            }
            LoadString(name);
        }

        /// <summary>
        /// 编写读取栈顶变量值指令
        /// </summary>
        public void LoadVariable() {
            OpCode(OpCodeType.LDLOC);
        }
        
        /// <summary>
        /// 编写写入栈顶变量值指令
        /// </summary>
        public void SetVariable() {
            OpCode(OpCodeType.STLOC);
        }
        
        /// <summary>
        /// 编写删除变量指令
        /// </summary>
        /// <param name="name">目标变量名</param>
        public void DeleteVariable(string name) {
            LoadString(name);
            OpCode(OpCodeType.DEL);
        }
        
        /// <summary>
        /// 编写空指令
        /// </summary>
        public void EmptyCode() {
            OpCode(OpCodeType.EMPTY);
        }

        /// <summary>
        /// 编写布尔转换指令
        /// </summary>
        public void ToBoolean() {
            OpCode(OpCodeType.BVAL);
        }

        /// <summary>
        /// 编写切换脚本语言指令
        /// </summary>
        /// <param name="language">目标语言</param>
        public void Language(string language) {
            LoadString(language);
            OpCode(OpCodeType.LANG);
        }

        /// <summary>
        /// 编写出栈指令
        /// </summary>
        public void Pop() {
            OpCode(OpCodeType.POP);
        }

        /// <summary>
        /// 编写调用栈顶插件指令
        /// </summary>
        public void Call() {
            OpCode(OpCodeType.CALL);
        }
        
        /// <summary>
        /// 在下一条指令起始处创建跳转标签
        /// </summary>
        /// <param name="name">标签名</param>
        public void CreateLabel(string name) {
            if (_labels.ContainsKey(name)) {
                throw new ArgumentException($"Label {name} is already existed");
            }
            _labels.Add(name, _writer.BaseStream.Position);
        }

        public (byte[] content, IReadOnlyDictionary<string, long> labels, IReadOnlyDictionary<string, string> translations) Create() {
            return ((_writer.BaseStream as MemoryStream)?.ToArray(), _labels, _translations);
        }
    }
}