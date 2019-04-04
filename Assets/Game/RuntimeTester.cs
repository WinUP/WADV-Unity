using System.Collections.Generic;
using UnityEngine;
using WADV;
using WADV.MessageSystem;
using WADV.Plugins.Dialogue;
using WADV.Plugins.Image;
using WADV.Plugins.Image.Effects;
using WADV.Plugins.Image.Utilities;
using WADV.Plugins.Input;
using WADV.Plugins.Unity;
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

        public async void UseInput() {
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask,
                                                             DialoguePlugin.MessageIntegration.HideDialogueBox, 0.3F));
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
            await MessageService.ProcessAsync(Message<float>.Create(DialoguePlugin.MessageIntegration.Mask,
                                                                    DialoguePlugin.MessageIntegration.ShowDialogueBox, 0.3F));
        }

        public async void TestImage() {
            var content = new ImageMessageIntegration.ShowImageContent();
            var image1Transform = new TransformValue();
            image1Transform.Set(TransformValue.PropertyName.PositionX, -225);
            image1Transform.Set(TransformValue.PropertyName.PositionY, -156);
            var image1 = new ImageDisplayInformation("Tomo", new ImageValue {source = "Resources://tomo13i"}, image1Transform) {layer = 1000};
            var image2Transform = new TransformValue();
            image2Transform.Set(TransformValue.PropertyName.PositionX, 154);
            image2Transform.Set(TransformValue.PropertyName.PositionY, -156);
            var image2 = new ImageDisplayInformation("Tsubasa", new ImageValue {source = "Resources://tubasa37i"}, image2Transform) {layer = 1000};
            content.Images = new[] {image1, image2};
            content.Effect = GraphicEffect.CreateInstance<SingleGraphicEffect>(typeof(FadeIn), new Dictionary<string, SerializableValue>(), 2.0F, EasingType.QuadIn);
            await content.Effect.Initialize();
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }
    }
}