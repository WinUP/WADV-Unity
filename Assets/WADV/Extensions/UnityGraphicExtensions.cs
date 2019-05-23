using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace WADV.Extensions {
    public static class UnityGraphicExtensions {
        /// <summary>
        /// 复制为2D材质
        /// </summary>
        /// <param name="value">目标渲染材质</param>
        /// <param name="rect">截取区域</param>
        /// <returns></returns>
        public static Texture2D CopyAsTexture2D(this RenderTexture value, RectInt rect) {
            var result = new Texture2D(rect.width, rect.height, TextureFormat.ARGB32, false);
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

        /// <summary>
        /// 激活渲染材质
        /// </summary>
        /// <param name="value">目标渲染材质</param>
        /// <returns></returns>
        [CanBeNull]
        public static RenderTexture Active(this RenderTexture value) {
            var current = RenderTexture.active;
            RenderTexture.active = value;
            return current;
        }
        
        /// <summary>
        /// 获取2D纹理的不透明区域（全透明则返回RectInt{0, 0, 0, 0}）
        /// </summary>
        /// <param name="texture">目标2D纹理</param>
        /// <returns></returns>
        public static RectInt GetVisibleContentArea(this Texture2D texture) {
            var pixels = texture.GetPixels32().Select(e => e.a > 0).ToArray();
            GC.Collect();
            int borderLeft = 0, borderRight = 0, borderTop = 0, borderBottom = 0, width = texture.width, height = texture.height;
            bool found;
            // 从上到下、从左至右扫描，第一次发现非全透明像素时记为上边界、左边界、右边界
            for (var i = -1; ++i < height;) {
                found = false;
                for (var j = -1; ++j < width;) {
                    if (!pixels[i * width + j]) continue;
                    borderTop = i;
                    borderLeft = j;
                    borderRight = j;
                    found = true;
                    break;
                }
                if (found) break;
                if (i == width - 1) return new RectInt(0, 0, 0, 0);
            }
            // 从下到上（不超过上边界）、从左至右扫描，第一次发现非全透明像素时记为下边界，同时如果该像素纵坐标比左边界小则更新左边界，纵坐标比右边界大则更新右边界
            for (var i = height; --i >= borderTop;) {
                found = false;
                for (var j = -1; ++j < width;) {
                    if (!pixels[i * width + j]) continue;
                    borderBottom = i;
                    borderLeft = j < borderLeft ? j : borderLeft;
                    borderRight = j > borderRight ? j : borderRight;
                    found = true;
                    break;
                }
                if (found) break;
            }
            // 从左到右（小于左边界）、从上至下（上边界的下一行至下边界的上一行）扫描，第一次发现非全透明像素时记为左边界
            for (var i = -1; ++i < borderLeft;) {
                found = false;
                for (var j = borderTop; ++j < borderBottom;) {
                    if (!pixels[j * width + i]) continue;
                    borderLeft = i;
                    found = true;
                    break;
                }
                if (found) break;
            }
            // 从右到左（小于右边界）、从上至下（上边界至下边界）扫描，第一次发现非全透明像素时记为右边界
            for (var i = width; --i > borderRight;) {
                found = false;
                for (var j = borderTop - 1; ++j <= borderBottom;) {
                    if (!pixels[j * width + i]) continue;
                    borderRight = i;
                    found = true;
                    break;
                }
                if (found) break;
            }
            return new RectInt(borderLeft, borderTop, borderRight - borderLeft + 1, borderBottom - borderTop + 1);
        }

        /// <summary>
        /// 复制2D纹理的部分或全部到新的2D纹理
        /// </summary>
        /// <param name="texture">目标2D纹理</param>
        /// <param name="rect">复制区域</param>
        /// <param name="fillColor">超出材质大小部分的填充颜色</param>
        /// <returns></returns>
        public static Texture2D Cut(this Texture2D texture, RectInt rect, Color fillColor) {
            var result = new Texture2D(rect.width, rect.height, TextureFormat.RGBA32, false);
            result.SetPixels(Enumerable.Repeat(fillColor, rect.width * rect.height).ToArray());
            Graphics.CopyTexture(texture, 0, 0, rect.x, rect.y, Mathf.Min(texture.width, rect.width), Mathf.Min(texture.height, rect.height), result, 0, 0, 0, 0);
            result.Apply(false);
            return result;
        }
        
        /// <summary>
        /// 复制2D纹理的部分或全部到新的2D纹理，超出大小的部分填充为透明
        /// </summary>
        /// <param name="texture">目标2D纹理</param>
        /// <param name="rect">复制区域</param>
        /// <returns></returns>
        public static Texture2D Cut(this Texture2D texture, RectInt rect) {
            return Cut(texture, rect, Vector4.zero);
        }
    }
}