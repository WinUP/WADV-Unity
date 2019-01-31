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
            ctx.AddObjectToAsset($"VNBinary:{ctx.assetPath}", script, EditorGUIUtility.Load("File Icon/VNB Icon.png") as Texture2D);
            ctx.SetMainObject(script);
            ScriptInformation.CreateInformationFromAsset(ctx.assetPath);
        }
    }
}