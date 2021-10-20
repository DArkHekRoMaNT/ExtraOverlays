using Vintagestory.API.MathTools;

namespace MoreOverlays
{
    public class HealthBarRenderConfig
    {
        public float FadeIn = 0.2f;
        public float FadeOut = 0.4f;


        public float Width = 100;
        public float Height = 10;
        public float YOffset = 10;

        public Vec4f HighHPColor = new Vec4f(0.5f, 0.75f, 0.5f, 1);
        public Vec4f MidHPColor = new Vec4f(0.75f, 0.75f, 0.5f, 1);
        public Vec4f LowHPColor = new Vec4f(0.75f, 0.5f, 0.5f, 1);

        public float LowHPThreshold = 0.25f;
        public float MidHPThreshold = 0.5f;
    }
}