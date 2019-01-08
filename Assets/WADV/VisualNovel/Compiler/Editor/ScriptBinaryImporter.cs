using System;
using System.IO;
using WADV.VisualNovel.Runtime;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace WADV.VisualNovel.Compiler.Editor {
    /// <inheritdoc />
    /// <summary>
    /// 适用于Unity 2017.2+的二进制脚本导入器
    /// </summary>
    [ScriptedImporter(1, "vnb")]
    [UsedImplicitly]
    public class ScriptBinaryImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var script = ScriptableObject.CreateInstance<ScriptAsset>();
            script.content = File.ReadAllBytes(ctx.assetPath);
            script.id = CodeCompiler.CreatePathFromAsset(ctx.assetPath)?.SourceResource;
            if (string.IsNullOrEmpty(script.id)) {
                throw new NotSupportedException($"Unable to import visual novel binary {ctx.assetPath}: script id recognize failed");
            }
            ctx.AddObjectToAsset($"VNBinary:{ctx.assetPath}", script, AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/VNB Icon.png"));
            ctx.SetMainObject(script);
        }
    }
}