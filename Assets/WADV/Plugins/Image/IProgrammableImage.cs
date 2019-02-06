using System.Threading.Tasks;
using UnityEngine;

namespace WADV.Plugins.Image {
    public abstract class ProgrammableImage : ScriptableObject {
        public abstract Task ShowImage();

        public virtual void UpdateImageFrame() { }

        public abstract Task HideImage();
    }
}