using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Reflection;

namespace WADV.Resource.Providers {
    [StaticRegistrationInfo("Shader")]
    [UsedImplicitly]
    public class UnityShaderResourceProvider : IResourceProvider {
        public async Task<object> Load(string id) {
            var index = id.IndexOf("://", StringComparison.InvariantCulture);
            return index > 0 && index < id.Length - 1 ? await ResourceManager.Load<Shader>(id) : Shader.Find(id);
        }
    }
}