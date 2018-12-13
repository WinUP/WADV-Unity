using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler.Tokens;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// WADV VNS 词法分析器
    /// </summary>
    public static class Lexer {
        /// <summary>
        /// 执行词法分析
        /// </summary>
        /// <param name="source">源文件内容</param>
        /// <param name="identifier">源文件ID</param>
        /// <returns></returns>
        /// <exception cref="CompileException"></exception>
        public static IEnumerable<BasicToken> Lex(string source, CodeIdentifier identifier) {
            var file = new SourceCode(source);
            var position = new SourcePosition();
            var tokens = new List<BasicToken>();
            var indent = new SourceIndent();
            var language = "default";
            while (file.HasNext) {
                if (file.Current == '/' && file.Next == '/') { // 跳过所有注释
                    file.MoveToNextLineBreak();
                    continue;
                }
                if (file.Current == '\n') { // 缩进处理及跳过空行、空逻辑行
                    position = position.NextLine();
                    if (file.Next == '\n') { // 空行直接跳过
                        file.MoveToNext();
                        continue;
                    }
                    if (file.Next != ' ' && file.Next != '/' && file[2] != '/') { // 如果行不以空格开头，尝试取消所有缩进
                        if (file.Next == '/' && file[2] == '/') { // 如果行仅包含注释则不予处理
                            file.MoveToNextLineBreak();
                            continue;
                        }
                        tokens.Add(new BasicToken(TokenType.LineBreak, position));
                        // ReSharper disable once AccessToModifiedClosure
                        tokens.AddRange(indent.ShrinkTo(0).Select(i => new BasicToken(TokenType.LeaveScope, position)));
                    } else { // 否则计算该行缩进值
                        file.MoveToNext();
                        var offset = file.IndexOfUntilNot(' ');
                        if (file[offset] == '\n' || file[offset] == '/' && file[offset + 1] == '/') { // 空行和只有注释的行不予任何处理
                            file.MoveToNextLineBreak();
                            continue;
                        }
                        tokens.Add(new BasicToken(TokenType.LineBreak, position));
                        if (offset < indent.Length) { // 缩进小于当前缩进则取消部分缩进
                            var approachingIndent = indent.FindApproachingIndent(offset);
                            if (approachingIndent != offset) { // 当缩进比最大缩进小时，不接受未出现过的缩进值
                                throw new CompileException(identifier, position, $"Unable to create indent: Indent length {offset} is not acceptable, try {approachingIndent}");
                            }
                            // ReSharper disable once AccessToModifiedClosure
                            tokens.AddRange(indent.ShrinkTo(approachingIndent).Select(i => new BasicToken(TokenType.LeaveScope, position)));
                        } else if (offset > indent.Length) { // 缩进大于当前缩进则增加缩进
                            indent.Push(offset - indent.Length);
                            tokens.Add(new BasicToken(TokenType.CreateScope, position));
                        }
                        file.Move(offset - 1);
                        position = position.MoveColumn(offset);
                    }
                    file.MoveToNext(); // 换行符处理结束后移动游标一位以对应CodePosition
                }

                if (file.Current == '\0') {
                    break;
                }
                
                if (file.Current == '\'') { // 不可翻译字符串常量
                    file.MoveToNext();
                    var stringEnd = file.IndexOfWithEscapeRecognize('\'');
                    if (stringEnd < 0) {
                        throw new CompileException(identifier, position, "String constant has no end mark");
                    }
                    var result = file.CopyContent(stringEnd);
                    tokens.Add(new StringToken(TokenType.String, position, result.ExecuteEscapeCharacters(), false));
                    position = result.Contains('\n')
                        ? position.NextLine().MoveLine(result.Count(e => e == '\n') - 1).MoveColumn(result.Length - result.LastIndexOf('\n') - 1)
                        : position.MoveColumn(stringEnd + 2);
                    file.Move(stringEnd + 1);
                    continue;
                }
                
                if (file.Current == '"') { // 可翻译字符串常量
                    file.MoveToNext();
                    var stringEnd = file.IndexOfWithEscapeRecognize('"');
                    if (stringEnd < 0) {
                        throw new CompileException(identifier, position, "Translatable string constant has no end mark");
                    }
                    var result = file.CopyContent(stringEnd);
                    tokens.Add(new StringToken(TokenType.String, position, result.ExecuteEscapeCharacters(), true));
                    position = result.Contains('\n')
                        ? position.NextLine().MoveLine(result.Count(e => e == '\n') - 1).MoveColumn(result.Length - result.LastIndexOf('\n') - 1)
                        : position.MoveColumn(stringEnd + 2);
                    file.Move(stringEnd + 1);
                    continue;
                }
                
                if (file.Current >= '0' && file.Current <= '9') { // 数字常量
                    var numberEnd = file.IndexOfUntilNot('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', 'x', 'X', '.');
                    var number = file.CopyContent(numberEnd);
                    CreateNumberToken(identifier, tokens, position, number);
                    file.Move(numberEnd);
                    position = position.MoveColumn(numberEnd - file.Offset);
                    continue;
                }
                
                if (file.Current == ';' && file.Previous != '\\') { // 指令
                    file.MoveToNext();
                    position = position.NextColumn();
                    if (file.StartsWith(Keywords.SyntaxLanguage)) {
                        tokens.Add(new BasicToken(TokenType.Language, position));
                        file.Move(5);
                        position = position.MoveColumn(5);
                        var index = file.IndexOf(Keywords.Separators);
                        if (index <= 0) {
                            throw new CompileException(identifier, position, "Need language name for language command");
                        }
                        var optionContent = file.CopyContent(index);
                        language = optionContent;
                        tokens.Add(new StringToken(TokenType.String, position, language, false));
                        file.Move(index);
                        position = position.MoveColumn(index);
                    } else {
                        var index = file.IndexOf(' ', '\n');
                        if (file.StartsWith(Keywords.SyntaxFunction)) {
                            tokens.Add(new BasicToken(TokenType.Function, position));
                        } else if (file.StartsWith(Keywords.SyntaxIf)) {
                            tokens.Add(new BasicToken(TokenType.If, position));
                        } else if (file.StartsWith(Keywords.SyntaxElseIf)) {
                            tokens.Add(new BasicToken(TokenType.ElseIf, position));
                        } else if (file.StartsWith(Keywords.SyntaxElse)) {
                            tokens.Add(new BasicToken(TokenType.Else, position));
                        } else if (file.StartsWith(Keywords.SyntaxWhileLoop + ' ')) {
                            tokens.Add(new BasicToken(TokenType.Loop, position));
                        } else if (file.StartsWith(Keywords.SyntaxReturn)) {
                            tokens.Add(new BasicToken(TokenType.Return, position));
                        } else if (file.StartsWith(Keywords.SyntaxCall)) {
                            tokens.Add(new BasicToken(TokenType.FunctionCall, position));
                        } else if (file.StartsWith(Keywords.SyntaxImport)) {
                            tokens.Add(new BasicToken(TokenType.Import, position));
                        } else if (file.StartsWith(Keywords.SyntaxExport)) {
                            tokens.Add(new BasicToken(TokenType.Export, position));
                        } else {
                            throw new CompileException(identifier, position, $"Unknown command {file.CopyContent(index)} in language {language}, may cause by command format error or typo mistake");
                        }
                        file.Move(index);
                        position = position.MoveColumn(index);
                    }
                    continue;
                }

                if (file.StartsWith(Keywords.Separators)) { // 操作符和分隔符
                    AddSeparator(file, tokens, position);
                    if (file.Current == '-' && (file.Next == '>' || file.Next == '=') ||
                        file.Current == '@' && file.Next == '#' ||
                        file.Current == '+' && file.Next == '=' ||
                        file.Current == '*' && file.Next == '=' ||
                        file.Current == '>' && file.Next == '=' ||
                        file.Current == '<' && file.Next == '=' ||
                        file.Current == '=' && file.Next == '=' ||
                        file.Current == '/' && file.Next == '=') {
                        file.MoveToNext();
                        position = position.NextColumn();
                    }
                    file.MoveToNext();
                    position = position.NextColumn();
                    var nextSeparator = file.IndexOfWithEscapeRecognize(Keywords.Separators); // 有可能是换行，如果有问题待到语法分析时再报错，这里因为没有足够的状态记录，无法检测是否合法
                    if (nextSeparator < 0) {
                        break;
                    }
                    var content = file.CopyContent(nextSeparator).Trim();
                    if (content.Length > 0) {
                        if (content[0] >= '0' && content[0] <= '9') {
                            CreateNumberToken(identifier, tokens, position, content);
                        } else {
                            tokens.Add(new StringToken(TokenType.String, position, content, false));
                        }
                    }
                    file.Move(nextSeparator);
                    position = position.MoveColumn(nextSeparator);
                    continue;
                }

                if (file.Current == '#' && file.Previous != '\\') { // 带角色描述的快速对话。快速对话中不能使用可编程语法，也不能中途换行，直接解析至行尾即可。
                    file.MoveToNext();
                    position = position.NextColumn();
                    var nextSpace = file.IndexOf(' ');
                    var lineBreak = file.IndexOf('\n');
                    if (nextSpace < 0 || nextSpace > lineBreak) { // 对话内容不能为空
                        throw new CompileException(identifier, position, "Unable to create quick dialogue: Dialogue starts with character must has content");
                    }
                    if (file.Current == ' ') { // 角色描述不能为空
                        throw new CompileException(identifier, position, "Unable to create quick dialogue: Dialogue starts with character must has character definition");
                    }
                    tokens.Add(new StringToken(TokenType.DialogueSpeaker, position, file.CopyContent(nextSpace).ExecuteEscapeCharacters(), false));
                    position = position.MoveColumn(nextSpace);
                    tokens.Add(new StringToken(TokenType.DialogueContent, position, file.CopyContent(nextSpace + 1, lineBreak).ExecuteEscapeCharacters(), false));
                    file.Move(lineBreak);
                    position = position.MoveColumn(lineBreak - nextSpace);
                    continue;
                }

                if (file.Current == ' ') { // 空格
                    file.MoveToNext();
                    position = position.NextColumn();
                    continue;
                }

                // 如果所有条件都不满足，那么一定是普通的无角色对话
                var nextLine = file.IndexOf('\n');
                tokens.Add(new StringToken(TokenType.DialogueContent, position, file.CopyContent(nextLine).ExecuteEscapeCharacters(), false));
                tokens.Add(new BasicToken(TokenType.LineBreak, position));
                file.Move(nextLine);
            }
            file.Reset(); // 重置游标至-1
            if (tokens.Count == 0 || tokens.Last().Type != TokenType.LineBreak) {
                tokens.Add(new BasicToken(TokenType.LineBreak, position));
            }
            if (indent.Length <= 0) return tokens;
            tokens.AddRange(indent.ShrinkTo(0).Select(i => new BasicToken(TokenType.LeaveScope, position)));
            tokens.Add(new BasicToken(TokenType.LineBreak, position));
            return tokens;
        }

        private static void CreateNumberToken(CodeIdentifier identifier, ICollection<BasicToken> tokens, SourcePosition position, string number) {
            number = number.ExecuteEscapeCharacters().ToUpper();
            if (number.StartsWith("0X")) { // 二进制整数转十进制
                number = number.Substring(2);
                if (number.Any(e => e != '0' && e != '1')) {
                    throw new CompileException(identifier, position, "Unable to create number: Binary number must be integer and only allows 0/1");
                }
                tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number, 2)));
            } else if (number.StartsWith("0B")) { // 十六进制整数转十进制
                number = number.Substring(2);
                if (number.Any(e => e == '.')) {
                    throw new CompileException(identifier, position, "Unable to create number: Hex number must be integer");
                }
                tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number, 16)));
            } else if (number.Any(e => e != '.' && e != '-' && e != 'E' && (e < '0' || e > '9'))) { // 错误格式
                throw new CompileException(identifier, position, $"Unable to create number: Unknown format {number}");
            } else { // 十进制整数和浮点数转换
                var dotCount = number.Count(e => e == '.');
                if (dotCount > 1) { // 浮点数格式错误
                    throw new CompileException(identifier, position, "Unable to create number: Float number format error");
                } else if (dotCount == 1 || number.Contains('E')) { // 浮点数
                    tokens.Add(new FloatToken(TokenType.Number, position, float.Parse(number)));
                } else { // 整数
                    tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number)));
                }
            }
        }

        private static void AddSeparator(SourceCode file, ICollection<BasicToken> tokens, SourcePosition position) {
            switch (file.Current) {
                case '-':
                    switch (file.Next) {
                        case '>':
                            tokens.Add(new BasicToken(TokenType.PickChild, position));
                            break;
                        case '=':
                            tokens.Add(new BasicToken(TokenType.MinusEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Minus, position));
                            break;
                    }
                    break;
                case '+':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.AddEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Add, position));
                            break;
                    }
                    break;
                case '*':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.MultiplyEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Multiply, position));
                            break;
                    }
                    break;
                case '/':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.DivideEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Divide, position));
                            break;
                    }
                    break;
                case '>':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.GreaterEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Greater, position));
                            break;
                    }
                    break;
                case '<':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.LesserEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Lesser, position));
                            break;
                    }
                    break;
                case '[':
                    tokens.Add(new BasicToken(TokenType.PluginCallStart, position));
                    break;
                case ']':
                    tokens.Add(new BasicToken(TokenType.PluginCallEnd, position));
                    break;
                case '!':
                    tokens.Add(new BasicToken(TokenType.LogicNot, position));
                    break;
                case '@':
                    switch (file.Next) {
                        case '#':
                            tokens.Add(new BasicToken(TokenType.Constant, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Variable, position));
                            break;
                    }
                    break;
                case '=':
                    switch (file.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.LogicEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Equal, position));
                            break;
                    }
                    break;
                case '(':
                    tokens.Add(new BasicToken(TokenType.LeftParenthesis, position));
                    break;
                case ')':
                    tokens.Add(new BasicToken(TokenType.RightParenthesis, position));
                    break;
            }
        }
    }
}