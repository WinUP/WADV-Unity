using System.Collections.Generic;
using UnityEngine;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("FadeOut")]
    public class FadeOut : SingleShaderGraphicEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        public FadeOut() : base("UI/Unlit/Fade") { }

        protected override Material CreateMaterial(Dictionary<string, SerializableValue> parameters, Texture2D targetTexrure) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 1.0F);
            return material;
        }

        protected override void OnFrame(Material material, float totalTime, float progress) {
            material.SetFloat(Alpha, 1.0F - progress);
        }
    }
}