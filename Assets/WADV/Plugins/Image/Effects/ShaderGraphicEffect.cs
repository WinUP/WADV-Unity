using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public abstract class ShaderGraphicEffect {
        /// <summary>
        /// 该效果使用的Shader
        /// </summary>
        protected readonly Shader EffectShader;
        protected Dictionary<string, SerializableValue> Parameters;

        private static readonly Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        
        /// <summary>
        /// 创建一个基于Shader的单程UI元素效果
        /// </summary>
        /// <param name="shaderName">Shader名称</param>
        protected ShaderGraphicEffect(string shaderName) {
            if (Shaders.ContainsKey(shaderName)) {
                EffectShader = Shaders[shaderName];
            } else {
                EffectShader = Shader.Find(shaderName);
                if (EffectShader == null) throw new FileNotFoundException($"Unable to create ShaderImageEffect: expected shader {shaderName} not exist");
                Shaders.Add(shaderName, EffectShader);
            }
        }
        
        public void SetEffect(Dictionary<string, SerializableValue> parameters) {
            Parameters = parameters;
        }
        
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <param name="parameters">效果参数</param>
        /// <returns></returns>
        protected abstract Material CreateMaterial(Dictionary<string, SerializableValue> parameters);
    }
}