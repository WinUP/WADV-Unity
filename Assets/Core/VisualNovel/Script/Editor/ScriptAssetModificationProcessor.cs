//using System;
//using System.Linq;
//using Core.VisualNovel.Translation;
//using UnityEditor;
//
//namespace Core.VisualNovel.Script.Editor {
//    /// <inheritdoc />
//    /// <summary>
//    /// 用于监控文件树变化并自动应用至编译选项的监听器
//    /// </summary>
//    public class ScriptAssetModificationProcessor : UnityEditor.AssetModificationProcessor {
//        private static void OnWillCreateAsset(string assetName) {
//            if (!assetName.StartsWith("Assets/Resources/")) {
//                return;
//            }
//            var (fileId, fileLanguage) = SplitPath(assetName);
//            if (fileLanguage == null) {
//                // VNS file
//                var option = CompileOptions.Get(fileId);
//                foreach (var file in PathUtilities.FindFileNameGroup(fileId).Where(e => e.EndsWith(".txt"))) {
//                    var (_, language) = SplitPath(file);
//                    if (TranslationManager.CheckLanguageName(language) && !option.ExtraTranslationLanguages.Contains(language)) {
//                        option.ExtraTranslationLanguages.Add(language);
//                        CompileOptions.Save();
//                    }
//                }
//            } else {
//                // Translation file
//                if (!CompileOptions.Has(fileId)) {
//                    return;
//                }
//                var option = CompileOptions.Get(fileId);
//                if (TranslationManager.CheckLanguageName(fileLanguage) && !option.ExtraTranslationLanguages.Contains(fileLanguage)) {
//                    option.ExtraTranslationLanguages.Add(fileLanguage);
//                    CompileOptions.Save();
//                }
//            }
//        }
//
//        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options) {
//            if (!assetPath.StartsWith(PathUtilities.BaseDirectory)) {
//                return AssetDeleteResult.DidNotDelete;
//            }
//            var (fileId, fileLanguage) = SplitPath(assetPath);
//            if (fileLanguage == null) {
//                // VNS file
//                CompileOptions.Remove(fileId);
//            } else {
//                // Translation file
//                if (!CompileOptions.Has(fileId)) {
//                    return AssetDeleteResult.DidNotDelete;
//                }
//                var option = CompileOptions.Get(fileId);
//                if (TranslationManager.CheckLanguageName(fileLanguage) && option.ExtraTranslationLanguages.Contains(fileLanguage)) {
//                    option.ExtraTranslationLanguages.Remove(fileLanguage);
//                    CompileOptions.Save();
//                }
//            }
//            return AssetDeleteResult.DidNotDelete;
//        }
//        
//        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath) {
//            OnWillDeleteAsset(sourcePath, RemoveAssetOptions.MoveAssetToTrash);
//            OnWillCreateAsset(destinationPath);
//            return AssetMoveResult.DidNotMove;
//        }
//
//        private static (string ID, string Language) SplitPath(string source) {
//            source = PathUtilities.DropBase(source);
//            if (source.EndsWith(".meta")) {
//                source = PathUtilities.DropExtension(source);
//            }
//            source = PathUtilities.DropExtension(source);
//            var languageMark = string.Format(PathUtilities.TranslationResourceFormat, "");
//            var trMarkIndex = source.LastIndexOf(languageMark, StringComparison.Ordinal);
//            return trMarkIndex < 0
//                ? (source, null)
//                : (source.Substring(0, trMarkIndex), source.Substring(trMarkIndex + languageMark.Length));
//        }
//    }
//}