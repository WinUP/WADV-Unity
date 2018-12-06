using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler.Expressions;
using Core.VisualNovel.Translation;
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
          str_segment | dynamic
          label_count | 32
        label_segment | dynamic
          scene_count | 32
        scene_segment | dynamic
       position_count | 32
     position_segment | dynamic
         code_segment | dynamic */
        
        /// <summary>
        /// 生成程序码
        /// </summary>
        /// <returns></returns>
        public (byte[] Content, string Translations) Assemble() {
            var context = new AssemblerContext();
            // 生成主程序段
            Assemble(context, RootExpression);
            context.File.OperationCode(OperationCode.RET, new CodePosition());
            // 生成场景程序段
            for (var i = -1; ++i < context.Scenarios.Count;) {
                var description = context.Scenarios[i];
                description.Offset = context.File.Position;
                context.Scope = description.Scope;
                foreach (var parameter in description.Function.Parameters) {
                    if (!(parameter.Value is EmptyExpression)) {
                        var nextParameterCheck = $"{description.Label}P{context.NextLabelId}";
                        context.File.LoadNull(parameter.Position);
                        Assemble(context, parameter.Name);
                        context.File.OperationCode(OperationCode.EQL, parameter.Position);
                        context.File.OperationCode(OperationCode.BF_S, parameter.Position);
                        context.File.DirectWrite(nextParameterCheck);
                        Assemble(context, parameter.Value);
                        Assemble(context, parameter.Name);
                        context.File.OperationCode(OperationCode.STLOC, parameter.Position);
                        context.File.CreateLabel(nextParameterCheck);
                    }
                }
                Assemble(context, description.Function.Body);
                context.File.OperationCode(OperationCode.RET, description.Function.Position);
                context.Scope = 0;
            }
            // 准备文件头
            var baseFile = context.File.CreateSegment();
            context.File = new AssembleFile();
            // 写入文件标志 (VisualNovelScript Version 1, assembly format)
            context.File.DirectWrite(0x564E5331);
            // 写入字符串常量数
            context.File.DirectWrite(baseFile.Strings.Count);
            // 写入字符串常量表
            foreach (var stringConstant in baseFile.Strings) {
                context.File.DirectWrite(stringConstant);
            }
            // 写入跳转标签数
            context.File.DirectWrite(context.Scenarios.Count + baseFile.Labels.Count);
            // 写入跳转表
            foreach (var scenario in context.Scenarios) {
                context.File.DirectWrite(scenario.Offset);
                context.File.DirectWrite(scenario.Label);
            }
            foreach (var label in baseFile.Labels) {
                context.File.DirectWrite(label.Value);
                context.File.DirectWrite(label.Key);
            }
            // 写入场景数
            context.File.DirectWrite(context.Scenarios.Count);
            // 写入场景表（即函数表）
            foreach (var scenario in context.Scenarios) {
                context.File.DirectWrite(scenario.Function.Name);
                context.File.DirectWrite(scenario.Label);
            }
            // 写入调试信息
            context.File.DirectWrite(baseFile.Positions.Count);
            foreach (var position in baseFile.Positions) {
                context.File.DirectWrite(position.Line);
                context.File.DirectWrite(position.Column);
            }
            // 复制程序段
            context.File.DirectWrite(baseFile.Code);
            baseFile.Code = context.File.CreateSegment().Code;
            // 更新默认翻译
            var existedTranslationContent = Resources.Load<TextAsset>($"{Identifier}_tr_default").text;
            ScriptTranslation targetTranslation;
            if (string.IsNullOrEmpty(existedTranslationContent)) {
                var translationContent = new ScriptTranslation(baseFile.Translations);
                targetTranslation = translationContent;
            } else {
                var existedTranslation = new ScriptTranslation(existedTranslationContent);
                existedTranslation.MergeWith(baseFile.Translations);
                targetTranslation = existedTranslation;
            }
            return (baseFile.Code, "// Translation file: default\n// Notice: one translation only allows one line, accept all escape characters but only \\n is needed \n\n" + targetTranslation.Pack());
        }

        private void Assemble(AssemblerContext context, Expression expression, params CompilerFlag[] flags) {
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
                    var conditionEndLabel = $"{context.Scope}C{context.NextLabelId}";
                    var conditionNextLabel = $"{context.Scope}C{context.NextLabelId}";
                    foreach (var branch in conditionExpression.Contents) {
                        context.File.CreateLabel(conditionNextLabel);
                        conditionNextLabel = $"{context.Scope}C{context.NextLabelId}";
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
                case DialogueExpression dialogueExpression:
                    context.File.LoadDialogue(dialogueExpression.Character, dialogueExpression.Content, dialogueExpression.Position);
                    break;
                case EmptyExpression emptyExpression:
                    context.File.LoadNull(emptyExpression.Position);
                    break;
                case FloatExpression floatExpression:
                    context.File.LoadFloat(floatExpression.Value, floatExpression.Position);
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
                    var loopStartLabel = $"{context.Scope}L{context.NextLabelId}";
                    var loopEndLabel = $"{context.Scope}L{context.NextLabelId}";
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
                case ScopeExpression scopeExpression: // 协同处理ScenarioExpression
                    context.File.OperationCode(OperationCode.SCOPE, scopeExpression.Position);
                    ++context.Scope;
                    foreach (var (item, i) in scopeExpression.Content.WithIndex()) {
                        Assemble(context, item);
                        if (item is FunctionExpression scenarioExpression) {
                            var description = new ScenarioDescription {
                                Function = scenarioExpression,
                                Label = $"{context.Scope}S{scenarioExpression.Name}",
                                Scope = context.Scope
                            };
                            context.Scenarios.Add(description);
                            if (i == scopeExpression.Content.Count - 1) {
                                context.File.OperationCode(OperationCode.LDADDR, scenarioExpression.Position);
                                context.File.DirectWrite(description.Label);
                            }
                            continue;
                        }
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
                    var variableAssignMode = flags.Any(e => e == CompilerFlag.UseSetLocalVariable);
                    if (variableExpression.Name is StringExpression nameExpression) {
                        switch (nameExpression.Value.ToUpper()) {
                            case "TRUE":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadBoolean(true, variableExpression.Position);
                                return;
                            case "FALSE":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadBoolean(false, variableExpression.Position);
                                return;
                            case "NULL":
                                if (variableAssignMode) {
                                    throw new UnassignedReferenceException("Cannot assign value to constant variable TRUE/FALSE/NULL");
                                }
                                context.File.LoadNull(variableExpression.Position);
                                return;
                            default:
                                Assemble(context, nameExpression);
                                break;
                        }
                    } else {
                        Assemble(context, variableExpression.Name);
                    }
                    context.File.OperationCode(variableAssignMode ? OperationCode.STLOC : OperationCode.LDLOC, variableExpression.Position);
                    break;
            }
        }
    }
}