using System.IO;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Translation;
using JetBrains.Annotations;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个可执行VNB脚本
    /// </summary>
    public class ScriptFile {
        /// <summary>
        /// 代码段当前读取偏移地址
        /// </summary>
        public long CurrentPosition => _reader.BaseStream.Position;

        /// <summary>
        /// 获取激活的翻译
        /// </summary>
        public ScriptTranslation ActiveTranslation { get; private set; }
        
        /// <summary>
        /// 获取脚本文件头
        /// </summary>
        public ScriptHeader Header { get; }

        private readonly ExtendedBinaryReader _reader;

        /// <summary>
        /// 创建一个运行时脚本
        /// </summary>
        /// <param name="header">脚本执行内容文件头</param>
        /// <param name="code">脚本代码段</param>
        public ScriptFile([NotNull] ScriptHeader header, [NotNull] byte[] code) {
            Header = header;
            _reader = new ExtendedBinaryReader(new MemoryStream(code));
            UseTranslation();
        }

        ~ScriptFile() {
            _reader.Close();
        }

        /// <summary>
        /// 设置激活的翻译
        /// <para>如果目标翻译不存在会自动使用默认翻译</para>
        /// </summary>
        /// <param name="name">语言名称</param>
        public void UseTranslation(string name = TranslationManager.DefaultLanguage) {
            ActiveTranslation = Header.LoadTranslation(name) ?? Header.LoadTranslation(TranslationManager.DefaultLanguage);
        }

        /// <summary>
        /// 移动到代码段指定偏移处
        /// </summary>
        /// <param name="offset">目标偏移</param>
        public void MoveTo(long offset) {
            _reader.BaseStream.Position = offset;
        }

        /// <summary>
        /// 跳转到指定标签处
        /// </summary>
        /// <param name="labelId">标签ID</param>
        public void JumpTo(int labelId) {
            MoveTo(Header.Labels[labelId]);
        }

        public OperationCode? ReadOperationCode() {
            if (_reader.BaseStream.Position >= _reader.BaseStream.Length) {
                return null;
            }
            var value = _reader.ReadByte();
            return value <= 0x45 ? (OperationCode) value : (OperationCode?) null;
        }

        public int ReadInteger() {
            return _reader.ReadInt32();
        }

        public int Read7BitEncodedInt() {
            return _reader.Read7BitEncodedInt();
        }

        public float ReadFloat() {
            return _reader.ReadSingle();
        }

        [CanBeNull]
        public string ReadStringConstant() {
            var stringId = _reader.Read7BitEncodedInt();
            return stringId < Header.Strings.Count ? Header.Strings[stringId] : null;
        }

        public string ReadString() {
            return _reader.ReadString();
        }

        public long? ReadLabelOffset() {
            var labelId = _reader.Read7BitEncodedInt();
            return labelId < Header.Labels.Count ? Header.Labels[labelId] : (long?) null;
        }

        public uint ReadUInt32() {
            return _reader.ReadUInt32();
        }
    }
}