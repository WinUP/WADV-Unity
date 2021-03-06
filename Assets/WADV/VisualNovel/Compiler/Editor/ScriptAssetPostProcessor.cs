using System.IO;
using System.Linq;
using UnityEditor;
using WADV.Extensions;
using WADV.VisualNovel.ScriptStatus;

namespace WADV.VisualNovel.Compiler.Editor {
    public class ScriptAssetPostProcessor : AssetPostprocessor {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            // 重命名
            var movingFiles = movedFromAssetPaths
                              .WithIndex()
                              .Where(e => e.Item.EndsWith(".vns") || e.Item.EndsWith(".vnb") || e.Item.EndsWith(".txt"))
                              .Select(e => (From: e.Item, To: movedAssets[e.Index]));
            foreach (var (movedFromAsset, moveToAsset) in movingFiles) {
                var origin = ScriptInformation.CreateInformationFromAsset(movedFromAsset);
                if (origin == null) continue;
                var target = ScriptInformation.CreateInformationFromAsset(moveToAsset);
                if (target == null) {
                    CompileConfiguration.Content.Scripts.Remove(origin.id);
                    continue;
                }
                if (movedFromAsset.EndsWith(".vns")) {
                    if (moveToAsset.EndsWith(".vns")) { // vns -> vns
                        // 同步Hash
                        target.hash = target.hash ?? origin.hash;
                        // 移动翻译文件
                        foreach (var (language, path) in origin.Translations) {
                            if (!target.Translations.ContainsKey(language)) {
                                target.Translations.Add(language, path);
                            }
                            var from = origin.LanguageAssetPath(language);
                            var to = target.LanguageAssetPath(language);
                            if (from != to && File.Exists(from)) {
                                File.Move(from, to);
                            }
                        }
                        // 移动编译文件
                        target.recordedHash = target.recordedHash ?? origin.recordedHash;
                        var binaryFile = origin.BinaryAssetPath();
                        var targetBinaryFile = target.BinaryAssetPath();
                        if (binaryFile != targetBinaryFile && File.Exists(binaryFile)) {
                            File.Move(binaryFile, targetBinaryFile);
                        }
                    }
                    // 删除旧配置
                    CompileConfiguration.Content.Scripts.Remove(origin.id);
                } else if (movedFromAsset.EndsWith(".vnb")) {
                    if (moveToAsset.EndsWith(".vnb")) { // vnb -> vnb
                        // 同步Hash
                        target.recordedHash = target.recordedHash ?? origin.recordedHash;
                        // 移动翻译文件
                        foreach (var (language, path) in origin.Translations) {
                            if (!target.Translations.ContainsKey(language)) {
                                target.Translations.Add(language, path);
                            }
                            var from = origin.LanguageAssetPath(language);
                            var to = target.LanguageAssetPath(language);
                            if (from != to && File.Exists(from)) {
                                File.Move(from, to);
                            }
                        }
                        // 移动源文件
                        target.hash = target.hash ?? origin.hash;
                        var sourceFile = origin.SourceAssetPath();
                        var targetSourceFile = target.SourceAssetPath();
                        if (sourceFile != targetSourceFile && File.Exists(sourceFile)) {
                            File.Move(sourceFile, targetSourceFile);
                        }
                    }
                    // 删除旧配置
                    CompileConfiguration.Content.Scripts.Remove(origin.id);
                }
            }
            // 处理删除
            foreach (var file in deletedAssets.Where(e => e.EndsWith(".vns") || e.EndsWith(".txt") || e.EndsWith(".vnb"))) {
                var id = ScriptInformation.CreateIdFromAsset(file);
                if (id == null) continue;
                if (CompileConfiguration.Content.Scripts.ContainsKey(id)) {
                    CompileConfigurationWindow.RescanScriptInformation(CompileConfiguration.Content.Scripts[id]);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}