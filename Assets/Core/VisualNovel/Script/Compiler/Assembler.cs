using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler.Expressions;
using UnityEngine;

namespace Core.VisualNovel.Script.Compiler {
    public class Assembler {
        private ScopeExpression RootExpression { get; }
        private string Identifier { get; }

        public Assembler(ScopeExpression root, string identifier) {
            RootExpression = root;
            Identifier = identifier;
        }

        // VNS1 File
        /*   564E5331 | 32
            str_count | 32
          str_content | dynamic
          label_count | 32
        label_content | dynamic
          scene_count | 32
        scene_content | dynamic
         code_segment | dynamic */
        
        /// <summary>
        /// 生成程序码
        /// </summary>
        /// <returns></returns>
        public (byte[] Content, IReadOnlyDictionary<string, string> Translations) Assemble() {
            var context = new AssemblerContext();
            // 生成主程序段
            Assemble(context, RootExpression);
            context.File.OpCode(OpCodeType.RET);
            // 生成场景程序段
            for (var i = -1; ++i < context.Scenarios.Count;) {
                var description = context.Scenarios[i];
                description.Offset = context.File.Position;
                context.Scope = description.Scope;
                foreach (var parameter in description.Scenario.Parameters) {
                    if (!(parameter.Value is EmptyExpression)) {
                        var nextParameterCheck = $"{description.Label}P{context.NextLabelId}";
                        context.File.LoadNull();
                        Assemble(context, parameter.Name);
                        context.File.OpCode(OpCodeType.EQL);
                        context.File.OpCode(OpCodeType.BF_S);
                        context.File.String(nextParameterCheck);
                        Assemble(context, parameter.Value);
                        Assemble(context, parameter.Name);
                        context.File.OpCode(OpCodeType.STLOC);
                        context.File.CreateLabel(nextParameterCheck);
                    }
                }
                Assemble(context, description.Scenario.Body);
                context.File.OpCode(OpCodeType.RET);
                context.Scope = 0;
            }
            // 准备文件头
            var baseFile = context.File.Create();
            context.File = new AssembleFile();
            // 写入文件标志 (VisualNovelScript Version 1, assembly format)
            context.File.Number(0x564E5331);
            // 写入字符串常量数
            context.File.Number(baseFile.Strings.Count);
            // 写入字符串常量表
            foreach (var stringConstant in baseFile.Strings) {
                context.File.String(stringConstant);
            }
            // 写入跳转标签数
            context.File.Number(context.Scenarios.Count + baseFile.Labels.Count);
            // 写入跳转表
            foreach (var scenario in context.Scenarios) {
                context.File.Number(scenario.Offset);
                context.File.String(scenario.Label);
            }
            foreach (var label in baseFile.Labels) {
                context.File.Number(label.Value);
                context.File.String(label.Key);
            }
            // 写入场景数
            context.File.Number(context.Scenarios.Count);
            // 写入场景表
            foreach (var scenario in context.Scenarios) {
                context.File.String(scenario.Scenario.Name);
                context.File.String(scenario.Label);
            }
            // 复制程序段
            context.File.Array(baseFile.Content);
            baseFile.Content = context.File.Create().Content;
            // TODO 生成翻译文件
            return (baseFile.Content, baseFile.Translations);
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
                    if (binaryExpression.Operator == OperatorType.EqualsTo || binaryExpression.Operator == OperatorType.AddBy ||
                        binaryExpression.Operator == OperatorType.MinusBy || binaryExpression.Operator == OperatorType.MultiplyBy ||
                        binaryExpression.Operator == OperatorType.DivideBy) {
                        Assemble(context, binaryExpression.Left, CompilerFlag.UseSetLocalVariable);
                        if (!(binaryExpression.Left is VariableExpression)) {
                            context.File.OpCode(OpCodeType.STLOC);
                        }
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
                    foreach (var parameter in commandExpression.Parameters) {
                        Assemble(context, parameter);
                    }
                    context.File.LoadInteger(commandExpression.Parameters.Count);
                    Assemble(context, commandExpression.Target);
                    context.File.Call();
                    break;
                case ConditionExpression conditionExpression: // 协同处理ConditionContentExpression
                    var conditionEndLabel = $"{context.Scope}C{context.NextLabelId}";
                    var conditionNextLabel = $"{context.Scope}C{context.NextLabelId}";
                    foreach (var branch in conditionExpression.Contents) {
                        context.File.CreateLabel(conditionNextLabel);
                        conditionNextLabel = $"{context.Scope}C{context.NextLabelId}";
                        Assemble(context, branch.Condition);
                        context.File.OpCode(OpCodeType.BVAL);
                        context.File.OpCode(OpCodeType.BF_S);
                        context.File.String(conditionNextLabel);
                        Assemble(context, branch.Body);
                        context.File.OpCode(OpCodeType.BR_S);
                        context.File.String(conditionEndLabel);
                    }
                    context.File.CreateLabel(conditionEndLabel);
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
                    var loopStartLabel = $"{context.Scope}L{context.NextLabelId}";
                    var loopEndLabel = $"{context.Scope}L{context.NextLabelId}";
                    context.File.CreateLabel(loopStartLabel);
                    Assemble(context, loopExpression.Condition);
                    context.File.OpCode(OpCodeType.BVAL);
                    context.File.OpCode(OpCodeType.BF_S);
                    context.File.String(loopEndLabel);
                    Assemble(context, loopExpression.Body);
                    context.File.OpCode(OpCodeType.BR_S);
                    context.File.String(loopStartLabel);
                    context.File.CreateLabel(loopEndLabel);
                    break;
                case ParameterExpression parameterExpression:
                    Assemble(context, parameterExpression.Value);
                    Assemble(context, parameterExpression.Name);
                    break;
                case ReturnExpression returnExpression:
                    Assemble(context, returnExpression.Value);
                    context.File.OpCode(OpCodeType.RET);
                    break;
                case ScopeExpression scopeExpression: // 协同处理ScenarioExpression
                    context.File.OpCode(OpCodeType.SCOPE);
                    ++context.Scope;
                    foreach (var (item, i) in scopeExpression.Content.WithIndex()) {
                        Assemble(context, item);
                        if (item is ScenarioExpression scenarioExpression) {
                            var description = new ScenarioDescription {
                                Scenario = scenarioExpression,
                                Label = $"{context.Scope}S{scenarioExpression.Name}",
                                Scope = context.Scope
                            };
                            context.Scenarios.Add(description);
                            if (i == scopeExpression.Content.Count - 1) {
                                context.File.OpCode(OpCodeType.LDADDR);
                                context.File.String(description.Label);
                            }
                            continue;
                        }
                        if (i < scopeExpression.Content.Count - 1) {
                            context.File.Pop();
                        }
                    }
                    --context.Scope;
                    context.File.OpCode(OpCodeType.LEAVE);
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
                    var variableAssignMode = flags.Any(e => e == CompilerFlag.UseSetLocalVariable);
                    if (variableExpression.Name is StringExpression nameExpression) {
                        switch (nameExpression.Value.ToUpper()) {
                            case "TRUE":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadBoolean(true);
                                return;
                            case "FALSE":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadBoolean(false);
                                return;
                            case "NULL":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadNull();
                                return;
                            default:
                                Assemble(context, nameExpression);
                                break;
                        }
                    } else {
                        Assemble(context, variableExpression.Name);
                    }
                    context.File.OpCode(variableAssignMode ? OpCodeType.STLOC : OpCodeType.LDLOC);
                    break;
            }
        }
    }
}