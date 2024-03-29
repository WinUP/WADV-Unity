using System.Linq;
using WADV.Extensions;
using WADV.Translation;
using WADV.VisualNovel.Compiler.Expressions;

namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// VNB脚本生成器
    /// </summary>
    public static class ByteCodeGenerator {
        /// <summary>
        /// 生成字节码
        /// </summary>
        /// <param name="root">根表达式</param>
        /// <param name="identifier">源文件ID</param>
        /// <returns></returns>
        public static (byte[] Content, ScriptTranslation DefaultTranslations) Generate(ScopeExpression root, CodeIdentifier identifier) {
            var context = new ByteCodeGeneratorContext();
            // 生成主程序段
            Generate(context, root);
            // 生成各种段
            var (code, labels, strings, positions, translations) = context.File.CreateSegments();
            context.File.Close();
            // 准备目标文件
            var targetFile = new ByteCodeWriter();
            // 写入文件标志 ("VisualNovelScript Version 1, assembly format"的CRC32)
            targetFile.DirectWrite(0x963EFE4A);
            // 写入源文件哈希用于跳过重复编译
            targetFile.DirectWrite(identifier.Hash);
            // 写入各种段
            targetFile.DirectWrite(translations.ToByteArray());
            targetFile.DirectWrite(strings);
            targetFile.DirectWrite(labels);
            targetFile.DirectWrite(positions);
            targetFile.DirectWrite(code);
            return (targetFile.CreateMainSegment(), translations);
        }

        private static void Generate(ByteCodeGeneratorContext context, Expression expression, params CompilerFlag[] flags) {
            switch (expression) {
                case BinaryExpression binaryExpression:
                    Generate(context, binaryExpression.Right);
                    switch (binaryExpression.Operator) {
                        case OperatorType.AddBy:
                            Generate(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.ADD, binaryExpression.Position);
                            break;
                        case OperatorType.MinusBy:
                            Generate(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.SUB, binaryExpression.Position);
                            break;
                        case OperatorType.MultiplyBy:
                            Generate(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.MUL, binaryExpression.Position);
                            break;
                        case OperatorType.DivideBy:
                            Generate(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.DIV, binaryExpression.Position);
                            break;
                    }
                    if (binaryExpression.Operator == OperatorType.EqualsTo || binaryExpression.Operator == OperatorType.AddBy ||
                        binaryExpression.Operator == OperatorType.MinusBy || binaryExpression.Operator == OperatorType.MultiplyBy ||
                        binaryExpression.Operator == OperatorType.DivideBy) {
                        Generate(context, binaryExpression.Left, CompilerFlag.UseSetLocalVariable);
                        if (!(binaryExpression.Left is VariableExpression) && !(binaryExpression.Left is ConstantExpression)) { // 对于所有赋值类语句，如果左侧不是自带赋值指令的变量表达式则补充一个赋值指令来改写内存堆栈值
                            context.File.OperationCode(OperationCode.STMEM, binaryExpression.Left.Position);
                        }
                    } else {
                        Generate(context, binaryExpression.Left);
                        switch (binaryExpression.Operator) {
                            case OperatorType.PickChild:
                                context.File.OperationCode(OperationCode.PICK, binaryExpression.Position);
                                break;
                            case OperatorType.Add:
                                context.File.OperationCode(OperationCode.ADD, binaryExpression.Position);
                                break;
                            case OperatorType.Minus:
                                context.File.OperationCode(OperationCode.SUB, binaryExpression.Position);
                                break;
                            case OperatorType.Multiply:
                                context.File.OperationCode(OperationCode.MUL, binaryExpression.Position);
                                break;
                            case OperatorType.Divide:
                                context.File.OperationCode(OperationCode.DIV, binaryExpression.Position);
                                break;
                            case OperatorType.GreaterThan:
                                context.File.OperationCode(OperationCode.CGT, binaryExpression.Position);
                                break;
                            case OperatorType.LesserThan:
                                context.File.OperationCode(OperationCode.CLT, binaryExpression.Position);
                                break;
                            case OperatorType.NotLessThan:
                                context.File.OperationCode(OperationCode.CGE, binaryExpression.Position);
                                break;
                            case OperatorType.NotGreaterThan:
                                context.File.OperationCode(OperationCode.CLE, binaryExpression.Position);
                                break;
                            case OperatorType.LogicEqualsTo:
                                context.File.OperationCode(OperationCode.EQL, binaryExpression.Position);
                                break;
                            case OperatorType.LogicNotEqualsTo:
                                context.File.OperationCode(OperationCode.EQL, binaryExpression.Position);
                                context.File.OperationCode(OperationCode.NEG, binaryExpression.Position);
                                break;
                        }
                    }
                    break;
                case CallExpression callExpression:
                    foreach (var parameter in callExpression.Parameters) {
                        Generate(context, parameter);
                    }
                    context.File.LoadInteger(callExpression.Parameters.Count, callExpression.Position);
                    Generate(context, callExpression.Target);
                    context.File.Call(callExpression.Position);
                    break;
                case ConditionExpression conditionExpression: // 协同处理ConditionContentExpression
                    var conditionEndLabel = context.NextLabelId;
                    var conditionNextLabel = context.NextLabelId;
                    var conditionCount = conditionExpression.Contents.Count;
                    for (var i = -1; ++i < conditionCount;) {
                        var branch = conditionExpression.Contents[i];
                        context.File.CreateLabel(conditionNextLabel);
                        if (i < conditionCount - 1) {
                            conditionNextLabel = context.NextLabelId;
                        }
                        Generate(context, branch.Condition);
                        context.File.OperationCode(OperationCode.BVAL, branch.Position);
                        context.File.OperationCode(OperationCode.BF, branch.Position);
                        context.File.Write7BitEncodedInteger(i == conditionCount - 1 ? conditionEndLabel : conditionNextLabel);
                        Generate(context, branch.Body);
                        context.File.OperationCode(OperationCode.BR, branch.Body.Position);
                        context.File.Write7BitEncodedInteger(conditionEndLabel);
                    }
                    context.File.CreateLabel(conditionEndLabel);
                    break;
                case ConstantExpression constantExpression:
                    var isSetConstant = flags.Any(e => e == CompilerFlag.UseSetLocalVariable);
                    if (constantExpression.Name is StringExpression constantNameExpression && !isSetConstant) {
                        switch (constantNameExpression.Value.ToUpper()) {
                            case "TRUE":
                                context.File.LoadBoolean(true, constantExpression.Position);
                                return;
                            case "FALSE":
                                context.File.LoadBoolean(false, constantExpression.Position);
                                return;
                            case "NULL":
                                context.File.LoadNull(constantExpression.Position);
                                return;
                            default:
                                Generate(context, constantNameExpression);
                                break;
                        }
                    } else {
                        Generate(context, constantExpression.Name);
                    }
                    context.File.OperationCode(isSetConstant ? OperationCode.STCON : OperationCode.LDCON, constantExpression.Position);
                    break;
                case DialogueExpression dialogueExpression:
                    context.File.LoadDialogue(dialogueExpression.Character, dialogueExpression.Content, dialogueExpression.Position);
                    break;
                case EmptyExpression emptyExpression:
                    context.File.LoadNull(emptyExpression.Position);
                    break;
                case ExportExpression exportExpression:
                    Generate(context, exportExpression.Value);
                    Generate(context, exportExpression.Name);
                    context.File.OperationCode(OperationCode.EXP, expression.Position);
                    break;
                case FloatExpression floatExpression:
                    context.File.LoadFloat(floatExpression.Value, floatExpression.Position);
                    break;
                case FunctionCallExpression functionCallExpression:
                    foreach (var parameter in functionCallExpression.Parameters) {
                        Generate(context, parameter);
                    }
                    context.File.LoadInteger(functionCallExpression.Parameters.Count, functionCallExpression.Position);
                    Generate(context, functionCallExpression.Target);
                    context.File.Func(functionCallExpression.Position);
                    break;
                case FunctionExpression functionExpression:
                    var functionStart = context.NextLabelId;
                    var functionEnd = context.NextLabelId;
                    // 场景表现为一个声明在当前作用域内的变量
                    context.File.OperationCode(OperationCode.LDENTRY, functionExpression.Position);
                    context.File.Write7BitEncodedInteger(functionStart);
                    context.File.LoadString(functionExpression.Name, functionExpression.Position);
                    context.File.OperationCode(OperationCode.STLOC, functionExpression.Position);
                    // 令外部代码执行时跳过函数部分
                    context.File.OperationCode(OperationCode.BR, SourcePosition.UnavailablePosition);
                    context.File.Write7BitEncodedInteger(functionEnd);
                    // 开始函数生成
                    context.File.CreateLabel(functionStart);
                    // 默认值赋值
                    foreach (var parameter in functionExpression.Parameters) {
                        if (!(parameter.Value is EmptyExpression)) {
                            var nextParameterCheck = context.NextLabelId;
                            context.File.LoadNull(parameter.Position);
                            Generate(context, parameter.Name);
                            context.File.OperationCode(OperationCode.EQL, parameter.Position);
                            context.File.OperationCode(OperationCode.BF, parameter.Position);
                            context.File.Write7BitEncodedInteger(nextParameterCheck);
                            Generate(context, parameter.Value);
                            Generate(context, parameter.Name);
                            context.File.OperationCode(OperationCode.STLOC, parameter.Position);
                            context.File.Pop(parameter.Position);
                            context.File.CreateLabel(nextParameterCheck);
                        }
                    }
                    // 生成函数体
                    Generate(context, functionExpression.Body, CompilerFlag.NotCreateScope);
                    // 结束函数生成
                    context.File.OperationCode(OperationCode.RET, SourcePosition.UnavailablePosition);
                    context.File.CreateLabel(functionEnd);
                    break;
                case ImportExpression importExpression:
                    Generate(context, importExpression.Target);
                    context.File.OperationCode(OperationCode.LOAD, importExpression.Position);
                    break;
                case IntegerExpression integerExpression:
                    context.File.LoadInteger(integerExpression.Value, integerExpression.Position);
                    break;
                case LogicNotExpression logicNotExpression:
                    Generate(context, logicNotExpression.Content);
                    context.File.OperationCode(OperationCode.NEG, logicNotExpression.Position);
                    break;
                case LoopExpression loopExpression:
                    var loopStartLabel = context.NextLabelId;
                    var loopEndLabel = context.NextLabelId;
                    context.File.CreateLabel(loopStartLabel);
                    Generate(context, loopExpression.Condition);
                    context.File.OperationCode(OperationCode.BVAL, loopExpression.Position);
                    context.File.OperationCode(OperationCode.BF, loopExpression.Condition.Position);
                    context.File.Write7BitEncodedInteger(loopEndLabel);
                    Generate(context, loopExpression.Body);
                    context.File.OperationCode(OperationCode.BR, loopExpression.Body.Position);
                    context.File.Write7BitEncodedInteger(loopStartLabel);
                    context.File.CreateLabel(loopEndLabel);
                    break;
                case ParameterExpression parameterExpression:
                    Generate(context, parameterExpression.Value);
                    Generate(context, parameterExpression.Name);
                    break;
                case ReturnExpression returnExpression:
                    // RET是用于结束作用域执行的，不是用来返回结果的，返回结果实际上是先入栈要返回的结果后插入RET实现的
                    Generate(context, returnExpression.Value);
                    context.File.OperationCode(OperationCode.RET, returnExpression.Position);
                    break;
                case ScopeExpression scopeExpression:
                    if (!flags.Contains(CompilerFlag.NotCreateScope)) {
                        context.File.OperationCode(OperationCode.SCOPE, scopeExpression.Position);
                    }
                    ++context.Scope;
                    foreach (var (item, i) in scopeExpression.Content.WithIndex()) {
                        Generate(context, item);
                        if (i < scopeExpression.Content.Count - 1) {
                            // 只要不是域内最后一行就出栈执行结果，最后一行保留，这样作用上等同于返回了最后一行执行结果
                            context.File.Pop(item.Position);
                        }
                    }
                    --context.Scope;
                    if (!flags.Contains(CompilerFlag.NotCreateScope)) {
                        context.File.OperationCode(OperationCode.LEAVE, scopeExpression.Position);
                    }
                    break;
                case StringExpression stringExpression:
                    if (stringExpression.Translatable) {
                        context.File.LoadTranslatableString(stringExpression.Value, stringExpression.Position);
                    } else {
                        context.File.LoadString(stringExpression.Value, stringExpression.Position);
                    }
                    break;
                case ToBooleanExpression toBooleanExpression:
                    Generate(context, toBooleanExpression.Value);
                    context.File.OperationCode(OperationCode.BVAL, toBooleanExpression.Position);
                    break;
                case VariableExpression variableExpression:
                    var isSetVariable = flags.Any(e => e == CompilerFlag.UseSetLocalVariable);
                    if (variableExpression.Name is StringExpression variableNameExpression && !isSetVariable) {
                        switch (variableNameExpression.Value.ToUpper()) {
                            case "TRUE":
                                context.File.LoadBoolean(true, variableNameExpression.Position);
                                return;
                            case "FALSE":
                                context.File.LoadBoolean(false, variableNameExpression.Position);
                                return;
                            case "NULL":
                                context.File.LoadNull(variableNameExpression.Position);
                                return;
                            default:
                                Generate(context, variableNameExpression);
                                break;
                        }
                    } else {
                        Generate(context, variableExpression.Name);
                    }
                    context.File.OperationCode(isSetVariable ? OperationCode.STLOC : OperationCode.LDLOC, variableExpression.Position);
                    break;
            }
        }
    }
}