using CommonLib.Config;

namespace ExtraOverlays
{
    [Config("extraoverlay.json")]
    public class HealthBarRenderConfig
    {
        [ConfigItem(typeof(float), 0.2f)] public float FadeIn { get; set; }
        [ConfigItem(typeof(float), 0.4f)] public float FadeOut { get; set; }

        [ConfigItem(typeof(float), 100)] public float Width { get; set; }
        [ConfigItem(typeof(float), 10)] public float Height { get; set; }
        [ConfigItem(typeof(float), 10)] public float YOffset { get; set; }

        [ConfigItem(typeof(string), "#7FBF7F")] public string HighHPColor { get; set; }
        [ConfigItem(typeof(string), "#BFBF7F")] public string MidHPColor { get; set; }
        [ConfigItem(typeof(string), "#BF7F7F")] public string LowHPColor { get; set; }

        [ConfigItem(typeof(float), 0.25f)] public float LowHPThreshold { get; set; }
        [ConfigItem(typeof(float), 0.5f)] public float MidHPThreshold { get; set; }
    }
}
