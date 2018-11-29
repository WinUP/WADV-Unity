using System;

namespace Core.VisualNovel.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class VisualNovelPluginParameterAttribute : Attribute {
        public string Language { get; }
        public string[] Parameters { get; }

        public VisualNovelPluginParameterAttribute(string language, params string[] parameters) {
            Language = language;
            Parameters = parameters;
        }
    }
}