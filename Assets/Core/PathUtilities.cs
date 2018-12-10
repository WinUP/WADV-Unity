using System;
using System.IO;

namespace Core {
    public static class PathUtilities {
        public const string BinaryFile = BinaryResource + ".bytes";
        public const string BinaryResource = ".bin";
        public const string TranslationFileFormat = TranslationResourceFormat + ".txt";
        public const string TranslationResourceFormat = ".tr.{0}";
        public const string BaseDirectory = "Assets/Resources/";

        public static string DropExtension(string path) {
            return Path.Combine(Path.GetDirectoryName(path) ?? "", Path.GetFileNameWithoutExtension(path) ?? "");
        }

        public static string Combine(string path, string extensionFormat, params object[] parts) {
            return parts.Length > 0
                ? $"{path}{string.Format(extensionFormat, parts)}"
                : $"{path}{extensionFormat}";
        }
    }
}