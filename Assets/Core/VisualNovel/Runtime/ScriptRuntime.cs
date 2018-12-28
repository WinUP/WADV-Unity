using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Runtime.MemoryValues;
using Core.VisualNovel.Runtime.Variables;
using Core.VisualNovel.Runtime.Variables.Values;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 脚本运行环境
    /// </summary>
    public class ScriptRuntime {
//        /// <summary>
//        /// 获取所有已经加载的脚本
//        /// </summary>
//        public static Dictionary<string, ScriptFile> LoadedScripts { get; } = new Dictionary<string, ScriptFile>();
//        /// <summary>
//        /// 获取正在执行的脚本文件
//        /// </summary>
//        public ScriptFile Script { get; private set; }
//        /// <summary>
//        /// 获取或设置使用的翻译
//        /// </summary>
//        public string ActiveLanguage {
//            get => _activeLanguage;
//            set {
//                _activeLanguage = value;
//                Script?.UseTranslation(_activeLanguage);
//            }
//        }
//        /// <summary>
//        /// 获取当前激活的顶层作用域
//        /// </summary>
//        public CallStack ActiveCallStack => _callStack.Peek();
//        /// <summary>
//        /// 获取当前内存堆栈
//        /// </summary>
//        public Stack<IMemoryValue> MemoryStack => new Stack<IMemoryValue>();
//
//        private readonly Stack<CallStack> _callStack = new Stack<CallStack>();
//        private string _activeLanguage = "default";
//
//        /// <summary>
//        /// 加载脚本
//        /// </summary>
//        /// <param name="id">脚本ID</param>e
//        public void LoadScript(string id) {
//            ScriptFile script;
//            if (LoadedScripts.ContainsKey(id)) {
//                script = LoadedScripts[id];
//            } else {
//                script = new ScriptFile(id, this);
//                LoadedScripts.Add(id, script);
//            }
//            Script = script;
//        }
//
//        /// <summary>
//        /// 保存当前执行位置到调用堆栈
//        /// </summary>
//        public void SaveStack() {
//            if (Script != null) {
//                _callStack.Push(new CallStack {Script = Script, Offset = Script.CurrentPosition});
//            }
//        }
//
//        /// <summary>
//        /// 跳转到代码段指定偏移处
//        /// </summary>
//        /// <param name="offset">目标偏移值</param>
//        public void JumpTo(long offset) {
//            Script?.MoveTo(offset);
//        }
//
//        /// <summary>
//        /// 跳转到目标脚本代码段指定偏移处
//        /// </summary>
//        /// <param name="id">目标脚本ID</param>
//        /// <param name="offset">目标偏移值</param>
//        public void JumpTo(string id, long offset) {
//            LoadScript(id);
//            JumpTo(offset);
//        }
//
//        public (IVariable Target, CallStack Stack, bool IsConstant) FindVariable(string name, bool onlyConstant) {
//            foreach (var callStack in _callStack) {
//                if (!onlyConstant && callStack.Variables.ContainsKey(name)) {
//                    return (callStack.Variables[name], callStack, false);
//                }
//                if (callStack.Constants.ContainsKey(name)) {
//                    return (callStack.Constants[name], callStack, true);
//                }
//            }
//            return (null, null, false);
//        }
//
//        private Task ApplyPluginCall() {
//            if (MemoryStack.Count < 2) {
//                throw new RuntimeException(_callStack, "Unable to initialize plugin call: memory stack information not enough");
//            }
//            return Task.CompletedTask;
//        }
//
//        
//
//        public async Task ExecuteScript() {
//            var context = new ExecutionContext();
//            while (await ExecuteSingleLine(context)) {}
//        }
//
//        private async Task<bool> ExecuteSingleLine(ExecutionContext context) {
//            if (Script == null) {
//                throw new NotSupportedException("No active script detected, must load at least one script before execute");
//            }
//            var code = Script.ReadOperationCode();
//            if (code == null) return false;
//            switch (code) {
//                case OperationCode.LDC_I4_0:
//                case OperationCode.LDC_I4_1:
//                case OperationCode.LDC_I4_2:
//                case OperationCode.LDC_I4_3:
//                case OperationCode.LDC_I4_4:
//                case OperationCode.LDC_I4_5:
//                case OperationCode.LDC_I4_6:
//                case OperationCode.LDC_I4_7:
//                case OperationCode.LDC_I4_8:
//                    LoadStaticValue((byte) code - (byte) OperationCode.LDC_I4_0);
//                    break;
//                case OperationCode.LDC_I4:
//                    LoadStaticValue(Script.ReadInteger());
//                    break;
//                case OperationCode.LDC_R4_0:
//                case OperationCode.LDC_R4_025:
//                case OperationCode.LDC_R4_05:
//                case OperationCode.LDC_R4_075:
//                case OperationCode.LDC_R4_1:
//                case OperationCode.LDC_R4_125:
//                case OperationCode.LDC_R4_15:
//                case OperationCode.LDC_R4_175:
//                case OperationCode.LDC_R4_2:
//                case OperationCode.LDC_R4_225:
//                case OperationCode.LDC_R4_25:
//                case OperationCode.LDC_R4_275:
//                case OperationCode.LDC_R4_3:
//                case OperationCode.LDC_R4_325:
//                case OperationCode.LDC_R4_35:
//                case OperationCode.LDC_R4_375:
//                case OperationCode.LDC_R4_4:
//                case OperationCode.LDC_R4_425:
//                case OperationCode.LDC_R4_45:
//                case OperationCode.LDC_R4_475:
//                case OperationCode.LDC_R4_5:
//                case OperationCode.LDC_R4_525:
//                case OperationCode.LDC_R4_55:
//                case OperationCode.LDC_R4_575:
//                    LoadStaticValue(((byte) code - (byte) OperationCode.LDC_R4_0) * (float) 0.25);
//                    break;
//                case OperationCode.LDC_R4:
//                    LoadStaticValue(Script.ReadFloat());
//                    break;
//                case OperationCode.LDSTR:
//                    LoadStaticValue(Script.ReadStringConstant());
//                    break;
//                case OperationCode.LDENTRY:
//                    LoadOffsetValue(Script.ReadLabelOffset());
//                    break;
//                case OperationCode.LDSTT:
//                    LoadTranslatableValue(Script.ReadUInt32());
//                    break;
//                case OperationCode.LDNUL:
//                    LoadNull();
//                    break;
//                case OperationCode.LDLOC:
//                    var variableName = LoadVariableName();
//                    var (loadedVariable, _, _) = FindVariable(variableName, false);
//                    if (loadedVariable == null) {
//                        throw new RuntimeException(_callStack, $"Unable to load variable: name ${variableName} not existed");
//                    }
//                    LoadVariable(loadedVariable);
//                    break;
//                case OperationCode.LDCON:
//                    var constantName = LoadVariableName();
//                    var (loadedConstant, _, _) = FindVariable(constantName, true);
//                    if (loadedConstant == null) {
//                        throw new RuntimeException(_callStack, $"Unable to load constant: name ${constantName} not existed");
//                    }
//                    LoadVariable(loadedConstant);
//                    break;
//                case OperationCode.LDT:
//                    MemoryStack.Push(new StaticMemoryValue<bool> {Value = true});
//                    break;
//                case OperationCode.LDF:
//                    MemoryStack.Push(new StaticMemoryValue<bool> {Value = false});
//                    break;
//                case OperationCode.CALL:
//                    await ApplyPluginCall();
//                    break;
//                case OperationCode.POP:
//                    break;
//                case OperationCode.DIALOGUE:
//                    break;
//                case OperationCode.BVAL:
//                    break;
//                case OperationCode.ADD:
//                    break;
//                case OperationCode.SUB:
//                    break;
//                case OperationCode.MUL:
//                    break;
//                case OperationCode.DIV:
//                    break;
//                case OperationCode.NOT:
//                    break;
//                case OperationCode.EQL:
//                    break;
//                case OperationCode.CGE:
//                    break;
//                case OperationCode.CGT:
//                    break;
//                case OperationCode.CLE:
//                    break;
//                case OperationCode.CLT:
//                    break;
//                case OperationCode.STLOC:
//                    break;
//                case OperationCode.PICK:
//                    break;
//                case OperationCode.SCOPE:
//                    break;
//                case OperationCode.LEAVE:
//                    break;
//                case OperationCode.LANG:
//                    break;
//                case OperationCode.RET:
//                    break;
//                case OperationCode.FUNC:
//                    break;
//                case OperationCode.BF_S:
//                    break;
//                case OperationCode.BR_S:
//                    break;
//                case OperationCode.BF:
//                    break;
//                case OperationCode.BR:
//                    break;
//                case OperationCode.LOAD:
//                    break;
//                case OperationCode.EXP:
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//            return true;
//        }
//
//        private void LoadStaticValue<T>(T value) {
//            MemoryStack.Push(new StaticMemoryValue<T> {Value = value});
//        }
//        
//        private void LoadOffsetValue(long value, ScriptFile script = null, Stack<CallStack> stack = null) {
//            MemoryStack.Push(new OffsetMemoryValue {Script = script, Offset = value, RunningStack = stack});
//        }
//
//        private void LoadTranslatableValue(uint id) {
//            MemoryStack.Push(new TranslatableMemoryValue {Id = id, Value = Script.ActiveTranslation.GetTranslation(id)});
//        }
//
//        private void LoadNull() {
//            MemoryStack.Push(new NullMemoryValue());
//        }
//        
//        private string LoadVariableName() {
//            if (MemoryStack.Count < 1) {
//                throw new RuntimeException(_callStack, "Unable to load variable: variable name must be in top of memory stack");
//            }
//            string name;
//            if (MemoryStack.Pop() is StaticMemoryValue<string> varName) {
//                name = varName.Value;
//            } else if (MemoryStack.Pop() is TranslatableMemoryValue varTName) {
//                name = varTName.Value;
//            } else {
//                throw new RuntimeException(_callStack, "Unable to load variable: variable name must be in top of memory stack");
//            }
//            return name;
//        }
//
//        private void LoadVariable(IVariable variable) {
//            switch (variable) {
//                case OffsetVariable functionVariable:
//                    MemoryStack.Push(new OffsetMemoryValue {Script = functionVariable.Script, Offset = functionVariable.Offset, RunningStack = functionVariable.TargetStack});
//                    break;
//                case ValueVariable valueVariable:
//                    switch (valueVariable.Value) {
//                        case BooleanVariableValue booleanVariableValue:
//                            LoadStaticValue(booleanVariableValue.Value);
//                            break;
//                        case FloatVariableValue floatVariableValue:
//                            LoadStaticValue(floatVariableValue.Value);
//                            break;
//                        case IntegerVariableValue integerVariableValue:
//                            LoadStaticValue(integerVariableValue.Value);
//                            break;
//                        case NullVariableValue _:
//                            LoadNull();
//                            break;
//                        case StringVariableValue stringVariableValue:
//                            LoadStaticValue(stringVariableValue.Value);
//                            break;
//                    }
//                    break;
//            }
//        }
    }
}