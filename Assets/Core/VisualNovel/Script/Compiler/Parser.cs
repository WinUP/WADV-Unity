using System.Collections.Generic;
using System.Linq;
using Core.VisualNovel.Script.Compiler.Expressions;
using Core.VisualNovel.Script.Compiler.Tokens;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// WADV VNS 语法分析器
    /// </summary>
    public class Parser {
        /// <summary>
        /// Token序列内容
        /// </summary>
        private SourceTokens Tokens { get; }
        /// <summary>
        /// Token序列标识符
        /// </summary>
        public CodeIdentifier Identifier { get; }

        public Parser(IEnumerable<BasicToken> tokens, CodeIdentifier identifier) {
            Tokens = new SourceTokens(tokens);
            Identifier = identifier;
        }

        /// <summary>
        /// 构建抽象语法树
        /// </summary>
        public ScopeExpression Parse() {
            var result = new ScopeExpression(new SourcePosition());
            while (Tokens.HasNext) {
                switch (Tokens.Current.Type) {
                    case TokenType.DialogueContent:
                    case TokenType.DialogueSpeaker:
                        result.Content.Add(ParseBinaryOperator(ParseDialogue()));
                        break;
                    case TokenType.Number:
                        result.Content.Add(ParseBinaryOperator(ParseNumber()));
                        break;
                    case TokenType.String:
                        result.Content.Add(ParseBinaryOperator(ParseString()));
                        break;
                    case TokenType.LineBreak:
                        Tokens.MoveToNext();
                        break;
                    case TokenType.CreateScope:
                        result.Content.Add(ParseScope());
                        break;
                    case TokenType.PluginCallStart:
                        result.Content.Add(ParseBinaryOperator(ParsePluginCall()));
                        break;
                    case TokenType.Variable:
                        result.Content.Add(ParseBinaryOperator(ParseVariable()));
                        break;
                    case TokenType.Constant:
                        result.Content.Add(ParseBinaryOperator(ParseConstant()));
                        break;
                    case TokenType.LeftBracket:
                        result.Content.Add(ParseBinaryOperator(ParseBracket()));
                        break;
                    case TokenType.LogicNot:
                        result.Content.Add(ParseBinaryOperator(ParseLogicNot()));
                        break;
                    case TokenType.Function:
                        result.Content.Add(ParseBinaryOperator(ParseFunctionDefinition()));
                        break;
                    case TokenType.If:
                        result.Content.Add(ParseBinaryOperator(ParseCondition()));
                        break;
                    case TokenType.Loop:
                        result.Content.Add(ParseBinaryOperator(ParseLoop()));
                        break;
                    case TokenType.Return:
                        result.Content.Add(ParseReturn());
                        break;
                    case TokenType.FunctionCall:
                        result.Content.Add(ParseBinaryOperator(ParseFunctionCall()));
                        break;
                    case TokenType.Language:
                        result.Content.Add(ParseBinaryOperator(ParseLanguage()));
                        break;
                    case TokenType.LeaveScope:
                        Tokens.MoveToNext();
                        // 解析文件域时LeaveScope一定不会出现，如果出现则证明这是脚本中一个子域，那么这一定是在ParseScope的递归中，可以直接返回
                        return result;
                    case TokenType.PluginCallEnd:
                        throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected CallEnd");
                    case TokenType.RightBracket:
                        throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected RightBracket");
                    case TokenType.ElseIf:
                        throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected ElseIf");
                    case TokenType.Else:
                        throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected Else");
                    case TokenType.Add:
                    case TokenType.Divide:
                    case TokenType.Equal:
                    case TokenType.Greater:
                    case TokenType.Lesser:
                    case TokenType.Minus:
                    case TokenType.Multiply:
                    case TokenType.AddEqual:
                    case TokenType.DivideEqual:
                    case TokenType.GreaterEqual:
                    case TokenType.LesserEqual:
                    case TokenType.MultiplyEqual:
                    case TokenType.PickChild:
                    case TokenType.MinusEqual:
                    case TokenType.LogicEqual:
                        // 二元运算符是由其他部分酌情自动调用的，出现在主switch里说明非法使用
                        throw new CompileException(Identifier, Tokens.Current.Position, $"Unexpected binary operator {Tokens.Current.Type.ToString()}");
                    default:
                        throw new CompileException(Identifier, Tokens.Current.Position, $"Unknown token type {Tokens.Current.Type}");
                }
            }
            Tokens.Reset();
            return result;
        }

        /// <summary>
        /// 处理函数调用
        /// </summary>
        /// <returns></returns>
        private Expression ParseFunctionCall() {
            Tokens.MoveToNext();
            var function = new FunctionCallExpression(Tokens.Current.Position) {
                Target = ParseBinaryOperator(GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket))
            };
            while (Tokens.Current.Type != TokenType.LineBreak && Tokens.Current.Type != TokenType.RightBracket) {
                var parameter = new ParameterExpression(Tokens.Current.Position) {
                    Name = ParseBinaryOperator(
                        GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket),
                        0,
                        TokenType.Equal)
                };
                if (Tokens.Current.Type != TokenType.Equal) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Function call parameters must have value");
                }
                Tokens.MoveToNext();
                parameter.Value = ParseBinaryOperator(GeneralParser(
                    TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot));
                function.Parameters.Add(parameter);
            }
            return function;
        }

        /// <summary>
        /// 处理对话
        /// </summary>
        /// <returns></returns>
        private Expression ParseDialogue() {
            Expression dialogue;
            if (!(Tokens.Current is StringToken speakerToken)) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected token type when parsing dialogue (should be StringToken)");
            }
            if (speakerToken.Type == TokenType.DialogueSpeaker) {
                Tokens.MoveToNext();
                if (!(Tokens.Current is StringToken contentToken)) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected token type when parsing dialogue content (should be StringToken)");
                }
                dialogue = new DialogueExpression(speakerToken.Position) {Character = speakerToken.Content, Content = contentToken.Content};
            } else {
                dialogue = new DialogueExpression(Tokens.Current.Position) {Character = null, Content = speakerToken.Content};
            }
            Tokens.MoveToNext();
            return dialogue;
        }

        /// <summary>
        /// 处理数字
        /// </summary>
        /// <returns></returns>
        private Expression ParseNumber() {
            Expression expression;
            switch (Tokens.Current) {
                case IntegerToken integerToken:
                    expression = new IntegerExpression(integerToken.Position) {Value = integerToken.Content};
                    break;
                case FloatToken floatToken:
                    expression = new FloatExpression(floatToken.Position) {Value = floatToken.Content};
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected Number (like 123, 1.23, 0x7B or 0b1111011)");
            }
            Tokens.MoveToNext();
            return expression;
        }

        /// <summary>
        /// 处理字符串常量
        /// </summary>
        /// <returns></returns>
        private Expression ParseString() {
            Expression expression;
            switch (Tokens.Current) {
                case StringToken stringToken:
                    expression = new StringExpression(stringToken.Position) {Value = stringToken.Content, Translatable = stringToken.Translatable};
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected String");
            }
            Tokens.MoveToNext();
            return expression;
        }

        /// <summary>
        /// 处理语言变更
        /// </summary>
        /// <returns></returns>
        private Expression ParseLanguage() {
            Tokens.MoveToNext();
            if (!(Tokens.Current is StringToken stringToken)) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected token type when parsing language info (should be StringToken)");
            }
            var expression = new LanguageExpression(Tokens.Current.Position){Language = stringToken.Content};
            Tokens.MoveToNext();
            return expression;
        }

        /// <summary>
        /// 处理变量
        /// </summary>
        /// <returns></returns>
        private Expression ParseVariable() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            var content = GeneralParser(TokenType.String, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.Number, TokenType.LeftBracket);
            return new VariableExpression(position) {Name = content};
        }
        
        /// <summary>
        /// 处理变量
        /// </summary>
        /// <returns></returns>
        private Expression ParseConstant() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            var content = GeneralParser(TokenType.String, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.Number, TokenType.LeftBracket);
            return new ConstantExpression(position) {Name = content};
        }

        /// <summary>
        /// 处理插件调用
        /// </summary>
        /// <returns></returns>
        private Expression ParsePluginCall() {
            Tokens.MoveToNext();
            var command = new CallExpression(Tokens.Current.Position) {
                Target = ParseBinaryOperator(GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket))
            };
            while (Tokens.Current.Type != TokenType.PluginCallEnd) {
                if (Tokens.Current.Type == TokenType.LineBreak) {
                    Tokens.MoveToNext();
                    continue;
                }
                var parameter = new ParameterExpression(Tokens.Current.Position) {
                    Name = ParseBinaryOperator(
                        GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket),
                        0,
                        TokenType.Equal)
                };
                if (Tokens.Current.Type != TokenType.Equal) {
                    parameter.Value = new EmptyExpression(parameter.Name.Position);
                    command.Parameters.Add(parameter);
                    continue;
                }
                Tokens.MoveToNext();
                parameter.Value = ParseBinaryOperator(GeneralParser(
                    TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot));
                command.Parameters.Add(parameter);
            }
            Tokens.MoveToNext();
            return command;
        }

        /// <summary>
        /// 处理逻辑否
        /// </summary>
        /// <returns></returns>
        private Expression ParseLogicNot() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            var content = GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot);
            // 双重取否改布尔转换
            Expression result = new LogicNotExpression(position) {Content = content};
            if (!(content is LogicNotExpression contentLevel1)) return result;
            result = new ToBooleanExpression(position) {Value = contentLevel1.Content};
            content = contentLevel1.Content;
            // 布尔转换时将后续连续取否收缩至不超过一个
            while (content is LogicNotExpression contentLevel2 && contentLevel2.Content is LogicNotExpression contentLevel3) {
                ((ToBooleanExpression) result).Value = contentLevel3.Content;
                content = contentLevel3.Content;
            }
            return result;
        }

        /// <summary>
        /// 处理二元运算符
        /// </summary>
        /// <param name="left">当前缓存的运算符左端表达式</param>
        /// <param name="minimumOperatorPrecedence">可处理运算符的最低优先级</param>
        /// <param name="extraSeparator">本次处理中额外被当做非二元运算符的Token类型（不会递归传导）</param>
        /// <returns></returns>
        private Expression ParseBinaryOperator(Expression left, int minimumOperatorPrecedence = 0, TokenType? extraSeparator = null) {
            if (extraSeparator.HasValue && extraSeparator == Tokens.Current.Type) {
                return left;
            }
            var currentPrecedence = GetOperatorPrecedence();
            if (currentPrecedence < 0) {
                return left;
            }
            var result = new BinaryExpression(Tokens.Current.Position) {Left = left};
            while (true) {
                switch (Tokens.Current.Type) {
                    case TokenType.PickChild:
                        result.Operator = OperatorType.PickChild;
                        break;
                    case TokenType.MinusEqual:
                        result.Operator = OperatorType.MinusBy;
                        break;
                    case TokenType.Minus:
                        result.Operator = OperatorType.Minus;
                        break;
                    case TokenType.AddEqual:
                        result.Operator = OperatorType.AddBy;
                        break;
                    case TokenType.Add:
                        result.Operator = OperatorType.Add;
                        break;
                    case TokenType.MultiplyEqual:
                        result.Operator = OperatorType.MultiplyBy;
                        break;
                    case TokenType.Multiply:
                        result.Operator = OperatorType.Multiply;
                        break;
                    case TokenType.DivideEqual:
                        result.Operator = OperatorType.DivideBy;
                        break;
                    case TokenType.Divide:
                        result.Operator = OperatorType.Divide;
                        break;
                    case TokenType.GreaterEqual:
                        result.Operator = OperatorType.NotLessThan;
                        break;
                    case TokenType.Equal:
                        result.Operator = OperatorType.EqualsTo;
                        break;
                    case TokenType.Greater:
                        result.Operator = OperatorType.GreaterThan;
                        break;
                    case TokenType.LesserEqual:
                        result.Operator = OperatorType.NotGreaterThan;
                        break;
                    case TokenType.Lesser:
                        result.Operator = OperatorType.LesserThan;
                        break;
                    case TokenType.LogicEqual:
                        result.Operator = OperatorType.LogicEqualsTo;
                        break;
                    default:
                        throw new CompileException(Identifier, Tokens.Current.Position, $"Unrecognized binary operator type {Tokens.Current.Type}");
                }
                Tokens.MoveToNext();
                if (Tokens.Current.Type == TokenType.LineBreak) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected LineBreak");
                }
                var right = GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot);
                if (extraSeparator.HasValue && extraSeparator == Tokens.Current.Type) { // 下一个是自定义终结符，视为不是二元运算符
                    result.Right = right;
                    return result;
                }
                var nextPrecedence = GetOperatorPrecedence();
                if (nextPrecedence < 0 || nextPrecedence < minimumOperatorPrecedence) { // 下一个运算符优先级低于阈值
                    result.Right = right;
                    return result;
                } else if (nextPrecedence <= currentPrecedence) { // 下一个运算符优先级小于等于当前
                    result.Right = right;
                    return ParseBinaryOperator(result);
                } else { // 下一个运算符优先级大于当前
                    result.Right = ParseBinaryOperator(right, currentPrecedence + 1);
                    currentPrecedence = GetOperatorPrecedence();
                    if (currentPrecedence < 0) {
                        return result;
                    }
                    result = new BinaryExpression(Tokens.Current.Position) {Left = result};
                }
            }
        }

        /// <summary>
        /// 处理选择分支
        /// </summary>
        /// <returns></returns>
        private Expression ParseCondition() {
            var result = new ConditionExpression(Tokens.Current.Position);
            while (Tokens.Current.Type == TokenType.If || Tokens.Current.Type == TokenType.Else || Tokens.Current.Type == TokenType.ElseIf) {
                var condition = new ConditionContentExpression(Tokens.Current.Position);
                if (Tokens.Current.Type == TokenType.Else) {
                    condition.Condition = new VariableExpression(Tokens.Current.Position) {Name = new StringExpression(Tokens.Current.Position) {Value = "true"}};
                    Tokens.MoveToNext();
                } else {
                    Tokens.MoveToNext();
                    condition.Condition = ParseBinaryOperator(GeneralParser(
                        TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot));
                }
                if (Tokens.Current.Type != TokenType.LineBreak) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected LineBreak");
                }
                Tokens.MoveToNext();
                if (Tokens.Current.Type != TokenType.CreateScope) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected CreateScope");
                }
                condition.Body = ParseScope();
                result.Contents.Add(condition);
            }
            return result;
        }
        
        /// <summary>
        /// 处理循环
        /// </summary>
        /// <returns></returns>
        private Expression ParseLoop() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            var content = ParseBinaryOperator(GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot));
            if (Tokens.Current.Type != TokenType.LineBreak) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected LineBreak");
            }
            Tokens.MoveToNext();
            if (Tokens.Current.Type != TokenType.CreateScope) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected CreateScope (indent)");
            }
            return new LoopExpression(position) {Condition = content, Body = ParseScope()};
        }
        
        /// <summary>
        /// 处理函数声明
        /// </summary>
        /// <returns></returns>
        private Expression ParseFunctionDefinition() {
            var result = new FunctionExpression(Tokens.Current.Position);
            Tokens.MoveToNext();
            if (Tokens.Current.Type != TokenType.String || !(Tokens.Current is StringToken nameToken)) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected String as scenario name");
            }
            result.Name = nameToken.Content;
            Tokens.MoveToNext();
            while (Tokens.Current.Type != TokenType.LineBreak) {
                var parameter = new StaticNameParameterExpression(Tokens.Current.Position);
                if (Tokens.Current.Type != TokenType.Variable) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected scenario parameter name starts with variable mark");
                }
                Tokens.MoveToNext();
                if (Tokens.Current.Type != TokenType.String || !(Tokens.Current is StringToken paramNameToken)) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected scenario parameter name starts with variable mark");
                }
                parameter.Name = new StringExpression(Tokens.Current.Position) {Value = paramNameToken.Content};
                Tokens.MoveToNext();
                if (Tokens.Current.Type == TokenType.Equal) {
                    Tokens.MoveToNext();
                    parameter.Value = ParseBinaryOperator(GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket));
                } else {
                    parameter.Value = new EmptyExpression(parameter.Name.Position);
                }
                result.Parameters.Add(parameter);
            }
            Tokens.MoveToNext();
            if (Tokens.Current.Type != TokenType.CreateScope) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected CreateScope (indent)");
            }
            result.Body = ParseScope();
            return result;
        }
        
        /// <summary>
        /// 处理返回
        /// </summary>
        /// <returns></returns>
        private Expression ParseReturn() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            if (Tokens.Current.Type == TokenType.LineBreak) {
                return new ReturnExpression(position) {Value = new EmptyExpression(position)};
            }
            var value = ParseBinaryOperator(GeneralParser(
                TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket, TokenType.LogicNot));
            if (Tokens.Current.Type != TokenType.LineBreak) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected LineBreak");
            }
            Tokens.MoveToNext();
            return new ReturnExpression(position) {Value = value};
        }
        
        /// <summary>
        /// 处理括号
        /// </summary>
        /// <returns></returns>
        private Expression ParseBracket() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            if (Tokens.Current.Type == TokenType.RightBracket) {
                // 空括号等于返回@null
                return new EmptyExpression(position);
            }
            var content = ParseBinaryOperator(GeneralParser(TokenType.String, TokenType.Number, TokenType.PluginCallStart, TokenType.Variable, TokenType.Constant, TokenType.LeftBracket,
                TokenType.LogicNot, TokenType.Function, TokenType.If, TokenType.Loop, TokenType.Language, TokenType.FunctionCall));
            while (Tokens.Current.Type == TokenType.LineBreak) {
                Tokens.MoveToNext();
            }
            if (Tokens.Current.Type != TokenType.RightBracket) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected RightBracket");
            }
            Tokens.MoveToNext();
            return content;
        }
        
        /// <summary>
        /// 处理域
        /// </summary>
        /// <returns></returns>
        private ScopeExpression ParseScope() {
            var currentToken = Tokens.Current;
            Tokens.MoveToNext();
            // 由于词法分析时忽略了所有空行，因而ParseScope绝对不会解析出空结果
            return new ScopeExpression(currentToken.Position) {Content = Parse().Content};
        }

        /// <summary>
        /// 获取正在处理的运算符的优先级
        /// <para>所有一元运算符的优先级为6，而该函数的返回值为-1，除此之外其返回值等于各运算符的实际优先级</para>
        /// </summary>
        /// <returns></returns>
        private int GetOperatorPrecedence() {
            switch (Tokens.Current.Type) {
                case TokenType.Equal:
                    return 0;
                case TokenType.Greater:
                case TokenType.GreaterEqual:
                case TokenType.Lesser:
                case TokenType.LesserEqual:
                    return 1;
                case TokenType.MinusEqual:
                case TokenType.Minus:
                case TokenType.AddEqual:
                case TokenType.Add:
                    return 2;
                case TokenType.MultiplyEqual:
                case TokenType.Multiply:
                case TokenType.DivideEqual:
                case TokenType.Divide:
                    return 3;
                case TokenType.LogicEqual:
                    return 4;
                case TokenType.PickChild:
                    return 5;
                default:
                    return -1;
            }
        }
        
        /// <summary>
        /// 公共标记解析器
        /// </summary>
        /// <param name="acceptableTokenTypes">可接受的标记</param>
        /// <returns></returns>
        private Expression GeneralParser(params TokenType[] acceptableTokenTypes) {
            Expression result = null;
            switch (Tokens.Current.Type) {
                case TokenType.DialogueSpeaker:
                    if (acceptableTokenTypes.Contains(TokenType.DialogueSpeaker)) {
                        result = ParseDialogue();
                    }
                    break;
                case TokenType.DialogueContent:
                    if (acceptableTokenTypes.Contains(TokenType.DialogueContent)) {
                        result = ParseDialogue();
                    }
                    break;
                case TokenType.String:
                    if (acceptableTokenTypes.Contains(TokenType.String)) {
                        result = ParseString();
                    }
                    break;
                case TokenType.Number:
                    if (acceptableTokenTypes.Contains(TokenType.Number)) {
                        result = ParseNumber();
                    }
                    break;
                case TokenType.CreateScope:
                    if (acceptableTokenTypes.Contains(TokenType.CreateScope)) {
                        result = ParseScope();
                    }
                    break;
                case TokenType.PluginCallStart:
                    if (acceptableTokenTypes.Contains(TokenType.PluginCallStart)) {
                        result = ParsePluginCall();
                    }
                    break;
                case TokenType.Variable:
                    if (acceptableTokenTypes.Contains(TokenType.Variable)) {
                        result = ParseVariable();
                    }
                    break;
                case TokenType.Constant:
                    if (acceptableTokenTypes.Contains(TokenType.Constant)) {
                        result = ParseConstant();
                    }
                    break;
                case TokenType.LeftBracket:
                    if (acceptableTokenTypes.Contains(TokenType.LeftBracket)) {
                        result = ParseBracket();
                    }
                    break;
                case TokenType.LogicNot:
                    if (acceptableTokenTypes.Contains(TokenType.LogicNot)) {
                        result = ParseLogicNot();
                    }
                    break;
                case TokenType.Function:
                    if (acceptableTokenTypes.Contains(TokenType.Function)) {
                        result = ParseFunctionDefinition();
                    }
                    break;
                case TokenType.If:
                    if (acceptableTokenTypes.Contains(TokenType.If)) {
                        result = ParseCondition();
                    }
                    break;
                case TokenType.Loop:
                    if (acceptableTokenTypes.Contains(TokenType.Loop)) {
                        result = ParseLoop();
                    }
                    break;
                case TokenType.Language:
                    if (acceptableTokenTypes.Contains(TokenType.Language)) {
                        result = ParseLanguage();
                    }
                    break;
                case TokenType.Return:
                    if (acceptableTokenTypes.Contains(TokenType.Return)) {
                        result = ParseReturn();
                    }
                    break;
                case TokenType.FunctionCall:
                    if (acceptableTokenTypes.Contains(TokenType.FunctionCall)) {
                        result = ParseFunctionCall();
                    }
                    break;
            }
            if (result == null) {
                throw new CompileException(Identifier, Tokens.Current.Position, $"Expected {string.Join(", ", acceptableTokenTypes)}");
            }
            return result;
        }
        
    }
}