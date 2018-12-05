using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Extensions;

namespace Core.VisualNovel.Script.Compiler {
    public class AssembleFile {
        private readonly BinaryWriter _writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        private readonly List<CodePosition> _positions = new List<CodePosition>();
        private readonly Dictionary<string, long> _labels = new Dictionary<string, long>();
        private readonly Dictionary<int, string> _translations = new Dictionary<int, string>();
        private readonly List<string> _strings = new List<string>();
        private uint _stringCount;

        public long Position => _writer.BaseStream.Position;

        /// <summary>
        /// 编写指令
        /// </summary>
        /// <param name="code">指令类型</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void OpCode(OpCodeType code, CodePosition position) {
            _writer.Write((byte) code);
            _positions.Add(position);
        }

        /// <summary>
        /// 编写字符串
        /// </summary>
        /// <param name="value">目标字符串</param>
        public void DirectWrite(string value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写数组内容
        /// </summary>
        /// <param name="values">目标数组</param>
        public void DirectWrite(params byte[] values) {
            _writer.Write(values);
        }

        /// <summary>
        /// 编写32位整数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void DirectWrite(int value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写64位整数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void DirectWrite(long value) {
            _writer.Write(value);
        }
        
        /// <summary>
        /// 编写32位浮点数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void DirectWrite(float value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写入栈普通字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadString(string content, CodePosition position) {
            var currentIndex = _strings.IndexOf(content);
            if (currentIndex < 0) {
                _strings.Add(content);
                currentIndex = _strings.Count - 1;
            }
            OpCode(OpCodeType.LDSTR, position);
            DirectWrite(currentIndex);
        }
        
        /// <summary>
        /// 编写入栈可翻译字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadTranslatableString(string content, CodePosition position) {
            if (_translations.Count >= 0xFFFF) {
                throw new OutOfMemoryException("Only 65535 translatable strings are allowed in one VNS file");
            }
            var key = (_translations.Count << 16) + Hasher.Crc16(Encoding.UTF8.GetBytes(content));
            _translations.Add(key, content);
            OpCode(OpCodeType.LDSTT, position);
            DirectWrite(key);
        }

        /// <summary>
        /// 编写入栈32位有符号整数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadInteger(int value, CodePosition position) {
            switch (value) {
                case 0:
                    OpCode(OpCodeType.LDC_I4_0, position);
                    break;
                case 1:
                    OpCode(OpCodeType.LDC_I4_1, position);
                    break;
                case 2:
                    OpCode(OpCodeType.LDC_I4_2, position);
                    break;
                case 3:
                    OpCode(OpCodeType.LDC_I4_3, position);
                    break;
                case 4:
                    OpCode(OpCodeType.LDC_I4_4, position);
                    break;
                case 5:
                    OpCode(OpCodeType.LDC_I4_5, position);
                    break;
                case 6:
                    OpCode(OpCodeType.LDC_I4_6, position);
                    break;
                case 7:
                    OpCode(OpCodeType.LDC_I4_7, position);
                    break;
                case 8:
                    OpCode(OpCodeType.LDC_I4_8, position);
                    break;
                default:
                    OpCode(OpCodeType.LDC_I4, position);
                    DirectWrite(value);
                    break;
            }
        }
        
        /// <summary>
        /// 编写入栈32位浮点数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadFloat(float value, CodePosition position) {
            switch (value) {
                case 0.0F:
                    OpCode(OpCodeType.LDC_R4_0, position);
                    break;
                case 0.25F:
                    OpCode(OpCodeType.LDC_R4_025, position);
                    break;
                case 0.5F:
                    OpCode(OpCodeType.LDC_R4_05, position);
                    break;
                case 0.75F:
                    OpCode(OpCodeType.LDC_R4_075, position);
                    break;
                case 1.0F:
                    OpCode(OpCodeType.LDC_R4_1, position);
                    break;
                case 1.25F:
                    OpCode(OpCodeType.LDC_R4_125, position);
                    break;
                case 1.5F:
                    OpCode(OpCodeType.LDC_R4_15, position);
                    break;
                case 1.75F:
                    OpCode(OpCodeType.LDC_R4_175, position);
                    break;
                case 2.0F:
                    OpCode(OpCodeType.LDC_R4_2, position);
                    break;
                case 2.25F:
                    OpCode(OpCodeType.LDC_R4_225, position);
                    break;
                case 2.5F:
                    OpCode(OpCodeType.LDC_R4_25, position);
                    break;
                case 2.75F:
                    OpCode(OpCodeType.LDC_R4_275, position);
                    break;
                case 3F:
                    OpCode(OpCodeType.LDC_R4_3, position);
                    break;
                case 3.25F:
                    OpCode(OpCodeType.LDC_R4_325, position);
                    break;
                case 3.5F:
                    OpCode(OpCodeType.LDC_R4_35, position);
                    break;
                case 3.75F:
                    OpCode(OpCodeType.LDC_R4_375, position);
                    break;
                case 4F:
                    OpCode(OpCodeType.LDC_R4_4, position);
                    break;
                case 4.25F:
                    OpCode(OpCodeType.LDC_R4_425, position);
                    break;
                case 4.5F:
                    OpCode(OpCodeType.LDC_R4_45, position);
                    break;
                case 4.75F:
                    OpCode(OpCodeType.LDC_R4_475, position);
                    break;
                case 5F:
                    OpCode(OpCodeType.LDC_R4_5, position);
                    break;
                case 5.25F:
                    OpCode(OpCodeType.LDC_R4_525, position);
                    break;
                case 5.5F:
                    OpCode(OpCodeType.LDC_R4_55, position);
                    break;
                case 5.75F:
                    OpCode(OpCodeType.LDC_R4_575, position);
                    break;
                default:
                    OpCode(OpCodeType.LDC_R4, position);
                    DirectWrite(value);
                    break;
            }
        }

        /// <summary>
        /// 编写入栈空值指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadNull(CodePosition position) {
            OpCode(OpCodeType.LDNUL, position);
        }

        /// <summary>
        /// 编写入栈布尔常量指令
        /// </summary>
        /// <param name="value">目标布尔值</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadBoolean(bool value, CodePosition position) {
            OpCode(value ? OpCodeType.LDT : OpCodeType.LDF, position);
        }

        /// <summary>
        /// 编写入栈对话指令
        /// </summary>
        /// <param name="speaker">角色</param>
        /// <param name="content">对话内容</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadDialogue(string speaker, string content, CodePosition position) {
            LoadTranslatableString(content, position);
            if (speaker == null) {
                LoadNull(position);
            } else {
                LoadString(speaker, position);
            }
            OpCode(OpCodeType.DIALOGUE, position);
        }

        /// <summary>
        /// 编写切换脚本语言指令
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void Language(string language, CodePosition position) {
            LoadString(language, position);
            OpCode(OpCodeType.LANG, position);
        }

        /// <summary>
        /// 编写出栈指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void Pop(CodePosition position) {
            OpCode(OpCodeType.POP, position);
        }

        /// <summary>
        /// 编写调用栈顶插件指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void Call(CodePosition position) {
            OpCode(OpCodeType.CALL, position);
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

        public (byte[] Code, IReadOnlyDictionary<string, long> Labels, IReadOnlyDictionary<int, string> Translations, IReadOnlyCollection<string> Strings,
            IReadOnlyCollection<CodePosition> Positions) Create() {
            return ((_writer.BaseStream as MemoryStream)?.ToArray(), _labels, _translations, _strings, _positions);
        }
    }
}