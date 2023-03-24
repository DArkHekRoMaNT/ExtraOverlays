using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace ExtraOverlays
{
    public class Core : ModSystem
    {
        public override void StartClientSide(ICoreClientAPI api)
        {
            api.RegisterEntityBehaviorClass("extraoverlay", typeof(ExtraOverlayEntityBehavior));
            api.Event.PlayerEntitySpawn += OnPlayerEntitySpawn;
        }

        private void OnPlayerEntitySpawn(IClientPlayer byPlayer)
        {
            byPlayer.Entity.AddBehavior(new ExtraOverlayEntityBehavior(byPlayer.Entity));
        }
    }
}
