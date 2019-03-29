using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Plugins.Image.Utilities;
using WADV.Plugins.Unity;

namespace WADV.Plugins.Image {
    public partial class ShowPlugin : IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        public bool IsStandaloneMessage { get; } = false;
        
        public async Task<Message> Receive(Message message) {
            if (message.HasTag(CoreConstant.DumpRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                data.AddValue("ShowPlugin/DefaultTransform", _defaultTransform);
                data.AddValue("ShowPlugin/DefaultLayer", _defaultLayer);
                data.AddValue("ShowPlugin/Images", _images);
            } else if (message.HasTag(CoreConstant.LoadRuntime)) {
                if (_placeholder != null) {
                    await _placeholder;
                }
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                _defaultTransform = data.GetValue<TransformValue>("ShowPlugin/DefaultTransform");
                _defaultLayer = data.GetInteger("ShowPlugin/DefaultLayer");
                _images = data.GetValue<Dictionary<string, ImageDisplayInformation>>("ShowPlugin/Images");
            }
            return message;
        }
    }
}