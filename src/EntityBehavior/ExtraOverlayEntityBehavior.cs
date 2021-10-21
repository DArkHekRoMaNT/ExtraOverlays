using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;

namespace MoreOverlays
{
    public class ExtraOverlayEntityBehavior : EntityBehavior
    {
        ICoreClientAPI Api { get; }
        HealthBarRenderer HealthBarRenderer { get; }

        public ExtraOverlayEntityBehavior(Entity entity) : base(entity)
        {
            Api = entity.Api as ICoreClientAPI;
            HealthBarRenderer = new HealthBarRenderer(Api, Api.ModLoader.GetModSystem<Core>().HealthBarConfig);
        }

        public override void OnGameTick(float dt)
        {
            var selectedEntity = Api.World.Player.CurrentEntitySelection?.Entity;

            if (selectedEntity == null)
            {
                HealthBarRenderer.Active = false;
            }
            else
            {
                HealthBarRenderer.ForEntity = selectedEntity;
                HealthBarRenderer.Active = true;
            }
        }

        public override string PropertyName() => "extraoverlay";

        public override void OnEntityDespawn(EntityDespawnReason despawn)
        {
            base.OnEntityDespawn(despawn);
            Api.Event.UnregisterRenderer(HealthBarRenderer, EnumRenderStage.Ortho);
        }
    }
}