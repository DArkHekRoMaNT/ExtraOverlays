using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace ExtraOverlays
{
    public class HealthBarRenderer : IRenderer, IDisposable
    {
        private readonly ICoreClientAPI _api;
        private readonly HealthBarRenderConfig _config;
        private readonly Matrixf _mvMatrix = new();
        private readonly MeshRef? _healthBarRef;
        private readonly MeshRef? _backRef;
        private Vec4f _color = new();
        private float _alpha = 0f;

        public Entity? ForEntity { get; set; }
        public bool Active { get; set; }

        public double RenderOrder => 0.41; // After Entity 0.4
        public int RenderRange => 10;

        public HealthBarRenderer(ICoreClientAPI api, HealthBarRenderConfig config)
        {
            _api = api;
            _config = config;

            MeshData backData = LineMeshUtil.GetRectangle(ColorUtil.WhiteArgb);
            _backRef = _api.Render.UploadMesh(backData);
            _healthBarRef = _api.Render.UploadMesh(QuadMeshUtil.GetQuad());

            _api.Event.RegisterRenderer(this, EnumRenderStage.Ortho);
        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            // no entity
            if (ForEntity == null)
            {
                return;
            }

            // no health
            if (!ForEntity.WatchedAttributes.HasAttribute("health"))
            {
                return;
            }

            // invisible
            if (_alpha <= 0 && !Active)
            {
                return;
            }

            ITreeAttribute healthTree = ForEntity.WatchedAttributes.GetTreeAttribute("health");
            float currentHealth = healthTree.GetFloat("currenthealth");
            float maxHealth = healthTree.GetFloat("maxhealth");
            float progress = currentHealth / maxHealth;

            float deltaAlpha = deltaTime / (Active ? _config.FadeIn : -_config.FadeOut);
            _alpha = Math.Max(0f, Math.Min(1f, _alpha + deltaAlpha));

            GetHealthBarColor(progress, ref _color);

            IShaderProgram shader = _api.Render.CurrentActiveShader;
            shader.Uniform("rgbaIn", _color);
            shader.Uniform("extraGlow", 0);
            shader.Uniform("applyColor", 0);
            shader.Uniform("tex2d", 0);
            shader.Uniform("noTexture", 1f);

            var aboveHeadPos = new Vec3d(
                ForEntity.Pos.X,
                ForEntity.Pos.Y + ForEntity.CollisionBox.Y2,
                ForEntity.Pos.Z);

            double offX = ForEntity.CollisionBox.X2 - ForEntity.OriginCollisionBox.X2;
            double offZ = ForEntity.CollisionBox.Z2 - ForEntity.OriginCollisionBox.Z2;
            aboveHeadPos.Add(offX, 0, offZ);

            Vec3d pos = MatrixToolsd.Project(aboveHeadPos,
                _api.Render.PerspectiveProjectionMat,
                _api.Render.PerspectiveViewMat,
                _api.Render.FrameWidth,
                _api.Render.FrameHeight);

            // Z negative seems to indicate that the name tag is behind us \o/
            if (pos.Z < 0)
            {
                return;
            }

            float scale = 4f / Math.Max(1, (float)pos.Z);

            float cappedScale = Math.Min(1f, scale);
            if (cappedScale > 0.75f) cappedScale = 0.75f + (cappedScale - 0.75f) / 2;

            float x = (float)pos.X - cappedScale * _config.Width / 2;
            float y = _api.Render.FrameHeight - (float)pos.Y - (_config.Height * Math.Max(0, cappedScale)) - _config.YOffset;
            float z = 20;

            float width = cappedScale * _config.Width;
            float height = cappedScale * _config.Height;

            // Render back
            _mvMatrix
                .Set(_api.Render.CurrentModelviewMatrix)
                .Translate(x, y, z)
                .Scale(width, height, 0)
                .Translate(0.5f, 0.5f, 0)
                .Scale(0.5f, 0.5f, 0);

            shader.UniformMatrix("projectionMatrix", _api.Render.CurrentProjectionMatrix);
            shader.UniformMatrix("modelViewMatrix", _mvMatrix.Values);

            _api.Render.RenderMesh(_backRef);


            // Render health bar
            _mvMatrix
                .Set(_api.Render.CurrentModelviewMatrix)
                .Translate(x, y, z)
                .Scale(width * progress, height, 0)
                .Translate(0.5f, 0.5f, 0)
                .Scale(0.5f, 0.5f, 0);

            shader.UniformMatrix("projectionMatrix", _api.Render.CurrentProjectionMatrix);
            shader.UniformMatrix("modelViewMatrix", _mvMatrix.Values);

            _api.Render.RenderMesh(_healthBarRef);
        }

        private void GetHealthBarColor(float progress, ref Vec4f color)
        {
            HexToVec(_config.HighHPColor, ref color);

            if (progress <= _config.LowHPThreshold)
            {
                HexToVec(_config.LowHPColor, ref color);
            }
            else if (progress <= _config.MidHPThreshold)
            {
                HexToVec(_config.MidHPColor, ref color);
            }

            color.A = _alpha;
        }

        private static void HexToVec(string hexColor, ref Vec4f color)
        {
            int intColor = ColorUtil.Hex2Int(hexColor);
            ColorUtil.ToRGBAVec4f(intColor, ref color);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _api.Render.DeleteMesh(_backRef);
            _api.Render.DeleteMesh(_healthBarRef);
            _api.Event.UnregisterRenderer(this, EnumRenderStage.Ortho);
        }
    }
}
