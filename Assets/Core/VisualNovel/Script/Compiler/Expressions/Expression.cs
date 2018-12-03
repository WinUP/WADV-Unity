using System;

namespace Core.VisualNovel.Script.Compiler.Expressions {
    public abstract class Expression {
        public CodePosition Position { get; }

        protected Expression(CodePosition position) {
            Position = position;
        }

        public ExpressionType GetExpressionType() {
            switch (this) {
                case BinaryExpression _:
                    return ExpressionType.Binary;
                case CommandExpression _:
                    return ExpressionType.Command;
                case ConditionContentExpression _:
                    return ExpressionType.ConditionContent;
                case ConditionExpression _:
                    return ExpressionType.Condition;
                case DialogueExpression _:
                    return ExpressionType.Dialogue;
                case EmptyExpression _:
                    return ExpressionType.Empty;
                case FloatExpression _:
                    return ExpressionType.Float;
                case IntegerExpression _:
                    return ExpressionType.Integer;
                case LanguageExpression _:
                    return ExpressionType.Language;
                case LogicNotExpression _:
                    return ExpressionType.LogicNot;
                case LoopExpression _:
                    return ExpressionType.Loop;
                case ParameterExpression _:
                    return ExpressionType.Parameter;
                case ReturnExpression _:
                    return ExpressionType.Return;
                case ScenarioExpression _:
                    return ExpressionType.Scenario;
                case ScopeExpression _:
                    return ExpressionType.Scope;
                case StringExpression _:
                    return ExpressionType.String;
                case ToBooleanExpression _:
                    return ExpressionType.ToBoolean;
                case VariableExpression _:
                    return ExpressionType.Variable;
                default:
                    throw new ArgumentException("Unrecognized expression type");
            }
        }
    }
}