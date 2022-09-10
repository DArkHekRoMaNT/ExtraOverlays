using SharedUtils.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ExtraOverlays
{
    public class Core : ModSystem
    {
        public HealthBarRenderConfig HealthBarConfig { get; private set; }

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.RegisterEntityBehaviorClass("extraoverlay", typeof(ExtraOverlayEntityBehavior));
            api.Event.PlayerEntitySpawn += OnPlayerEntitySpawn;

            HealthBarConfig = api.LoadOrCreateConfig<HealthBarRenderConfig>("extraoverlays.json");
        }

        private void OnPlayerEntitySpawn(IClientPlayer byPlayer)
        {
            byPlayer.Entity.AddBehavior(new ExtraOverlayEntityBehavior(byPlayer.Entity));
        }
    }
}