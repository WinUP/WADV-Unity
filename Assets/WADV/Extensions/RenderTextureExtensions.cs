using UnityEngine;

namespace WADV.Extensions {
    public static class RenderTextureExtensions {
        public static Texture2D CopyAsTexture2D(this RenderTexture value) {
            var result = new Texture2D(value.width, value.height, TextureFormat.RGBA32, false);
            var currentRenderTarget = RenderTexture.active;
            RenderTexture.active = value;
            result.ReadPixels(new Rect(0, 0, value.width, value.height), 0, 0); // 从当前RenderTexture读取数据，天知道Unity为何把函数叫这个名
            result.Apply();
            RenderTexture.active = currentRenderTarget;
            return result;
        }
    }
}