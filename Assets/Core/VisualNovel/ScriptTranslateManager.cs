using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.VisualNovel.Attributes;

namespace Core.VisualNovel {
    /// <summary>
    /// 指令翻译表
    /// </summary>
    public static class ScriptTranslateManager {
        private static readonly Dictionary<string, Dictionary<string, string>> Translates = new Dictionary<string, Dictionary<string, string>>();

        static ScriptTranslateManager() {
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass)) {
                foreach (var translation in item.GetCustomAttributes<RegisterTranslateAttribute>()) {
                    Add(translation.Language, translation.Name, translation.Value);
                }
            }
        }

        /// <summary>
        /// 获取指定语言中目标指令的字面表示
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="name">目标指令</param>
        /// <returns></returns>
        public static string Find(string language, string name) {
            while (true) {
                var items = GetItemList(language);
                if (items == null) {
                    return null;
                }
                if (items.ContainsKey(name)) return items[name];
                if (language == "default") {
                    return null;
                }
                language = "default";
            }
        }

        /// <summary>
        /// 为特定指令在某一语言中添加字面表示
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="name">目标指令</param>
        /// <param name="value">指令的字面表示</param>
        public static void Add(string language, string name, string value) {
            var items = GetItemList(language);
            if (items.ContainsKey(name)) {
                items[name] = value;
            } else {
                items.Add(name, value);
            }
        }

        private static Dictionary<string, string> GetItemList(string language) {
            Dictionary<string, string> commandList;
            if (Translates.ContainsKey(language)) {
                commandList = Translates[language];
            } else {
                if (!language.All(e => e >= '0' && e <= '9' || e >= 'a' && e <= 'z' || e >= 'A' && e <= 'Z' || e =='_')) {
                    throw new ArgumentException("Language name can only has numbers, alphabets and underlines");
                }
                commandList = new Dictionary<string, string>();
                Translates.Add(language, commandList);
            }
            return commandList;
        }
    }
}