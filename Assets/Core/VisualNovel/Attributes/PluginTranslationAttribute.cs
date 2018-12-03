using System;

namespace Core.VisualNovel.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PluginTranslationAttribute : Attribute {
        public string Language { get; }
        public string Name { get; }

        public PluginTranslationAttribute(string language, string name) {
            Language = language;
            Name = name;
        }
    }
}