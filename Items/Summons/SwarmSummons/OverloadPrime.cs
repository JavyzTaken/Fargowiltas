using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadPrime : SwarmSummonBase
    {
        public OverloadPrime() : base("Primal Control Chip", "Skeletron Prime", NPCID.SkeletronPrime, "A sickly chill envelops the world!", "MechSkull")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.SkeletronPrimeBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.SkeletronPrimeTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerPrime>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && !Main.dayTime;
        }
    }
}