using Core.VisualNovel.Attributes;

namespace Core.VisualNovel.Plugins {
    [VisualNovelPlugin(new byte[] {0, 0, 0, 0})]
    [VisualNovelPluginName("default", "声音")]
    [VisualNovelPluginParameter("default", "声道", "资源", "淡入", "淡出", "时长", "循环")]
    [VisualNovelPluginName("en", "Sound")]
    [VisualNovelPluginParameter("en", "Channel", "Resource", "FadeIn", "FadeOut", "Time", "Loop")]
    public class SoundPlugin {
    }
}