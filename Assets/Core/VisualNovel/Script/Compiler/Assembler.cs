using System;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler.Expressions;
using Core.VisualNovel.Translation;
using UnityEngine;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// WADV VNS 字节码生成器
    /// </summary>
    public class Assembler {
        private ScopeExpression RootExpression { get; }
        private CodeIdentifier Identifier { get; }

        public Assembler(ScopeExpression root, CodeIdentifier identifier) {
            RootExpression = root;
            Identifier = identifier;
        }
        
        /// <summary>
        /// 生成字节码
        /// </summary>
        /// <returns></returns>
        public (byte[] Content, string Translations) Assemble() {
            var context = new AssemblerContext();
            // 生成主程序段
            Assemble(context, RootExpression);
            context.File.OperationCode(OperationCode.RET, new SourcePosition());
            // 生成各种段
            var segments = context.File.CreateSegments();
            context.File.Close();
            // 准备目标文件
            var targetFile = new ByteCodeWriter();
            // 写入文件标志 ("VisualNovelScript Version 1, assembly format"的CRC32)
            targetFile.DirectWrite(0x963EFE4A);
            // 写入源文件哈希用于跳过重复编译
            targetFile.DirectWrite(Identifier.Hash);
            // 写入各种段
            targetFile.DirectWrite(segments.Strings);
            targetFile.DirectWrite(segments.Labels);
            targetFile.DirectWrite(segments.Positions);
            targetFile.DirectWrite(segments.Code);
            // 更新默认翻译
            var existedTranslationContent = Resources.Load<TextAsset>($"{Identifier.Name}_tr_default")?.text;
            ScriptTranslation targetTranslation;
            if (string.IsNullOrEmpty(existedTranslationContent)) {
                var translationContent = segments.Translations;
                targetTranslation = translationContent;
            } else {
                var existedTranslation = new ScriptTranslation(existedTranslationContent);
                existedTranslation.MergeWith(segments.Translations);
                targetTranslation = existedTranslation;
            }
            context.File.Close();
            return (targetFile.CreateCodeSegment(), "// Translation file: default\n// Notice: one translation only allows one line, accept all escape characters but only \\n is needed \n\n" + targetTranslation.Pack());
        }

        private static void Assemble(AssemblerContext context, Expression expression, params CompilerFlag[] flags) {
            switch (expression) {
                case BinaryExpression binaryExpression:
                    Assemble(context, binaryExpression.Right);
                    switch (binaryExpression.Operator) {
                        case OperatorType.AddBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.ADD, binaryExpression.Position);
                            break;
                        case OperatorType.MinusBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.SUB, binaryExpression.Position);
                            break;
                        case OperatorType.MultiplyBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.MUL, binaryExpression.Position);
                            break;
                        case OperatorType.DivideBy:
                            Assemble(context, binaryExpression.Left);
                            context.File.OperationCode(OperationCode.DIV, binaryExpression.Position);
                            break;
                    }
                    if (binaryExpression.Operator == OperatorType.EqualsTo || binaryExpression.Operator == OperatorType.AddBy ||
                        binaryExpression.Operator == OperatorType.MinusBy || binaryExpression.Operator == OperatorType.MultiplyBy ||
                        binaryExpression.Operator == OperatorType.DivideBy) {
                        Assemble(context, binaryExpression.Left, CompilerFlag.UseSetLocalVariable);
                        if (!(binaryExpression.Left is VariableExpression)) {
                            context.File.OperationCode(OperationCode.STLOC, binaryExpression.Left.Position);
                        }
                    } else {
                        Assemble(context, binaryExpression.Left);
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
                        }
                    }
                    break;
                case CallExpression commandExpression:
                    foreach (var parameter in commandExpression.Parameters) {
                        Assemble(context, parameter);
                    }
                    context.File.LoadInteger(commandExpression.Parameters.Count, commandExpression.Position);
                    Assemble(context, commandExpression.Target);
                    context.File.Call(commandExpression.Position);
                    break;
                case ConditionExpression conditionExpression: // 协同处理ConditionContentExpression
                    var conditionEndLabel = context.NextLabelId;
                    var conditionNextLabel = context.NextLabelId;
                    foreach (var branch in conditionExpression.Contents) {
                        context.File.CreateLabel(conditionNextLabel);
                        conditionNextLabel = context.NextLabelId;
                        Assemble(context, branch.Condition);
                        context.File.OperationCode(OperationCode.BVAL, branch.Position);
                        context.File.OperationCode(OperationCode.BF_S, branch.Position);
                        context.File.DirectWrite(conditionNextLabel);
                        Assemble(context, branch.Body);
                        context.File.OperationCode(OperationCode.BR_S, branch.Body.Position);
                        context.File.DirectWrite(conditionEndLabel);
                    }
                    context.File.CreateLabel(conditionEndLabel);
                    break;
                case ConstantExpression constantExpression:
                    if (flags.Any(e => e == CompilerFlag.UseSetLocalVariable)) {
                        throw new NotSupportedException("Cannot assign value to constant variable");
                    }
                    if (constantExpression.Name is StringExpression constantNameExpression) {
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
                                Assemble(context, constantNameExpression);
                                break;
                        }
                    } else {
                        Assemble(context, constantExpression.Name);
                    }
                    context.File.OperationCode(OperationCode.LDCONS, constantExpression.Position);
                    break;
                case DialogueExpression dialogueExpression:
                    context.File.LoadDialogue(dialogueExpression.Character, dialogueExpression.Content, dialogueExpression.Position);
                    break;
                case EmptyExpression emptyExpression:
                    context.File.LoadNull(emptyExpression.Position);
                    break;
                case FloatExpression floatExpression:
                    context.File.LoadFloat(floatExpression.Value, floatExpression.Position);
                    break;
                case FunctionCallExpression functionCallExpression:
                    foreach (var parameter in functionCallExpression.Parameters) {
                        Assemble(context, parameter);
                    }
                    context.File.LoadInteger(functionCallExpression.Parameters.Count, functionCallExpression.Position);
                    Assemble(context, functionCallExpression.Target);
                    context.File.Func(functionCallExpression.Position);
                    break;
                case FunctionExpression functionExpression:
                    var functionStart = context.NextLabelId;
                    var functionEnd = context.NextLabelId;
                    // 场景表现为一个声明在当前作用域内的变量
                    context.File.OperationCode(OperationCode.LDADDR, functionExpression.Position);
                    context.File.DirectWrite(functionStart);
                    context.File.LoadString(functionExpression.Name, functionExpression.Position);
                    context.File.OperationCode(OperationCode.STLOC, functionExpression.Position);
                    context.File.OperationCode(OperationCode.POP, functionExpression.Position);
                    // 令外部代码执行时跳过函数部分
                    context.File.OperationCode(OperationCode.BR_S, SourcePosition.UnavailablePosition);
                    context.File.DirectWrite(functionEnd);
                    // 开始函数生成
                    context.File.CreateLabel(functionStart);
                    // 默认值赋值
                    foreach (var parameter in functionExpression.Parameters) {
                        if (!(parameter.Value is EmptyExpression)) {
                            var nextParameterCheck = context.NextLabelId;
                            context.File.LoadNull(parameter.Position);
                            Assemble(context, parameter.Name);
                            context.File.OperationCode(OperationCode.EQL, parameter.Position);
                            context.File.OperationCode(OperationCode.BF_S, parameter.Position);
                            context.File.DirectWrite(nextParameterCheck);
                            Assemble(context, parameter.Value);
                            Assemble(context, parameter.Name);
                            context.File.OperationCode(OperationCode.STLOC, parameter.Position);
                            context.File.Pop(parameter.Position);
                            context.File.CreateLabel(nextParameterCheck);
                        }
                    }
                    // 生成函数体
                    Assemble(context, functionExpression.Body);
                    // 结束函数生成
                    context.File.OperationCode(OperationCode.RET, SourcePosition.UnavailablePosition);
                    context.File.CreateLabel(functionEnd);
                    break;
                case IntegerExpression integerExpression:
                    context.File.LoadInteger(integerExpression.Value, integerExpression.Position);
                    break;
                case LanguageExpression languageExpression:
                    context.File.Language(languageExpression.Language, languageExpression.Position);
                    break;
                case LogicNotExpression logicNotExpression:
                    Assemble(context, logicNotExpression.Content);
                    context.File.OperationCode(OperationCode.NOT, logicNotExpression.Position);
                    break;
                case LoopExpression loopExpression:
                    var loopStartLabel = context.NextLabelId;
                    var loopEndLabel = context.NextLabelId;
                    context.File.CreateLabel(loopStartLabel);
                    Assemble(context, loopExpression.Condition);
                    context.File.OperationCode(OperationCode.BVAL, loopExpression.Position);
                    context.File.OperationCode(OperationCode.BF_S, loopExpression.Condition.Position);
                    context.File.DirectWrite(loopEndLabel);
                    Assemble(context, loopExpression.Body);
                    context.File.OperationCode(OperationCode.BR_S, loopExpression.Body.Position);
                    context.File.DirectWrite(loopStartLabel);
                    context.File.CreateLabel(loopEndLabel);
                    break;
                case ParameterExpression parameterExpression:
                    Assemble(context, parameterExpression.Value);
                    Assemble(context, parameterExpression.Name);
                    break;
                case ReturnExpression returnExpression:
                    Assemble(context, returnExpression.Value);
                    context.File.OperationCode(OperationCode.RET, returnExpression.Position);
                    break;
                case ScopeExpression scopeExpression:
                    context.File.OperationCode(OperationCode.SCOPE, scopeExpression.Position);
                    ++context.Scope;
                    foreach (var (item, i) in scopeExpression.Content.WithIndex()) {
                        Assemble(context, item);
                        if (i < scopeExpression.Content.Count - 1) {
                            context.File.Pop(item.Position);
                        }
                    }
                    --context.Scope;
                    context.File.OperationCode(OperationCode.LEAVE, scopeExpression.Position);
                    break;
                case StringExpression stringExpression:
                    if (stringExpression.Translatable) {
                        context.File.LoadTranslatableString(stringExpression.Value, stringExpression.Position);
                    } else {
                        context.File.LoadString(stringExpression.Value, stringExpression.Position);
                    }
                    break;
                case ToBooleanExpression toBooleanExpression:
                    Assemble(context, toBooleanExpression.Value);
                    context.File.OperationCode(OperationCode.BVAL, toBooleanExpression.Position);
                    break;
                case VariableExpression variableExpression:
                    Assemble(context, variableExpression.Name);
                    context.File.OperationCode(flags.Any(e => e == CompilerFlag.UseSetLocalVariable) ? OperationCode.STLOC : OperationCode.LDLOC, variableExpression.Position);
                    break;
            }
        }
    }
}