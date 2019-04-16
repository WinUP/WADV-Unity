using System;
using UnityEngine;
using WADV.Extensions;

namespace WADV {
    /// <summary>
    /// 支持GPU加速的Texture2D绘制工具
    /// </summary>
    public class Texture2DCombiner {
        public const string ShaderDrawTextureOverlayKernelName = "DrawTextureOverlay";
        public const string ShaderDrawTextureAlphaMaskKernelName = "DrawTextureAlphaMask";
        public const string ShaderDrawTextureReversedAlphaMaskKernelName = "DrawTextureReversedAlphaMask";
        public const string ShaderFillAreaKernelName = "FillArea";
        public static readonly int ShaderCanvasName = Shader.PropertyToID("Canvas");
        public static readonly int ShaderSizeName = Shader.PropertyToID("Size");
        public static readonly int ShaderSourceSizeName = Shader.PropertyToID("SourceSize");
        public static readonly int ShaderSourceName = Shader.PropertyToID("Source");
        public static readonly int ShaderPivotDistanceName = Shader.PropertyToID("PivotDistance");
        public static readonly int ShaderTransformName = Shader.PropertyToID("Transform");
        public static readonly int ShaderColorName = Shader.PropertyToID("Color");

        private static readonly bool SupportsComputeShaders = SystemInfo.supportsComputeShaders;
        private static readonly Vector2 InterpolationOffset = new Vector2(0.5F, 0.5F);
        private readonly ComputeShader _shader;
        private readonly RenderTexture _renderCanvas;
        private readonly Texture2D _canvas;
        private readonly int _overlayKernel;
        private readonly int _alphaMaskKernel;
        private readonly int _reversedAlphaMaskKernel;
        private readonly int _fillKernel;

        /// <summary>
        /// 初始化一个新的Texture2D绘制工具
        /// </summary>
        /// <param name="width">画板宽度</param>
        /// <param name="height">画板高度</param>
        /// <param name="shader">线程组设置为24x24x1的ComputeShader（仅当平台支持时可用）</param>
        public Texture2DCombiner(int width, int height, ComputeShader shader = null) {
            if (SupportsComputeShaders && shader != null) {
                _renderCanvas = new RenderTexture(width, height, 24) {enableRandomWrite = true};
                _renderCanvas.enableRandomWrite = true;
                _shader = shader;
                _overlayKernel = _shader.FindKernel(ShaderDrawTextureOverlayKernelName);
                _alphaMaskKernel = _shader.FindKernel(ShaderDrawTextureAlphaMaskKernelName);
                _reversedAlphaMaskKernel = _shader.FindKernel(ShaderDrawTextureReversedAlphaMaskKernelName);
                _fillKernel = _shader.FindKernel(ShaderFillAreaKernelName);
                shader.SetTexture(_overlayKernel, ShaderCanvasName, _renderCanvas);
                shader.SetTexture(_alphaMaskKernel, ShaderCanvasName, _renderCanvas);
                shader.SetTexture(_reversedAlphaMaskKernel, ShaderCanvasName, _renderCanvas);
                shader.SetTexture(_fillKernel, ShaderCanvasName, _renderCanvas);
            } else {
                _canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);
            }
            Clear(new RectInt(0, 0, width, height));
        }
        
        /// <summary>
        /// 以(0,0)为轴点不叠加颜色下绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="mode">图像混合模式</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, MixMode mode = MixMode.Overlay) {
            return DrawTexture(texture, transform, Color.white, Vector2Int.zero, mode);
        }
        
        /// <summary>
        /// 不叠加颜色下绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="pivot">变换轴点</param>
        /// <param name="mode">图像混合模式</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, Vector2 pivot, MixMode mode = MixMode.Overlay) {
            return DrawTexture(texture, transform, Color.white, pivot, mode);
        }
        
        /// <summary>
        /// 以(0,0)为轴点绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="overlayColor">要叠加的颜色</param>
        /// <param name="mode">图像混合模式</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, Color overlayColor, MixMode mode = MixMode.Overlay) {
            return DrawTexture(texture, transform, overlayColor, Vector2Int.zero, mode);
        }

        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="overlayColor">要叠加的颜色</param>
        /// <param name="pivot">变换轴点</param>
        /// <param name="mode">图像混合模式</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, Color overlayColor, Vector2 pivot, MixMode mode = MixMode.Overlay) {
            var width = texture.width;
            var height = texture.height;
            PrepareTransform(ref transform, in pivot, in width, in height, out var area);
            if (_renderCanvas == null) {
                var pixels = texture.GetPixels();
                var canvasPixels = _canvas.GetPixels(area.x, area.y, area.width, area.height);
                var areaX = area.xMax;
                var areaY = area.yMax;
                for (var i = area.y - 1; ++i < areaY;) {
                    for (var j = area.x - 1; ++j < areaX;) {
                        var position = (Vector2) transform.MultiplyPoint(new Vector2(j, i) + InterpolationOffset) - InterpolationOffset;
                        if (!position.x.InRange(0, width) || !position.y.InRange(0, height)) continue;
                        var canvasPixelIndex = j - area.x + (i - area.y) * area.width;
                        var origin = canvasPixels[canvasPixelIndex];
                        var target = BilinearInterpolation(position, pixels, width) * overlayColor;
                        canvasPixels[canvasPixelIndex] = MixColor(in origin, in target, mode);
                    }
                }
                _canvas.SetPixels(area.x, area.y, area.width, area.height, canvasPixels);
            } else {
                var kernel = GetKernel(mode);
                _shader.SetVector(ShaderSizeName, new Vector4(area.x, area.y, width, height));
                _shader.SetTexture(kernel, ShaderSourceName, texture);
                _shader.SetVector(ShaderColorName, overlayColor);
                _shader.SetMatrix(ShaderTransformName, transform);
                var currentTexture = RenderTexture.active;
                RenderTexture.active = _renderCanvas;
                _shader.Dispatch(kernel, Mathf.CeilToInt(area.width / 24.0F), Mathf.CeilToInt(area.height / 24.0F), 1);
                RenderTexture.active = currentTexture;
            }
            return this;
        }
        
        /// <summary>
        /// 以(0,0)为轴点向指定区域填充颜色
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(RectInt area, Color targetColor) {
            return FillArea(area.size, Matrix4x4.Translate(area.position.ToVector2()), targetColor, Vector2Int.zero);
        }
        
        /// <summary>
        /// 向指定区域填充颜色
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(RectInt area, Color targetColor, Vector2 pivot) {
            return FillArea(area.size, Matrix4x4.Translate(area.position.ToVector2()), targetColor, pivot);
        }

        /// <summary>
        /// 以(0,0)为轴点填充颜色
        /// </summary>
        /// <param name="size">区域大小</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(Vector2Int size, Matrix4x4 transform, Color targetColor) {
            return FillArea(size, transform, targetColor, Vector2Int.zero);
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="size">区域大小</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(Vector2Int size, Matrix4x4 transform, Color targetColor, Vector2 pivot) {
            var width = size.x;
            var height = size.y;
            PrepareTransform(ref transform, in pivot, in width, in height, out var area);
            if (_renderCanvas == null) {
                var canvasPixels = _canvas.GetPixels(area.x, area.y, area.width, area.height);
                var areaX = area.xMax;
                var areaY = area.yMax;
                for (var i = area.y - 1; ++i < areaY;) {
                    for (var j = area.x - 1; ++j < areaX;) {
                        var position = (Vector2) transform.MultiplyPoint(new Vector2(j, i) + InterpolationOffset) - InterpolationOffset;
                        if (!position.x.InRange(0, width) || !position.y.InRange(0, height)) continue;
                        canvasPixels[j - area.x + (i - area.y) * area.width] = targetColor;
                    }
                }
                _canvas.SetPixels(area.x, area.y, area.width, area.height, canvasPixels);
            } else {
                _shader.SetVector(ShaderSizeName, new Vector4(area.x, area.y, width, height));
                _shader.SetVector(ShaderColorName, targetColor);
                _shader.SetMatrix(ShaderTransformName, transform);
                var currentTexture = RenderTexture.active;
                RenderTexture.active = _renderCanvas;
                var x = Mathf.CeilToInt(width / 24.0F);
                _shader.Dispatch(_fillKernel, Mathf.CeilToInt(area.width / 24.0F), Mathf.CeilToInt(area.height / 24.0F), 1);
                RenderTexture.active = currentTexture;
            }
            return this;
        }
        
        /// <summary>
        /// 以(0,0)为轴点将指定区域填充为透明
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(RectInt area) {
            return FillArea(area, Color.clear, Vector2Int.zero);
        }
        
        /// <summary>
        /// 将指定区域填充为透明
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(RectInt area, Vector2 pivot) {
            return FillArea(area, Color.clear, pivot);
        }
        
        /// <summary>
        /// 以(0,0)为轴点填充为透明
        /// </summary>
        /// <param name="size">目标区域</param>
        /// <param name="transform">变换矩阵</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(Vector2Int size, Matrix4x4 transform) {
            return FillArea(size, transform, Color.clear, Vector2Int.zero);
        }

        /// <summary>
        /// 填充为透明
        /// </summary>
        /// <param name="size">目标区域</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(Vector2Int size, Matrix4x4 transform, Vector2 pivot) {
            return FillArea(size, transform, Color.clear, pivot);
        }

        /// <summary>
        /// 应用绘图操作并获取画板
        /// </summary>
        /// <returns></returns>
        public Texture2D Combine() {
            if (_renderCanvas != null)
                return _renderCanvas.CopyAsTexture2D();
            _canvas.Apply(false);
            return _canvas;
        }

        private static Color BilinearInterpolation(Vector2 position, Color[] pixels, int width) {
            var height = pixels.Length / width;
            var u = position.x - (int) position.x;
            var v = position.y - (int) position.y;
            if (u.Equals(0) && v.Equals(0)) return pixels[(int) position.y * width + (int) position.x];
            var topLeft = pixels[Math.Min((int) position.y + 1, height) * width + (int) position.x];
            var bottomLeft = pixels[(int) position.y * width + (int) position.x];
            var topRight = pixels[Math.Min((int) position.y + 1, height) * width + Math.Min((int) position.x + 1, width)];
            var bottomRight = pixels[(int) position.y * width + Math.Min((int) position.x + 1, width)];
            return v * (u * topRight + (1 - u) * topLeft) + (1 - v) * (u * bottomRight + (1 - u) * bottomLeft);
        }
        
        private void PrepareTransform(ref Matrix4x4 transform, in Vector2 pivot, in int width, in int height, out RectInt area) {
            var distance = new Vector2(width, height) * pivot;
            var translation = transform.GetTranslation();
            transform.SetTranslation(Vector3.zero);
            transform.SetTranslation(translation - transform.MultiplyPoint(distance));
            area = transform.MultiplyRect(new Rect(0, 0, width, height))
                            .MaximizeToRectInt()
                            .OverlapWith(_renderCanvas == null
                                             ? new RectInt(0, 0, _canvas.width, _canvas.height)
                                             : new RectInt(0, 0, _renderCanvas.width, _renderCanvas.height));
            transform = transform.inverse;
        }

        private static Color MixColor(in Color origin, in Color target, MixMode mode) {
            Color result;
            switch (mode) {
                case MixMode.AlphaMask:
                    result = origin;
                    result.a = 1.0F - target.a;
                    break;
                case MixMode.ReversedAlphaMask:
                    result = origin;
                    result.a = target.a;
                    break;
                default:
                    result = target * target.a + origin * (1.0F - target.a);
                    result.a = origin.a.Equals(0) ? target.a : origin.a;
                    break;
            }
            return result;
        }

        private int GetKernel(MixMode mode) {
            switch (mode) {
                case MixMode.AlphaMask:
                    return _alphaMaskKernel;
                case MixMode.ReversedAlphaMask:
                    return _reversedAlphaMaskKernel;
                default:
                    return _overlayKernel;
            }
        }

        /// <summary>
        /// 图像混合模式
        /// </summary>
        public enum MixMode {
            /// <summary>
            /// 叠加
            /// </summary>
            Overlay,
            /// <summary>
            /// 透明蒙版
            /// </summary>
            AlphaMask,
            /// <summary>
            /// 不透明蒙版
            /// </summary>
            ReversedAlphaMask
        }
    }
}