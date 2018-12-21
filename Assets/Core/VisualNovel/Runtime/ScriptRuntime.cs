using System;
using System.Collections.Generic;
using Core.VisualNovel.Compiler;
using Core.VisualNovel.Runtime.StackItems;

namespace Core.VisualNovel.Runtime {
    /// <summary>
    /// 脚本运行环境
    /// </summary>
    public partial class ScriptRuntime {
        /// <summary>
        /// 获取所有已经加载的脚本
        /// </summary>
        public static Dictionary<string, RuntimeFile> LoadedScripts { get; } = new Dictionary<string, RuntimeFile>();
        /// <summary>
        /// 获取内存堆栈
        /// </summary>
        public Stack<IStackItem> MemoryStack { get; } = new Stack<IStackItem>();
        /// <summary>
        /// 获取正在执行的脚本文件
        /// </summary>
        public RuntimeFile Script { get; private set; }
        /// <summary>
        /// 获取或设置使用的翻译
        /// </summary>
        public string ActiveLanguage {
            get => _activeLanguage;
            set {
                _activeLanguage = value;
                Script?.UseTranslation(_activeLanguage);
            }
        }

        private readonly Stack<(RuntimeFile Script, long Position)> _callStack = new Stack<(RuntimeFile Script, long Position)>();
        private string _activeLanguage = "default";

        /// <summary>
        /// 加载脚本
        /// </summary>
        /// <param name="id">脚本ID</param>e
        public void LoadScript(string id) {
            RuntimeFile script;
            if (LoadedScripts.ContainsKey(id)) {
                script = LoadedScripts[id];
            } else {
                script = new RuntimeFile(id, this);
                LoadedScripts.Add(id, script);
            }
            Script = script;
        }

        /// <summary>
        /// 保存当前执行位置到调用堆栈
        /// </summary>
        public void SaveStack() {
            if (Script != null) {
                _callStack.Push((Script: Script, Position: Script.CurrentPosition));
            }
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

        public bool ExecuteSingleLine() {
            var code = Script.ReadOperationCode();
            if (code == null) return false;
            switch (code) {
                case OperationCode.LDC_I4_0:
                    break;
                case OperationCode.LDC_I4_1:
                    break;
                case OperationCode.LDC_I4_2:
                    break;
                case OperationCode.LDC_I4_3:
                    break;
                case OperationCode.LDC_I4_4:
                    break;
                case OperationCode.LDC_I4_5:
                    break;
                case OperationCode.LDC_I4_6:
                    break;
                case OperationCode.LDC_I4_7:
                    break;
                case OperationCode.LDC_I4_8:
                    break;
                case OperationCode.LDC_I4:
                    break;
                case OperationCode.LDC_R4_0:
                    break;
                case OperationCode.LDC_R4_025:
                    break;
                case OperationCode.LDC_R4_05:
                    break;
                case OperationCode.LDC_R4_075:
                    break;
                case OperationCode.LDC_R4_1:
                    break;
                case OperationCode.LDC_R4_125:
                    break;
                case OperationCode.LDC_R4_15:
                    break;
                case OperationCode.LDC_R4_175:
                    break;
                case OperationCode.LDC_R4_2:
                    break;
                case OperationCode.LDC_R4_225:
                    break;
                case OperationCode.LDC_R4_25:
                    break;
                case OperationCode.LDC_R4_275:
                    break;
                case OperationCode.LDC_R4_3:
                    break;
                case OperationCode.LDC_R4_325:
                    break;
                case OperationCode.LDC_R4_35:
                    break;
                case OperationCode.LDC_R4_375:
                    break;
                case OperationCode.LDC_R4_4:
                    break;
                case OperationCode.LDC_R4_425:
                    break;
                case OperationCode.LDC_R4_45:
                    break;
                case OperationCode.LDC_R4_475:
                    break;
                case OperationCode.LDC_R4_5:
                    break;
                case OperationCode.LDC_R4_525:
                    break;
                case OperationCode.LDC_R4_55:
                    break;
                case OperationCode.LDC_R4_575:
                    break;
                case OperationCode.LDC_R4:
                    break;
                case OperationCode.LDSTR:
                    break;
                case OperationCode.LDADDR:
                    break;
                case OperationCode.LDSTT:
                    break;
                case OperationCode.LDNUL:
                    break;
                case OperationCode.LDLOC:
                    break;
                case OperationCode.LDCON:
                    break;
                case OperationCode.LDT:
                    break;
                case OperationCode.LDF:
                    break;
                case OperationCode.CALL:
                    break;
                case OperationCode.POP:
                    break;
                case OperationCode.DIALOGUE:
                    break;
                case OperationCode.BVAL:
                    break;
                case OperationCode.ADD:
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
                case OperationCode.PICK:
                    break;
                case OperationCode.SCOPE:
                    break;
                case OperationCode.LEAVE:
                    break;
                case OperationCode.LANG:
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
    }
}