using System.Threading.Tasks;
using WADV.VisualNovel.Interoperation;

namespace WADV.Plugins.Image.Effects {
    public interface ILoopGraphicEffect : IGraphicEffect {
        Task StartEffect(float totalTime, SerializableValue[] parameters);

        Task EndEffect(float totalTime, SerializableValue[] parameters);

        void OnFrame(float progress);
    }
}