using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.VisualNovel.Provider;

namespace WADV.Providers {
    /// <inheritdoc />
    /// <summary>
    /// 用于读取Unity中Persistent目录内容的资源提供器
    /// </summary>
    public class UnityPersistentResourceProvider : ResourceProvider {
        /// <inheritdoc />
        public UnityPersistentResourceProvider() : base("Persistent", 0) { }

        /// <inheritdoc />
        public override Task<object> Load(string id) {
            id = id.UnifySlash();
            var path = id.StartsWith("/") ? $"{Application.persistentDataPath}{id}" : $"{Application.persistentDataPath}/{id}";
            return !File.Exists(path) ? null : Task.FromResult((object) new BinaryData(File.ReadAllBytes(path)));
        }

        /// <inheritdoc />
        public override async Task<T> Load<T>(string id) {
            var result = await Load(id);
            if (result == null) return null;
            if (typeof(T) == typeof(BinaryData)) return (T) result;
            try {
                var deserializer = new BinaryFormatter();
                var stream = new MemoryStream(((BinaryData) result).Data);
                var item = deserializer.Deserialize(stream);
                stream.Close();
                return item.GetType() == typeof(T) ? (T) item : null;
            } catch {
                return null;
            }
        }

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
}