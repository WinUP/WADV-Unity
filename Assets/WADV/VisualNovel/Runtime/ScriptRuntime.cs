using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WADV.Extensions;
using WADV.Intents;
using WADV.MessageSystem;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Runtime.Utilities;
using WADV.VisualNovel.Translation;
using JetBrains.Annotations;

// ! 为求效率，VNB运行环境在文件头正确的情况下假设文件格式绝对正确，只会做运行时数据检查，不会进行任何格式检查

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