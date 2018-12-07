using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEditor;

namespace Core.VisualNovel.Script {
    /// <inheritdoc />
    [ScriptedImporter(1, "vns")]
    [UsedImplicitly]
    public class ScriptImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var text = new TextAsset(File.ReadAllText(ctx.assetPath, Encoding.UTF8));
            ctx.AddObjectToAsset($"VNScript:{ctx.assetPath}", text);
            ctx.SetMainObject(text);
        }

        [MenuItem("Assets/Create/VisualNovelScript", false, 0)]
        public static void CreateScriptFile() {
            var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            File.WriteAllText(Path.Combine(selectPath, "NewScript.vns"), "// Write your script here\n\n", Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}