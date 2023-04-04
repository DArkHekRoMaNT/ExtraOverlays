using CommonLib.Config;

namespace ExtraOverlays
{
    [Config("extraoverlays.json")]
    public class HealthBarRenderConfig
    {
        public float FadeIn { get; set; } = 0.2f;
        public float FadeOut { get; set; } = 0.4f;

        public float Width { get; set; } = 100;
        public float Height { get; set; } = 10;
        public float YOffset { get; set; } = 10;

        public string HighHPColor { get; set; } = "#7FBF7F";
        public string MidHPColor { get; set; } = "#BFBF7F";
        public string LowHPColor { get; set; } = "#BF7F7F";

        public float LowHPThreshold { get; set; } = 0.25f;
        public float MidHPThreshold { get; set; } = 0.5f;
    }
}
