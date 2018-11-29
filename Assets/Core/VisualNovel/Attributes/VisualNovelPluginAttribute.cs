using System;

namespace Core.VisualNovel.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class VisualNovelPluginAttribute : Attribute {
        public byte[] Identifier { get; }

        public VisualNovelPluginAttribute(byte[] identifier) {
            if (identifier.Length != 4) {
                throw new ArgumentException("Identifier array's length must be 4");
            }
            Identifier = identifier;
        }
    }
}