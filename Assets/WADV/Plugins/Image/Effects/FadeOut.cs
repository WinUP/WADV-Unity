using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public class FadeOut : ShaderImageEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        public FadeOut() : base("ImageEffects/Fade") { }
        
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