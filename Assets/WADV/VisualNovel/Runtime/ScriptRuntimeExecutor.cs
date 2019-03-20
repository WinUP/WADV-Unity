using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Compiler.Expressions;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovel.Runtime.Utilities.Object;

namespace WADV.VisualNovel.Runtime {
    public partial class ScriptRuntime {
        /// <summary>
        /// 从当前代码段偏移位置开始执行脚本
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteScript() {
            if (Script == null) {
                throw new NotSupportedException("Unable to execute bytecode: no active script detected");
            }
            if (_callStack.Count == 0) {
                _callStack.Push(Script);
            }
            while (await ExecuteSingleLine()) {
                if (_stopRequest == null) continue;
                _stopRequest.Complete();
                _stopRequest = null;
                break;
            }
            _callStack.Clear();
        }

        private async Task<bool> ExecuteSingleLine() {
            if (_callStack.Count == 0) return false;
            _callStack.Last.Offset = Script.CurrentPosition;
            var code = Script.ReadOperationCode();
            if (code == null) return false;
            switch (code) {
                case OperationCode.LDC_I4_0:
                case OperationCode.LDC_I4_1:
                case OperationCode.LDC_I4_2:
                case OperationCode.LDC_I4_3:
                case OperationCode.LDC_I4_4:
                case OperationCode.LDC_I4_5:
                case OperationCode.LDC_I4_6:
                case OperationCode.LDC_I4_7:
                case OperationCode.LDC_I4_8:
                    MemoryStack.Push(new IntegerValue {value = (byte) code - (byte) OperationCode.LDC_I4_0});
                    break;
                case OperationCode.LDC_I4:
                    MemoryStack.Push(new IntegerValue {value = Script.ReadInteger()});
                    break;
                case OperationCode.LDC_R4_0:
                case OperationCode.LDC_R4_025:
                case OperationCode.LDC_R4_05:
                case OperationCode.LDC_R4_075:
                case OperationCode.LDC_R4_1:
                case OperationCode.LDC_R4_125:
                case OperationCode.LDC_R4_15:
                case OperationCode.LDC_R4_175:
                case OperationCode.LDC_R4_2:
                case OperationCode.LDC_R4_225:
                case OperationCode.LDC_R4_25:
                case OperationCode.LDC_R4_275:
                case OperationCode.LDC_R4_3:
                case OperationCode.LDC_R4_325:
                case OperationCode.LDC_R4_35:
                case OperationCode.LDC_R4_375:
                case OperationCode.LDC_R4_4:
                case OperationCode.LDC_R4_425:
                case OperationCode.LDC_R4_45:
                case OperationCode.LDC_R4_475:
                case OperationCode.LDC_R4_5:
                case OperationCode.LDC_R4_525:
                case OperationCode.LDC_R4_55:
                case OperationCode.LDC_R4_575:
                    MemoryStack.Push(new FloatValue {value = ((byte) code - (byte) OperationCode.LDC_R4_0) * 0.25F});
                    break;
                case OperationCode.LDC_R4:
                    MemoryStack.Push(new FloatValue {value = Script.ReadFloat()});
                    break;
                case OperationCode.LDSTR:
                    MemoryStack.Push(new StringValue {value = Script.ReadStringConstant()});
                    break;
                case OperationCode.LDENTRY:
                    LoadEntrance();
                    break;
                case OperationCode.LDSTT:
                    LoadTranslate();
                    break;
                case OperationCode.LDNUL:
                    LoadNull();
                    break;
                case OperationCode.LDLOC:
                    LoadVariable(VariableSearchMode.All);
                    break;
                case OperationCode.LDCON:
                    LoadVariable(VariableSearchMode.OnlyConstant);
                    break;
                case OperationCode.LDT:
                    MemoryStack.Push(new BooleanValue {value = true});
                    break;
                case OperationCode.LDF:
                    MemoryStack.Push(new BooleanValue {value = false});
                    break;
                case OperationCode.CALL:
                    await CreatePluginCall();
                    break;
                case OperationCode.POP:
                    MemoryStack.Pop();
                    break;
                case OperationCode.DIALOGUE:
                    await CreateDialogue();
                    break;
                case OperationCode.BVAL:
                    CreateToBoolean();
                    break;
                case OperationCode.ADD:
                    CreateBinaryOperation(OperatorType.Add);
                    break;
                case OperationCode.SUB:
                    CreateBinaryOperation(OperatorType.Minus);
                    break;
                case OperationCode.MUL:
                    CreateBinaryOperation(OperatorType.Multiply);
                    break;
                case OperationCode.DIV:
                    CreateBinaryOperation(OperatorType.Divide);
                    break;
                case OperationCode.NEGATIVE:
                    CreateToNegative();
                    break;
                case OperationCode.EQL:
                    CreateBinaryOperation(OperatorType.EqualsTo);
                    break;
                case OperationCode.CGE:
                    CreateBinaryOperation(OperatorType.NotLessThan);
                    break;
                case OperationCode.CGT:
                    CreateBinaryOperation(OperatorType.GreaterThan);
                    break;
                case OperationCode.CLE:
                    CreateBinaryOperation(OperatorType.NotGreaterThan);
                    break;
                case OperationCode.CLT:
                    CreateBinaryOperation(OperatorType.LesserThan);
                    break;
                case OperationCode.STLOC:
                    SetVariable(VariableSearchMode.All);
                    break;
                case OperationCode.STCON:
                    SetVariable(VariableSearchMode.OnlyConstant);
                    break;
                case OperationCode.STMEM:
                    SetMemory();
                    break;
                case OperationCode.PICK:
                    CreateBinaryOperation(OperatorType.PickChild);
                    break;
                case OperationCode.SCOPE:
                    CreateScope();
                    break;
                case OperationCode.LEAVE:
                    LeaveScope();
                    break;
                case OperationCode.RET:
                    await ReturnToPreviousScript();
                    if (_callStack.Count == 0) return false;
                    break;
                case OperationCode.FUNC:
                    await CreateFunctionCall();
                    break;
                case OperationCode.BF:
                    var condition = MemoryStack.Pop();
                    if (!(condition is NullValue) && condition is IBooleanConverter booleanConverter && booleanConverter.ConvertToBoolean(ActiveLanguage)) {
                        Move();
                    }
                    break;
                case OperationCode.BR:
                    Move();
                    break;
                case OperationCode.LOAD:
                    await Load();
                    break;
                case OperationCode.EXP:
                    Export();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        private void Move() {
            Script.MoveTo(Script.ReadLabelOffset());
        }

        private void SetVariable(VariableSearchMode mode) {
            var name = PopString();
            var value = MemoryStack.Pop();
            value = value is ReferenceValue referenceValue ? referenceValue.ReferenceTarget : value;
            if (string.IsNullOrEmpty(name)) throw new RuntimeException(_callStack, $"Unable to set variable: expected name {name} is not string value");
            var variable = ActiveScope?.FindVariableAndScope(name, true, mode);
            if (name.Equals("true", StringComparison.InvariantCultureIgnoreCase) || name.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                throw new RuntimeException(_callStack, "Unable to set variable: system value true/false is readonly");
            if (variable.HasValue) {
                try {
                    if (value == null || value is NullValue) {
                        variable.Value.Scope.LocalVariables.Remove(name);
                    } else {
                        variable.Value.Target.ReferenceTarget = value;
                    }
                } catch (Exception ex) {
                    throw new RuntimeException(_callStack, ex);
                }
            } else if (value != null && !(value is NullValue)) {
                ActiveScope?.LocalVariables.Add(name, new ReferenceValue {ReferenceTarget = value, IsConstant = mode == VariableSearchMode.OnlyConstant});
            }
            MemoryStack.Push(value);
        }

        private void SetMemory() {
            var target = MemoryStack.Pop();
            var value = MemoryStack.Pop();
            if (target is ReferenceValue referenceTarget) {
                value = value is ReferenceValue referenceValue ? referenceValue.ReferenceTarget : value;
                referenceTarget.ReferenceTarget = value;
            } else {
                throw new RuntimeException(_callStack, $"Unable to set memory: expected target {target} is not reference/variable value");
            }
            MemoryStack.Push(value);
        }
        
        private void LoadEntrance() {
            MemoryStack.Push(new ScopeValue {
                scriptId = Script.Header.Id,
                entrance = Script.ReadLabelOffset(),
                parentScope = ActiveScope?.Duplicate() as ScopeValue
            });
        }

        private void LoadTranslate() {
            MemoryStack.Push(new TranslatableValue {translationId = Script.ReadUInt32(), scriptId = Script.Header.Id});
        }

        private void LoadNull() {
            MemoryStack.Push(new NullValue());
        }

        private void LoadVariable(VariableSearchMode mode) {
            string name;
            if (MemoryStack.Peek() is ReferenceValue referenceValue) {
                name = PopString(referenceValue.ReferenceTarget);
                MemoryStack.Pop();
            } else {
                name = PopString();
            }
            if (name.Equals("true", StringComparison.InvariantCultureIgnoreCase)) {
                MemoryStack.Push(new BooleanValue {value = true});
            } else if (name.Equals("false", StringComparison.InvariantCultureIgnoreCase)) {
                MemoryStack.Push(new BooleanValue {value = false});
            } else {
                var target = ActiveScope?.FindVariable(name, true, mode);
                if (target == null) {
                    LoadNull();
                } else {
                    MemoryStack.Push(target.ReferenceTarget);
                }
            }
        }
        
        private string PopString(SerializableValue target = null) {
            var rawValue = target ?? MemoryStack.Pop();
            switch (rawValue) {
                case IStringConverter stringConverter:
                    return stringConverter.ConvertToString(ActiveLanguage);
                default:
                    return null;
            }
        }

        private void CreateScope() {
            var newScope = new ScopeValue {scriptId = Script.Header.Id, entrance = Script.CurrentPosition};
            if (ActiveScope != null) {
                ActiveScope.parentScope = newScope;
            }
            ActiveScope = newScope;
        }

        private async Task CreateFunctionCall() {
            var functionName = MemoryStack.Pop();
            while (functionName is ReferenceValue referenceFunction) {
                functionName = referenceFunction.ReferenceTarget;
            }
            ScopeValue functionBody;
            if (functionName is ScopeValue directBody) {
                functionBody = directBody;
            } else {
                if (!(functionName is IStringConverter stringConverter)) throw new RuntimeException(_callStack, $"Unable to call scene: name {functionName} is not string value");
                var function = ActiveScope?.FindVariable(stringConverter.ConvertToString(ActiveLanguage), true, VariableSearchMode.All);
                if (function == null || !(function.ReferenceTarget is ScopeValue scopeValue)) {
                    throw new RuntimeException(_callStack, $"Unable to call function: expected function {stringConverter.ConvertToString(ActiveLanguage)} not existed in current scope");
                }
                functionBody = scopeValue;
            }
            // 生成形参
            var paramCount = ((IntegerValue) MemoryStack.Pop()).value;
            for (var i = -1; ++i < paramCount;) {
                var paramName = PopString();
                if (string.IsNullOrEmpty(paramName)) throw new RuntimeException(_callStack, $"Unable to call {functionName}: expected parameter name {paramName} is not string value");
                functionBody.LocalVariables.Add(paramName, new ReferenceValue {ReferenceTarget = MemoryStack.Pop()});
            }
            // 切换作用域
            _historyScope.Push(ActiveScope);
            ActiveScope = functionBody;
            // 重定向执行位置
            Script = await ScriptFile.Load(ActiveScope.scriptId);
            Script.MoveTo(ActiveScope.entrance);
            await Script.UseTranslation(ActiveLanguage);
            _callStack.Push(Script);
        }

        private void LeaveScope() {
            if (ActiveScope == null) throw new RuntimeException(_callStack, "Unable to leave scope: No scope activated");
            // 清空局部作用域
            ActiveScope?.LocalVariables.Clear();
            ActiveScope = ActiveScope.parentScope;
        }

        private async Task ReturnToPreviousScript() {
            // 切换历史作用域
            ActiveScope = _historyScope.Pop();
            // 重定向执行位置
            _callStack.Pop();
            var pointer = _callStack.Last;
            Script = ScriptFile.LoadSync(pointer.ScriptId);
            Script.MoveTo(pointer.Offset);
            await Script.UseTranslation(ActiveLanguage);
            // 跳过刚刚执行完的跳转指令
            Script.ReadOperationCode();
        }
        
        private IVisualNovelPlugin FindPlugin() {
            var pluginName = PopString();
            if (string.IsNullOrEmpty(pluginName)) throw new RuntimeException(_callStack, "Unable to find plugin: expected string name");
            var plugin = PluginManager.Find(pluginName);
            if (plugin == null) throw new RuntimeException(_callStack, $"Unable to find plugin: expected plugin {pluginName} not existed");
            return plugin;
        }
        
        private async Task CreatePluginCall() {
            var plugin = FindPlugin();
            var parameterCount = ((IIntegerConverter) MemoryStack.Pop()).ConvertToInteger(ActiveLanguage);
            var context = PluginExecuteContext.Create(this);
            for (var i = -1; ++i < parameterCount;) {
                context.Parameters.Add(MemoryStack.Pop() ?? new NullValue(), MemoryStack.Pop() ?? new NullValue());
            }
            try {
                MemoryStack.Push(await plugin.Execute(context) ?? new NullValue());
            } catch (Exception ex) {
                throw new RuntimeException(_callStack, ex);
            }
        }

        private async Task CreateDialogue() {
            var plugin = PluginManager.Find("Dialogue");
            if (plugin == null) {
                throw new RuntimeException(_callStack, "Unable to create dialogue: no dialogue plugin registered");
            }
            MemoryStack.Push(await plugin.Execute(PluginExecuteContext.Create(this, new Dictionary<SerializableValue, SerializableValue> {
                {new StringValue {value = "Character"}, MemoryStack.Pop()},
                {new StringValue {value = "Content"}, MemoryStack.Pop()}
            })) ?? new NullValue());
        }

        private void CreateToNegative() {
            var rawValue = MemoryStack.Pop();
            if (rawValue is INegativeOperator negativeOperator) {
                try {
                    MemoryStack.Push(negativeOperator.ToNegative(ActiveLanguage));
                } catch (Exception ex) {
                    throw new RuntimeException(_callStack, ex);
                }
            } else {
                throw new RuntimeException(_callStack, $"Unable to get negative value of {rawValue}: target expression has no negative operator implementation");
            }
        }

        private void CreateToBoolean() {
            var rawValue = MemoryStack.Pop();
            try {
                var result = new BooleanValue {value = rawValue is IBooleanConverter booleanValue ? booleanValue.ConvertToBoolean(ActiveLanguage) : rawValue != null};
                MemoryStack.Push(result);
            } catch (Exception ex) {
                throw new RuntimeException(_callStack, ex);
            }
        }

        private void CreateBinaryOperation(OperatorType operatorType) {
            var valueLeft = MemoryStack.Pop();
            var valueRight = MemoryStack.Pop();
            switch (operatorType) {
                case OperatorType.PickChild:
                    if (valueRight is IStringConverter rightStringConverter && rightStringConverter.ConvertToString(ActiveLanguage) == "Duplicate") {
                        // 复制(Duplicate)由于统一实现的困难暂且由运行环境处理
                        MemoryStack.Push(valueLeft.Duplicate());
                    } else if (valueLeft is IPickChildOperator leftPick) {
                        try {
                            MemoryStack.Push(leftPick.PickChild(valueRight, ActiveLanguage));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to pick {valueRight} from {valueLeft}: parent expression has no pick child operator implementation");
                    }
                    break;
                case OperatorType.Add:
                    if (valueLeft is IAddOperator leftAdd) {
                        try {
                            MemoryStack.Push(leftAdd.AddWith(valueRight, ActiveLanguage));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to add {valueLeft} and {valueRight}: left expression has no add operator implementation");
                    }
                    break;
                case OperatorType.Minus:
                    if (valueLeft is ISubtractOperator leftSubtract) {
                        try {
                            MemoryStack.Push(leftSubtract.SubtractWith(valueRight, ActiveLanguage));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to subtract {valueLeft} and {valueRight}: left expression has no subtract operator implementation");
                    }
                    break;
                case OperatorType.Multiply:
                    if (valueLeft is IMultiplyOperator leftMultiply) {
                        try {
                            MemoryStack.Push(leftMultiply.MultiplyWith(valueRight, ActiveLanguage));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to multiply {valueLeft} and {valueRight}: left expression has no multiply operator implementation");
                    }
                    break;
                case OperatorType.Divide:
                    if (valueLeft is IDivideOperator leftDivide) {
                        try {
                            MemoryStack.Push(leftDivide.DivideWith(valueRight, ActiveLanguage));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to divide {valueLeft} and {valueRight}: left expression has no divide operator implementation");
                    }
                    break;
                case OperatorType.GreaterThan:
                case OperatorType.LesserThan:
                case OperatorType.NotLessThan:
                case OperatorType.NotGreaterThan:
                    if (valueLeft is ICompareOperator leftCompare) {
                        try {
                            var compareResult = leftCompare.CompareWith(valueRight, ActiveLanguage);
                            if (compareResult < 0) {
                                MemoryStack.Push(new BooleanValue {value = operatorType == OperatorType.LesserThan || operatorType == OperatorType.NotGreaterThan});
                            } else if (compareResult == 0) {
                                MemoryStack.Push(new BooleanValue {value = operatorType == OperatorType.NotLessThan || operatorType == OperatorType.NotGreaterThan});
                            } else {
                                MemoryStack.Push(new BooleanValue {value = operatorType == OperatorType.GreaterThan || operatorType == OperatorType.NotLessThan});
                            }
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to compare {valueLeft} and {valueRight}: left expression has no compare operator implementation");
                    }
                    break;
                case OperatorType.LogicEqualsTo:
                case OperatorType.LogicNotEqualsTo:
                    if (valueLeft is IEqualOperator leftEqual) {
                        try {
                            var result = new BooleanValue {value = leftEqual.EqualsWith(valueRight, ActiveLanguage)};
                            if (operatorType == OperatorType.LogicNotEqualsTo) {
                                result.value = !result.value;
                            }
                            MemoryStack.Push(result);
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to compare {valueLeft} and {valueRight}: left expression has no equal operator implementation");
                    }
                    break;
            }
        }

        private void Export() {
            var exportName = PopString();
            if (string.IsNullOrEmpty(exportName)) throw new RuntimeException(_callStack, $"Unable to export value: export name {exportName} is not string value");
            if (Exported.ContainsKey(exportName)) {
                Exported.Remove(exportName);
            }
            Exported.Add(exportName, MemoryStack.Pop());
        }

        private async Task Load() {
            ScriptRuntime runtime;
            if (_loadingScript == null) {
                var scriptId = PopString();
                if (string.IsNullOrEmpty(scriptId)) throw new RuntimeException(_callStack, $"Unable to load script: script id {scriptId} is not string value");
                runtime = new ScriptRuntime(scriptId, _callStack) {ActiveLanguage = ActiveLanguage};
                _loadingScript = runtime;
            } else {
                runtime = _loadingScript;
            }
            await runtime.ExecuteScript();
            _loadingScript = null;
            var result = new ObjectValue();
            foreach (var (name, value) in runtime.Exported) {
                result.Add(new StringValue {value = name}, value);
            }
            MemoryStack.Push(result);
        }
    }
}