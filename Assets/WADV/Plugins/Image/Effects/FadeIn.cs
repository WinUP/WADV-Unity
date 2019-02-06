using System.Collections.Generic;
using UnityEngine;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    [StaticRegistrationInfo("FadeIn")]
    public class FadeIn : SingleShaderGraphicEffect {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        
        public FadeIn() : base("UI/Unlit/Fade") { }

        protected override Material OnStart(float time, Dictionary<string, SerializableValue> parameters) {
            var material = new Material(EffectShader);
            material.SetFloat(Alpha, 0.0F);
            return material;
        }

        protected override void OnFrame(Material material, float progress) {
            material.SetFloat(Alpha, progress);
        }
    }
}