using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using WADV.VisualNovel.ScriptStatus;

namespace WADV.VisualNovel.Compiler.Editor {
    /// <inheritdoc />
    /// <summary>
    /// 适用于Unity 2017.2+的VNS导入器
    /// </summary>
    [ScriptedImporter(1, "vns")]
    public class ScriptImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var id = ScriptInformation.CreateIdFromAsset(ctx.assetPath);
            if (string.IsNullOrEmpty(id))
                throw new NotSupportedException($"Unable to import visual novel script {ctx.assetPath}: script id recognize failed");
            var text = new TextAsset(File.ReadAllText(ctx.assetPath, Encoding.UTF8));
            ctx.AddObjectToAsset($"VNScript:{ctx.assetPath}", text, EditorGUIUtility.Load("File Icon/VNS Icon.png") as Texture2D);
            ctx.SetMainObject(text);
            if (CompileConfiguration.Content.Scripts.ContainsKey(id)) {
                var information = CompileConfiguration.Content.Scripts[id];
                if (information.Source.HasValue && !string.IsNullOrEmpty(information.Source.Value.Asset) && information.Source.Value.Asset == ctx.assetPath) return;
                information.Source = new RelativePath {Asset = ctx.assetPath};
                CompileConfiguration.Save();
            } else {
                ScriptInformation.CreateInformationFromAsset(ctx.assetPath);
            }
        }

        /// <summary>
        /// 用于新建VNS文件的Unity资源管理器新建项目选单扩展
        /// </summary>
        [MenuItem("Assets/Create/VisualNovel Script", false, 82)]
        public static void CreateScriptFile() {
            var selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (File.Exists(selectPath)) {
                selectPath = Path.GetDirectoryName(selectPath) ?? selectPath;
            }
            ProjectWindowUtil.CreateAssetWithContent(Path.Combine(selectPath, "NewScript.vns"), "// Write your script here\n\n", EditorGUIUtility.Load("File Icon/VNS Icon.png") as Texture2D);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}