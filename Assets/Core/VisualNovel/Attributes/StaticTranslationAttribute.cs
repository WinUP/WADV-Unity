using System;
using Core.VisualNovel.Translation;

namespace Core.VisualNovel.Attributes {
    /// <summary>
    /// 静态翻译自动注册特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class StaticTranslationAttribute : Attribute {
        public string Language { get; }
        public string Name { get; }
        public string Value { get; }

        /// <summary>
        /// 自动注册一个静态翻译
        /// </summary>
        /// <param name="name">项名</param>
        /// <param name="value">项值</param>
        /// <param name="language">目标语言</param>
        public StaticTranslationAttribute(string name, string value, string language = TranslationManager.DefaultLanguage) {
            Name = name;
            Value = value;
            Language = language;
        }
    }
}