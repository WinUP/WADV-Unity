using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Effect;
using WADV.Plugins.Image.Utilities;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image {
    [StaticRegistrationInfo("PlayEffect")]
    [StaticRegistrationInfo("StopEffect")]
    [UsedImplicitly]
    public partial class EffectControlPlugin : IVisualNovelPlugin {
        private static bool _enableEffectLoop;

        private static Dictionary<EffectValue, string[]> _backgroundEffects = new Dictionary<EffectValue, string[]>();

        public EffectControlPlugin() {
            PluginManager.ListenerRoot.CreateChild(this, -1);
        }
        
        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            EffectValue effect = null;
            var images = new List<string>();
            foreach (var (key, value) in context.Parameters) {
                switch (key) {
                    case EffectValue effectValue:
                        effect = effectValue;
                        break;
                    case IStringConverter stringConverter:
                        var target = stringConverter.ConvertToString(context.Language);
                        if (value != null && !(value is NullValue)) {
                            switch (target) {
                                case "Image":
                                    var name = StringValue.TryParse(value);
                                    if (!images.Contains(name)) {
                                        images.Add(name);
                                    }
                                    break;
                                case "Effect":
                                    if (value is EffectValue effectValue) {
                                        effect = effectValue;
                                    } else {
                                        throw new ArgumentException($"Unable to set effect: {value} is not EffectValue", nameof(value));
                                    }
                                    break;
                            }
                        } else {
                            if (!images.Contains(target)) {
                                images.Add(target);
                            }
                        }
                        break;
                }
            }
            if (context.CommandName == "PlayEffect") {
                await PlayEffect(effect, images.ToArray());
            } else {
                await StopEffect(images.ToArray());
            }
            return new NullValue();
        }

        private static async Task PlayEffect(EffectValue effect, string[] images) {
            if (effect == null || images.Length == 0) return;
            if (effect.Effect is StaticGraphicEffect) {
                string[] targets;
                if (_backgroundEffects.ContainsKey(effect)) {
                    targets = _backgroundEffects[effect];
                    _backgroundEffects[effect] = targets.Concat(images).ToArray();
                } else {
                    targets = images.ToArray();
                    _backgroundEffects.Add(effect, targets);
                }
            }
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.PlayEffectContent>.Create(ImageMessageIntegration.Mask,
                ImageMessageIntegration.PlayEffect, new ImageMessageIntegration.PlayEffectContent {
                    Effect = effect.Effect,
                    Names = images
                }));
        }

        private static async Task StopEffect(string[] images) {
            await MessageService.ProcessAsync(Message<string[]>.Create(ImageMessageIntegration.Mask,
                ImageMessageIntegration.StopEffect, images));
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}