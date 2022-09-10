using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace ExtraOverlays
{
    public class HealthBarRenderer : IRenderer
    {
        private ICoreClientAPI Api { get; }
        public HealthBarRenderConfig Config { get; }


        private MeshRef _healthBarRef;
        private MeshRef _backRef;
        private Matrixf _mvMatrix = new Matrixf();
        private float _alpha = 0f;


        public Entity ForEntity { get; set; }
        public bool Active { get; set; }


        public HealthBarRenderer(ICoreClientAPI api, HealthBarRenderConfig config)
        {
            Api = api;
            Config = config;

            Api.Event.RegisterRenderer(this, EnumRenderStage.Ortho);

            MeshData backData = LineMeshUtil.GetRectangle(ColorUtil.WhiteArgb);
            _backRef = Api.Render.UploadMesh(backData);

            _healthBarRef = Api.Render.UploadMesh(QuadMeshUtil.GetQuad());
        }


        public double RenderOrder => 0.41; // After Entity 0.4
        public int RenderRange => 10;

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            if (ForEntity == null || !ForEntity.WatchedAttributes.HasAttribute("health") || (_alpha <= 0 && !Active)) return;

            var healthTree = ForEntity.WatchedAttributes.GetTreeAttribute("health");
            var progress = healthTree.GetFloat("currenthealth") / healthTree.GetFloat("maxhealth");

            _alpha = Math.Max(0f, Math.Min(1f, _alpha + deltaTime / (Active ? Config.FadeIn : -Config.FadeOut)));
            Vec4f color = GetHealthBarColor(progress);


            var curShader = Api.Render.CurrentActiveShader;
            curShader.Uniform("rgbaIn", color);
            curShader.Uniform("extraGlow", 0);
            curShader.Uniform("applyColor", 0);
            curShader.Uniform("tex2d", 0);
            curShader.Uniform("noTexture", 1f);



            Vec3d aboveHeadPos = new Vec3d(ForEntity.Pos.X, ForEntity.Pos.Y + ForEntity.CollisionBox.Y2, ForEntity.Pos.Z);

            double offX = ForEntity.CollisionBox.X2 - ForEntity.OriginCollisionBox.X2;
            double offZ = ForEntity.CollisionBox.Z2 - ForEntity.OriginCollisionBox.Z2;
            aboveHeadPos.Add(offX, 0, offZ);

            Vec3d pos = MatrixToolsd.Project(aboveHeadPos, Api.Render.PerspectiveProjectionMat, Api.Render.PerspectiveViewMat, Api.Render.FrameWidth, Api.Render.FrameHeight);

            // Z negative seems to indicate that the name tag is behind us \o/
            if (pos.Z < 0) return;

            float scale = 4f / Math.Max(1, (float)pos.Z);

            float cappedScale = Math.Min(1f, scale);
            if (cappedScale > 0.75f) cappedScale = 0.75f + (cappedScale - 0.75f) / 2;

            float x = (float)pos.X - cappedScale * Config.Width / 2;
            float y = Api.Render.FrameHeight - (float)pos.Y - (Config.Height * Math.Max(0, cappedScale)) - Config.YOffset;
            float z = 20;

            float width = cappedScale * Config.Width;
            float height = cappedScale * Config.Height;



            // Render back
            _mvMatrix
                .Set(Api.Render.CurrentModelviewMatrix)
                .Translate(x, y, z)
                .Scale(width, height, 0)
                .Translate(0.5f, 0.5f, 0)
                .Scale(0.5f, 0.5f, 0)
            ;

            curShader.UniformMatrix("projectionMatrix", Api.Render.CurrentProjectionMatrix);
            curShader.UniformMatrix("modelViewMatrix", _mvMatrix.Values);

            Api.Render.RenderMesh(_backRef);


            // Render health bar
            _mvMatrix
                .Set(Api.Render.CurrentModelviewMatrix)
                .Translate(x, y, z)
                .Scale(width * progress, height, 0)
                .Translate(0.5f, 0.5f, 0)
                .Scale(0.5f, 0.5f, 0)
            ;

            curShader.UniformMatrix("projectionMatrix", Api.Render.CurrentProjectionMatrix);
            curShader.UniformMatrix("modelViewMatrix", _mvMatrix.Values);

            Api.Render.RenderMesh(_healthBarRef);
        }

        private Vec4f GetHealthBarColor(float progress)
        {
            Vec4f color = Config.HighHPColor;

            if (progress <= Config.LowHPThreshold)
            {
                color = Config.LowHPColor;
            }
            else if (progress <= Config.MidHPThreshold)
            {
                color = Config.MidHPColor;
            }

            color.A = _alpha;
            return color;
        }

        public void Dispose()
        {
            Api.Render.DeleteMesh(_backRef);
            Api.Render.DeleteMesh(_healthBarRef);
        }
    }
}