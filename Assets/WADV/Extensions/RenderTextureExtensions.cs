using UnityEngine;

namespace WADV.Extensions {
    public static class RenderTextureExtensions {
        /// <summary>
        /// 复制为2D材质
        /// </summary>
        /// <param name="value">目标渲染材质</param>
        /// <param name="rect">截取区域</param>
        /// <returns></returns>
        public static Texture2D CopyAsTexture2D(this RenderTexture value, RectInt rect) {
            var result = new Texture2D(rect.width, rect.height, TextureFormat.RGBA32, false);
            var currentRenderTarget = RenderTexture.active;
            RenderTexture.active = value;
            result.ReadPixels(rect.ToRect(), 0, 0); // 从当前RenderTexture读取数据，天知道Unity为何把函数叫这个名
            result.Apply();
            RenderTexture.active = currentRenderTarget;
            return result;
        }
        
        /// <summary>
        /// 复制为2D材质
        /// </summary>
        /// <param name="value">目标渲染材质</param>
        /// <returns></returns>
        public static Texture2D CopyAsTexture2D(this RenderTexture value) {
            return CopyAsTexture2D(value, new RectInt(0, 0, value.width, value.height));
        }
    }
}