using System.IO;
using System.Linq;
using UnityEditor;
using WADV.Extensions;
using WADV.VisualNovel.ScriptStatus;

namespace WADV.VisualNovel.Compiler.Editor {
    public class ScriptAssetPostProcessor : AssetPostprocessor {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            // 重命名，按照vns -> bin -> lang的顺序处理
            var movingFiles = movedFromAssetPaths
                              .WithIndex()
                              .Where(e => e.item.EndsWith(".vns") || e.item.EndsWith(".vnb") || e.item.EndsWith(".txt"))
                              .Select(e => (From: e.item, To: movedAssets[e.index]))
                              .OrderBy(e => e.From.EndsWith(".vns") ? 0 : e.From.EndsWith(".vnb") ? 1 : 2);
            foreach (var (movedFromAsset, moveToAsset) in movingFiles) {
                var origin = ScriptInformation.CreateIdFromAsset(movedFromAsset);
                if (origin == null) continue;
                var originInfo = CompileConfiguration.Content.Scripts.ContainsKey(origin) ? CompileConfiguration.Content.Scripts[origin] : null;
                var target = ScriptInformation.CreateIdFromAsset(moveToAsset);
                var targetInfo = target != null && CompileConfiguration.Content.Scripts.ContainsKey(target) ? CompileConfiguration.Content.Scripts[target] : null;
                if (movedFromAsset.EndsWith(".vns")) {
                    if (moveToAsset.EndsWith(".vns") && originInfo != null && targetInfo != null) { // vns -> vns
                        targetInfo.Source = new RelativePath {Asset = targetInfo.Source?.Asset, Runtime = targetInfo.Source?.Runtime ?? originInfo.Source?.Runtime};
                        // 移动翻译文件
                        foreach (var (language, path) in originInfo.Translations) {
                            if (!targetInfo.Translations.ContainsKey(language)) {
                                targetInfo.Translations.Add(language, path);
                            }
                            var from = originInfo.GetLanguageAsset(language);
                            var to = targetInfo.GetLanguageAsset(language);
                            if (from != to && File.Exists(from)) {
                                File.Move(from, to);
                            }
                        }
                        // 移动编译文件
                        targetInfo.Binary = targetInfo.Binary ?? originInfo.Binary;
                        var binaryFile = originInfo.GetBinaryAsset();
                        var targetBinaryFile = targetInfo.GetBinaryAsset();
                        if (binaryFile != targetBinaryFile && File.Exists(binaryFile)) {
                            File.Move(binaryFile, targetBinaryFile);
                        }
                    }
                    // 删除旧配置
                    originInfo?.RemoveFromConfiguration();
                } else if (movedFromAsset.EndsWith(".vnb")) {
                    if (moveToAsset.EndsWith(".vnb") && originInfo != null && targetInfo != null) { // vnb -> vnb
                        targetInfo.Binary = new RelativePath {Asset = targetInfo.Binary?.Asset, Runtime = targetInfo.Binary?.Runtime ?? originInfo.Binary?.Runtime};
                        // 移动翻译文件
                        foreach (var (language, path) in originInfo.Translations) {
                            if (!targetInfo.Translations.ContainsKey(language)) {
                                targetInfo.Translations.Add(language, path);
                            }
                            var from = originInfo.GetLanguageAsset(language);
                            var to = targetInfo.GetLanguageAsset(language);
                            if (from != to && File.Exists(from)) {
                                File.Move(from, to);
                            }
                        }
                        // 移动源文件
                        targetInfo.Source = targetInfo.Source ?? originInfo.Source;
                        var sourceFile = originInfo.Source?.Asset;
                        var targetSourceFile = targetInfo.Source?.Asset;
                        if (!string.IsNullOrEmpty(sourceFile) && !string.IsNullOrEmpty(targetSourceFile) && sourceFile != targetSourceFile && File.Exists(sourceFile)) {
                            File.Move(sourceFile, targetSourceFile);
                        }
                    }
                    // 删除旧配置
                    originInfo?.RemoveFromConfiguration();
                }
            }
            // 处理删除
            foreach (var file in deletedAssets.Where(e => e.EndsWith(".vns") || e.EndsWith(".txt") || e.EndsWith(".vnb"))) {
                var id = ScriptInformation.CreateIdFromAsset(file);
                var target = CompileConfiguration.Content.Scripts.ContainsKey(id) ? CompileConfiguration.Content.Scripts[id] : null;
                target?.RemoveFromConfiguration();
            }
            AssetDatabase.Refresh();
        }
    }
}