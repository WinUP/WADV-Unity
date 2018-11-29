using System.Collections.Generic;
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
        private TokenSequence Tokens { get; }
        /// <summary>
        /// Token序列标识符
        /// </summary>
        private string Identifier { get; }

        public Parser(IEnumerable<BasicToken> tokens, string identifier) {
            Tokens = new TokenSequence(tokens);
            Identifier = identifier;
        }

        /// <summary>
        /// 构建抽象语法树
        /// </summary>
        public Expression Parse() {
            var result = new SequenceExpression(new CodePosition());
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
                    case TokenType.CallStart:
                        result.Content.Add(ParseBinaryOperator(ParseCall()));
                        break;
                    case TokenType.Variable:
                        result.Content.Add(ParseBinaryOperator(ParseVariable()));
                        break;
                    case TokenType.LeftBracket:
                        result.Content.Add(ParseBinaryOperator(ParseBracket()));
                        break;
                    case TokenType.LogicNot:
                        result.Content.Add(ParseBinaryOperator(ParseLogicNot()));
                        break;
                    case TokenType.Scenario:
                        result.Content.Add(ParseBinaryOperator(ParseScenario()));
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
                    case TokenType.Language:
                        result.Content.Add(ParseBinaryOperator(ParseLanguage()));
                        break;
                    case TokenType.LeaveScope:
                        Tokens.MoveToNext();
                        // 解析文件域时LeaveScope一定不会出现，如果出现则证明这是脚本中一个子域，那么这一定是在ParseScope的递归中，可以直接返回
                        return result.Content.Count == 0 ? null : result.Content.Count == 1 ? result.Content[0] : result;
                    case TokenType.CallEnd:
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
            return result.Content.Count == 0 ? null : result.Content.Count == 1 ? result.Content[0] : result;
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
            Expression content;
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    content = ParseString();
                    break;
                case TokenType.CallStart:
                    content = ParseCall();
                    break;
                case TokenType.Variable:
                    content = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    content = ParseBracket();
                    break;
                case TokenType.LogicNot:
                    content = ParseLogicNot();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected String, CallStart '[', Variable '@',  LeftBracket '(' or LogicNot '!'");
            }
            return new VariableExpression(position) {Name = content};
        }

        /// <summary>
        /// 处理调用
        /// </summary>
        /// <returns></returns>
        private Expression ParseCall() {
            // 指令采取自左至右贪心匹配，因此[A->B+ C+(@D=E)=F]被解析为[A->B+C+(@D=E)=F]不算错误，反正执行会崩
            Tokens.MoveToNext();
            var command = new CommandExpression(Tokens.Current.Position) {Target = ParseCallName()};
            while (Tokens.Current.Type != TokenType.CallEnd) {
                if (Tokens.Current.Type == TokenType.LineBreak) {
                    throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected LineBreak when parsing call");
                }
                var parameter = new ParameterExpression(Tokens.Current.Position);
                switch (Tokens.Current.Type) {
                    case TokenType.String:
                        parameter.Name = ParseString();
                        break;
                    case TokenType.Number:
                        parameter.Name = ParseNumber();
                        break;
                    case TokenType.CallStart:
                        parameter.Name = ParseCall();
                        break;
                    case TokenType.Variable:
                        parameter.Name = ParseVariable();
                        break;
                    case TokenType.LeftBracket:
                        parameter.Name = ParseScope();
                        break;
                    default:
                        throw new CompileException(Identifier, Tokens.Current.Position,
                            "Expected String, Number, CallStart '[', Variable '@' or LeftBracket '('");
                }
                parameter.Name = ParseBinaryOperator(parameter.Name, 0, TokenType.Equal);
                if (Tokens.Current.Type != TokenType.Equal) {
                    parameter.Value = new EmptyExpression(parameter.Name.Position);
                    command.Parameters.Add(parameter);
                    continue;
                }
                Tokens.MoveToNext();
                switch (Tokens.Current.Type) {
                    case TokenType.String:
                        parameter.Value = ParseString();
                        break;
                    case TokenType.Number:
                        parameter.Value = ParseNumber();
                        break;
                    case TokenType.CallStart:
                        parameter.Value = ParseCall();
                        break;
                    case TokenType.Variable:
                        parameter.Value = ParseVariable();
                        break;
                    case TokenType.LeftBracket:
                        parameter.Value = ParseBracket();
                        break;
                    case TokenType.LogicNot:
                        parameter.Value = ParseLogicNot();
                        break;
                    default:
                        throw new CompileException(Identifier, Tokens.Current.Position,
                            "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(' or LogicNot '!'");
                }
                parameter.Value = ParseBinaryOperator(parameter.Value);
                command.Parameters.Add(parameter);
            }
            Tokens.MoveToNext();
            return command;
        }

        /// <summary>
        /// 处理调用名
        /// </summary>
        /// <returns></returns>
        private Expression ParseCallName() {
            Expression content;
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    content = ParseString();
                    break;
                case TokenType.CallStart:
                    content = ParseCall();
                    break;
                case TokenType.Variable:
                    content = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    content = ParseScope();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position, "Expected String, CallStart '[', Variable '@' or LeftBracket '('");
            }
            return ParseBinaryOperator(content);
        }

        /// <summary>
        /// 处理逻辑否
        /// </summary>
        /// <returns></returns>
        private Expression ParseLogicNot() {
            var position = Tokens.Current.Position;
            Tokens.MoveToNext();
            Expression content;
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    content = ParseString();
                    break;
                case TokenType.Number:
                    content = ParseNumber();
                    break;
                case TokenType.CallStart:
                    content = ParseCall();
                    break;
                case TokenType.Variable:
                    content = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    content = ParseBracket();
                    break;
                case TokenType.LogicNot:
                    content = ParseLogicNot();
                    break;
                case TokenType.If:
                    content = ParseCondition();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position,
                        "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(', LogicNot '!' or If");
            }
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
                Expression right;
                switch (Tokens.Current.Type) {
                    case TokenType.DialogueSpeaker:
                    case TokenType.DialogueContent:
                        throw new CompileException(Identifier, Tokens.Current.Position, "Unexpected dialogue inside code, dialogue must be in a single line");
                    case TokenType.String:
                        right = ParseString();
                        break;
                    case TokenType.Number:
                        right = ParseNumber();
                        break;
                    case TokenType.CallStart:
                        right = ParseCall();
                        break;
                    case TokenType.Variable:
                        right = ParseVariable();
                        break;
                    case TokenType.LeftBracket:
                        right = ParseBracket();
                        break;
                    case TokenType.LogicNot:
                        right = ParseLogicNot();
                        break;
                    case TokenType.Scenario:
                        right = ParseScenario();
                        break;
                    case TokenType.If:
                        right = ParseCondition();
                        break;
                    case TokenType.Loop:
                        right = ParseLoop();
                        break;
                    case TokenType.Language:
                        right = ParseLanguage();
                        break;
                    default:
                        throw new CompileException(Identifier, Tokens.Current.Position,
                            "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(', LogicNot '!', Scenario, If, Loop or Language");
                }
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
                    condition.Condition = new EmptyExpression(Tokens.Current.Position);
                    Tokens.MoveToNext();
                } else {
                    Tokens.MoveToNext();
                    switch (Tokens.Current.Type) {
                        case TokenType.String:
                            condition.Condition = ParseString();
                            break;
                        case TokenType.Number:
                            condition.Condition = ParseNumber();
                            break;
                        case TokenType.CallStart:
                            condition.Condition = ParseCall();
                            break;
                        case TokenType.Variable:
                            condition.Condition = ParseVariable();
                            break;
                        case TokenType.LeftBracket:
                            condition.Condition = ParseBracket();
                            break;
                        case TokenType.LogicNot:
                            condition.Condition = ParseLogicNot();
                            break;
                        case TokenType.Language:
                            condition.Condition = ParseLanguage();
                            break;
                        default:
                            throw new CompileException(Identifier, Tokens.Current.Position,
                                "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(', LogicNot '!' or Language");
                    }
                    condition.Condition = ParseBinaryOperator(condition.Condition);
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
            Expression content;
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    content = ParseString();
                    break;
                case TokenType.Number:
                    content = ParseNumber();
                    break;
                case TokenType.CallStart:
                    content = ParseCall();
                    break;
                case TokenType.Variable:
                    content = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    content = ParseBracket();
                    break;
                case TokenType.LogicNot:
                    content = ParseLogicNot();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position,
                        "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(' or LogicNot");
            }
            content = ParseBinaryOperator(content);
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
        /// 处理场景
        /// </summary>
        /// <returns></returns>
        private Expression ParseScenario() {
            var result = new ScenarioExpression(Tokens.Current.Position);
            Tokens.MoveToNext();
            if (Tokens.Current.Type != TokenType.String || !(Tokens.Current is StringToken nameToken)) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected String as scenario name");
            }
            result.Name = nameToken.Content;
            Tokens.MoveToNext();
            while (Tokens.Current.Type != TokenType.LineBreak) {
                var parameter = new ParameterExpression(Tokens.Current.Position);
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
                    switch (Tokens.Current.Type) {
                        case TokenType.String:
                            parameter.Value = ParseString();
                            break;
                        case TokenType.Number:
                            parameter.Value = ParseNumber();
                            break;
                        case TokenType.CallStart:
                            parameter.Value = ParseCall();
                            break;
                        case TokenType.Variable:
                            parameter.Value = ParseVariable();
                            break;
                        case TokenType.LeftBracket:
                            parameter.Value = ParseBracket();
                            break;
                        case TokenType.Language:
                            parameter.Value = ParseLanguage();
                            break;
                        default:
                            throw new CompileException(Identifier, Tokens.Current.Position, "Expected String, Number, CallStart '[', LeftBracket '(' or Language");
                    }
                    parameter.Value = ParseBinaryOperator(parameter.Value);
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
            Expression value;
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    value = ParseString();
                    break;
                case TokenType.Number:
                    value = ParseNumber();
                    break;
                case TokenType.CallStart:
                    value = ParseCall();
                    break;
                case TokenType.Variable:
                    value = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    value = ParseBracket();
                    break;
                case TokenType.LogicNot:
                    value = ParseLogicNot();
                    break;
                case TokenType.Scenario:
                    value = ParseScenario();
                    break;
                case TokenType.If:
                    value = ParseCondition();
                    break;
                case TokenType.Loop:
                    value = ParseLoop();
                    break;
                case TokenType.Language:
                    value = ParseLanguage();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position,
                        "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(', LogicNot '!', Scenario, If, Loop or Language");
            }
            value = ParseBinaryOperator(value);
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
            Expression content;
            if (Tokens.Current.Type == TokenType.RightBracket) {
                // 空括号等于返回@null
                return new EmptyExpression(position);
            }
            switch (Tokens.Current.Type) {
                case TokenType.String:
                    content = ParseString();
                    break;
                case TokenType.Number:
                    content = ParseNumber();
                    break;
                case TokenType.CallStart:
                    content = ParseCall();
                    break;
                case TokenType.Variable:
                    content = ParseVariable();
                    break;
                case TokenType.LeftBracket:
                    content = ParseBracket();
                    break;
                case TokenType.LogicNot:
                    content = ParseLogicNot();
                    break;
                case TokenType.Scenario:
                    content = ParseScenario();
                    break;
                case TokenType.If:
                    content = ParseCondition();
                    break;
                case TokenType.Loop:
                    content = ParseLoop();
                    break;
                case TokenType.Language:
                    content = ParseLanguage();
                    break;
                default:
                    throw new CompileException(Identifier, Tokens.Current.Position,
                        "Expected String, Number, CallStart '[', Variable '@', LeftBracket '(', RightBracket ')', LogicNot '!', Scenario, If, Loop, or Language");
            }
            content = ParseBinaryOperator(content);
            if (Tokens.Current.Type != TokenType.RightBracket) {
                throw new CompileException(Identifier, Tokens.Current.Position, "Expected RightBracket");
            }
            Tokens.MoveToNext();
            return new ScopeExpression(position) {Content = content};
        }
        
        /// <summary>
        /// 处理域
        /// </summary>
        /// <returns></returns>
        private Expression ParseScope() {
            var currentToken = Tokens.Current;
            Tokens.MoveToNext();
            // 由于词法分析时忽略了所有空行，因而ParseScope绝对不会解析出空结果
            return new ScopeExpression(currentToken.Position) {Content = Parse()};
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
    }
}