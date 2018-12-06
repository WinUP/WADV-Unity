using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Attributes;
using Core.VisualNovel.Script.Compiler.Tokens;
using Core.VisualNovel.Translation;
using UnityEngine;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// WADV VNS词法分析器
    /// </summary>
    [StaticTranslation(SyntaxIf, "如果")]
    [StaticTranslation(SyntaxElseIf, "或者")]
    [StaticTranslation(SyntaxElse, "否则")]
    [StaticTranslation(SyntaxLoop, "循环")]
    [StaticTranslation(SyntaxScenario, "场景")]
    [StaticTranslation(SyntaxReturn, "返回")]
    [StaticTranslation(SyntaxIf, "if", "en")]
    [StaticTranslation(SyntaxElseIf, "elseIf", "en")]
    [StaticTranslation(SyntaxElse, "else", "en")]
    [StaticTranslation(SyntaxLoop, "loop", "en")]
    [StaticTranslation(SyntaxScenario, "scene", "en")]
    [StaticTranslation(SyntaxReturn, "return", "en")]
    public class Lexer {
        /// <summary>
        /// 编译语言选项
        /// <para>该指令在所有语言中表示一致，且不能对特定语言添加字面表示</para>
        /// </summary>
        public const string SyntaxLanguage = "lang";
        /// <summary>
        /// 选择指令
        /// </summary>
        public const string SyntaxIf = "SYNTAX_IF";
        /// <summary>
        /// 分支指令
        /// </summary>
        public const string SyntaxElseIf = "SYNTAX_ELSEIF";
        /// <summary>
        /// 否则指令
        /// </summary>
        public const string SyntaxElse = "SYNTAX_ELSE";
        /// <summary>
        /// 循环指令
        /// </summary>
        public const string SyntaxLoop = "SYNTAX_LOOP";
        /// <summary>
        /// 场景指令
        /// </summary>
        public const string SyntaxScenario = "SYNTAX_SCENARIO";
        /// <summary>
        /// 返回指令
        /// </summary>
        public const string SyntaxReturn = "SYNTAX_RETURN";
        /// <summary>
        /// 所有操作符
        /// </summary>
        public static readonly string[] Separators = {"->", "+=", "-=", "*=", "/=", ">", "<", ">=", "<=", "[", "]", "!", "+", "-", "*", "/", "@", ";", "=", "==", "(", ")", " ", "\"", "\n"};
        /// <summary>
        /// 脚本内容
        /// </summary>
        private CodeFile File { get; }
        /// <summary>
        /// 脚本标识符（通常是路径）
        /// </summary>
        public string Identifier { get; }
        
        /// <summary>
        /// 声明一个新的VNS词法分析器
        /// </summary>
        /// <param name="content">脚本内容</param>
        /// <param name="identifier">脚本标识符</param>
        public Lexer(string content, string identifier) {
            Identifier = identifier;
            File = new CodeFile(content);
        }

        /// <summary>
        /// 从文件创建词法分析器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static Lexer FromFile(string path) {
            return new Lexer(Resources.Load<TextAsset>(path).text, path);
        }

        /// <summary>
        /// 运行词法解析器并创建语法解析器
        /// </summary>
        /// <returns></returns>
        public Parser CreateParser() {
            return new Parser(Lex(), Identifier);
        }

        /// <summary>
        /// 执行词法分析
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BasicToken> Lex() {
            var position = new CodePosition();
            var tokens = new List<BasicToken>();
            var indent = new CodeIndent();
            var language = "default";
            while (File.HasNext) {
                if (File.Current == '/' && File.Next == '/') { // 跳过所有注释
                    File.MoveToNextLineBreak();
                    continue;
                }
                
                if (File.Current == '\n') { // 缩进处理及跳过空行、空逻辑行
                    position = position.NextLine();
                    if (File.Next == '\n') { // 空行直接跳过
                        File.MoveToNext();
                        continue;
                    }
                    if (File.Next != ' ' && File.Next != '/' && File[2] != '/') { // 如果行不以空格开头，尝试取消所有缩进
                        if (File.Next == '/' && File[2] == '/') { // 如果行仅包含注释则不予处理
                            File.MoveToNextLineBreak();
                            continue;
                        }
                        tokens.Add(new BasicToken(TokenType.LineBreak, position));
                        // ReSharper disable once AccessToModifiedClosure
                        tokens.AddRange(indent.ShrinkTo(0).Select(i => new BasicToken(TokenType.LeaveScope, position)));
                    } else { // 否则计算该行缩进值
                        File.MoveToNext();
                        var offset = File.IndexOfUntilNot(' ');
                        if (File[offset] == '\n' || File[offset] == '/' && File[offset + 1] == '/') { // 空行和只有注释的行不予任何处理
                            File.MoveToNextLineBreak();
                            continue;
                        }
                        tokens.Add(new BasicToken(TokenType.LineBreak, position));
                        if (offset < indent.Length) { // 缩进小于当前缩进则取消部分缩进
                            var approachingIndent = indent.FindApproachingIndent(offset);
                            if (approachingIndent != offset) { // 当缩进比最大缩进小时，不接受未出现过的缩进值
                                throw new CompileException(Identifier, position, $"Unable to create indent: Indent length {offset} is not acceptable, try {approachingIndent}");
                            }
                            // ReSharper disable once AccessToModifiedClosure
                            tokens.AddRange(indent.ShrinkTo(approachingIndent).Select(i => new BasicToken(TokenType.LeaveScope, position)));
                        } else if (offset > indent.Length) { // 缩进大于当前缩进则增加缩进
                            indent.Push(offset - indent.Length);
                            tokens.Add(new BasicToken(TokenType.CreateScope, position));
                        }
                        File.Move(offset - 1);
                        position = position.MoveColumn(offset);
                    }
                    File.MoveToNext(); // 换行符处理结束后移动游标一位以对应CodePosition
                }

                if (File.Current == '\0') {
                    break;
                }
                
                if (File.Current == '"') { // 字符串常量
                    File.MoveToNext();
                    var stringEnd = File.IndexOfWithEscapeRecognize('"');
                    var result = File.CopyContent(stringEnd);
                    tokens.Add(new StringToken(TokenType.String, position, result.ExecuteEscapeCharacters(), true));
                    position = result.Contains('\n')
                        ? position.NextLine().MoveLine(result.Count(e => e == '\n') - 1).MoveColumn(result.Length - result.LastIndexOf('\n') - 1)
                        : position.MoveColumn(stringEnd + 2);
                    File.Move(stringEnd + 1);
                    continue;
                }
                
                if (File.Current >= '0' && File.Current <= '9') { // 数字常量
                    var numberEnd = File.IndexOfUntilNot('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', 'x', 'X', '.');
                    var number = File.CopyContent(numberEnd);
                    CreateNumberToken(tokens, position, number);
                    File.Move(numberEnd);
                    position = position.MoveColumn(numberEnd - File.Offset);
                    continue;
                }
                
                if (File.Current == ';' && File.Previous != '\\') { // 指令
                    File.MoveToNext();
                    position = position.NextColumn();
                    if (File.StartsWith(SyntaxLanguage)) {
                        tokens.Add(new BasicToken(TokenType.Language, position));
                        File.Move(5);
                        position = position.MoveColumn(5);
                        var index = File.IndexOf(Separators);
                        if (index <= 0) {
                            throw new CompileException(Identifier, position, "Need language name for language command");
                        }
                        var optionContent = File.CopyContent(index);
                        language = optionContent;
                        tokens.Add(new StringToken(TokenType.String, position, language, false));
                        File.Move(index);
                        position = position.MoveColumn(index);
                    } else {
                        var index = File.IndexOf(' ', '\n');
                        if (File.StartsWith(TranslationManager.GetStatic(SyntaxScenario, language) + ' ')) {
                            tokens.Add(new BasicToken(TokenType.Function, position));
                        } else if (File.StartsWith(TranslationManager.GetStatic(SyntaxIf, language) + ' ')) {
                            tokens.Add(new BasicToken(TokenType.If, position));
                        } else if (File.StartsWith(TranslationManager.GetStatic(SyntaxElse, language) + ' ', TranslationManager.GetStatic(SyntaxElse, language) + '\n')) {
                            tokens.Add(new BasicToken(TokenType.Else, position));
                        } else if (File.StartsWith(TranslationManager.GetStatic(SyntaxElseIf, language) + ' ')) {
                            tokens.Add(new BasicToken(TokenType.ElseIf, position));
                        } else if (File.StartsWith(TranslationManager.GetStatic(SyntaxLoop, language) + ' ')) {
                            tokens.Add(new BasicToken(TokenType.Loop, position));
                        } else if (File.StartsWith(TranslationManager.GetStatic(SyntaxReturn, language) + ' ', TranslationManager.GetStatic(SyntaxReturn, language) + '\n')) {
                            tokens.Add(new BasicToken(TokenType.Return, position));
                        } else {
                            throw new CompileException(Identifier, position, $"Unknown command {File.CopyContent(index)} in language {language}, may cause by command format error or typo mistake");
                        }
                        File.Move(index);
                        position = position.MoveColumn(index);
                    }
                    continue;
                }

                if (File.StartsWith(Separators)) { // 操作符和分隔符
                    AddSeparator(tokens, position);
                    if (File.Current == '-' && (File.Next == '>' || File.Next == '=') ||
                        File.Current == '+' && File.Next == '=' ||
                        File.Current == '*' && File.Next == '=' ||
                        File.Current == '>' && File.Next == '=' ||
                        File.Current == '<' && File.Next == '=' ||
                        File.Current == '=' && File.Next == '=' ||
                        File.Current == '/' && File.Next == '=') {
                        File.MoveToNext();
                        position = position.NextColumn();
                    }
                    File.MoveToNext();
                    position = position.NextColumn();
                    var nextSeparator = File.IndexOfWithEscapeRecognize(Separators); // 有可能是换行，如果有问题待到语法分析时再报错，这里因为没有足够的状态记录，无法检测是否合法
                    if (nextSeparator < 0) {
                        break;
                    }
                    var content = File.CopyContent(nextSeparator).Trim();
                    if (content.Length > 0) {
                        if (content[0] >= '0' && content[0] <= '9') {
                            CreateNumberToken(tokens, position, content);
                        } else {
                            tokens.Add(new StringToken(TokenType.String, position, content, false));
                        }
                    }
                    File.Move(nextSeparator);
                    position = position.MoveColumn(nextSeparator);
                    continue;
                }

                if (File.Current == '#' && File.Previous != '\\') { // 带角色描述的快速对话。快速对话中不能使用可编程语法，也不能中途换行，直接解析至行尾即可。
                    File.MoveToNext();
                    position = position.NextColumn();
                    var nextSpace = File.IndexOf(' ');
                    var lineBreak = File.IndexOf('\n');
                    if (nextSpace < 0 || nextSpace > lineBreak) { // 对话内容不能为空
                        throw new CompileException(Identifier, position, "Unable to create quick dialogue: Dialogue starts with character must has content");
                    }
                    if (File.Current == ' ') { // 角色描述不能为空
                        throw new CompileException(Identifier, position, "Unable to create quick dialogue: Dialogue starts with character must has character definition");
                    }
                    tokens.Add(new StringToken(TokenType.DialogueSpeaker, position, File.CopyContent(nextSpace).ExecuteEscapeCharacters(), false));
                    position = position.MoveColumn(nextSpace);
                    tokens.Add(new StringToken(TokenType.DialogueContent, position, File.CopyContent(nextSpace + 1, lineBreak).ExecuteEscapeCharacters(), false));
                    File.Move(lineBreak);
                    position = position.MoveColumn(lineBreak - nextSpace);
                    continue;
                }

                if (File.Current == ' ') { // 空格
                    File.MoveToNext();
                    position = position.NextColumn();
                    continue;
                }

                // 如果所有条件都不满足，那么一定是普通的无角色对话
                var nextLine = File.IndexOf('\n');
                tokens.Add(new StringToken(TokenType.DialogueContent, position, File.CopyContent(nextLine).ExecuteEscapeCharacters(), false));
                tokens.Add(new BasicToken(TokenType.LineBreak, position));
                File.Move(nextLine);
            }
            File.Reset(); // 重置游标至-1
            if (tokens.Count == 0 || tokens.Last().Type != TokenType.LineBreak) {
                tokens.Add(new BasicToken(TokenType.LineBreak, position));
            }
            if (indent.Length <= 0) return tokens;
            tokens.AddRange(indent.ShrinkTo(0).Select(i => new BasicToken(TokenType.LeaveScope, position)));
            tokens.Add(new BasicToken(TokenType.LineBreak, position));
            return tokens;
        }

        private void CreateNumberToken(ICollection<BasicToken> tokens, CodePosition position, string number) {
            number = number.ExecuteEscapeCharacters().ToUpper();
            if (number.StartsWith("0X")) { // 二进制整数转十进制
                number = number.Substring(2);
                if (number.Any(e => e != '0' && e != '1')) {
                    throw new CompileException(Identifier, position, "Unable to create number: Binary number must be integer and only allows 0/1");
                }
                tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number, 2)));
            } else if (number.StartsWith("0B")) { // 十六进制整数转十进制
                number = number.Substring(2);
                if (number.Any(e => e == '.')) {
                    throw new CompileException(Identifier, position, "Unable to create number: Hex number must be integer");
                }
                tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number, 16)));
            } else if (number.Any(e => e != '.' && e != '-' && e != 'E' && (e < '0' || e > '9'))) { // 错误格式
                throw new CompileException(Identifier, position, $"Unable to create number: Unknown format {number}");
            } else { // 十进制整数和浮点数转换
                var dotCount = number.Count(e => e == '.');
                if (dotCount > 1) { // 浮点数格式错误
                    throw new CompileException(Identifier, position, "Unable to create number: Float number format error");
                } else if (dotCount == 1 || number.Contains('E')) { // 浮点数
                    tokens.Add(new FloatToken(TokenType.Number, position, float.Parse(number)));
                } else { // 整数
                    tokens.Add(new IntegerToken(TokenType.Number, position, Convert.ToInt32(number)));
                }
            }
        }

        private void AddSeparator(ICollection<BasicToken> tokens, CodePosition position) {
            switch (File.Current) {
                case '-':
                    switch (File.Next) {
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
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.AddEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Add, position));
                            break;
                    }
                    break;
                case '*':
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.MultiplyEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Multiply, position));
                            break;
                    }
                    break;
                case '/':
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.DivideEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Divide, position));
                            break;
                    }
                    break;
                case '>':
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.GreaterEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Greater, position));
                            break;
                    }
                    break;
                case '<':
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.LesserEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Lesser, position));
                            break;
                    }
                    break;
                case '[':
                    tokens.Add(new BasicToken(TokenType.CallStart, position));
                    break;
                case ']':
                    tokens.Add(new BasicToken(TokenType.CallEnd, position));
                    break;
                case '!':
                    tokens.Add(new BasicToken(TokenType.LogicNot, position));
                    break;
                case '@':
                    tokens.Add(new BasicToken(TokenType.Variable, position));
                    break;
                case '=':
                    switch (File.Next) {
                        case '=':
                            tokens.Add(new BasicToken(TokenType.LogicEqual, position));
                            break;
                        default:
                            tokens.Add(new BasicToken(TokenType.Equal, position));
                            break;
                    }
                    break;
                case '(':
                    tokens.Add(new BasicToken(TokenType.LeftBracket, position));
                    break;
                case ')':
                    tokens.Add(new BasicToken(TokenType.RightBracket, position));
                    break;
            }
        }
    }
}