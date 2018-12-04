using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Extensions;

namespace Core.VisualNovel.Script.Compiler {
    public class AssembleFile {
        private readonly BinaryWriter _writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        private readonly Dictionary<string, long> _labels = new Dictionary<string, long>();
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
        private readonly List<string> _strings = new List<string>();
        private uint _stringCount;

        public long Position => _writer.BaseStream.Position;

        /// <summary>
        /// 编写指令
        /// </summary>
        /// <param name="code">指令类型</param>
        public void OpCode(OpCodeType code) {
            _writer.Write((byte) code);
        }

        /// <summary>
        /// 编写字符串
        /// </summary>
        /// <param name="value">目标字符串</param>
        public void String(string value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写数组内容
        /// </summary>
        /// <param name="values">目标数组</param>
        public void Array(params byte[] values) {
            _writer.Write(values);
        }

        /// <summary>
        /// 编写32位整数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void Number(int value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写64位整数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void Number(long value) {
            _writer.Write(value);
        }

        /// <summary>
        /// 编写入栈普通字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        public void LoadString(string content) {
            var currentIndex = _strings.IndexOf(content);
            if (currentIndex < 0) {
                _strings.Add(content);
                currentIndex = _strings.Count - 1;
            }
            OpCode(OpCodeType.LDSTR);
            _writer.Write(currentIndex);
        }
        
        /// <summary>
        /// 编写入栈可翻译字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        public void LoadTranslatableString(string content) {
            var key = (_translations.Count << 16) + Hasher.Crc16(Encoding.UTF8.GetBytes(content));
            _translations.Add(Convert.ToString(key, 16).ToUpper().PadLeft(8, '0'), content);
            OpCode(OpCodeType.LDSTT);
            _writer.Write(key);
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

        public (byte[] Content, IReadOnlyDictionary<string, long> Labels, IReadOnlyDictionary<string, string> Translations, IReadOnlyCollection<string> Strings) Create() {
            return ((_writer.BaseStream as MemoryStream)?.ToArray(), _labels, _translations, _strings);
        }
    }
}