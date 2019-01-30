using System;
using System.IO;
using WADV.VisualNovel.Runtime;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using WADV.VisualNovel.ScriptStatus;

namespace WADV.VisualNovel.Compiler.Editor {
    /// <inheritdoc />
    /// <summary>
    /// 适用于Unity 2017.2+的VNB导入器
    /// </summary>
    [ScriptedImporter(1, "vnb")]
    [UsedImplicitly]
    public class ScriptBinaryImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var script = ScriptableObject.CreateInstance<ScriptAsset>();
            script.content = File.ReadAllBytes(ctx.assetPath);
            script.id = ScriptInformation.CreateIdFromAsset(ctx.assetPath);
            if (string.IsNullOrEmpty(script.id))
                throw new NotSupportedException($"Unable to import visual novel binary {ctx.assetPath}: script id recognize failed");
            ctx.AddObjectToAsset($"VNBinary:{ctx.assetPath}", script, EditorGUIUtility.Load("File Icon/VNB Icon.png") as Texture2D);
            ctx.SetMainObject(script);
            if (CompileConfiguration.Content.Scripts.ContainsKey(script.id)) {
                var information = CompileConfiguration.Content.Scripts[script.id];
                if (information.Binary.HasValue && !string.IsNullOrEmpty(information.Binary.Value.Asset) && information.Binary.Value.Asset == ctx.assetPath) return;
                information.Binary = new RelativePath {Asset = ctx.assetPath};
                information.RecordedHash = ScriptInformation.ReadBinaryHash(script.content);
                CompileConfiguration.Save();
            } else {
                ScriptInformation.CreateInformationFromAsset(ctx.assetPath);
            }
        }
    }
}