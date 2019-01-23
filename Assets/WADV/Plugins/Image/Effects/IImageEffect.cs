using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public interface IImageEffect {
        void Initialize(RawImage[] image);
        
        Task Apply(float time, Func<float, float> easing, SerializableValue[] parameters);

        void Reset();
    }
}