using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.MessageSystem;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using WADV.VisualNovelPlugins.Dialogue.Items;

// ! CJK+ASCII Range+JP: 0020-007E,3000-30FF,31D0-31FF,4E00-9FEF,FF00-FFEF

namespace WADV.VisualNovelPlugins.Dialogue {
    /// <inheritdoc cref="VisualNovelPlugin" />
    /// <summary>
    /// 对话解析插件
    /// </summary>
    [UsedImplicitly]
    public class DialoguePlugin : VisualNovelPlugin, IMessenger {
        /// <summary>
        /// 插件使用的消息掩码
        /// </summary>
        public const int MessageMask = CoreConstant.Mask;
        
        /// <summary>
        /// 表示新建对话的消息标记
        /// </summary>
        public const string NewDialogueMessageTag = "NEW_DIALOGUE";

        /// <summary>
        /// 表示显示对话框的消息标记
        /// </summary>
        public const string ShowDialogueBoxMessageTag = "SHOW_DIALOGUE_TEXT";
        
        /// <summary>
        /// 表示隐藏对话框的消息标记
        /// </summary>
        public const string HideDialogueBoxMessageTag = "HIDE_DIALOGUE_TEXT";

        /// <inheritdoc />
        public int Mask { get; } = CoreConstant.Mask;
        
        /// <inheritdoc />
        public bool IsStandaloneMessage { get; } = false;
        
        private static Regex CommandTester { get; } = new Regex(@"\s*([^=]+)\s*=\s*(([\d.]+))\s*$");
        private readonly Cache _cache = new Cache();
        
        public DialoguePlugin() : base("Dialogue") { }

        /// <summary>
        /// 分析对话内容
        /// </summary>
        /// <param name="runtime">脚本运行环境</param>
        /// <param name="data">原始对话内容</param>
        /// <returns></returns>
        public static (List<IDialogueItem> Content, bool NoWait, bool NoClear) ProcessDialogueContent(ScriptRuntime runtime, string data) {
            var result = new List<IDialogueItem>();
            var content = new StringBuilder();
            var style = new StyleList();
            var noWait = false;
            var noClear = false;
            var i = -1;
            if (data.StartsWith("[noclear]", StringComparison.OrdinalIgnoreCase)) {
                noClear = true;
                i = 8;
            }
            for (i = -1; ++i < data.Length;) {
                switch (data[i]) {
                    case '\\' when i < data.Length - 1:
                        switch (data[i + 1]) {
                            case 'n':
                                content.Append('\n');
                                break;
                            case 't':
                                content.Append('\t');
                                break;
                            case 's':
                                content.Append(' ');
                                break;
                            default:
                                content.Append(data[i + 1]);
                                break;
                        }
                        ++i;
                        break;
                    case '[' when i == data.Length - 1:
                        content.Append('[');
                        break;
                    case '[':
                        var endIndex = data.IndexOf(']', i + 1);
                        if (endIndex <= i) throw new ArgumentException($"Unable to create dialogue command: missing end bracket for {i}");
                        var command = data.Substring(i + 1, endIndex - i - 1);
                        i = endIndex;
                        var analyseFailed = false;
                        if (string.IsNullOrEmpty(command)) { // 空指令不处理
                            content.Append("[]");
                        } else if (command.StartsWith("@#")) { // 常量
                            var constantName = command.Substring(2);
                            if (string.IsNullOrEmpty(constantName)) throw new ArgumentException($"Unable to create dialog command: missing constant name at {i}");
                            var constant = runtime.ActiveScope?.FindVariable(constantName, true, VariableSearchMode.OnlyConstant);
                            if (constant == null) {
                                analyseFailed = true;
                            } else {
                                content.Append(constant.Value is IStringConverter stringConverter ? stringConverter.ConvertToString() : constant.Value.ToString());
                            }
                        } else if (command.StartsWith("@")) { // 变量
                            var variableName = command.Substring(1);
                            if (string.IsNullOrEmpty(variableName)) throw new ArgumentException($"Unable to create dialog command: missing variable name at {i}");
                            var variable = runtime.ActiveScope?.FindVariable(variableName, true, VariableSearchMode.All);
                            if (variable == null) {
                                analyseFailed = true;
                            } else {
                                content.Append(variable.Value is IStringConverter stringConverter ? stringConverter.ConvertToString() : variable.Value.ToString());
                            }
                        } else {
                            var commandContent = command.ToLower().Trim();
                            if (commandContent.StartsWith("size")) { // 字号
                                var parameter = ExtractParameter(commandContent);
                                var relative = parameter.Value.Value.StartsWith("+") || parameter.Value.Value.StartsWith("-");
                                if (parameter.HasValue && float.TryParse(parameter.Value.Value, out var number)) {
                                    style.Size.AddLast((Mathf.RoundToInt(number), relative));
                                } else {
                                    analyseFailed = true;
                                }
                            } else if (commandContent == "/size") { // 取消字号
                                if (style.Size.Any()) {
                                    style.Size.RemoveLast();
                                } else {
                                    analyseFailed = true;
                                }
                            } else if (commandContent.StartsWith("color")) { // 颜色
                                var parameter = ExtractParameter(commandContent);
                                if (parameter.HasValue) {
                                    style.Color.AddLast(parameter.Value.Value);
                                } else {
                                    analyseFailed = true;
                                }
                            } else if (commandContent == "/color") { // 取消颜色
                                if (style.Color.Any()) {
                                    style.Color.RemoveLast();
                                } else {
                                    analyseFailed = true;
                                }
                            } else if (commandContent.StartsWith("pause")) { // 暂停
                                if (commandContent == "pause") {
                                    result.Add(new PauseDialogueItem {Time = null});
                                } else {
                                    var parameter = ExtractParameter(commandContent);
                                    if (parameter.HasValue && float.TryParse(parameter.Value.Value, out var number)) {
                                        result.Add(new PauseDialogueItem {Time = number});
                                    } else {
                                        analyseFailed = true;
                                    }
                                }
                            } else if (commandContent == "clear") { // 清空
                                result.Add(new ClearDialogueItem());
                            } else if (commandContent == "nowait") {
                                if (i == data.Length - 1) {
                                    noWait = true;
                                }
                            } else switch (command) {
                                case "b": // 粗体
                                    style.Bold = true;
                                    break;
                                case "/b": // 取消粗体
                                    style.Bold = false;
                                    break;
                                case "i": // 斜体
                                    style.Italic = true;
                                    break;
                                case "/i": // 取消斜体
                                    style.Italic = false;
                                    break;
                                case "s": // 删除线
                                    style.Strikethrough = true;
                                    break;
                                case "/s": // 取消删除线
                                    style.Strikethrough = false;
                                    break;
                                case "u": // 下划线
                                    style.Underline = true;
                                    break;
                                case "/u": // 取消下划线
                                    style.Underline = false;
                                    break;
                                default:
                                    analyseFailed = true;
                                    break;
                            }
                            if (!analyseFailed && !commandContent.StartsWith("pause")) {
                                result.Add(style.Combine(content.ToString()));
                                content.Clear();
                            }
                        }
                        if (analyseFailed) {
                            content.Append($"[{command}]");
                        }
                        break;
                    default:
                        content.Append(data[i]);
                        break;
                }
            }
            return (result, noWait, noClear);
        }
        
        public Task<Message> Receive(Message message) {
            throw new NotImplementedException();
        }

        private static async Task ShowWindow(SerializableValue time) {
            float showValue;
            try {
                showValue = FloatValue.TryParse(time);
            } catch {
                showValue = 0.0F;
            }
            await MessageService.ProcessAsync(new Message<float>(showValue) {Mask = MessageMask, Tag = ShowDialogueBoxMessageTag});
        }
        
        private static async Task HideWindow(SerializableValue time) {
            float hideValue;
            try {
                hideValue = FloatValue.TryParse(time);
            } catch {
                hideValue = 0.0F;
            }
            await MessageService.ProcessAsync(new Message<float>(hideValue) {Mask = MessageMask, Tag = HideDialogueBoxMessageTag});
        }

        [CanBeNull]
        private static CharacterValue CreateCharacter(ScriptRuntime context, SerializableValue value) {
            switch (value) {
                case CharacterValue characterValue:
                    return characterValue;
                case NullValue _:
                    return null;
                case IStringConverter stringCharacter:
                    var variable = context.ActiveScope?.FindVariableValue<CharacterValue>(stringCharacter.ConvertToString(), true, VariableSearchMode.All);
                    if (variable == null) {
                        throw new ArgumentException($"Unable to create dialogue: no variable with name {stringCharacter} can be found to use as character");
                    }
                    return variable;
                default:
                    throw new ArgumentException($"Unable to create dialogue: unsupported character type {value}");
            }
        }
        
        public override async Task<SerializableValue> Execute(PluginExecuteContext context) {
            var dialogue = new DialogueDescription();
            foreach (var (name, value) in context.Parameters) {
                if (!(name is IStringConverter stringConverter)) continue;
                var option = stringConverter.ConvertToString();
                switch (option) {
                    case "Show":
                        await ShowWindow(value);
                        break;
                    case "Hide":
                        await HideWindow(value);
                        break;
                    case "Character":
                        dialogue.Character = CreateCharacter(context.Runtime, value);
                        _cache.Character = value;
                        break;
                    case "Content":
                        if (value is IStringConverter stringContent) {
                            _cache.Content = stringContent;
                            (dialogue.Content, dialogue.NoWait, dialogue.NoClear) = ProcessDialogueContent(context.Runtime, stringContent.ConvertToString(context.Runtime.ActiveLanguage));
                        } else {
                            throw new ArgumentException($"Unable to create dialogue: unsupported content type {value}");
                        }
                        break;
                }
            }
            dialogue.Language = context.Runtime.ActiveLanguage;
            await MessageService.ProcessAsync(new Message<DialogueDescription>(dialogue){Mask = MessageMask, Tag = NewDialogueMessageTag});
            return new NullValue();
        }

        private static (string Name, string Value)? ExtractParameter(string source) {
            var match = CommandTester.Match(source);
            if (match.Groups.Count < 4) return null;
            return (match.Groups[1].Value, match.Groups[2].Value);
        }

        private class Cache {
            public SerializableValue Character;
            public IStringConverter Content;
        }

        private class StyleList {
            public LinkedList<(int Value, bool Relative)> Size { get; } = new LinkedList<(int Value, bool Relative)>();
            public LinkedList<string> Color { get; } = new LinkedList<string>();
            public bool Bold { get; set; }
            public bool Italic { get; set; }
            public bool Strikethrough { get; set; }
            public bool Underline { get; set; }

            public TextDialogueItem Combine(string text) {
                var result = new TextDialogueItem(text) {Bold = Bold, Italic = Italic, Strikethrough = Strikethrough, Underline = Underline};
                if (Color.Any()) {
                    result.Color = Color.Last();
                }
                if (Size.Any()) {
                    foreach (var (value, isRelative) in Size) {
                        if (isRelative) {
                            result.FontSize = result.RelativeSize.HasValue && result.RelativeSize.Value ? result.FontSize + value : value;
                        } else {
                            result.FontSize = value;
                        }
                        result.RelativeSize = isRelative;
                    }
                }
                return result;
            }
        }
    }
}