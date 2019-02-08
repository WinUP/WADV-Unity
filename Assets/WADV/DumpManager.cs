using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.VisualNovel.Runtime;

namespace WADV {
    /// <summary>
    /// 数据转储管理器
    /// </summary>
    public static class DumpManager {
        /// <summary>
        /// 将脚本运行环境转储为二进制数组
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> Dump(ScriptRuntime runtime) {
            var intent = DumpRuntimeIntent.CreateEmpty();
            intent.runtime = runtime;
            var message = await MessageService.ProcessAsync(Message<DumpRuntimeIntent>.Create(CoreConstant.Mask, CoreConstant.DumpRuntime, intent));
            if (message is Message<DumpRuntimeIntent> returnMessage) {
                intent = returnMessage.Content;
            } else {
                throw new NotSupportedException($"Unable to dump runtime: message broadcast result is not DumpRuntimeIntent");
            }
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, intent);
            return stream.ToArray();
        }

        /// <summary>
        /// 将二进制数组转换为脚本运行环境
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        public static async Task<DumpRuntimeIntent> Read(byte[] data) {
            var deserializer = new BinaryFormatter();
            var intent = (DumpRuntimeIntent) deserializer.Deserialize(new MemoryStream(data));
            await MessageService.ProcessAsync(Message<DumpRuntimeIntent>.Create(CoreConstant.Mask, CoreConstant.DumpRuntime, intent));
            return intent;
        }
    }
}