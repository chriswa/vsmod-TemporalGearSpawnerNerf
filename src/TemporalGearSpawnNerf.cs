using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace TemporalGearSpawnNerf
{
    public class TemporalGearSpawnNerf : ModSystem
    {
        public override void StartServerSide(ICoreServerAPI api)
        {
            // OnEntitySpawn fires when player joins, not when player respawns
            api.Event.OnEntitySpawn += (Entity entity) =>
            {
                if (entity is EntityPlayer)
                {
                    api.Logger.Debug("Adding ClearSpawnPosAfterReviveEntityBehavior to spawned EntityPlayer");
                    entity.AddBehavior(new ClearSpawnPosAfterReviveEntityBehavior(entity));
                }
            };
        }
    }

    public class ClearSpawnPosAfterReviveEntityBehavior : EntityBehavior
    {
        public ClearSpawnPosAfterReviveEntityBehavior(Entity entity) : base(entity)
        {
        }
        public override string PropertyName()
        {
            return "ClearSpawnPosAfterReviveEntityBehavior";
        }
        public override void OnEntityRevive()
        {
            // be extra super careful to avoid crashes, even though this should never happen due to business logic
            if (this.entity.World.Side != EnumAppSide.Server || !(this.entity is EntityPlayer))
            {
                this.entity.World.Api.Logger.Debug("ClearSpawnPosAfterReviveEntityBehavior: unexpected: client side or entity not player, skipping");
                return;
            }
            
            // get IServerPlayer from Entity per https://github.com/anegostudios/vssurvivalmod/blob/master/Item/ItemTemporalGear.cs#L182
            IServerPlayer plr = this.entity.World.PlayerByUid((this.entity as EntityPlayer).PlayerUID) as IServerPlayer;

            this.entity.World.Api.Logger.Event("ClearSpawnPosAfterReviveEntityBehavior: clearing player's spawn position (whether or not it was set)");
            plr.ClearSpawnPosition();
        }
    }

}
