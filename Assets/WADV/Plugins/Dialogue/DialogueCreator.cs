using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Plugins.Dialogue.Items;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Dialogue {
    public partial class DialoguePlugin {
        /// <summary>
        /// 根据可序列化值获取角色定义
        /// </summary>
        /// <param name="context">执行环境</param>
        /// <param name="value">输入值</param>
        /// <returns></returns>
        [CanBeNull]
        public static CharacterValue CreateCharacter(ScriptRuntime context, SerializableValue value) {
            switch (value) {
                case CharacterValue characterValue:
                    return characterValue;
                case NullValue _:
                    return null;
                case IStringConverter stringCharacter:
                    var variable = context.ActiveScope?.FindVariableValue<CharacterValue>(stringCharacter.ConvertToString(context.ActiveLanguage), true, VariableSearchMode.All);
                    if (variable == null) {
                        throw new ArgumentException($"Unable to create dialogue: no variable with name {stringCharacter} can be found to use as character");
                    }
                    return variable;
                default:
                    throw new ArgumentException($"Unable to create dialogue: unsupported character type {value}");
            }
        }
        
        /// <summary>
        /// 分析对话内容
        /// </summary>
        /// <param name="runtime">脚本运行环境</param>
        /// <param name="raw">原始对话内容</param>
        /// <returns></returns>
        public static (List<IDialogueItem> Content, bool NoWait, bool NoClear) CreateDialogueContent(ScriptRuntime runtime, IStringConverter raw) {
            var result = new List<IDialogueItem>();
            var content = new StringBuilder();
            var style = new StyleList();
            var noWait = false;
            var noClear = false;
            var i = -1;
            var data = raw.ConvertToString(runtime.ActiveLanguage);
            if (data.StartsWith("[noclear]", StringComparison.OrdinalIgnoreCase)) {
                noClear = true;
                i = 8;
            }
            
            void FlushCache() {
                result.Add(style.Combine(content.ToString()));
                content.Clear();
            }

            void ApplyDefault(string command) {
                content.Append($"[{command}]");
            }
            
            for (; ++i < data.Length;) {
                switch (data[i]) {
                    // 转义字符
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
                    // 最后一个字符是[时的容错处理
                    case '[' when i == data.Length - 1:
                        content.Append('[');
                        break;
                    // 指令
                    case '[':
                        var endIndex = data.IndexOf(']', i + 1);
                        if (endIndex <= i) { // 容错处理
                            content.Append('[');
                            break;
                        }
                        var command = data.Substring(i + 1, endIndex - i - 1);
                        i = endIndex;
                        if (string.IsNullOrEmpty(command)) { // 空指令不处理
                            content.Append("[]");
                        } else if (command.StartsWith("@#")) { // 常量
                            var constantName = command.Substring(2);
                            if (string.IsNullOrEmpty(constantName)) throw new ArgumentException($"Unable to create dialog command: missing constant name at {i}");
                            var constant = runtime.ActiveScope?.FindVariable(constantName, true, VariableSearchMode.OnlyConstant);
                            if (constant == null) {
                                ApplyDefault(command);
                            } else {
                                content.Append(constant.ReferenceTarget is IStringConverter stringConverter ? stringConverter.ConvertToString(runtime.ActiveLanguage) : constant.ReferenceTarget.ToString());
                            }
                        } else if (command.StartsWith("@")) { // 变量
                            var variableName = command.Substring(1);
                            if (string.IsNullOrEmpty(variableName)) throw new ArgumentException($"Unable to create dialog command: missing variable name at {i}");
                            var variable = runtime.ActiveScope?.FindVariable(variableName, true, VariableSearchMode.All);
                            if (variable == null) {
                                ApplyDefault(command);
                            } else {
                                content.Append(variable.ReferenceTarget is IStringConverter stringConverter ? stringConverter.ConvertToString(runtime.ActiveLanguage) : variable.ReferenceTarget.ToString());
                            }
                        } else {
                            var commandContent = command.ToLower().Trim();
                            if (commandContent.StartsWith("size")) { // 字号
                                var parameter = ExtractParameter(commandContent);
                                var relative = parameter.Value.Value.StartsWith("+") || parameter.Value.Value.StartsWith("-");
                                if (parameter.HasValue && float.TryParse(parameter.Value.Value, out var number)) {
                                    FlushCache();
                                    style.Size.AddLast((Mathf.RoundToInt(number), relative));
                                } else {
                                    ApplyDefault(command);
                                }
                            } else if (commandContent == "/size") { // 取消字号
                                if (style.Size.Any()) {
                                    FlushCache();
                                    style.Size.RemoveLast();
                                } else {
                                    ApplyDefault(command);
                                }
                            } else if (commandContent.StartsWith("color")) { // 颜色
                                var parameter = ExtractParameter(commandContent);
                                if (parameter.HasValue) {
                                    FlushCache();
                                    style.Color.AddLast(parameter.Value.Value);
                                } else {
                                    ApplyDefault(command);
                                }
                            } else if (commandContent == "/color") { // 取消颜色
                                if (style.Color.Any()) {
                                    FlushCache();
                                    style.Color.RemoveLast();
                                } else {
                                    ApplyDefault(command);
                                }
                            } else if (commandContent.StartsWith("pause")) { // 暂停
                                if (commandContent == "pause") {
                                    FlushCache();
                                    result.Add(new PauseDialogueItem {Time = null});
                                } else {
                                    var parameter = ExtractParameter(commandContent);
                                    if (parameter.HasValue && float.TryParse(parameter.Value.Value, out var number)) {
                                        FlushCache();
                                        result.Add(new PauseDialogueItem {Time = number});
                                        break;
                                    }
                                    ApplyDefault(command);
                                }
                            } else switch (commandContent) {
                                case "clear": // 清空
                                    FlushCache();
                                    result.Add(new ClearDialogueItem());
                                    break;
                                case "nowait":
                                    if (i == data.Length - 1) {
                                        noWait = true;
                                    }
                                    break;
                                default:
                                    switch (command) {
                                        case "b": // 粗体
                                            FlushCache();
                                            style.Bold = true;
                                            break;
                                        case "/b": // 取消粗体
                                            FlushCache();
                                            style.Bold = false;
                                            break;
                                        case "i": // 斜体
                                            FlushCache();
                                            style.Italic = true;
                                            break;
                                        case "/i": // 取消斜体
                                            FlushCache();
                                            style.Italic = false;
                                            break;
                                        case "s": // 删除线
                                            FlushCache();
                                            style.Strikethrough = true;
                                            break;
                                        case "/s": // 取消删除线
                                            FlushCache();
                                            style.Strikethrough = false;
                                            break;
                                        case "u": // 下划线
                                            FlushCache();
                                            style.Underline = true;
                                            break;
                                        case "/u": // 取消下划线
                                            FlushCache();
                                            style.Underline = false;
                                            break;
                                        default:
                                            ApplyDefault(command);
                                            break;
                                    }
                                    break;
                            }
                        }
                        break;
                    default:
                        content.Append(data[i]);
                        break;
                }
            }
            if (content.Length > 0) {
                result.Add(style.Combine(content.ToString()));
            }
            return (result, noWait, noClear);
        }
        
        private static (string Name, string Value)? ExtractParameter(string source) {
            var match = CommandTester.Match(source);
            if (match.Groups.Count < 3) return null;
            return (match.Groups[1].Value, match.Groups[2].Value);
        }
        
        private class StyleList {
            public LinkedList<(int Value, bool Relative)> Size { get; } = new LinkedList<(int Value, bool Relative)>();
            public LinkedList<string> Color { get; } = new LinkedList<string>();
            public bool Bold { private get; set; }
            public bool Italic { private get; set; }
            public bool Strikethrough { private get; set; }
            public bool Underline { private get; set; }

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