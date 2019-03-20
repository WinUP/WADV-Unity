using System;
using System.Collections.Generic;
using System.Linq;

namespace WADV.Translation {
    /// <summary>
    /// 静态翻译管理器
    /// </summary>
    public static class TranslationManager {
        public const string DefaultLanguage = "default";
        /// <summary>
        /// 静态翻译列表（项名, Name=语言, Value=项值）
        /// </summary>
        private static readonly Dictionary<string, List<Translation>> StaticTranslations = new Dictionary<string, List<Translation>>();

        /// <summary>
        /// 获取静态翻译
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string Get(string name, string language = DefaultLanguage) {
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
        public static void Set(string name, string value, string language = DefaultLanguage) {
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
        public static void Remove(string name, string language = null) {
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
            return !string.IsNullOrEmpty(language) && language.Length < 127 && language.All(e => e >= '0' && e <= '9' || e >= 'a' && e <= 'z' || e >= 'A' && e <= 'Z' || e =='_');
        }

        private static void EnsureLanguageName(string language) {
            if (!CheckLanguageName(language)) {
                throw new ArgumentException("Language name must less than 127 characters and can only has numbers, alphabets, underlines");
            }
        }
    }
}