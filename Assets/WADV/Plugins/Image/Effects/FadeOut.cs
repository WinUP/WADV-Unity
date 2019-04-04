using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("FadeOut")]
    [UsedImplicitly]
    public class FadeOut : SingleShaderGraphicEffect {
        protected override string ShaderName { get; } = "UI/Unlit/Fade";
        
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        protected override Material CreateMaterial(Texture2D nextTexture) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 1.0F);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Alpha, 1.0F - progress);
        }
    }
}