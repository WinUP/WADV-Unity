using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.VisualNovel.Attributes;

namespace Core.VisualNovel.Translation {
    public static class TranslationManager {
        public const string DefaultLanguage = "default";
        /// <summary>
        /// 静态翻译列表（项名, Name=语言, Value=项值）
        /// </summary>
        private static readonly Dictionary<string, List<Translation>> StaticTranslations = new Dictionary<string, List<Translation>>();
        /// <summary>
        /// 插件名翻译列表（语言, Name=插件名, Value=翻译名）
        /// </summary>
        private static readonly Dictionary<string, List<Translation>> PluginTranslations = new Dictionary<string, List<Translation>>();
        
        static TranslationManager() {
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass)) {
                foreach (var translation in item.GetCustomAttributes<StaticTranslationAttribute>()) {
                    SetStatic(translation.Name, translation.Value, translation.Language);
                }
            }
        }

        /// <summary>
        /// 获取插件名
        /// </summary>
        /// <param name="translatedName">目标语言下的插件名</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string GetPluginName(string translatedName, string language) {
            EnsureLanguageName(language);
            if (language == DefaultLanguage) {
                return translatedName;
            }
            if (!PluginTranslations.ContainsKey(language)) {
                return translatedName;
            }
            var result = PluginTranslations[language].FirstOrDefault(e => e.Value == translatedName);
            return result == null ? translatedName : result.Name;
        }

        /// <summary>
        /// 添加插件名翻译
        /// <para>如果发生冲突，该操作会覆盖已有的同项同语言翻译</para>
        /// </summary>
        /// <param name="name">插件名</param>
        /// <param name="translatedName">目标语言下的插件名</param>
        /// <param name="language">目标语言</param>
        public static void SetPluginName(string name, string translatedName, string language) {
            EnsureLanguageName(language);
            if (language == DefaultLanguage) {
                return;
            }
            List<Translation> translations;
            if (PluginTranslations.ContainsKey(language)) {
                translations = StaticTranslations[language];
            } else {
                PluginTranslations.Add(language, translations = new List<Translation>());
            }
            var result = translations.FirstOrDefault(e => e.Name == name);
            if (result == null) {
                result = new Translation {Name = name, Value = translatedName};
                translations.Add(result);
            } else {
                result.Value = translatedName;
            }
        }

        /// <summary>
        /// 删除插件名翻译
        /// </summary>
        /// <param name="name">插件名</param>
        /// <param name="language">目标语言（为空代表移除所有语言中的对应翻译）</param>
        public static void RemovePluginName(string name, string language = null) {
            EnsureLanguageName(language);
            foreach (var list in PluginTranslations.Where(e => language == null || e.Key == language).Select(e => e.Value)) {
                list.RemoveAll(e => e.Name == name);
            }
        }

        /// <summary>
        /// 获取静态翻译
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string GetStatic(string name, string language = DefaultLanguage) {
            EnsureLanguageName(language);
            return StaticTranslations.ContainsKey(name) ? StaticTranslations[name].FirstOrDefault(e => e.Name == language)?.Value : null;
        }

        /// <summary>
        /// 添加静态翻译
        /// <para>如果发生冲突，该操作会覆盖已有的同项同语言翻译</para>
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="value">项值</param>
        /// <param name="language">目标语言</param>
        public static void SetStatic(string name, string value, string language = DefaultLanguage) {
            EnsureLanguageName(language);
            List<Translation> translations;
            if (StaticTranslations.ContainsKey(name)) {
                translations = StaticTranslations[name];
            } else {
                StaticTranslations.Add(name, translations = new List<Translation>());
            }
            var result = translations.FirstOrDefault(e => e.Name == language);
            if (result == null) {
                result = new Translation {Name = language, Value = value};
                translations.Add(result);
            } else {
                result.Value = value;
            }
        }

        /// <summary>
        /// 删除静态翻译
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="language">目标语言（为空代表移除所有语言中的对应翻译）</param>
        public static void RemoveStatic(string name, string language = null) {
            EnsureLanguageName(language);
            if (!StaticTranslations.ContainsKey(name)) return;
            if (language == null) {
                StaticTranslations.Remove(name);
            } else {
                StaticTranslations[name].RemoveAll(e => e.Name == language);
            }
        }
        
        /// <summary>
        /// 检查语言名是否包含非法字符
        /// </summary>
        /// <param name="language">目标语言</param>
        public static bool CheckLanguageName(string language) {
            // 127: 7位int最大长度
            return language != null && language.Length < 127 && language.All(e => e >= '0' && e <= '9' || e >= 'a' && e <= 'z' || e >= 'A' && e <= 'Z' || e =='_');
        }

        private static void EnsureLanguageName(string language) {
            if (!CheckLanguageName(language)) {
                throw new ArgumentException("Language name must less than 127 characters and can only has numbers, alphabets, underlines");
            }
        }
    }
}