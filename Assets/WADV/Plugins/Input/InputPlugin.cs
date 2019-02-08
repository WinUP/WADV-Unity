using System;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Input {
    /// <inheritdoc />
    /// <summary>
    /// 输入框插件
    /// </summary>
    [StaticRegistrationInfo("Input")]
    public partial class InputPlugin : IVisualNovelPlugin {
        /// <inheritdoc />
        public async Task<SerializableValue> Execute(PluginExecuteContext context) {
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
                    default:
                        Debug.LogWarning($"Input plugin: unknown parameter {name}");
                        break;
                }
            }
            var message = await MessageService.ProcessAsync(ContextMessage<MessageIntegration.Content>.Create(MessageIntegration.Mask, MessageIntegration.CreateInput, description, context));
            return message is Message<string> stringMessage ? new StringValue {Value = stringMessage.Content} : null;
        }

        /// <inheritdoc />
        public bool OnRegister() => true;

        /// <inheritdoc />
        public bool OnUnregister(bool isReplace) => true;
    }
}