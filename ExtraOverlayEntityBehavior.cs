using CommonLib.Config;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace ExtraOverlays
{
    public class ExtraOverlayEntityBehavior : EntityBehavior
    {
        private readonly ICoreClientAPI _api;
        private readonly HealthBarRenderer _healthBarRenderer;

        public ExtraOverlayEntityBehavior(Entity entity) : base(entity)
        {
            _api = (ICoreClientAPI)entity.Api;
            var configs = _api.ModLoader.GetModSystem<ConfigManager>();
            _healthBarRenderer = new HealthBarRenderer(_api, configs.GetConfig<HealthBarRenderConfig>());
        }

        public override void OnGameTick(float dt)
        {
            var selectedEntity = _api.World.Player.CurrentEntitySelection?.Entity;
            if (selectedEntity == null)
            {
                _healthBarRenderer.Active = false;
            }
            else
            {
                _healthBarRenderer.ForEntity = selectedEntity;
                _healthBarRenderer.Active = true;
            }
        }

        public override string PropertyName() => "extraoverlay";

        // Fired only if client player exit, not died
        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            base.OnEntityDespawn(despawn);
            _healthBarRenderer?.Dispose();
        }
    }
}
