using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.Plugins.Effect.Effects {
    [StaticRegistrationInfo("FadeIn")]
    [UsedImplicitly]
    public class FadeIn : SingleShaderGraphicEffect {
        protected override string ShaderId { get; } = "UI/Unlit/Fade";
        
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        protected override Material CreateMaterial(Texture2D nextTexture) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 0.0F);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Alpha, progress);
        }
    }
}