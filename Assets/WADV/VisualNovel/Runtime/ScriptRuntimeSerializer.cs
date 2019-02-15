using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using JetBrains.Annotations;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.VisualNovel.Runtime {
    [Serializable]
    public partial class ScriptRuntime : ISerializable {
        [UsedImplicitly]
        protected ScriptRuntime(SerializationInfo info, StreamingContext context) {
            MemoryStack = (Stack<SerializableValue>) info.GetValue("memory", typeof(Stack<SerializableValue>));
            Exported = (Dictionary<string, SerializableValue>) info.GetValue("exported", typeof(Dictionary<string, SerializableValue>));
            _callStack = (CallStack) info.GetValue("callstack", typeof(CallStack));
            _historyScope = (Stack<ScopeValue>) info.GetValue("history", typeof(Stack<ScopeValue>));
            _loadingScript = (ScriptRuntime) info.GetValue("loading", typeof(ScriptRuntime));
            ActiveScope = (ScopeValue) info.GetValue("scope", typeof(ScopeValue));
            if (ActiveScope != null) {
                Script = ScriptFile.LoadSync(ActiveScope.scriptId);
                Script.MoveTo(info.GetInt64("offset"));
                Script.UseTranslation(ActiveLanguage).Wait();
            }
            ActiveLanguage = info.GetString("language");
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("memory", MemoryStack);
            info.AddValue("exported", Exported);
            info.AddValue("callstack", _callStack);
            info.AddValue("history", _historyScope);
            info.AddValue("scope", ActiveScope);
            info.AddValue("language", ActiveLanguage);
            if (ActiveScope != null) {
                info.AddValue("offset", Script.CurrentPosition);
            }
            info.AddValue("loading", _loadingScript);
        }
    }
}