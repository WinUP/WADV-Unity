using System;

namespace Core.VisualNovel.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class VisualNovelPluginNameAttribute : Attribute {
        public string Language { get; }
        public string Name { get; }

        public VisualNovelPluginNameAttribute(string language, string name) {
            Language = language;
            Name = name;
        }
    }
}