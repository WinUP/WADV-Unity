using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static string DropBase(string path) {
            return path.StartsWith(BaseDirectory) ? path.Substring(BaseDirectory.Length) : path;
        }

        public static string Combine(string path, string extensionFormat, params object[] parts) {
            return parts.Length > 0 ? $"{path}{string.Format(extensionFormat, parts)}" : $"{path}{extensionFormat}";
        }

        public static IEnumerable<string> FindFileNameGroup(string id) {
            var basePath = Path.GetDirectoryName(Path.Combine(BaseDirectory, id));
            if (string.IsNullOrEmpty(basePath)) {
                return new string[] { };
            }
            id = Path.GetFileName(id) ?? id;
            return Directory.GetFiles(basePath).Where(e => e.StartsWith(id));
        }
    }
}