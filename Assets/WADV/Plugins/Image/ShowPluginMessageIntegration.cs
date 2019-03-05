using System.Threading.Tasks;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Plugins.Unity;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image {
    public partial class ShowPlugin : IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        public bool IsStandaloneMessage { get; } = false;
        
        public Task<Message> Receive(Message message) {
            if (message.HasTag(CoreConstant.DumpRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                data.AddValue((string) "ShowPlugin/DefaultTransform", (SerializableValue) _default);
            } else if (message.HasTag(CoreConstant.LoadRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                _default = data.GetValue<TransformValue>("ShowPlugin/DefaultTransform");
            }
            return Task.FromResult(message);
        }
    }
}