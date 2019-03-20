using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;
using JetBrains.Annotations;
using WADV.Translation;

namespace WADV.VisualNovel.Runtime {
    /// <summary>
    /// 脚本运行环境
    /// </summary>
    public partial class ScriptRuntime {
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
                var message = MessageService.Process(Message<ChangeLanguageIntent>.Create(CoreConstant.Mask, CoreConstant.PrepareLanguageChange, new ChangeLanguageIntent {Runtime = this, NewLanguage = value}));
                switch (message) {
                    case Message<ChangeLanguageIntent> result:
                        _activeLanguage = result.Content.NewLanguage;
                        if (!TranslationManager.CheckLanguageName(_activeLanguage)) throw new RuntimeException(_callStack, $"Unable to change language: {_activeLanguage} is not legal language name");
                        break;
                    default:
                        throw new RuntimeException(_callStack, $"Unable to change language: message was modified to non-string type during broadcast");
                }
                Script.UseTranslation(_activeLanguage).Wait();
                MessageService.Process(Message<ChangeLanguageIntent>.Create(CoreConstant.Mask, CoreConstant.LanguageChange, new ChangeLanguageIntent {Runtime = this, NewLanguage = _activeLanguage}));
            }
        }
        
        /// <summary>
        /// 记录当前激活的语言
        /// </summary>
        private string _activeLanguage = TranslationManager.DefaultLanguage;
        /// <summary>
        /// 记录调用堆栈
        /// </summary>
        private readonly CallStack _callStack = new CallStack();
        /// <summary>
        /// 记录RET使用的历史作用域
        /// </summary>
        private readonly Stack<ScopeValue> _historyScope = new Stack<ScopeValue>();
        /// <summary>
        /// 记录用于停止执行的主线程占位符
        /// </summary>
        [CanBeNull] private MainThreadPlaceholder _stopRequest;
        /// <summary>
        /// 记录LOAD指令正在处理的目标运行环境
        /// </summary>
        [CanBeNull] private ScriptRuntime _loadingScript;

        /// <summary>
        /// 新建一个脚本运行环境
        /// </summary>
        /// <param name="script">目标脚本</param>
        public ScriptRuntime(ScriptFile script) {
            Script = script ?? throw new ArgumentException("Unable to load script: expected script is not existed", nameof(script));
            Script.UseTranslation(ActiveLanguage).Wait();
        }

        /// <inheritdoc />
        /// <summary>
        /// 新建一个脚本运行环境
        /// </summary>
        /// <param name="scriptId">目标脚本的ID</param>
        public ScriptRuntime(string scriptId) : this(ScriptFile.LoadSync(scriptId)) { }

        /// <inheritdoc />
        /// <summary>
        /// 新建一个脚本运行环境
        /// </summary>
        /// <param name="scriptId">目标脚本的ID</param>
        /// <param name="initialCallStack">初始调用堆栈</param>
        private ScriptRuntime(string scriptId, IEnumerable<CallStack.StackItem> initialCallStack) : this(ScriptFile.LoadSync(scriptId), initialCallStack) { }

        /// <inheritdoc />
        /// <summary>
        /// 新建一个脚本运行环境
        /// </summary>
        /// <param name="script">目标脚本</param>
        /// <param name="initialCallStack">初始调用堆栈</param>
        private ScriptRuntime(ScriptFile script, IEnumerable<CallStack.StackItem> initialCallStack) : this(script) {
            _callStack.Push(initialCallStack);
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
    }
}