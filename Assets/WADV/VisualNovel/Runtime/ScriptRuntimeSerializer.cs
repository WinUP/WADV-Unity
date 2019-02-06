using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.VisualNovel.Runtime {
    public partial class ScriptRuntime : ISerializable {
        [UsedImplicitly]
        protected ScriptRuntime(SerializationInfo info, StreamingContext context) {
            MemoryStack = (Stack<SerializableValue>) info.GetValue("memory", typeof(Stack<SerializableValue>));
            Exported = (Dictionary<string, SerializableValue>) info.GetValue("exported", typeof(Dictionary<string, SerializableValue>));
            _callStack = (CallStack) info.GetValue("callstack", typeof(CallStack));
            _historyScope = (Stack<ScopeValue>) info.GetValue("historyScope", typeof(Stack<ScopeValue>));
            ActiveScope = (ScopeValue) info.GetValue("scope", typeof(ScopeValue));
            if (ActiveScope != null) {
                Script = ScriptFile.LoadSync(ActiveScope.ScriptId);
                Script.MoveTo(info.GetInt64("offset"));
                Script.UseTranslation(ActiveLanguage).Wait();
            }
            ActiveLanguage = info.GetString("language");
        }
        
        /// <summary>
        /// 将二进制数组转换为脚本运行环境
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        [CanBeNull]
        public static ScriptRuntime Load(byte[] data) {
            var deserializer = new BinaryFormatter();
            return (ScriptRuntime) deserializer.Deserialize(new MemoryStream(data));
        }
      
        /// <summary>
        /// 将脚本运行环境转换为二进制数组
        /// </summary>
        /// <returns></returns>
        public byte[] Dump() {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, this);
            return stream.ToArray();
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("memory", MemoryStack);
            info.AddValue("exported", Exported);
            info.AddValue("callstack", _callStack);
            info.AddValue("historyScope", _historyScope);
            info.AddValue("scope", ActiveScope);
            info.AddValue("language", ActiveLanguage);
            if (ActiveScope != null) {
                info.AddValue("offset", Script.CurrentPosition);
            }
        }
    }
}