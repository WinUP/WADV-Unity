using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovel.Compiler;
using WADV.VisualNovel.Compiler.Expressions;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;

// ! 为求效率，VNB运行环境在文件头正确的情况下假设文件格式绝对正确，只会做运行时数据检查，不会进行任何格式检查

namespace WADV.VisualNovel.Runtime {
    /// <summary>
    /// 脚本运行环境
    /// </summary>
    public class ScriptRuntime {
        /// <summary>
        /// 获取正在执行的脚本文件
        /// </summary>
        public ScriptFile Script { get; private set; }
        
        /// <summary>
        /// 获取当前激活的顶层作用域
        /// </summary>
        [CanBeNull]
        public ScopeValue ActiveScope { get; private set; }

        /// <summary>
        /// 获取当前内存堆栈
        /// </summary>
        public Stack<SerializableValue> MemoryStack { get; } = new Stack<SerializableValue>();
        
        /// <summary>
        /// 获取或修改脚本导出的数据
        /// <para>在脚本尚未执行结束的情况下，导出数据可能不完整</para>
        /// </summary>
        public Dictionary<string, SerializableValue> Exported { get; } = new Dictionary<string, SerializableValue>();

        /// <summary>
        /// 获取或设置当前激活的语言
        /// </summary>
        public string ActiveLanguage {
            get => _activeLanguage;
            set {
                if (_activeLanguage == value) return;
                if (!TranslationManager.CheckLanguageName(value)) throw new RuntimeException(_callStack, $"Unable to change language: {value} is not legal language name");
                var message = MessageService.Process(new Message<ChangeLanguageIntent>(new ChangeLanguageIntent {Runtime = this, NewLanguage = value}, CoreConstant.Mask, CoreConstant.PrepareLanguageChange));
                switch (message) {
                    case Message<ChangeLanguageIntent> result:
                        _activeLanguage = result.Content.NewLanguage;
                        if (!TranslationManager.CheckLanguageName(_activeLanguage)) throw new RuntimeException(_callStack, $"Unable to change language: {_activeLanguage} is not legal language name");
                        break;
                    default:
                        throw new RuntimeException(_callStack, $"Unable to change language: Message was modified to non-string type during broadcast");
                }
                Script.UseTranslation(_activeLanguage).Wait();
                MessageService.Process(Message<ChangeLanguageIntent>.Create(new ChangeLanguageIntent {Runtime = this, NewLanguage = _activeLanguage}, CoreConstant.Mask, CoreConstant.LanguageChange));
            }
        }
        
        private string _activeLanguage = TranslationManager.DefaultLanguage;
        private readonly CallStack _callStack = new CallStack();
        private readonly Stack<ScopeValue> _historyScope = new Stack<ScopeValue>();
        [CanBeNull] private MainThreadPlaceholder _stopRequest;

        public ScriptRuntime(ScriptFile script) {
            Script = script ?? throw new ArgumentException("Unable to load script: expected script is not existed", nameof(script));
            Script.UseTranslation(ActiveLanguage).Wait();
        }

        public ScriptRuntime(string scriptId) : this(ScriptFile.LoadSync(scriptId)) { }

        public ScriptRuntime(string scriptId, IEnumerable<CallStack.StackItem> initialCallStack) : this(ScriptFile.LoadSync(scriptId), initialCallStack) { }
        
        private ScriptRuntime(ScriptFile script, IEnumerable<CallStack.StackItem> initialCallStack) : this(script) {
            _callStack.Push(initialCallStack);
        }

        private ScriptRuntime(Stack<SerializableValue> memoryStack, Dictionary<string, SerializableValue> exported, CallStack callStack, Stack<ScopeValue> historyScope) {
            MemoryStack = memoryStack;
            Exported = exported;
            _callStack = callStack;
            _historyScope = historyScope;
        }

        /// <summary>
        /// 将二进制数组转换为脚本运行环境
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        [CanBeNull]
        public static ScriptRuntime LoadFrom(byte[] data) {
            var deserializer = new BinaryFormatter();
            var record = deserializer.Deserialize(new MemoryStream(data)) as RuntimeDump;
            if (record == null) return null;
            var target = new ScriptRuntime(record.Memory, record.Exported, record.CallStack, record.HistoryScope);
            if (record.Scope != null) {
                target.ActiveScope = record.Scope;
                target.Script = ScriptFile.LoadSync(target.ActiveScope.ScriptId);
                target.Script.MoveTo(record.Offset);
                target.Script.UseTranslation(target.ActiveLanguage).Wait();
            }
            target.ActiveLanguage = record.Language;
            return target;
        }

        /// <summary>
        /// 将脚本运行环境转换为二进制数组
        /// </summary>
        /// <returns></returns>
        public byte[] Dump() {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, new RuntimeDump {
                Offset = Script.CurrentPosition,
                Scope = ActiveScope,
                Memory = MemoryStack,
                Exported = Exported,
                Language = ActiveLanguage,
                CallStack = _callStack,
                HistoryScope = _historyScope
            });
            return stream.ToArray();
        }

        /// <summary>
        /// 等待当前字节码执行完成后停止脚本执行
        /// </summary>
        /// <returns></returns>
        public async Task StopRunning() {
            if (_stopRequest == null) {
                _stopRequest = new MainThreadPlaceholder();
            }
            await _stopRequest;
        }
        
        /// <summary>
        /// 从当前代码段偏移位置开始执行脚本
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteScript() {
            if (Script == null) {
                throw new NotSupportedException("Unable to execute bytecode: no active script detected");
            }
            _callStack.Clear();
            _callStack.Push(Script);
            while (await ExecuteSingleLine()) {
                if (_stopRequest == null) continue;
                _stopRequest.Complete();
                _stopRequest = null;
                break;
            }
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
                    MemoryStack.Push(new IntegerValue {Value = (byte) code - (byte) OperationCode.LDC_I4_0});
                    break;
                case OperationCode.LDC_I4:
                    MemoryStack.Push(new IntegerValue {Value = Script.ReadInteger()});
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
                    MemoryStack.Push(new FloatValue {Value = ((byte) code - (byte) OperationCode.LDC_R4_0) * 0.25F});
                    break;
                case OperationCode.LDC_R4:
                    MemoryStack.Push(new FloatValue {Value = Script.ReadFloat()});
                    break;
                case OperationCode.LDSTR:
                    MemoryStack.Push(new StringValue {Value = Script.ReadStringConstant()});
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
                    MemoryStack.Push(new BooleanValue {Value = true});
                    break;
                case OperationCode.LDF:
                    MemoryStack.Push(new BooleanValue {Value = false});
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
                case OperationCode.NOT:
                    CreateToBoolean(true);
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
                    if (!(condition is NullValue) && condition is IBooleanConverter booleanConverter && booleanConverter.ConvertToBoolean()) {
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
            value = value is ReferenceValue referenceValue ? referenceValue.Value : value;
            if (string.IsNullOrEmpty(name)) throw new RuntimeException(_callStack, $"Unable to set variable: expected name {name} is not string value");
            var variable = ActiveScope?.FindVariableAndScope(name, true, mode);
            if (name.Equals("true", StringComparison.InvariantCultureIgnoreCase) || name.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                throw new RuntimeException(_callStack, "Unable to set variable: system value true/false is readonly");
            if (variable.HasValue) {
                try {
                    if (value == null || value is NullValue) {
                        variable.Value.Scope.LocalVariables.Remove(name);
                    } else {
                        variable.Value.Target.Value = value;
                    }
                } catch (Exception ex) {
                    throw new RuntimeException(_callStack, ex);
                }
            } else if (value != null && !(value is NullValue)) {
                ActiveScope?.LocalVariables.Add(name, new ReferenceValue {Value = value, IsConstant = mode == VariableSearchMode.OnlyConstant});
            }
            MemoryStack.Push(value);
        }

        private void SetMemory() {
            var target = MemoryStack.Pop();
            var value = MemoryStack.Pop();
            if (target is ReferenceValue referenceTarget) {
                value = value is ReferenceValue referenceValue ? referenceValue.Value : value;
                referenceTarget.Value = value;
            } else {
                throw new RuntimeException(_callStack, $"Unable to set memory: expected target {target} is not reference/variable value");
            }
            MemoryStack.Push(value);
        }
        
        private void LoadEntrance() {
            MemoryStack.Push(new ScopeValue {
                ScriptId = Script.Header.Id,
                Entrance = Script.ReadLabelOffset(),
                ParentScope = ActiveScope?.Duplicate() as ScopeValue
            });
        }

        private void LoadTranslate() {
            MemoryStack.Push(new TranslatableValue {TranslationId = Script.ReadUInt32(), ScriptId = Script.Header.Id});
        }

        private void LoadNull() {
            MemoryStack.Push(new NullValue());
        }

        private void LoadVariable(VariableSearchMode mode) {
            string name;
            if (MemoryStack.Peek() is ReferenceValue referenceValue) {
                name = PopString(referenceValue.Value);
                MemoryStack.Pop();
            } else {
                name = PopString();
            }
            if (name.Equals("true", StringComparison.InvariantCultureIgnoreCase)) {
                MemoryStack.Push(new BooleanValue {Value = true});
            } else if (name.Equals("false", StringComparison.InvariantCultureIgnoreCase)) {
                MemoryStack.Push(new BooleanValue {Value = false});
            } else {
                var target = ActiveScope?.FindVariable(name, true, mode);
                if (target == null) {
                    LoadNull();
                } else {
                    MemoryStack.Push(target.Value);
                }
            }
        }
        
        private string PopString(SerializableValue target = null) {
            var rawValue = target ?? MemoryStack.Pop();
            switch (rawValue) {
                case StringValue stringMemoryValue:
                    return stringMemoryValue.Value;
                case TranslatableValue translatableMemoryValue:
                    return ScriptHeader.LoadSync(translatableMemoryValue.ScriptId).Header.GetTranslation(ActiveLanguage, translatableMemoryValue.TranslationId);
                default:
                    return null;
            }
        }

        private void CreateScope() {
            var newScope = new ScopeValue {ScriptId = Script.Header.Id, Entrance = Script.CurrentPosition};
            if (ActiveScope != null) {
                ActiveScope.ParentScope = newScope;
            }
            ActiveScope = newScope;
        }

        private async Task CreateFunctionCall() {
            var functionName = MemoryStack.Pop();
            while (functionName is ReferenceValue referenceFunction) {
                functionName = referenceFunction.Value;
            }
            ScopeValue functionBody;
            if (functionName is ScopeValue directBody) {
                functionBody = directBody;
            } else {
                if (!(functionName is IStringConverter stringConverter)) throw new RuntimeException(_callStack, $"Unable to call scene: name {functionName} is not string value");
                var function = ActiveScope?.FindVariable(stringConverter.ConvertToString(), true, VariableSearchMode.All);
                if (function == null || !(function.Value is ScopeValue scopeValue)) {
                    throw new RuntimeException(_callStack, $"Unable to call function: expected function {stringConverter.ConvertToString()} not existed in current scope");
                }
                functionBody = scopeValue;
            }
            // 生成形参
            var paramCount = ((IntegerValue) MemoryStack.Pop()).Value;
            for (var i = -1; ++i < paramCount;) {
                var paramName = PopString();
                if (string.IsNullOrEmpty(paramName)) throw new RuntimeException(_callStack, $"Unable to call {functionName}: expected parameter name {paramName} is not string value");
                functionBody.LocalVariables.Add(paramName, new ReferenceValue {Value = MemoryStack.Pop()});
            }
            // 切换作用域
            _historyScope.Push(ActiveScope);
            ActiveScope = functionBody;
            // 重定向执行位置
            Script = await ScriptFile.Load(ActiveScope.ScriptId);
            Script.MoveTo(ActiveScope.Entrance);
            await Script.UseTranslation(ActiveLanguage);
            _callStack.Push(Script);
        }

        private void LeaveScope() {
            if (ActiveScope == null) throw new RuntimeException(_callStack, "Unable to leave scope: No scope activated");
            // 清空局部作用域
            ActiveScope?.LocalVariables.Clear();
            ActiveScope = ActiveScope.ParentScope;
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
        
        private VisualNovelPlugin FindPlugin() {
            var pluginName = PopString();
            if (string.IsNullOrEmpty(pluginName)) throw new RuntimeException(_callStack, "Unable to find plugin: expected string name");
            var plugin = PluginManager.Find(pluginName);
            if (plugin == null) throw new RuntimeException(_callStack, $"Unable to find plugin: expected plugin {pluginName} not existed");
            return plugin;
        }
        
        private async Task CreatePluginCall() {
            var plugin = FindPlugin();
            var parameterCount = ((IIntegerConverter) MemoryStack.Pop()).ConvertToInteger();
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
                {new StringValue {Value = "Character"}, MemoryStack.Pop()},
                {new StringValue {Value = "Content"}, MemoryStack.Pop()}
            })) ?? new NullValue());
        }

        private void CreateToBoolean(bool reverse = false) {
            var rawValue = MemoryStack.Pop();
            try {
                var result = new BooleanValue {Value = rawValue is IBooleanConverter booleanValue ? booleanValue.ConvertToBoolean() : rawValue != null};
                if (reverse) {
                    result.Value = !result.Value;
                }
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
                    if (valueRight is IStringConverter rightStringConverter && rightStringConverter.ConvertToString() == "Duplicate") {
                        // 复制(Duplicate)由于统一实现的困难暂且由运行环境处理
                        MemoryStack.Push(valueLeft.Duplicate());
                    } else if (valueLeft is IPickChildOperator leftPick) {
                        try {
                            MemoryStack.Push(leftPick.PickChild(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to pick {valueRight} from {valueLeft}: Parent expression has no pick child operator implementation");
                    }
                    break;
                case OperatorType.Add:
                    if (valueLeft is IAddOperator leftAdd) {
                        try {
                            MemoryStack.Push(leftAdd.AddWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to add {valueLeft} and {valueRight}: Left expression has no add operator implementation");
                    }
                    break;
                case OperatorType.Minus:
                    if (valueLeft is ISubtractOperator leftSubtract) {
                        try {
                            MemoryStack.Push(leftSubtract.SubtractWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to subtract {valueLeft} and {valueRight}: Left expression has no subtract operator implementation");
                    }
                    break;
                case OperatorType.Multiply:
                    if (valueLeft is IMultiplyOperator leftMultiply) {
                        try {
                            MemoryStack.Push(leftMultiply.MultiplyWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to multiply {valueLeft} and {valueRight}: Left expression has no multiply operator implementation");
                    }
                    break;
                case OperatorType.Divide:
                    if (valueLeft is IDivideOperator leftDivide) {
                        try {
                            MemoryStack.Push(leftDivide.DivideWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to divide {valueLeft} and {valueRight}: Left expression has no divide operator implementation");
                    }
                    break;
                case OperatorType.GreaterThan:
                case OperatorType.LesserThan:
                case OperatorType.NotLessThan:
                case OperatorType.NotGreaterThan:
                    if (valueLeft is ICompareOperator leftCompare) {
                        try {
                            var compareResult = leftCompare.CompareWith(valueRight);
                            if (compareResult < 0) {
                                MemoryStack.Push(new BooleanValue {Value = operatorType == OperatorType.LesserThan || operatorType == OperatorType.NotGreaterThan});
                            } else if (compareResult == 0) {
                                MemoryStack.Push(new BooleanValue {Value = operatorType == OperatorType.NotLessThan || operatorType == OperatorType.NotGreaterThan});
                            } else {
                                MemoryStack.Push(new BooleanValue {Value = operatorType == OperatorType.GreaterThan || operatorType == OperatorType.NotLessThan});
                            }
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to compare {valueLeft} and {valueRight}: Left expression has no compare operator implementation");
                    }
                    break;
                case OperatorType.LogicEqualsTo:
                case OperatorType.LogicNotEqualsTo:
                    if (valueLeft is IEqualOperator leftEqual) {
                        try {
                            var result = new BooleanValue {Value = leftEqual.EqualsWith(valueRight)};
                            if (operatorType == OperatorType.LogicNotEqualsTo) {
                                result.Value = !result.Value;
                            }
                            MemoryStack.Push(result);
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStack, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to compare {valueLeft} and {valueRight}: Left expression has no equal operator implementation");
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
            var scriptId = PopString();
            if (string.IsNullOrEmpty(scriptId)) throw new RuntimeException(_callStack, $"Unable to load script: script id {scriptId} is not string value");
            var runtime = new ScriptRuntime(scriptId, _callStack);
            await runtime.ExecuteScript();
            var result = new ObjectPlugin.ObjectValue();
            foreach (var (name, value) in runtime.Exported) {
                result.Add(new StringValue {Value = name}, value);
            }
            MemoryStack.Push(result);
        }
        
        [Serializable]
        private class RuntimeDump {
            // ReSharper disable InconsistentNaming
            public long Offset;
            public ScopeValue Scope;
            public Stack<SerializableValue> Memory;
            public Dictionary<string, SerializableValue> Exported;
            public string Language;
            public CallStack CallStack;
            public Stack<ScopeValue> HistoryScope;
            // ReSharper restore InconsistentNaming
        }
    }
}