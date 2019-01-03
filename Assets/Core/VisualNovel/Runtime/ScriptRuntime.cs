using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.MessageSystem;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Plugin;
using Core.VisualNovel.Runtime.MemoryValues;
using Core.VisualNovel.Runtime.Variables;
using Core.VisualNovel.Runtime.Variables.Values;

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
        public FunctionScope ActiveFunctionScope => _callStack.Peek();
        
        /// <summary>
        /// 获取当前内存堆栈
        /// </summary>
        public Stack<IMemoryValue> MemoryStack => new Stack<IMemoryValue>();
        
        /// <summary>
        /// 获取或修改脚本导出的数据
        /// <para>在脚本尚未执行结束的情况下，导出数据可能不完整</para>
        /// </summary>
        public Dictionary<string, IMemoryValue> Export => new Dictionary<string, IMemoryValue>();

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
                        throw new RuntimeException(_callStack, $"Unable to change language: Message was modified to non-string type during broadcast");
                }
            }
        }

        private readonly Stack<FunctionScope> _callStack = new Stack<FunctionScope>();
        private string _activeLanguage;

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
        /// 在当前执行上下文中递归向上查找变量
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="onlyConstant">是否只查找常量表</param>
        /// <returns></returns>
        public Variable FindVariable(string name, bool onlyConstant) {
            if (string.IsNullOrEmpty(name)) {
                throw new RuntimeException(_callStack, "Unable to find variable: expected name is empty or null");
            }
            foreach (var stack in _callStack) {
                var item = (onlyConstant ? stack.Variables.Where(e => e.Value.IsConstant) : stack.Variables).Where(e => e.Key == name).ToList();
                if (item.Any()) return item.First().Value;
            }
            return null;
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
                    LoadStaticValue((byte) code - (byte) OperationCode.LDC_I4_0);
                    break;
                case OperationCode.LDC_I4:
                    LoadStaticValue(Script.ReadInteger());
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
                    LoadStaticValue(((byte) code - (byte) OperationCode.LDC_R4_0) * (float) 0.25);
                    break;
                case OperationCode.LDC_R4:
                    LoadStaticValue(Script.ReadFloat());
                    break;
                case OperationCode.LDSTR:
                    LoadStaticValue(Script.ReadStringConstant());
                    break;
                case OperationCode.LDENTRY:
                    LoadOffsetValue(Script.ReadLabelOffset());
                    break;
                case OperationCode.LDSTT:
                    LoadTranslatableValue(Script.ReadUInt32());
                    break;
                case OperationCode.LDNUL:
                    LoadNull();
                    break;
                case OperationCode.LDLOC:
                    var variableName = PopString();
                    var loadedVariable = FindVariable(variableName, false);
                    if (loadedVariable == null) {
                        throw new RuntimeException(_callStack, $"Unable to load variable: expected variable/constant ${variableName} not existed");
                    }
                    LoadVariable(loadedVariable);
                    break;
                case OperationCode.LDCON:
                    var constantName = PopString();
                    var loadedConstant = FindVariable(constantName, true);
                    if (loadedConstant == null) {
                        throw new RuntimeException(_callStack, $"Unable to load constant: expected constant ${constantName} not existed");
                    }
                    LoadVariable(loadedConstant);
                    break;
                case OperationCode.LDT:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {Value = true});
                    break;
                case OperationCode.LDF:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {Value = false});
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
                    CreateAdd();
                    break;
                case OperationCode.SUB:
                    break;
                case OperationCode.MUL:
                    break;
                case OperationCode.DIV:
                    break;
                case OperationCode.NOT:
                    break;
                case OperationCode.EQL:
                    break;
                case OperationCode.CGE:
                    break;
                case OperationCode.CGT:
                    break;
                case OperationCode.CLE:
                    break;
                case OperationCode.CLT:
                    break;
                case OperationCode.STLOC:
                    break;
                case OperationCode.STCON:
                    break;
                case OperationCode.PICK:
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

        private void LoadStaticValue<T>(T value) {
            MemoryStack.Push(new SerializableMemoryValue<T> {Value = value});
        }
        
        private void LoadOffsetValue(long value, ScriptFile script = null, Stack<FunctionScope> stack = null) {
            MemoryStack.Push(new ScopeMemoryValue {ScriptId = (script ?? Script).Header.Id, Entrance = value, RunningStack = stack});
        }

        private void LoadTranslatableValue(uint id) {
            MemoryStack.Push(new TranslatableMemoryValue {TranslationId = id, ScriptId = Script.Header.Id});
        }

        private void LoadNull() {
            MemoryStack.Push(new NullMemoryValue());
        }

        private void LoadVariable(Variable variable) {
            switch (variable.Value) {
                case BooleanVariableValue booleanVariableValue:
                    LoadStaticValue(booleanVariableValue.Value);
                    break;
                case ExternVariableValue externVariableValue:
                    LoadStaticValue(externVariableValue.Value);
                    break;
                case FloatVariableValue floatVariableValue:
                    LoadStaticValue(floatVariableValue.Value);
                    break;
                case IntegerVariableValue integerVariableValue:
                    LoadStaticValue(integerVariableValue.Value);
                    break;
                case NullVariableValue _:
                    LoadNull();
                    break;
                case OffsetVariableValue offsetVariableValue:
                    MemoryStack.Push(new ScopeMemoryValue {ScriptId = offsetVariableValue.ScriptId, Entrance = offsetVariableValue.Offset, RunningStack = offsetVariableValue.RunningStack});
                    break;
                case StringVariableValue stringVariableValue:
                    LoadStaticValue(stringVariableValue.Value);
                    break;
            }
        }
        
        private string PopString() {
            string name;
            if (MemoryStack.Pop() is SerializableMemoryValue<string> stringMemoryValue) {
                name = stringMemoryValue.Value;
            } else if (MemoryStack.Pop() is TranslatableMemoryValue translatableMemoryValue) {
                name = ScriptHeader.LoadAsset(translatableMemoryValue.ScriptId).Header.GetTranslation(ActiveLanguage, translatableMemoryValue.TranslationId);
            } else {
                return null;
            }
            return name;
        }
        
        private async Task CreatePluginCall() {
            var pluginNameValue = PopString();
            if (string.IsNullOrEmpty(pluginNameValue)) {
                throw new RuntimeException(_callStack, "Unable to find plugin: expected plugin name is empty or null");
            }
            var parameterCount = Script.ReadInteger();
            var parameters = new Dictionary<IMemoryValue, IMemoryValue>();
            for (var i = -1; ++i < parameterCount;) {
                parameters.Add(MemoryStack.Pop(), MemoryStack.Pop());
            }
            var plugin = PluginManager.Find(pluginNameValue);
            if (plugin == null) {
                throw new RuntimeException(_callStack, $"Unable to find plugin: expected plugin {pluginNameValue} not existed");
            }
            MemoryStack.Push(await plugin.Execute(this, parameters) ?? new NullMemoryValue());
        }

        private async Task CreateDialogue() {
            var plugin = PluginManager.Find("Dialogue");
            if (plugin == null) {
                throw new RuntimeException(_callStack, $"Unable to create dialogue: no dialogue plugin registered");
            }
            MemoryStack.Push(await plugin.Execute(this, new Dictionary<IMemoryValue, IMemoryValue> {
                {new SerializableMemoryValue<string> {Value = "Character"}, MemoryStack.Pop()},
                {new SerializableMemoryValue<string> {Value = "Content"}, MemoryStack.Pop()}
            }));
        }

        private void CreateToBoolean() {
            var rawValue = MemoryStack.Pop();
            switch (rawValue) {
                case NullMemoryValue _:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {Value = false});
                    break;
                case ScopeMemoryValue offsetMemoryValue:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {Value = offsetMemoryValue.Entrance != 0});
                    break;
                case TranslatableMemoryValue translatableMemoryValue:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {
                        Value = ScriptHeader.LoadAsset(translatableMemoryValue.ScriptId).Header.HasTranslation(ActiveLanguage, translatableMemoryValue.TranslationId)
                    });
                    break;
                case SerializableMemoryValue staticMemoryValue:
                    MemoryStack.Push(new SerializableMemoryValue<bool> {Value = staticMemoryValue.ToBoolean()});
                    break;
            }
        }

        // * ? + null = ?
        // * null + ? = ?
        // * offset + * = ERROR
        // * translatable + static = static<string>
        // * translatable + * = ERROR
        private void CreateAdd() {
            var valueRight = MemoryStack.Pop();
            var valueLeft = MemoryStack.Pop();
            if (valueRight is NullMemoryValue) {
                MemoryStack.Push(valueLeft.Duplicate());
                return;
            }
            if (valueLeft is NullMemoryValue) {
                MemoryStack.Push(valueRight.Duplicate());
                return;
            }
            switch (valueLeft) {
                case ScopeMemoryValue _:
                    throw new RuntimeException(_callStack, $"Unable to add values: scene entrance/code offset is not allowed to join any binary operation");
                case SerializableMemoryValue staticLeft:
                    
                    break;
                case TranslatableMemoryValue translatableLeft:
                    if (valueRight is SerializableMemoryValue stringStaticRight) {
                        MemoryStack.Push(new SerializableMemoryValue<string> {
                            Value = ScriptHeader.LoadAsset(translatableLeft.ScriptId).Header.GetTranslation(ActiveLanguage, translatableLeft.TranslationId) + stringStaticRight
                        });
                    } else {
                        throw new RuntimeException(_callStack, $"Unable to add values: translatable string is only allowed to add with static value or null");
                    }
                    break;
            }
        }
    }
}