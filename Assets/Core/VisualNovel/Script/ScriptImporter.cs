using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.VisualNovel.Script.Compiler;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Core.VisualNovel.Script {
    [ScriptedImporter(1, "vns")]
    [UsedImplicitly]
    public class ScriptImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var text = new TextAsset(File.ReadAllText(ctx.assetPath, Encoding.UTF8));
            ctx.AddObjectToAsset($"VNScript:{ctx.assetPath}", text);
            ctx.SetMainObject(text);
        }

        [MenuItem("Assets/Create/VisualNovel/Script", false, 82)]
        public static void CreateScriptFile() {
            var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            File.WriteAllText(Path.Combine(selectPath, $"{Guid.NewGuid().ToString()}.vns"), "// Write your script here\n\n", Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}