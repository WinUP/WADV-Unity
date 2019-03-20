using System.Text;

namespace WADV.Resource {
    /// <summary>
    /// 表示二进制数据
    /// </summary>
    public class BinaryData {
        /// <summary>
        /// 获取数据内容
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// 获取数据的UTF8字符表示
        /// </summary>
        public string Text {
            get {
                if (string.IsNullOrEmpty(_text)) {
                    _text = Encoding.UTF8.GetString(Data);
                }
                return _text;
            }
        }

        private string _text;

        /// <summary>
        /// 创建一个二进制数据
        /// </summary>
        /// <param name="data">数据内容</param>
        public BinaryData(byte[] data) {
            Data = data;
        }
    }
}