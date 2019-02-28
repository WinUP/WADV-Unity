using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
    /// <para>输入框插件</para>
    /// <list type="bullet">
    ///     <listheader><description>可选有值参数</description></listheader>
    ///     <item><description>Title: 标题字符串</description></item>
    ///     <item><description>Default: 默认内容字符串</description></item>
    ///     <item><description>ButtonText: 确认按钮文本字符串</description></item>
    /// </list>
    /// </summary>
    [StaticRegistrationInfo("Input")]
    [UsedImplicitly]
    public partial class InputPlugin : IVisualNovelPlugin {
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
            return message is Message<string> stringMessage ? new StringValue {value = stringMessage.Content} : null;
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }
    }
}