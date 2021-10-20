using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

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
    }
}