using UnityEngine;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("FadeOut")]
    public class FadeOut : ShaderGraphicEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        public FadeOut() : base("UI/Unlit/Fade") { }
        
        protected override Material OnStart(float time, SerializableValue[] parameters) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 1.0F);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Alpha, 1.0F - progress);
        }
    }
}