using System.Linq;
using UnityEngine;
using WADV.Extensions;

namespace WADV {
    /// <summary>
    /// 支持GPU加速的Texture2D绘制工具
    /// </summary>
    public class Texture2DCombiner {
        public const string ShaderSetTextureKernelName = "SetTexture";
        public const string ShaderSetColorKernelName = "SetColor";
        public static readonly int ShaderCanvasName = Shader.PropertyToID("Canvas");
        public static readonly int ShaderSizeName = Shader.PropertyToID("Size");
        public static readonly int ShaderSourceName = Shader.PropertyToID("Source");
        public static readonly int ShaderTransformName = Shader.PropertyToID("Transform");
        public static readonly int ShaderColorName = Shader.PropertyToID("Color");

        private static readonly bool SupportsComputeShaders = SystemInfo.supportsComputeShaders;
        private readonly ComputeShader _shader;
        private readonly RenderTexture _renderCanvas;
        private readonly Texture2D _canvas;
        private readonly int _addKernel;
        private readonly int _fillKernel;

        /// <summary>
        /// 初始化一个新的Texture2D绘制工具
        /// </summary>
        /// <param name="width">画板宽度</param>
        /// <param name="height">画板高度</param>
        /// <param name="shader">线程组设置为24x24x1的ComputeShader（仅当平台支持时可用）</param>
        public Texture2DCombiner(int width, int height, ComputeShader shader = null) {
            if (SupportsComputeShaders && shader != null) {
                _renderCanvas = new RenderTexture(width + 2, height + 2, 24) {enableRandomWrite = true};
                _renderCanvas.enableRandomWrite = true;
                _shader = shader;
                _addKernel = _shader.FindKernel(ShaderSetTextureKernelName);
                _fillKernel = _shader.FindKernel(ShaderSetColorKernelName);
                shader.SetTexture(_addKernel, ShaderCanvasName, _renderCanvas);
                shader.SetTexture(_fillKernel, ShaderCanvasName, _renderCanvas);
            } else {
                _canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);
            }
            FillArea(new RectInt(0, 0, width, height), Color.clear);
        }
        
        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="position">要绘制到的坐标</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Vector2Int position) {
            var target = (Texture) _renderCanvas ?? _canvas;
            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, target, 0, 0, position.x, position.y);
            return this;
        }

        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="position">要绘制到的坐标</param>
        /// <param name="overlayColor">要叠加的颜色</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Vector2Int position, Color overlayColor) {
            return Color.white.Equals(overlayColor)
                ? DrawTexture(texture, position)
                : DrawTexture(texture, Matrix4x4.Translate(new Vector3(position.x, position.y, 0)), overlayColor, Vector2.zero);
        }
        
        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform) {
            return DrawTexture(texture, transform, Color.white, Vector2.zero);
        }
        
        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, Vector2 pivot) {
            return DrawTexture(texture, transform, Color.white, pivot);
        }

        /// <summary>
        /// 绘制Texture2D
        /// </summary>
        /// <param name="texture">目标Texture2D</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="overlayColor">要叠加的颜色</param>
        /// <param name="pivot">变换轴点</param>
        /// <returns></returns>
        public Texture2DCombiner DrawTexture(Texture2D texture, Matrix4x4 transform, Color overlayColor, Vector2 pivot) {
            var width = texture.width;
            var height = texture.height;
            var distance = new Vector3(texture.width * pivot.x, texture.height * pivot.y, 0);
            var area = transform.MultiplyRect(new Rect(-distance, new Vector2(texture.width, texture.height)))
                                .Move(distance)
                                .CeilToRectInt();
            transform = transform.RevertTRS();
            if (_renderCanvas == null) {
                var pixels = texture.GetPixels();
                var sizeX = _canvas.width;
                var sizeY = _canvas.height;
                var areaX = area.xMax;
                var areaY = area.yMax;
                for (var i = area.y - 1; ++i < areaY;) {
                    if (!i.InRange(0, sizeY)) continue;
                    for (var j = area.x - 1; ++j < areaX;) {
                        if (!j.InRange(0, sizeX)) continue;
                        var position = transform.MultiplyPoint(new Vector3(j, i, 0) - distance) + distance;
                        var (x, y) = (Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
                        if (!x.InRange(0, width) || !y.InRange(0, height)) continue;
                        var origin = _canvas.GetPixel(j, i);
                        var target = pixels[x + y * width];
                        var color = target.PickSolid() * target.a + origin.PickSolid() * (1.0F - target.a);
                        _canvas.SetPixel(j, i, new Color(color.x, color.y, color.z, origin.a.Equals(0) ? target.a : origin.a));
                    }
                }
            } else {
                _shader.SetTexture(_addKernel, ShaderSourceName, texture);
                _shader.SetVector(ShaderColorName, overlayColor);
                _shader.SetMatrix(ShaderTransformName, transform);
                _shader.SetVector(ShaderSizeName, new Vector4(_renderCanvas.width, _renderCanvas.height, texture.width, texture.height));
                _shader.Dispatch(_addKernel, Mathf.CeilToInt(width / 24.0F), Mathf.CeilToInt(height / 24.0F), 1);
            }
            return this;
        }
        
        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(RectInt area, Color targetColor) {
            var colors = Enumerable.Repeat(targetColor, area.width * area.height).ToArray();
            if (_renderCanvas == null) {
                _canvas.SetPixels(area.x, area.y, area.width, area.height, colors);
            } else {
                var texture = new Texture2D(area.width, area.height, TextureFormat.RGBA32, false);
                texture.SetPixels(colors);
                texture.Apply(false);
                Graphics.CopyTexture(texture, 0, 0, 0, 0, area.width, area.height, _renderCanvas, 0, 0, area.x, area.y);
            }
            return this;
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="transform">变换矩阵</param>
        /// <param name="targetColor">要填充的颜色</param>
        /// <returns></returns>
        public Texture2DCombiner FillArea(RectInt area, Matrix4x4 transform, Color targetColor) {
            var width = area.width;
            var height = area.height;
            if (_renderCanvas == null) {
                var sizeX = _canvas.width;
                var sizeY = _canvas.height;
                for (var i = -1; ++i < width;) {
                    for (var j = -1; ++j < height;) {
                        var position = transform * new Vector4(i, j, 0, 0);
                        if (position.x >= 0 && position.x < sizeX && position.y >= 0 && position.y < sizeY) {
                            _canvas.SetPixel(i, j, targetColor);
                        }
                    }
                }
            } else {
                _shader.SetVector(ShaderColorName, targetColor);
                _shader.SetMatrix(ShaderTransformName, transform);
                _shader.SetVector(ShaderSizeName, new Vector4(_renderCanvas.width, _renderCanvas.height, area.width, area.height));
                _shader.Dispatch(_fillKernel, Mathf.CeilToInt(width / 24.0F), Mathf.CeilToInt(height / 24.0F), 1);
            }
            return this;
        }
        
        /// <summary>
        /// 填充为透明
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(RectInt area) {
            return FillArea(area, Vector4.zero);
        }

        /// <summary>
        /// 填充为透明
        /// </summary>
        /// <param name="area">目标区域</param>
        /// <param name="transform">变换矩阵</param>
        /// <returns></returns>
        public Texture2DCombiner Clear(RectInt area, Matrix4x4 transform) {
            return FillArea(area, transform, Vector4.zero);
        }

        /// <summary>
        /// 应用绘图操作并获取画板
        /// </summary>
        /// <returns></returns>
        public Texture2D Combine() {
            if (_renderCanvas != null)
                return _renderCanvas.CopyAsTexture2D(new RectInt(1, 1, _renderCanvas.width - 2, _renderCanvas.height - 2));
            _canvas.Apply(false);
            return _canvas;
        }
    }
}