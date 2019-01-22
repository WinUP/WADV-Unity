/*
 * Created by C.J. Kimberlin (http://cjkimberlin.com)
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * 
 * TERMS OF USE - EASING EQUATIONS
 * Open source under the BSD License.
 * Copyright (c)2001 Robert Penner
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using UnityEngine;

namespace WADV {
    /// <summary>
    /// 缓动函数库
    /// </summary>
    public static class Easing {
        public static Func<float, float> GetEasingFunction(EasingType type) {
            switch (type) {
                case EasingType.Linear:
                    return Linear;
                case EasingType.Spring:
                    return Spring;
                case EasingType.QuadIn:
                    return QuadIn;
                case EasingType.QuadOut:
                    return QuadOut;
                case EasingType.QuadInOut:
                    return QuadInOut;
                case EasingType.CubicIn:
                    return CubicIn;
                case EasingType.CubicOut:
                    return CubicOut;
                case EasingType.CubicInOut:
                    return CubicInOut;
                case EasingType.QuartIn:
                    return QuartIn;
                case EasingType.QuartOut:
                    return QuartOut;
                case EasingType.QuartInOut:
                    return QuartInOut;
                case EasingType.QuintIn:
                    return QuintIn;
                case EasingType.QuintOut:
                    return QuintOut;
                case EasingType.QuintInOut:
                    return QuintInOut;
                case EasingType.SineIn:
                    return SineIn;
                case EasingType.SineOut:
                    return SineOut;
                case EasingType.SineInOut:
                    return SineInOut;
                case EasingType.ExponentIn:
                    return ExponentIn;
                case EasingType.ExponentOut:
                    return ExponentOut;
                case EasingType.ExponentInOut:
                    return ExponentInOut;
                case EasingType.CircleIn:
                    return CircleIn;
                case EasingType.CircleOut:
                    return CircleOut;
                case EasingType.CircleInOut:
                    return CircleInOut;
                case EasingType.BounceIn:
                    return BounceIn;
                case EasingType.BounceOut:
                    return BounceOut;
                case EasingType.BounceInOut:
                    return BounceInOut;
                case EasingType.BackIn:
                    return BackIn;
                case EasingType.BackOut:
                    return BackOut;
                case EasingType.BackInOut:
                    return BackInOut;
                case EasingType.ElasticIn:
                    return ElasticIn;
                case EasingType.ElasticOut:
                    return ElasticOut;
                case EasingType.ElasticInOut:
                    return ElasticInOut;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"Unable to create ease function: unknown ease type {type}");
            }
        }
        
        /// <summary>
        /// 使用函数y = x计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float Linear(float value) => value;

        /// <summary>
        /// 使用函数y = (sin(πx(2.5x^3 + 0.2)) * (1 - x)^2.2 + x) * (2.2 - 1.2x)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float Spring(float value) {
            return (Mathf.Sin(value * Mathf.PI * (0.2F + 2.5F * Mathf.Pow(value, 3))) * Mathf.Pow(1F - value, 2.2F) + value) * (2.2F - 1.2F * value);
        }

        /// <summary>
        /// 使用函数y = x^2计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float QuadIn( float value) {
            return Mathf.Pow(value, 2.0F);
        }

        /// <summary>
        /// 使用函数y = -x^2 + 2x计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        /// <remarks>实际使用算式 y = x(2-x)</remarks>
        public static float QuadOut(float value) {
            return value * (2.0F - value);
        }

        /// <summary>
        /// 使用函数y = {2x^2}[0, 0.5) + {-2x^2 + 4x - 1}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        /// <remarks>实际使用算式 y = {2x^2}[0, 0.5) + {(1 - (2x-1)(2x-3)) / 2}[0.5, 1]</remarks>
        public static float QuadInOut(float value) {
            if (value < 0.5F) return 2.0F * Mathf.Pow(value, 2.0F);
            value *= 2.0F;
            return (1.0F - (value - 1.0F) * (value - 3.0F)) / 2.0F;
        }

        /// <summary>
        /// 使用函数y = x^3计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float CubicIn(float value) {
            return Mathf.Pow(value, 3.0F);
        }

        /// <summary>
        /// 使用函数y = (x-1)^3 + 1计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float CubicOut(float value) {
            return Mathf.Pow(value - 1.0F, 3.0F) + 1.0F;
        }

        /// <summary>
        /// 使用函数y = {4x^3}[0, 0.5) + {4x^3 - 12x^2 + 12x - 3}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        /// <remarks>实际使用算式 y = {4x^3}[0, 0.5) + {(2x - 2)^3 / 2 + 1}[0.5, 1]</remarks>
        public static float CubicInOut(float value) {
            return value < 0.5F ? 4.0F * Mathf.Pow(value, 3.0F) : Mathf.Pow(2.0F * value - 2.0F, 3.0F) / 2.0F + 1.0F;
        }

        /// <summary>
        /// 使用函数y = x^4计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float QuartIn(float value) {
            return Mathf.Pow(value, 4.0F);
        }

        /// <summary>
        /// 使用函数y = 1 - (x-1)^4计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float QuartOut(float value) {
            return 1 - Mathf.Pow(value - 1.0F, 4.0F);
        }

        /// <summary>
        /// 使用函数y = {8x^4}[0, 0.5) + {-8x^4 + 32x^3 - 48x^2 + 32x - 7}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        /// <remarks>实际使用算式 y = {8x^4}[0, 0.5) + {1 - 8(x - 1)^4}[0.5, 1]</remarks>
        public static float QuartInOut(float value) {
            return value < 0.5F ? 8.0F * Mathf.Pow(value, 4.0F) : 1.0F - 8.0F * Mathf.Pow(value - 1.0F, 4.0F);
        }

        /// <summary>
        /// 使用函数y = x^5计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float QuintIn(float value) {
            return Mathf.Pow(value, 5.0F);
        }

        /// <summary>
        /// 使用函数y = (x - 1)^5 + 1计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float QuintOut(float value) {
            return Mathf.Pow(value - 1.0F, 5.0F) + 1.0F;
        }

        /// <summary>
        /// 使用函数y = {16x^5}[0, 0.5) + {16x^5 - 80x^4 + 160x^3 - 160x^2 + 80x - 15}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        /// <remarks>实际使用算式 y = {16x^5}[0, 0.5) + {16(x - 1)^5 + 1}[0.5, 1]</remarks>
        public static float QuintInOut(float value) {
            return value < 0.5F ? 16.0F * Mathf.Pow(value, 5.0F) : 16.0F * Mathf.Pow(value - 1.0F, 5.0F) + 1.0F;
        }

        /// <summary>
        /// 使用函数y = 1 - cos(π/2 * x)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float SineIn(float value) {
            return 1.0F - Mathf.Cos(Mathf.PI / 2.0F * value);
        }

        /// <summary>
        /// 使用函数y = sin(π/2 * x)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float SineOut(float value) {
            return Mathf.Sin(Mathf.PI / 2.0F * value);
        }

        /// <summary>
        /// 使用函数y = (cos(πx) - 1) / -2计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float SineInOut(float value) {
            return (Mathf.Cos(Mathf.PI * value) - 1.0F) / -2.0F;
        }

        /// <summary>
        /// 使用函数y = 2^(10(x-1))计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ExponentIn(float value) {
            return Mathf.Pow(2.0F, 10.0F * (value - 1.0F));
        }

        /// <summary>
        /// 使用函数y = 1 - 2^(-10x)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ExponentOut(float value) {
            return 1.0F - Mathf.Pow(2.0F, -10.0F * value);
        }

        /// <summary>
        /// 使用函数y = {2^(20x - 11)}[0, 0.5) + {1 - 2^(9 - 20x)}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ExponentInOut(float value) {
            return value < 0.5F ? Mathf.Pow(2.0F, 20.0F * value - 11) : 1.0F - Mathf.Pow(2.0F, 9.0F - 20.0F * value);
        }

        /// <summary>
        /// 使用函数y = 1 - √(1 - x^2)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float CircleIn(float value) {
            return 1.0F - Mathf.Sqrt(1.0F - value * value);
        }

        /// <summary>
        /// 使用函数y = √(1 - (x - 1)^2)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float CircleOut(float value) {
            return Mathf.Sqrt(1.0F - Mathf.Pow(value - 1.0F, 2.0F));
        }

        /// <summary>
        /// 使用函数y = {(sqrt(1 - 4x^2) - 1) / 2}[0, 0.5) + {(sqrt(1 - (2x - 2)^2) + 1) / 2}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float CircleInOut(float value) {
            return value < 0.5F ? (Mathf.Sqrt(1.0F - 4.0F * Mathf.Pow(value, 2)) - 1.0F) / 2.0F : (Mathf.Sqrt(1.0F - Mathf.Pow(2 * value - 2, 2)) + 1.0F) / 2.0F;
        }

        /// <summary>
        /// 使用函数y = 1 - EaseOutBounce(1 - x)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BounceIn(float value) {
            return 1 - BounceOut(1 - value);
        }

        /// <summary>
        /// 使用函数y = {7.5625 * x^2}[0, 0.363636) + {7.5625 * (x - 0.545454)^2 + 0.75}[0.363636, 0.727272) + {7.5625 * (x - 0.818182)^2 + 0.9375}[0.727272, 0.909091) + {7.5625 * (x - 0.954545)^2 + 0.984375}[0.909091, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BounceOut(float value) {
            if (value < 0.363636F)
                return 7.5625F * Mathf.Pow(value, 2.0F);
            if (value < 0.727273F)
                return 7.5625F * Mathf.Pow(value - 0.545454F, 2.0F) + 0.75F;
            if (value < 0.909091F)
                return 7.5625F * Mathf.Pow(value - 0.818182F, 2.0F) + 0.9375F;
            return 7.5625F * Mathf.Pow(value - 0.954545F, 2.0F) + 0.984375F;
        }

        /// <summary>
        /// 使用函数y = {EaseInBounce(2x) / 2}[0, 0.5) + {EaseOutBounce(2x - 1) / 2 + 0.5}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BounceInOut(float value) {
            return value < 0.5F ? BounceIn(value * 2.0F) / 2.0F : BounceOut(value * 2.0F - 1.0F) / 2.0F + 0.5F;
        }

        /// <summary>
        /// 使用函数y = x^2 * (2.70158x - 1.70158)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BackIn(float value) {
            return Mathf.Pow(value, 2.0F) * (2.70158F * value - 1.70158F);
        }

        /// <summary>
        /// 使用函数y = (x - 1)^2 * (2.70158x - 1) + 1计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BackOut(float value) {
            return Mathf.Pow(value - 1.0F, 2.0F) * (2.70158F * value - 1) + 1.0F;
        }

        /// <summary>
        /// 使用函数y = {2x^2 * (7.189819x - 2.5949095)}[0, 0.5) + {2(x - 1)^2 * (7.189819x - 4.5949095) + 1}[0.5, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float BackInOut(float value) {
            return value < 0.5F ? 2.0F * Mathf.Pow(value, 2.0F) * (7.189819F * value - 2.5949095F) : 2.0F * (Mathf.Pow(value - 1.0F, 2.0F) * (7.189819F * value - 4.5949095F) + 2.0F);
        }

        /// <summary>
        /// 使用函数y = x^4 * sin(4.5πx)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ElasticIn(float value) {
            return Mathf.Pow(value, 4.0F) * Mathf.Sin(value * Mathf.PI * 4.5F);
        }

        /// <summary>
        /// 使用函数y = 1 - (x - 1)^4 * cos(4.5πx)计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ElasticOut(float value) {
            return 1.0F - Mathf.Pow(value - 1.0F, 4.0F) * Mathf.Cos(value * Mathf.PI * 4.5F);
        }

        /// <summary>
        /// 使用函数y = {8x^4 * sin(9πx)}[0, 0.45) + {0.5 + 0.75sin(4πx)}[0.45, 0.55) + {1 - 8(x - 1)^4 * sin(9πx)}[0.55, 1]计算过渡进度
        /// </summary>
        /// <param name="value">原始过渡进度</param>
        /// <returns></returns>
        public static float ElasticInOut(float value) {
            if (value < 0.45F)
                return 8 * Mathf.Pow(value, 4) * Mathf.Sin(value * Mathf.PI * 9.0F);
            if (value < 0.55F)
                return 0.5F + 0.75F * Mathf.Sin(value * Mathf.PI * 4.0F);
            return 1.0F - 8.0F * Mathf.Pow(value - 1.0F, 4.0F) * Mathf.Sin(value * Mathf.PI * 9.0F);
        }
    }
}