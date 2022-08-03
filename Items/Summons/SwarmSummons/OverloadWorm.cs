using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadWorm : SwarmSummonBase
    {
        public OverloadWorm() : base("Worm Chicken", "Eater of Worlds", NPCID.EaterofWorldsHead, "The ground shifts with formulated precision!", "WormyFood")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 100;
            FargoGlobalNPC.SwarmMinionSpawnCount = 0;
            FargoGlobalNPC.SwarmBagId = ItemID.EaterOfWorldsBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.EaterofWorldsTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerWorm>();
        }
    }
}