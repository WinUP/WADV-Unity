using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler.Expressions;

namespace Core.VisualNovel.Script.Compiler {
    public class Assembler {
        private ScopeExpression RootExpression { get; }
        private string Identifier { get; }

        public Assembler(ScopeExpression root, string identifier) {
            RootExpression = root;
            Identifier = identifier;
        }

        public AssembleFile Assemble() {
            var context = new AssemblerContext();
            Assemble(context, RootExpression);
            return context.File;
        }

        private void Assemble(AssemblerContext context, Expression expression, params CompilerFlag[] flags) {
            switch (expression) {
                case BinaryExpression binaryExpression:
                    Assemble(context, binaryExpression.Right);
                    switch (binaryExpression.Operator) {
                        case OperatorType.AddBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OpCode(OpCodeType.ADD);
                            break;
                        case OperatorType.MinusBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OpCode(OpCodeType.SUB);
                            break;
                        case OperatorType.MultiplyBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OpCode(OpCodeType.MUL);
                            break;
                        case OperatorType.DivideBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OpCode(OpCodeType.DIV);
                            break;
                    }
                    if (binaryExpression.Operator == OperatorType.EqualsTo) {
                        Assemble(context, binaryExpression.Left, CompilerFlag.UseSetLocalVariable);
                    } else {
                        Assemble(context, binaryExpression.Left);
                        switch (binaryExpression.Operator) {
                            case OperatorType.PickChild:
                                context.File.OpCode(OpCodeType.PICK);
                                break;
                            case OperatorType.Add:
                                context.File.OpCode(OpCodeType.ADD);
                                break;
                            case OperatorType.Minus:
                                context.File.OpCode(OpCodeType.SUB);
                                break;
                            case OperatorType.Multiply:
                                context.File.OpCode(OpCodeType.MUL);
                                break;
                            case OperatorType.Divide:
                                context.File.OpCode(OpCodeType.DIV);
                                break;
                            case OperatorType.GreaterThan:
                                context.File.OpCode(OpCodeType.CGT);
                                break;
                            case OperatorType.LesserThan:
                                context.File.OpCode(OpCodeType.CLT);
                                break;
                            case OperatorType.NotLessThan:
                                context.File.OpCode(OpCodeType.CGE);
                                break;
                            case OperatorType.NotGreaterThan:
                                context.File.OpCode(OpCodeType.CLE);
                                break;
                            case OperatorType.LogicEqualsTo:
                                context.File.OpCode(OpCodeType.EQL);
                                break;
                        }
                    }
                    break;
                case CommandExpression commandExpression:
                    break;
                case ConditionContentExpression conditionContentExpression:
                    break;
                case ConditionExpression conditionExpression:
                    break;
                case DialogueExpression dialogueExpression:
                    context.File.LoadDialogue(dialogueExpression.Character, dialogueExpression.Content);
                    break;
                case EmptyExpression _:
                    context.File.LoadNull();
                    break;
                case FloatExpression floatExpression:
                    context.File.LoadFloat(floatExpression.Value);
                    break;
                case IntegerExpression integerExpression:
                    context.File.LoadInteger(integerExpression.Value);
                    break;
                case LanguageExpression languageExpression:
                    context.File.Language(context.Language = languageExpression.Language);
                    break;
                case LogicNotExpression logicNotExpression:
                    Assemble(context, logicNotExpression.Content);
                    context.File.OpCode(OpCodeType.NOT);
                    break;
                case LoopExpression loopExpression:
                    var startLabel = $"{context.Scopes}L{context.NextLabelId}";
                    var endLabel = $"{context.Scopes}L{context.NextLabelId}";
                    context.File.CreateLabel(startLabel);
                    Assemble(context, loopExpression.Condition);
                    context.File.OpCode(OpCodeType.BF_S);
                    context.File.LoadString(endLabel);
                    Assemble(context, loopExpression.Body);
                    context.File.OpCode(OpCodeType.BR_S);
                    context.File.LoadString(startLabel);
                    context.File.CreateLabel(endLabel);
                    break;
                case ParameterExpression parameterExpression:
                    Assemble(context, parameterExpression.Value);
                    Assemble(context, parameterExpression.Name);
                    break;
                case ReturnExpression returnExpression:
                    Assemble(context, returnExpression.Value);
                    context.File.OpCode(OpCodeType.RET);
                    break;
                case ScenarioExpression scenarioExpression:
                    scenarioExpression.Name = $"{context.Scopes}S{scenarioExpression.Name}";
                    context.Scenarios.Add(scenarioExpression);
                    break;
                case ScopeExpression scopeExpression:
                    context.File.OpCode(OpCodeType.SCOPE);
                    ++context.Scopes;
                    foreach (var (item, i) in scopeExpression.Content.WithIndex()) {
                        Assemble(context, item);
                        if (i < scopeExpression.Content.Count - 1) {
                            context.File.Pop();
                        }
                    }
                    --context.Scopes;
                    context.File.OpCode(OpCodeType.LEAVE);
                    context.File.OpCode(OpCodeType.RET);
                    break;
                case StringExpression stringExpression:
                    if (stringExpression.Translatable) {
                        context.File.LoadTranslatableString(stringExpression.Value);
                    } else {
                        context.File.LoadString(stringExpression.Value);
                    }
                    break;
                case ToBooleanExpression toBooleanExpression:
                    Assemble(context, toBooleanExpression.Value);
                    context.File.OpCode(OpCodeType.BVAL);
                    break;
                case VariableExpression variableExpression:
                    if (variableExpression.Name is StringExpression nameExpression) {
                        switch (nameExpression.Value.ToUpper()) {
                            case "TRUE":
                                context.File.LoadBoolean(true);
                                break;
                            case "FALSE":
                                context.File.LoadBoolean(false);
                                break;
                            case "NULL":
                                context.File.LoadNull();
                                break;
                            default:
                                Assemble(context, nameExpression);
                                break;
                        }
                    } else {
                        Assemble(context, variableExpression.Name);
                    }
                    context.File.OpCode(flags.Any(e => e == CompilerFlag.UseSetLocalVariable) ? OpCodeType.STLOC : OpCodeType.LDLOC);
                    break;
            }
        }
    }
}