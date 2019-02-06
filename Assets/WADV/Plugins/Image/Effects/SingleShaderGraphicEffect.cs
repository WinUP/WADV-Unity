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
    /// <inheritdoc />
    /// <summary>
    /// 基于Shader的单程UI元素效果
    /// </summary>
    public abstract class SingleShaderGraphicEffect : ISingleGraphicEffect {
        /// <summary>
        /// 该效果使用的Shader
        /// </summary>
        protected readonly Shader EffectShader;
        private Dictionary<string, SerializableValue> _parameters;

        private static readonly Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();

        /// <summary>
        /// 创建一个基于Shader的单程UI元素效果
        /// </summary>
        /// <param name="shaderName">Shader名称</param>
        protected SingleShaderGraphicEffect(string shaderName) {
            if (Shaders.ContainsKey(shaderName)) {
                EffectShader = Shaders[shaderName];
            } else {
                EffectShader = Shader.Find(shaderName);
                if (EffectShader == null) throw new FileNotFoundException($"Unable to create ShaderImageEffect: expected shader {shaderName} not exist");
                Shaders.Add(shaderName, EffectShader);
            }
        }

        public void CreateEffect(Dictionary<string, SerializableValue> parameters) {
            _parameters = parameters;
        }
        
        public async Task PlayEffect(IEnumerable<Graphic> targets, float totalTime, Func<float, float> easing) {
            var material = OnStart(totalTime, _parameters);
            foreach (var target in targets) {
                target.material = material;
            }
            var currentTime = 0.0F;
            while (currentTime < totalTime) {
                currentTime += Time.deltaTime;
                OnFrame(material, Mathf.Clamp01(easing(currentTime / totalTime)));
                await Dispatcher.NextUpdate();
            }
        }

        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <param name="time">效果预计播放时间</param>
        /// <param name="parameters">效果参数</param>
        /// <returns></returns>
        protected abstract Material OnStart(float time, Dictionary<string, SerializableValue> parameters);

        /// <summary>
        /// 逐帧更新材质参数
        /// </summary>
        /// <param name="material">目标材质</param>
        /// <param name="progress">效果播放进度</param>
        protected abstract void OnFrame(Material material, float progress);
    }
}