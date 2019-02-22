using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("FadeIn")]
    [UsedImplicitly]
    public class FadeIn : SingleShaderGraphicEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        public FadeIn() : base("UI/Unlit/Fade") { }

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