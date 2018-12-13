using System;
using System.IO;
using Core.Extensions;
using Core.VisualNovel.Translation;

namespace Core.VisualNovel.Script.Compiler {
    /// <summary>
    /// 编译器快捷入口
    /// </summary>
    public static class CodeCompiler {
        /// <summary>
        /// 编译源代码
        /// </summary>
        /// <param name="source">源代码文本</param>
        /// <param name="identifier">源代码脚本ID</param>
        /// <returns></returns>
        public static (byte[] Content, ScriptTranslation DefaultTranslation) Compile(string source, CodeIdentifier identifier) {
            return ByteCodeGenerator.Generate(Parser.Parse(Lexer.Lex(source, identifier), identifier), identifier);
        }

        /// <summary>
        /// 根据脚本ID生成可能的各种此脚本的同组资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <returns></returns>
        public static (string Source, string SourceResource, string Binary, string BinaryResource, string Directory, string DirectoryResource) CreatePathFromId(string id) {
            var directory = Path.GetDirectoryName(id) ?? id;
            return (id + ".vns", id, id + ".bin.bytes", id + ".bin", $"Assets/Resources/{directory}", directory);
        }
        
        /// <summary>
        /// 根据资源路径生成可能的各种此脚本的同组资源路径
        /// </summary>
        /// <param name="asset">资源路径</param>
        /// <returns></returns>
        public static (string Source, string SourceResource, string Binary, string BinaryResource, string Directory, string DirectoryResource) CreatePathFromAsset(string asset) {
            var assetDirectory = Path.GetDirectoryName(asset) ?? asset;
            var assetWithoutExtension = asset.RemoveLast(".vns");
            var idDirectory = assetDirectory.Remove("Assets/Resources/");
            var id = assetWithoutExtension.Remove("Assets/Resources/");
            return (asset, id, assetWithoutExtension + ".bin.bytes", id + ".bin",  assetDirectory, idDirectory);
        }
        
        /// <summary>
        /// 根据翻译资源路径生成可能的各种此脚本的同组资源路径
        /// </summary>
        /// <param name="asset">资源路径</param>
        /// <returns></returns>
        public static (string Source, string SourceResource, string Binary, string BinaryResource, string Directory, string DirectoryResource, string Language) CreatePathFromLanguageAsset(string asset) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 确定指定路径中包含的文件名是否符合翻译文件格式
        /// </summary>
        /// <param name="path">目标路径</param>
        /// <returns></returns>
        public static bool IsLanguageFile(string path) {
            return (Path.GetFileName(path) ?? path).IndexOf(".tr.", StringComparison.Ordinal) > 0;
        }

        /// <summary>
        /// 根据脚本ID生成语言资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string CreateLanguageAssetPathFromId(string id, string language) {
            return $"Assets/Resources/{id}.tr.{language}.txt";
        }

        /// <summary>
        /// 根据脚本ID生成供Resources.Load使用的语言资源路径
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="language">目标语言</param>
        /// <returns></returns>
        public static string CreateLanguageResourcePathFromId(string id, string language) {
            return $"{id}.tr.{language}";
        }
    }
}