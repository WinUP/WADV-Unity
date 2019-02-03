using System;
using System.IO;
using System.Linq;
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
            var text = new TextAsset(File.ReadAllText(ctx.assetPath, Encoding.UTF8));
            ctx.AddObjectToAsset($"VNScript:{ctx.assetPath}", text, EditorGUIUtility.Load("File Icon/VNS Icon.png") as Texture2D);
            ctx.SetMainObject(text);
            ScriptInformation.CreateInformationFromAsset(ctx.assetPath);
            if (!CompileConfiguration.Content.AutoCompile) return;
            try {
                CodeCompiler.CompileAsset(ctx.assetPath);
            } catch (CompileException compileException) {
                Debug.LogError($"Script {ctx.assetPath} contains error\n{compileException.Message}");
            } catch (Exception exception) {
                Debug.LogError($"Script {ctx.assetPath} compile failed\n{exception.Message}");
            }
            AssetDatabase.Refresh();
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