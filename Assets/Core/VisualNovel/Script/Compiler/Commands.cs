using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 指令翻译表
    /// </summary>
    public static class Commands {
        private static readonly Dictionary<string, Dictionary<string, string>> Translates = new Dictionary<string, Dictionary<string, string>>();
        
        /// <summary>
        /// 编译语言选项
        /// <para>该指令在所有语言中表示一致，且不能对特定语言添加字面表示</para>
        /// </summary>
        public const string SyntaxLanguage = "lang";
        /// <summary>
        /// 选择指令
        /// </summary>
        public const string SyntaxIf = "SYNTAX_IF";
        /// <summary>
        /// 分支指令
        /// </summary>
        public const string SyntaxElseIf = "SYNTAX_ELSEIF";
        /// <summary>
        /// 否则指令
        /// </summary>
        public const string SyntaxElse = "SYNTAX_ELSE";
        /// <summary>
        /// 循环指令
        /// </summary>
        public const string SyntaxLoop = "SYNTAX_LOOP";
        /// <summary>
        /// 场景指令
        /// </summary>
        public const string SyntaxScenario = "SYNTAX_SCENARIO";
        /// <summary>
        /// 返回指令
        /// </summary>
        public const string SyntaxReturn = "SYNTAX_RETURN";
        
        static Commands() {
            AddTranslate("default", SyntaxIf, "如果");
            AddTranslate("default", SyntaxElseIf, "或者");
            AddTranslate("default", SyntaxElse, "否则");
            AddTranslate("default", SyntaxLoop, "循环");
            AddTranslate("default", SyntaxScenario, "场景");
            AddTranslate("default", SyntaxReturn, "返回");
            
            AddTranslate("en", SyntaxIf, "if");
            AddTranslate("en", SyntaxElseIf, "elseif");
            AddTranslate("en", SyntaxElse, "else");
            AddTranslate("en", SyntaxLoop, "loop");
            AddTranslate("en", SyntaxScenario, "scene");
            AddTranslate("en", SyntaxReturn, "return");
        }

        /// <summary>
        /// 获取指定语言中目标指令的字面表示
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="command">目标指令</param>
        /// <returns></returns>
        public static string FindCommand(string language, string command) {
            while (true) {
                var commandList = GetCommandList(language);
                if (commandList == null) {
                    return null;
                }
                if (commandList.ContainsKey(command)) return commandList[command];
                if (language == "default") {
                    throw new CultureNotFoundException($"Cannot find command ${command} in any available language");
                }
                language = "default";
            }
        }

        /// <summary>
        /// 为特定指令在某一语言中添加字面表示
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="command">目标指令</param>
        /// <param name="name">指令的字面表示</param>
        public static void AddTranslate(string language, string command, string name) {
            var commandList = GetCommandList(language);
            if (commandList.ContainsKey(command)) {
                commandList[command] = name;
            } else {
                commandList.Add(command, name);
            }
        }

        private static Dictionary<string, string> GetCommandList(string language) {
            Dictionary<string, string> commandList;
            if (Translates.ContainsKey(language)) {
                commandList = Translates[language];
            } else {
                if (!language.All(e => e >= '0' && e <= '9' || e >= 'a' && e <= 'z' || e >= 'A' && e <= 'Z' || e =='_')) {
                    throw new ArgumentException("Language names can only have numbers, alphabets and underlines");
                }
                commandList = new Dictionary<string, string>();
                Translates.Add(language, commandList);
            }
            return commandList;
        }
    }
}