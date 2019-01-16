using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WADV.Extensions;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;

namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 表示一个VNB脚本文件
    /// </summary>
    public class ByteCodeWriter {
        private readonly Writer _writer = new Writer(new MemoryStream(), Encoding.UTF8);
        private readonly Dictionary<long, SourcePosition> _positions = new Dictionary<long, SourcePosition>();
        private readonly Dictionary<int, long> _labels = new Dictionary<int, long>();
        private readonly Dictionary<uint, string> _translations = new Dictionary<uint, string>();
        private readonly List<string> _strings = new List<string>();
        private long _previousOffset;

        /// <summary>
        /// 编写指令
        /// </summary>
        /// <param name="code">指令类型</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void OperationCode(OperationCode code, SourcePosition position) {
            _positions.Add(_writer.BaseStream.Position, position);
            _writer.Write((byte) code);
        }

        /// <summary>
        /// 写入7位压缩32位整数
        /// </summary>
        /// <param name="value"></param>
        public void Write7BitEncodedInteger(int value) {
            _writer.Write7BitEncodedInt(value);
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
        /// 编写32无符号位整数
        /// </summary>
        /// <param name="value">目标数字</param>
        public void DirectWrite(uint value) {
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
        public void LoadString(string content, SourcePosition position) {
            var currentIndex = _strings.IndexOf(content);
            if (currentIndex < 0) {
                _strings.Add(content);
                currentIndex = _strings.Count - 1;
            }
            OperationCode(Compiler.OperationCode.LDSTR, position);
            _writer.Write7BitEncodedInt(currentIndex);
        }
        
        /// <summary>
        /// 编写入栈可翻译字符串指令
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadTranslatableString(string content, SourcePosition position) {
            if (_translations.Count >= 0xFFFF) {
                throw new OutOfMemoryException("Only 65535 translatable strings are allowed in one VNS file");
            }
            var key = ((uint) _translations.Count << 16) + Hasher.Crc16(Encoding.UTF8.GetBytes(content));
            _translations.Add(key, content);
            OperationCode(Compiler.OperationCode.LDSTT, position);
            DirectWrite(key);
        }

        /// <summary>
        /// 编写入栈32位有符号整数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadInteger(int value, SourcePosition position) {
            switch (value) {
                case 0:
                    OperationCode(Compiler.OperationCode.LDC_I4_0, position);
                    break;
                case 1:
                    OperationCode(Compiler.OperationCode.LDC_I4_1, position);
                    break;
                case 2:
                    OperationCode(Compiler.OperationCode.LDC_I4_2, position);
                    break;
                case 3:
                    OperationCode(Compiler.OperationCode.LDC_I4_3, position);
                    break;
                case 4:
                    OperationCode(Compiler.OperationCode.LDC_I4_4, position);
                    break;
                case 5:
                    OperationCode(Compiler.OperationCode.LDC_I4_5, position);
                    break;
                case 6:
                    OperationCode(Compiler.OperationCode.LDC_I4_6, position);
                    break;
                case 7:
                    OperationCode(Compiler.OperationCode.LDC_I4_7, position);
                    break;
                case 8:
                    OperationCode(Compiler.OperationCode.LDC_I4_8, position);
                    break;
                default:
                    OperationCode(Compiler.OperationCode.LDC_I4, position);
                    DirectWrite(value);
                    break;
            }
        }
        
        /// <summary>
        /// 编写入栈32位浮点数指令
        /// </summary>
        /// <param name="value">目标数字</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadFloat(float value, SourcePosition position) {
            switch (value) {
                case 0.0F:
                    OperationCode(Compiler.OperationCode.LDC_R4_0, position);
                    break;
                case 0.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_025, position);
                    break;
                case 0.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_05, position);
                    break;
                case 0.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_075, position);
                    break;
                case 1.0F:
                    OperationCode(Compiler.OperationCode.LDC_R4_1, position);
                    break;
                case 1.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_125, position);
                    break;
                case 1.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_15, position);
                    break;
                case 1.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_175, position);
                    break;
                case 2.0F:
                    OperationCode(Compiler.OperationCode.LDC_R4_2, position);
                    break;
                case 2.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_225, position);
                    break;
                case 2.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_25, position);
                    break;
                case 2.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_275, position);
                    break;
                case 3F:
                    OperationCode(Compiler.OperationCode.LDC_R4_3, position);
                    break;
                case 3.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_325, position);
                    break;
                case 3.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_35, position);
                    break;
                case 3.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_375, position);
                    break;
                case 4F:
                    OperationCode(Compiler.OperationCode.LDC_R4_4, position);
                    break;
                case 4.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_425, position);
                    break;
                case 4.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_45, position);
                    break;
                case 4.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_475, position);
                    break;
                case 5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_5, position);
                    break;
                case 5.25F:
                    OperationCode(Compiler.OperationCode.LDC_R4_525, position);
                    break;
                case 5.5F:
                    OperationCode(Compiler.OperationCode.LDC_R4_55, position);
                    break;
                case 5.75F:
                    OperationCode(Compiler.OperationCode.LDC_R4_575, position);
                    break;
                default:
                    OperationCode(Compiler.OperationCode.LDC_R4, position);
                    DirectWrite(value);
                    break;
            }
        }

        /// <summary>
        /// 编写入栈空值指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadNull(SourcePosition position) {
            OperationCode(Compiler.OperationCode.LDNUL, position);
        }

        /// <summary>
        /// 编写入栈布尔常量指令
        /// </summary>
        /// <param name="value">目标布尔值</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadBoolean(bool value, SourcePosition position) {
            OperationCode(value ? Compiler.OperationCode.LDT : Compiler.OperationCode.LDF, position);
        }

        /// <summary>
        /// 编写入栈对话指令
        /// </summary>
        /// <param name="speaker">角色</param>
        /// <param name="content">对话内容</param>
        /// <param name="position">指令在源文件中的位置</param>
        public void LoadDialogue(string speaker, string content, SourcePosition position) {
            LoadTranslatableString(content, position);
            if (speaker == null) {
                LoadNull(position);
            } else {
                LoadString(speaker, position);
            }
            OperationCode(Compiler.OperationCode.DIALOGUE, position);
        }

        /// <summary>
        /// 编写出栈指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void Pop(SourcePosition position) {
            OperationCode(Compiler.OperationCode.POP, position);
        }

        /// <summary>
        /// 编写调用栈顶插件指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void Call(SourcePosition position) {
            OperationCode(Compiler.OperationCode.CALL, position);
        }
        
        /// <summary>
        /// 编写调用栈顶函数指令
        /// </summary>
        /// <param name="position">指令在源文件中的位置</param>
        public void Func(SourcePosition position) {
            OperationCode(Compiler.OperationCode.FUNC, position);
        }
        
        /// <summary>
        /// 在下一条指令起始处创建跳转标签
        /// <param name="id">标签ID</param>
        /// </summary>
        public void CreateLabel(int id) {
            _labels.Add(id, _writer.BaseStream.Position);
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close() {
            _writer.Close();
        }

        /// <summary>
        /// 生成程序段
        /// </summary>
        /// <returns></returns>
        public byte[] CreateMainSegment() {
            return (_writer.BaseStream as MemoryStream)?.ToArray();
        }

        /// <summary>
        /// 返回汇编文件的各个数据段和程序段
        /// </summary>
        /// <returns></returns>
        public (byte[] Code, byte[] Labels, byte[] Strings, byte[] Positions, ScriptTranslation Translations) CreateSegments() {
            var segmentWriter = new Writer(new MemoryStream());
            segmentWriter.Write(_labels.Count);
            foreach (var (id, position) in _labels) {
                segmentWriter.Write7BitEncodedInt(id);
                segmentWriter.Write(position);
            }
            var labelSegment = (segmentWriter.BaseStream as MemoryStream)?.ToArray();
            segmentWriter.Close();
            segmentWriter = new Writer(new MemoryStream());
            segmentWriter.Write(_strings.Count);
            foreach (var stringConstant in _strings) {
                segmentWriter.Write(stringConstant);
            }
            var stringSegment = (segmentWriter.BaseStream as MemoryStream)?.ToArray();
            segmentWriter.Close();
            segmentWriter = new Writer(new MemoryStream());
            segmentWriter.Write(_positions.Count);
            var previousPosition = (long) 0;
            foreach (var (offset, position) in _positions) {
                segmentWriter.Write((byte) (offset - previousPosition)); // 已知直接写入中最长为32位，因此指令最大偏移间隔为32+8=40位，可以使用上限为255的byte
                previousPosition = offset;
                segmentWriter.Write7BitEncodedInt(position.Line);
                segmentWriter.Write7BitEncodedInt(position.Column);
            }
            var positionSegment = (segmentWriter.BaseStream as MemoryStream)?.ToArray();
            segmentWriter.Close();
            return (CreateMainSegment(), labelSegment, stringSegment, positionSegment, new ScriptTranslation(_translations));
        }

        /// <summary>
        /// 用于访问BinaryWriter内部函数的特定结构
        /// </summary>
        private class Writer : BinaryWriter {
            public Writer([NotNull] Stream input, [NotNull] Encoding encoding) : base(input, encoding) {}

            public Writer([NotNull] Stream output) : base(output) {}

            public new void Write7BitEncodedInt(int value) {
                base.Write7BitEncodedInt(value);
            }
        }
    }
}