using System.Collections.Generic;
using UnityEngine;
using WADV;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue;
using WADV.Plugins.Image;
using WADV.Plugins.Image.Effects;
using WADV.Plugins.Image.Utilities;
using WADV.Plugins.Input;
using WADV.Plugins.Unity;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;

namespace Game {
    public class RuntimeTester : MonoBehaviour {
        public async void TestScript() {
            var runtime = new ScriptRuntime("!Entrance");
            await runtime.ExecuteScript();
        }

        public async void NextDialogue() {
            await MessageService.ProcessAsync(Message.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.FinishContentWaiting));
        }

        public async void TestInput() {
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.HideDialogueBox, 0.3F));
            var title = new StringValue {value = "输入姓和名（空格隔开）"};
            var defaultText = new StringValue {value = "诹访部 翔平"};
            var confirmText = new StringValue {value = "继续"};
            var context = PluginExecuteContext.Create(new ScriptRuntime("Utilities"));
            var message = await MessageService.ProcessAsync(ContextMessage<InputPlugin.MessageIntegration.Content>.Create(
                                                                InputPlugin.MessageIntegration.Mask,
                                                                InputPlugin.MessageIntegration.CreateInput,
                                                                new InputPlugin.MessageIntegration.Content {Title = title, Default = defaultText, ButtonText = confirmText},
                                                                context));
            if (message is Message<string> stringMessage) {
                Debug.Log(stringMessage.Content);
            }
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask, DialoguePlugin.MessageIntegration.ShowDialogueBox, 0.3F));
        }

        public async void TestImage() {
            // @effect = [Effect Type=AlphaMask Time=1.0 Easing=QuadIn Threshold=0.2 Mask=[Texture2D Source='Resources://Mask/RoundFadeToLeft']]
            var effect = GraphicEffect.CreateInstance<SingleGraphicEffect>(typeof(TextureMaskTransition), new Dictionary<string, SerializableValue> {
                {"Mask", new Texture2DValue {source = "Resources://Mask/RoundFadeToLeft"}},
                {"Threshold", new FloatValue {value = 0.2F}}
            }, 1.0F, EasingType.QuadIn);
            await effect.Initialize();
            // [Show @effect Layer=100 Bind=Canvas [Image Source='Resources://tomo13i'] Name=Tomo PositionX=0 PositionY=-350]
            var context = PluginExecuteContext.Create(new ScriptRuntime("!Entrance"));
            context.Parameters.Add(new EffectValue("AlphaMask", effect), new NullValue());
            context.Parameters.Add(new StringValue {value = "Layer"}, new IntegerValue {value = 100});
            context.Parameters.Add(new StringValue {value = "Bind"}, new StringValue {value = "Canvas"});
            context.Parameters.Add(new ImageValue {Texture = new Texture2DValue {source = "Resources://tomo13i"}}, new NullValue());
            context.Parameters.Add(new StringValue {value = "Name"}, new StringValue {value = "Tomo"});
            context.Parameters.Add(new StringValue {value = "PositionX"}, new IntegerValue {value = 0});
            context.Parameters.Add(new StringValue {value = "PositionY"}, new IntegerValue {value = -350});
            // Call plugin
            var plugin = PluginManager.Find("Show");
            if (plugin != null) {
                await plugin.Execute(context);
            }
        }
    }
}