using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadWall : SwarmSummonBase
    {
        public OverloadWall() : base("Bundle of Dolls", "Wall of Flesh", NPCID.WallofFlesh, "A fortress of flesh arises from the depths!", "FleshyDoll")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 10;
            FargoGlobalNPC.SwarmBagId = ItemID.WallOfFleshBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.WallofFleshTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerWall>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && player.ZoneUnderworldHeight;
        }
    }
}