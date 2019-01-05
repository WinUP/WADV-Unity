using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.MessageSystem;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Compiler.Expressions;
using Core.VisualNovel.Interoperation;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime.MemoryValues;
using UnityEngine;

// ! 为求效率，VNB运行环境在文件头正确的情况下假设文件格式绝对正确，只会做运行时数据检查，不会进行任何格式检查

namespace Core.VisualNovel.Runtime {
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
        public ScopeMemoryValue ActiveScope { get; set; }
        
        /// <summary>
        /// 获取当前内存堆栈
        /// </summary>
        public Stack<SerializableValue> MemoryStack => new Stack<SerializableValue>();
        
        /// <summary>
        /// 获取或修改脚本导出的数据
        /// <para>在脚本尚未执行结束的情况下，导出数据可能不完整</para>
        /// </summary>
        public Dictionary<string, SerializableValue> Exported => new Dictionary<string, SerializableValue>();

        /// <summary>
        /// 获取或设置当前激活的语言
        /// </summary>
        public string ActiveLanguage {
            get => _activeLanguage;
            set {
                if (_activeLanguage == value) return;
                var message = MessageService.Process(new Message<string>(value));
                switch (message) {
                    case Message<string> stringMessage:
                        _activeLanguage = stringMessage.Content;
                        break;
                    default:
                        throw new RuntimeException(_callStacks, $"Unable to change language: Message was modified to non-string type during broadcast");
                }
            }
        }
        
        private string _activeLanguage;
        private readonly List<CallStack> _callStacks = new List<CallStack>();

        public ScriptRuntime() { }

        private ScriptRuntime(IEnumerable<CallStack> initialCallStack) {
            _callStacks.AddRange(initialCallStack);
        }

        /// <summary>
        /// 加载脚本
        /// </summary>
        /// <param name="id">脚本ID</param>e
        public void LoadScript(string id) {
            Script = ScriptFile.Load(id);
        }

        /// <summary>
        /// 跳转到代码段指定偏移处
        /// </summary>
        /// <param name="offset">目标偏移值</param>
        public void JumpTo(long offset) {
            Script?.MoveTo(offset);
        }

        /// <summary>
        /// 跳转到目标脚本代码段指定偏移处
        /// </summary>
        /// <param name="id">目标脚本ID</param>
        /// <param name="offset">目标偏移值</param>
        public void JumpTo(string id, long offset) {
            LoadScript(id);
            JumpTo(offset);
        }
        
        /// <summary>
        /// 从当前代码段偏移位置开始执行脚本
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteScript() {
            if (Script == null) {
                throw new NotSupportedException("Unable to execute bytecode: no active script detected");
            }
            while (await ExecuteSingleLine()) {}
        }

        private async Task<bool> ExecuteSingleLine() {
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
                    MemoryStack.Push(new IntegerMemoryValue {Value = (byte) code - (byte) OperationCode.LDC_I4_0});
                    break;
                case OperationCode.LDC_I4:
                    MemoryStack.Push(new IntegerMemoryValue {Value = Script.ReadInteger()});
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
                    MemoryStack.Push(new FloatMemoryValue {Value = ((byte) code - (byte) OperationCode.LDC_R4_0) * (float) 0.25});
                    break;
                case OperationCode.LDC_R4:
                    MemoryStack.Push(new FloatMemoryValue {Value = Script.ReadFloat()});
                    break;
                case OperationCode.LDSTR:
                    MemoryStack.Push(new StringMemoryValue {Value = Script.ReadStringConstant()});
                    break;
                case OperationCode.LDENTRY:
                    LoadEntrance(Script.Header.Id, Script.ReadLabelOffset(), ActiveScope.Duplicate() as ScopeMemoryValue);
                    break;
                case OperationCode.LDSTT:
                    LoadTranslate(Script.ReadUInt32());
                    break;
                case OperationCode.LDNUL:
                    LoadNull();
                    break;
                case OperationCode.LDLOC:
                    var variableName = PopString();
                    var loadedVariable = ActiveScope.FindVariable(variableName, true, VariableSearchMode.All);
                    if (loadedVariable == null) {
                        LoadNull();
                    } else {
                        MemoryStack.Push(loadedVariable.Value);
                    }
                    break;
                case OperationCode.LDCON:
                    var constantName = PopString();
                    var loadedConstant = ActiveScope.FindVariable(constantName, true, VariableSearchMode.OnlyConstant);
                    if (loadedConstant == null) {
                        LoadNull();
                    } else {
                        MemoryStack.Push(loadedConstant.Value);
                    }
                    break;
                case OperationCode.LDT:
                    MemoryStack.Push(new BooleanMemoryValue {Value = true});
                    break;
                case OperationCode.LDF:
                    MemoryStack.Push(new BooleanMemoryValue {Value = false});
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
                    break;
                case OperationCode.STCON:
                    break;
                case OperationCode.STMEM:
                    break;
                case OperationCode.PICK:
                    CreateBinaryOperation(OperatorType.PickChild);
                    break;
                case OperationCode.SCOPE:
                    break;
                case OperationCode.LEAVE:
                    break;
                case OperationCode.RET:
                    break;
                case OperationCode.FUNC:
                    break;
                case OperationCode.BF_S:
                    break;
                case OperationCode.BR_S:
                    break;
                case OperationCode.BF:
                    break;
                case OperationCode.BR:
                    break;
                case OperationCode.LOAD:
                    break;
                case OperationCode.EXP:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
        
        private void LoadEntrance(string scriptId, long entrance, ScopeMemoryValue parent) {
            MemoryStack.Push(new ScopeMemoryValue {ScriptId = scriptId, Entrance = entrance, ParentScope = parent});
        }

        private void LoadTranslate(uint id) {
            MemoryStack.Push(new TranslatableMemoryValue {TranslationId = id, ScriptId = Script.Header.Id});
        }

        private void LoadNull() {
            MemoryStack.Push(new NullMemoryValue());
        }
        
        private string PopString() {
            var rawValue = MemoryStack.Pop();
            switch (rawValue) {
                case StringMemoryValue stringMemoryValue:
                    return stringMemoryValue.Value;
                case TranslatableMemoryValue translatableMemoryValue:
                    return ScriptHeader.LoadAsset(translatableMemoryValue.ScriptId).Header.GetTranslation(ActiveLanguage, translatableMemoryValue.TranslationId);
                default:
                    return null;
            }
        }

        private VisualNovelPlugin FindPlugin() {
            var rawValue = MemoryStack.Pop();
            VisualNovelPlugin plugin;
            switch (rawValue) {
                case StringMemoryValue stringMemoryValue:
                    plugin = PluginManager.Find(stringMemoryValue.Value);
                    if (plugin == null) throw new RuntimeException(_callStacks, $"Unable to find plugin: expected plugin {stringMemoryValue.Value} not existed");
                    break;
                case TranslatableMemoryValue translatableMemoryValue:
                    var translation = ScriptHeader.LoadAsset(translatableMemoryValue.ScriptId).Header.GetTranslation(ActiveLanguage, translatableMemoryValue.TranslationId);
                    plugin = PluginManager.Find(translation);
                    if (plugin == null) throw new RuntimeException(_callStacks, $"Unable to find plugin: expected plugin {translation} not existed");
                    break;
                default:
                    throw new RuntimeException(_callStacks, $"Unable to find plugin: expected plugin name {rawValue} is not string value");
            }
            return plugin;
        }
        
        private async Task CreatePluginCall() {
            var plugin = FindPlugin();
            var parameterCount = Script.ReadInteger();
            var parameters = new Dictionary<SerializableValue, SerializableValue>();
            for (var i = -1; ++i < parameterCount;) {
                parameters.Add(MemoryStack.Pop(), MemoryStack.Pop());
            }
            try {
                MemoryStack.Push(await plugin.Execute(this, parameters) ?? new NullMemoryValue());
            } catch (Exception ex) {
                throw new RuntimeException(_callStacks, ex);
            }
        }

        private async Task CreateDialogue() {
            var plugin = PluginManager.Find("Dialogue");
            if (plugin == null) {
                throw new RuntimeException(_callStacks, $"Unable to create dialogue: no dialogue plugin registered");
            }
            MemoryStack.Push(await plugin.Execute(this, new Dictionary<SerializableValue, SerializableValue> {
                {new StringMemoryValue {Value = "Character"}, MemoryStack.Pop()},
                {new StringMemoryValue {Value = "Content"}, MemoryStack.Pop()}
            }));
        }

        private void CreateToBoolean(bool reverse = false) {
            var rawValue = MemoryStack.Pop();
            try {
                var result = new BooleanMemoryValue {Value = rawValue is IBooleanConverter booleanValue ? booleanValue.ConvertToBoolean() : rawValue != null};
                if (reverse) {
                    result.Value = !result.Value;
                }
                MemoryStack.Push(result);
            } catch (Exception ex) {
                throw new RuntimeException(_callStacks, ex);
            }
        }

        private void CreateBinaryOperation(OperatorType operatorType) {
            var valueRight = MemoryStack.Pop();
            var valueLeft = MemoryStack.Pop();
            switch (operatorType) {
                case OperatorType.PickChild:
                    // 处理复制
                    if (valueLeft is IPickChildOperator leftPick) {
                        try {
                            MemoryStack.Push(leftPick.PickChild(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to pick {valueRight} from {valueLeft}: Parent expression has no pick child operator implementation");
                    }
                    break;
                case OperatorType.Add:
                    if (valueLeft is IAddOperator leftAdd) {
                        try {
                            MemoryStack.Push(leftAdd.AddWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to add {valueLeft} and {valueRight}: Left expression has no add operator implementation");
                    }
                    break;
                case OperatorType.Minus:
                    if (valueLeft is ISubtractOperator leftSubtract) {
                        try {
                            MemoryStack.Push(leftSubtract.SubtractWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to subtract {valueLeft} and {valueRight}: Left expression has no subtract operator implementation");
                    }
                    break;
                case OperatorType.Multiply:
                    if (valueLeft is IMultiplyOperator leftMultiply) {
                        try {
                            MemoryStack.Push(leftMultiply.MultiplyWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to multiply {valueLeft} and {valueRight}: Left expression has no multiply operator implementation");
                    }
                    break;
                case OperatorType.Divide:
                    if (valueLeft is IDivideOperator leftDivide) {
                        try {
                            MemoryStack.Push(leftDivide.DivideWith(valueRight));
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to divide {valueLeft} and {valueRight}: Left expression has no divide operator implementation");
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
                                MemoryStack.Push(new BooleanMemoryValue {Value = operatorType == OperatorType.LesserThan || operatorType == OperatorType.NotGreaterThan});
                            } else if (compareResult == 0) {
                                MemoryStack.Push(new BooleanMemoryValue {Value = operatorType == OperatorType.NotLessThan || operatorType == OperatorType.NotGreaterThan});
                            } else {
                                MemoryStack.Push(new BooleanMemoryValue {Value = operatorType == OperatorType.GreaterThan || operatorType == OperatorType.NotLessThan});
                            }
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to compare {valueLeft} and {valueRight}: Left expression has no compare operator implementation");
                    }
                    break;
                case OperatorType.LogicEqualsTo:
                case OperatorType.LogicNotEqualsTo:
                    if (valueLeft is IEqualOperator leftEqual) {
                        try {
                            var result = new BooleanMemoryValue {Value = leftEqual.EqualsWith(valueRight)};
                            if (operatorType == OperatorType.LogicNotEqualsTo) {
                                result.Value = !result.Value;
                            }
                            MemoryStack.Push(result);
                        } catch (Exception ex) {
                            throw new RuntimeException(_callStacks, ex);
                        }
                    } else {
                        throw new RuntimeException(_callStacks, $"Unable to compare {valueLeft} and {valueRight}: Left expression has no equal operator implementation");
                    }
                    break;
            }
        }
    }
}