using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Script.Compiler;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    public class ScriptAssetPostProcessor : AssetPostprocessor {
        private enum AssetOperation {
            Import,
            Delete,
            MoveTo,
            MoveFrom
        }
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            var scripts = from script in importedAssets
                    .Where(e => e.EndsWith(".vns") || e.EndsWith(".txt") || e.EndsWith(".bin.bytes"))
                    .Select(e => (Item: CodeCompiler.CreatePathFromAsset(e), Operation: AssetOperation.Import))
                    .Concat(deletedAssets.Select(e => (Item: CodeCompiler.CreatePathFromAsset(e), Operation: AssetOperation.Delete)))
                    .Concat(movedAssets.Select(e => (Item: CodeCompiler.CreatePathFromAsset(e), Operation: AssetOperation.MoveTo)))
                    .Concat(movedFromAssetPaths.Select(e => (Item: CodeCompiler.CreatePathFromAsset(e), Operation: AssetOperation.MoveFrom)))
                group script by script.Item.SourceResource;
            foreach (var group in scripts) {
                var items = group.ToList();
            }
            
            // 处理重命名
            foreach (var (file, i) in movedFromAssetPaths.WithIndex()) {
                if (file.EndsWith(".vns")) {
                    
                }
            }
            Debug.Log(string.Join(", ", importedAssets)); // 新建，重命名后新文件名，内容变更
            Debug.Log(string.Join(", ", deletedAssets)); // 删除
            Debug.Log(string.Join(", ", movedAssets)); // 重命名后新文件名
            Debug.Log(string.Join(", ", movedFromAssetPaths)); // 重命名前的原文件名
        }
    }
}