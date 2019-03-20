using UnityEngine;

namespace WADV.Extensions {
    public static class RenderTextureExtensions {
        public static Texture2D CopyAsTexture2D(this RenderTexture value, RectInt rect) {
            var result = new Texture2D(rect.width, rect.height, TextureFormat.RGBA32, false);
            var currentRenderTarget = RenderTexture.active;
            RenderTexture.active = value;
            result.ReadPixels(rect.ToRect(), 0, 0); // 从当前RenderTexture读取数据，天知道Unity为何把函数叫这个名
            result.Apply();
            RenderTexture.active = currentRenderTarget;
            return result;
        }
        
        public static Texture2D CopyAsTexture2D(this RenderTexture value) {
            return CopyAsTexture2D(value, new RectInt(0, 0, value.width, value.height));
        }
    }
}