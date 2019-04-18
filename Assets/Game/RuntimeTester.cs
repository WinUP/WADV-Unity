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
            var background = new ImageValue {Texture = new Texture2DValue {source = "Resources://Classroom 2"}};
            await background.Texture.ReadTexture();
            var tomo = new ImageValue {Texture = new Texture2DValue {source = "Resources://tomo13i"}};
            await tomo.Texture.ReadTexture();
            var tsubasa = new ImageValue {Texture = new Texture2DValue {source = "Resources://tubasa37i"}};
            await tsubasa.Texture.ReadTexture();
            var effect1 = GraphicEffect.CreateInstance<SingleGraphicEffect>(typeof(FadeIn), new Dictionary<string, SerializableValue>(), 1.0F, EasingType.QuadIn);
            await effect1.Initialize();
            var effect2 = GraphicEffect.CreateInstance<SingleGraphicEffect>(typeof(TextureMaskTransition), new Dictionary<string, SerializableValue> {
                {"Mask", new Texture2DValue {source = "Resources://Mask/RoundFadeToLeft"}},
                {"Threshold", new FloatValue {value = 0.2F}}
            }, 1.0F, EasingType.QuadIn);
            await effect2.Initialize();
            
            var content = new ImageMessageIntegration.ShowImageContent();
            var transform = new TransformValue();
            transform.Set(TransformValue.PropertyName.PositionX, 0);
            transform.Set(TransformValue.PropertyName.PositionY, -350);
            transform.Set(TransformValue.PropertyName.PivotX, 0);
            transform.Set(TransformValue.PropertyName.PivotY, 0);
            transform.Set(TransformValue.PropertyName.AnchorMinX, 0);
            transform.Set(TransformValue.PropertyName.AnchorMinY, 0);
            transform.Set(TransformValue.PropertyName.AnchorMaxX, 0);
            transform.Set(TransformValue.PropertyName.AnchorMaxY, 0);
            content.Images = new[] {
                new ImageDisplayInformation("Tomo", tomo, transform)
            };
            content.Effect = effect1;
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
            await Dispatcher.WaitForSeconds(2.0F);
            content.Images = new[] {
                new ImageDisplayInformation("Tomo", tsubasa, transform)
            };
            content.Effect = effect2;
            await MessageService.ProcessAsync(Message<ImageMessageIntegration.ShowImageContent>.Create(ImageMessageIntegration.Mask, ImageMessageIntegration.ShowImage, content));
        }
    }
}