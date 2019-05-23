using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Plugins.Effect;
using WADV.Plugins.Image.Utilities;
using WADV.Thread;

namespace WADV.Plugins.Image {
    public partial class EffectControlPlugin : IMessenger {
        public int Mask { get; } = CoreConstant.Mask;

        public bool IsStandaloneMessage { get; } = false;

        public async Task<Message> Receive(Message message) {
            if (message.HasTag(CoreConstant.DumpRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                data.AddValue("list", _backgroundEffects);
            } else if (message.HasTag(CoreConstant.LoadRuntime)) {
                var data = ((Message<DumpRuntimeIntent>) message).Content;
                _backgroundEffects = data.GetValue<Dictionary<EffectValue, string[]>>("list");
                if (_backgroundEffects == null) {
                    _backgroundEffects = new Dictionary<EffectValue, string[]>();
                    return message;
                }
                var tasks = new List<Task>();
                foreach (var (effect, targets) in _backgroundEffects) {
                    tasks.Add(MessageService.ProcessAsync(Message<ImageMessageIntegration.PlayEffectContent>.Create(ImageMessageIntegration.Mask,
                        ImageMessageIntegration.PlayEffect, new ImageMessageIntegration.PlayEffectContent {
                            Effect = effect.Effect,
                            Names = targets
                        })));
                }
                await Dispatcher.WaitAll(tasks);
            }
            return message;
        }
    }
}