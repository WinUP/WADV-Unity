using System;
using System.Collections;
using UnityEngine;

namespace Assets.WADV.VisualNovel.Sound {
    /// <inheritdoc />
    /// <summary>
    /// 表示一个声道
    /// <para>声道同一时间可以播放一个声音资源并对其音量进行两级控制（基础音量+缩放），同时提供渐入渐出、交叉切换其他声音资源等功能</para>
    /// </summary>
    public class SoundChannel : MonoBehaviour {
        public string CurrentPlaying { get; private set; }

        /// <summary>
        /// 获取或设置基础音量 (0.0 - 1.0)
        /// </summary>
        public float BaseVolume {
            get { return _baseVolume; }
            set {
                if (value > 1.0F) {
                    _baseVolume = 1.0F;
                } else if (value < 0.0F) {
                    _baseVolume = 0.0F;
                } else {
                    _baseVolume = value;
                }
                ReCalculateVolume();
            }
        }

        /// <summary>
        /// 获取或设置音量缩放比例 (0.0 - 1.0)
        /// </summary>
        public float VolumeScale {
            get { return _volumeScale; }
            set {
                if (value > 1.0F) {
                    _volumeScale = 1.0F;
                } else if (value < 0.0F) {
                    _volumeScale = 0.0F;
                } else {
                    _volumeScale = value;
                }
                ReCalculateVolume();
            }
        }

        /// <summary>
        /// 获取或设置循环播放模式
        /// </summary>
        public bool Loop {
            get { return _loop; }
            set {
                _loop = value;
                if (_source != null) {
                    _source.loop = _loop;
                }
            }
        }

        /// <summary>
        /// 获取或设置播放进度
        /// </summary>
        public float Offset {
            get { return _source == null ? 0.0F : _source.time; }
            set {
                if (_source == null) {
                    return;
                }
                _source.time = value;
            }
        }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying => _source != null && _source.isPlaying;

        private AudioSource _source;
        private float _baseVolume = 1.0F;
        private float _volumeScale = 1.0F;
        private bool _loop;

        /// <summary>
        /// 播放指定声音资源并立即停止当前播放的声音
        /// </summary>
        /// <param name="source">资源位置</param>
        /// <param name="fade">渐入时间</param>
        /// <param name="offset">播放进度</param>
        /// <returns></returns>
        public IEnumerator Play(string source, float fade = 0.0F, float offset = 0.0F) {
            if (_source) {
                _source.Stop();
            }
            LoadAudio(source, fade <= 0.0F);
            _source.time = offset;
            if (fade > 0.0F) {
                VolumeScale = 0.0F;
            }
            _source.Play();
            if (fade > 0.0F) {
                var originalVolumeScale = VolumeScale;
                yield return StartCoroutine(FadeTo(originalVolumeScale, fade));
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="fade">渐出时间</param>
        /// <returns></returns>
        public IEnumerator Pause(float fade = 0.0F) {
            if (_source == null) {
                yield break;
            }
            var originalVolumeScale = VolumeScale;
            if (fade > 0.0F) {
                yield return StartCoroutine(FadeOut(fade));
            }
            _source.Pause();
            VolumeScale = originalVolumeScale;
        }

        /// <summary>
        /// 恢复播放
        /// </summary>
        /// <param name="fade">渐出时间</param>
        /// <returns></returns>
        public IEnumerator Resume(float fade = 0.0F) {
            if (_source == null) {
                yield break;
            }
            var originalVolumeScale = VolumeScale;
            if (fade > 0.0F) {
                yield return StartCoroutine(FadeOut(fade));
            }
            _source.UnPause();
            VolumeScale = originalVolumeScale;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="fade">渐出时间</param>
        /// <returns></returns>
        public IEnumerator Stop(float fade = 0.0F) {
            if (_source == null) {
                yield break;
            }
            var originalVolumeScale = VolumeScale;
            if (fade > 0.0F) {
                yield return StartCoroutine(FadeOut(fade));
            }
            _source.Stop();
            VolumeScale = originalVolumeScale;
        }
        
        /// <summary>
        /// 停止播放该声道的声音并重置音量缩放比例
        /// </summary>
        public void Clear() {
            if (_source != null) {
                _source.Stop();
                _source = null;
                CurrentPlaying = null;
            }
            VolumeScale = 1.0F;
        }

        /// <summary>
        /// 渐入渐出音量缩放比例
        /// </summary>
        /// <param name="value">目标缩放比例</param>
        /// <param name="time">渐入渐出时间</param>
        /// <returns></returns>
        public IEnumerator FadeTo(float value, float time) {
            if (_source == null) {
                yield break;
            }
            var initialScale = VolumeScale;
            var timeOffset = 0.0F;
            while (Math.Abs(VolumeScale - value) > 0.01) {
                VolumeScale = Mathf.Lerp(initialScale, value, time / timeOffset);
                timeOffset += Time.deltaTime;
                yield return true;
            }
            VolumeScale = value;
        }

        /// <summary>
        /// 同时渐出停止播放并渐入播放新的声音
        /// </summary>
        /// <param name="source">资源位置</param>
        /// <param name="time">渐入渐出时间</param>
        /// <returns></returns>
        public IEnumerator FadeTo(string source, float time) {
            if (_source == null) {
                yield return Play(source, time);
            }
            var initialVolumeScale = VolumeScale;
            var originSource = _source;
            var originVolume = _source.volume;
            LoadAudio(source, false);
            VolumeScale = 0.0F;
            _source.Play();
            var timeOffset = 0.0F;
            while (VolumeScale < initialVolumeScale) {
                var scale = time / timeOffset;
                timeOffset += Time.deltaTime;
                VolumeScale = Mathf.Lerp(0.0F, initialVolumeScale, scale);
                originSource.volume = originVolume * (1 - scale);
                yield return true;
            }
            originSource.Stop();
            VolumeScale = initialVolumeScale;
        }

        /// <summary>
        /// 渐出停止播放，等待一定时间后渐入播放新的声音
        /// </summary>
        /// <param name="source">资源位置</param>
        /// <param name="fadeOut">渐出时间</param>
        /// <param name="fadeIn">渐入时间</param>
        /// <param name="delay">等待时间</param>
        /// <returns></returns>
        public IEnumerator FadeTo(string source, float fadeOut, float fadeIn, float delay = 0.0F) {
            yield return StartCoroutine(Stop(fadeOut));
            if (delay > 0.0F) {
                yield return new WaitForSeconds(delay);
            }
            yield return StartCoroutine(Play(source, fadeIn));
        }

        /// <summary>
        /// 渐出音量缩放比例至0.0
        /// </summary>
        /// <param name="time">渐出时间</param>
        /// <returns></returns>
        public IEnumerator FadeOut(float time) {
            yield return StartCoroutine(FadeTo(0.0F, time));
        }

        /// <summary>
        /// 渐入音量缩放比例至1.0
        /// </summary>
        /// <param name="time">渐入时间</param>
        /// <returns></returns>
        public IEnumerator FadeIn(float time) {
            yield return StartCoroutine(FadeTo(1.0F, time));
        }

        private void ReCalculateVolume() {
            if (_source != null) {
                _source.volume = BaseVolume * VolumeScale;
            }
        }

        private void LoadAudio(string source, bool useCurrentVolume) {
            var target = new AudioSource {
                clip = Resources.Load<AudioClip>(source), // Unity自带的缓存系统可以满足要求
                volume = useCurrentVolume ? BaseVolume * VolumeScale : 0.0F,
                time = 0.0F,
                loop = Loop
            };
            CurrentPlaying = source;
            _source = target;
        }
    }
}
