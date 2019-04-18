using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Plugins.Unity;
using WADV.Reflection;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("TextureMask")]
    [UsedImplicitly]
    public class TextureMaskTransition : SingleShaderGraphicEffect {
        protected override string ShaderId { get; } = "UI/Unlit/TextureMaskTransition";
        
        private static readonly int Threshold = Shader.PropertyToID("_Threshold");
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        private static readonly int MaskTexture = Shader.PropertyToID("_MaskTex");
        private static readonly int TargetTexture = Shader.PropertyToID("_TargetTex");

        [CanBeNull]
        private Texture2D _texture;
        private float _threshold;
        

        public override async Task Initialize() {
            await base.Initialize();
            if (Parameters.ContainsKey("Mask") && Parameters["Mask"] is Texture2DValue texture2DValue) {
                await texture2DValue.ReadTexture();
                _texture = texture2DValue.texture;
            }
            if (Parameters.ContainsKey("Threshold")) {
                _threshold = FloatValue.TryParse(Parameters["Threshold"]);
            }
        }

        protected override Material CreateMaterial(Texture2D nextTexture) {
            var material = new Material(EffectShader);
            material.SetFloat(Threshold, _threshold);
            material.SetTexture(MaskTexture, _texture);
            material.SetTexture(TargetTexture, nextTexture);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Progress, progress);
        }
    }
}