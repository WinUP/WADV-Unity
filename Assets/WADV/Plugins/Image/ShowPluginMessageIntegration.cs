using System.Threading.Tasks;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Plugins.Unity;

namespace WADV.Plugins.Image {
    public partial class ShowPlugin : IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        public bool IsStandaloneMessage { get; } = false;
        
        public Task<Message> Receive(Message message) {
            if (message.HasTag(CoreConstant.DumpRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                data.AddValue("ShowPlugin/DefaultTransform", _defaultTransform);
                data.AddValue("ShowPlugin/DefaultLayer", _defaultLayer);
            } else if (message.HasTag(CoreConstant.LoadRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                _defaultTransform = data.GetValue<TransformValue>("ShowPlugin/DefaultTransform");
                _defaultLayer = data.GetInteger("ShowPlugin/DefaultLayer");
            }
            return Task.FromResult(message);
        }
    }
}