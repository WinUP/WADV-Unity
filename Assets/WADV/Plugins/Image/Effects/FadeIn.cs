using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public class FadeIn : ShaderGraphicEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        public FadeIn() : base("UI/Unlit/Fade") { }

        protected override Material OnStart(float time, SerializableValue[] parameters) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 0.0F);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Alpha, progress);
        }
    }
}