using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

// ReSharper disable once CommentTypo
// ! CJK+ASCII+JP+Punctuations: 0020-007E,2010,2012-2027,2030-205E,3000-30FF,31D0-31FF,4E00-9FEF,FF00-FFEF
// ! Not covered by current project

namespace WADV.Plugins.Dialogue {
    /// <inheritdoc cref="IVisualNovelPlugin" />
    /// <summary>
    /// 对话解析插件
    /// </summary>
    [StaticRegistrationInfo("Dialogue")]
    public partial class DialoguePlugin : IVisualNovelPlugin {
        private static Regex CommandTester { get; } = new Regex(@"\s*([^=]+)\s*=\s*(\S+)\s*$");
        
        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
        
        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var dialogue = new MessageIntegration.Content();
            foreach (var (name, value) in context.StringParameters) {
                switch (name.ConvertToString(context.Language)) {
                    case "Show":
                        await ShowWindow(value, context.Language);
                        return new NullValue();
                    case "Hide":
                        await HideWindow(value, context.Language);
                        return new NullValue();
                    case "Character":
                        dialogue.Character = value;
                        break;
                    case "Content":
                        if (value is IStringConverter stringContent) {
                            dialogue.Text = stringContent;
                        } else {
                            throw new ArgumentException($"Unable to create dialogue: unsupported content type {value}");
                        }
                        break;
                }
            }
            await MessageService.ProcessAsync(ContextMessage<MessageIntegration.Content>.Create(context, dialogue, MessageIntegration.Mask, MessageIntegration.NewDialogue));
            return new NullValue();
        }
        
        private static async Task ShowWindow(SerializableValue time, string language) {
            float showValue;
            try {
                showValue = FloatValue.TryParse(time, language);
            } catch {
                showValue = 0.0F;
            }
            await MessageService.ProcessAsync(Message<float>.Create(showValue, MessageIntegration.Mask, MessageIntegration.ShowDialogueBox));
        }
        
        private static async Task HideWindow(SerializableValue time, string language) {
            float hideValue;
            try {
                hideValue = FloatValue.TryParse(time, language);
            } catch {
                hideValue = 0.0F;
            }
            await MessageService.ProcessAsync(Message<float>.Create(hideValue, MessageIntegration.Mask, MessageIntegration.HideDialogueBox));
        }
    }
}