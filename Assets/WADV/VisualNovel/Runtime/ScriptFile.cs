using System.IO;
using System.Threading.Tasks;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;

namespace WADV.VisualNovel.Runtime {
    /// <summary>
    /// 表示一个VNB脚本
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
            ActiveTranslation = Header.LoadDefaultTranslation();
        }

        ~ScriptFile() {
            _reader.Close();
        }

        /// <summary>
        /// 根据脚本ID创建运行时脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ScriptFile LoadSync(string id) {
            return ScriptHeader.LoadSync(id).Header.CreateRuntimeFile();
        }
        
        /// <summary>
        /// 根据脚本ID创建运行时脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ScriptFile> Load(string id) {
            return (await ScriptHeader.Load(id)).Header.CreateRuntimeFile();
        }

        /// <summary>
        /// 设置激活的翻译
        /// <para>如果目标翻译不存在会自动使用默认翻译</para>
        /// </summary>
        /// <param name="name">语言名称</param>
        public async Task UseTranslation(string name = TranslationManager.DefaultLanguage) {
            ActiveTranslation = await Header.LoadTranslation(name) ?? Header.LoadDefaultTranslation();
        }

        /// <summary>
        /// 移动到代码段指定偏移处
        /// </summary>
        /// <param name="offset">目标偏移</param>
        public void MoveTo(long offset) {
            _reader.BaseStream.Position = offset;
        }

        /// <summary>
        /// 读取字节码
        /// </summary>
        /// <returns></returns>
        public OperationCode? ReadOperationCode() {
            if (_reader.BaseStream.Position >= _reader.BaseStream.Length) {
                return null;
            }
            var value = _reader.ReadByte();
            return (OperationCode) value;
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <returns></returns>
        public int ReadInteger() {
            return _reader.ReadInt32();
        }

        /// <summary>
        /// 读取7位压缩的32位整数
        /// </summary>
        /// <returns></returns>
        public int Read7BitEncodedInt() {
            return _reader.Read7BitEncodedInt();
        }

        /// <summary>
        /// 读取32位浮点数
        /// </summary>
        /// <returns></returns>
        public float ReadFloat() {
            return _reader.ReadSingle();
        }
        
        /// <summary>
        /// 读取字符串常量编号并返回其内容
        /// </summary>
        /// <returns></returns>
        public string ReadStringConstant() {
            var stringId = _reader.Read7BitEncodedInt();
            return Header.Strings[stringId];
        }

        /// <summary>
        /// 读取标签编号并返回其对应的偏移地址
        /// </summary>
        /// <returns></returns>
        public long ReadLabelOffset() {
            var labelId = _reader.Read7BitEncodedInt();
            return Header.Labels[labelId];
        }

        /// <summary>
        /// 读取32位无符号整数
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32() {
            return _reader.ReadUInt32();
        }
    }
}