using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WADV.Extensions;
using WADV.Thread;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public abstract class ShaderGraphicEffect : ISingleGraphicEffect {
        protected readonly Shader EffectShader;
        private Graphic[] _targets;

        private static readonly Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();

        protected ShaderGraphicEffect(string shaderName) {
            if (Shaders.ContainsKey(shaderName)) {
                EffectShader = Shaders[shaderName];
            } else {
                EffectShader = Shader.Find(shaderName);
                if (EffectShader == null) throw new FileNotFoundException($"Unable to create ShaderImageEffect: expected shader {shaderName} not exist");
                Shaders.Add(shaderName, EffectShader);
            }
        }
        
        public void Initialize(Graphic[] graphics) {
            _targets = graphics;
        }

        public async Task Apply(float totalTime, Func<float, float> easing, SerializableValue[] parameters) {
            var material = OnStart(totalTime, parameters);
            foreach (var target in _targets) {
                target.material = material;
            }
            var currentTime = 0.0F;
            while (currentTime < totalTime) {
                currentTime += Time.deltaTime;
                OnFrame(material, Mathf.Clamp01(easing(currentTime / totalTime)));
                await Dispatcher.NextUpdate();
            }
        }

        protected abstract Material OnStart(float time, SerializableValue[] parameters);

        protected abstract void OnFrame(Material material, float progress);

        public void Reset() {
            foreach (var target in _targets) {
                target.material = null;
            }
        }
    }
}