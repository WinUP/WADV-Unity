using System;
using System.IO;
using System.Linq;
using Core.Extensions;
using Core.VisualNovel.Translation;
using UnityEditor;
using UnityEngine;

namespace Core.VisualNovel.Script.Editor {
    public class ScriptAssetModificationProcessor : UnityEditor.AssetModificationProcessor {
        private static void OnWillCreateAsset(string assetName) {
            if (!assetName.StartsWith(PathUtilities.BaseDirectory)) {
                return;
            }
            assetName = PathUtilities.DropBase(assetName);
            if (assetName.EndsWith(".meta")) {
                assetName = PathUtilities.DropExtension(assetName);
            }
            if (assetName.EndsWith(".vns")) {
                assetName = PathUtilities.DropExtension(assetName);
                var fileName = Path.GetFileName(assetName);
                if (fileName == null) {
                    return;
                }
                var option = CompileOptions.Get(assetName);
                foreach (var file in Directory.GetFiles(PathUtilities.Combine(PathUtilities.BaseDirectory, assetName)).Where(e => e.StartsWith(fileName) && e.EndsWith(".txt"))) {
                    var language = FindLanguageName(file);
                    if (TranslationManager.CheckLanguageName(language) && !option.ExtraTranslationLanguages.Contains(language)) {
                        option.ExtraTranslationLanguages.Add(language);
                    }
                }
            } else if (assetName.EndsWith(".txt")) {
                assetName = PathUtilities.DropExtension(assetName);
                var language = FindLanguageName(assetName);
                
            }
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options) {
            Debug.Log(assetPath);
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath) {
            Debug.Log(sourcePath);
            Debug.Log(destinationPath);
            return AssetMoveResult.DidNotMove;
        }

        private static string FindLanguageName(string path) {
            var mark = string.Format(PathUtilities.TranslationResourceFormat, "");
            var trMarkIndex = path.LastIndexOf(mark, StringComparison.Ordinal);
            if (trMarkIndex < 0) {
                return null;
            }
            return !CompileOptions.Has(path.Substring(0, trMarkIndex)) ? null : path.Substring(trMarkIndex + mark.Length);
        }
    }
}