using System;

namespace Core.VisualNovel.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RegisterTranslateAttribute : Attribute {
        public string Language { get; }
        public string Name { get; }
        public string Value { get; }

        public RegisterTranslateAttribute(string language, string name, string value) {
            Language = language;
            Name = name;
            Value = value;
        }
    }
}