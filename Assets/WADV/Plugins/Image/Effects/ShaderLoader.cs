using System;
using System.Threading.Tasks;
using UnityEngine;
using WADV.Resource;

namespace WADV.Plugins.Image.Effects {
    public static class ShaderLoader {
        public static async Task<Shader> Load(string name) {
            var index = name.IndexOf("://", StringComparison.InvariantCulture);
            return index > 0 && index < name.Length - 1 ? await ResourceManager.Load<Shader>(name) : Shader.Find(name);
        }
    }
}