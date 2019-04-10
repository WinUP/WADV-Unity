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
            var effect = GraphicEffect.CreateInstance<SingleGraphicEffect>(typeof(FadeIn), new Dictionary<string, SerializableValue>(), 1.0F, EasingType.QuadIn);
            await effect.Initialize();
            var content = new ImageMessageIntegration.ShowImageContent();

            var shader = (await MessageService.ProcessAsync<ComputeShader>(Message.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.GetBindShader))).Content;
            var combiner = new Texture2DCombiner(800, 600, null);

            var background = new ImageValue {source = "Resources://Classroom 2"};
            await background.ReadTexture();
            combiner.DrawTexture(background.texture, Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one));
            var image1 = new ImageValue {source = "Resources://tomo13i"};
            await image1.ReadTexture();
            combiner.DrawTexture(image1.texture,
                                 Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, -15), new Vector3(1F, 1F, 1F)),
                                 Color.white,
                                 new Vector2(0.5F, 0.5F));
            content.Effect = effect;
            var combinedTransform = new TransformValue();
            combinedTransform.Set(TransformValue.PropertyName.PositionX, 0);
            combinedTransform.Set(TransformValue.PropertyName.PositionY, 0);
            content.Images = new[] {
                new ImageDisplayInformation("Combined", new ImageValue {texture = combiner.Combine()}, combinedTransform)
            };
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }
    }
}