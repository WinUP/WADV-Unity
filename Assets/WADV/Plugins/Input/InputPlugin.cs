using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Input {
    /// <inheritdoc />
    /// <summary>
    /// 输入框插件
    /// </summary>
    [UsedImplicitly]
    public partial class InputPlugin : VisualNovelPlugin {
        public InputPlugin() : base("Input") { }
        
        /// <inheritdoc />
        public override async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var description = new MessageIntegration.Content();
            foreach (var (name, value) in context.StringParameters) {
                switch (name.ConvertToString(context.Language)) {
                    case "Title":
                        if (value is IStringConverter stringTitle) {
                            description.Title = stringTitle;
                        } else
                            throw new NotSupportedException($"Unable to create input: title {value} is not string value");
                        break;
                    case "Default":
                        if (value is IStringConverter stringDefault) {
                            description.Default = stringDefault;
                        } else
                            throw new NotSupportedException($"Unable to create input: default {value} is not string value");
                        break;
                    case "ButtonText":
                        if (value is IStringConverter stringButton) {
                            description.ButtonText = stringButton;
                        } else
                            throw new NotSupportedException($"Unable to create input: button text {value} is not string value");
                        break;
                }
            }
            var message = await MessageService.ProcessAsync(ContextMessage<MessageIntegration.Content>.Create(context, description, MessageIntegration.Mask, MessageIntegration.CreateInput));
            return message is Message<string> stringMessage ? new StringValue {Value = stringMessage.Content} : null;
        }
    }
}