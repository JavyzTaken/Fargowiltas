using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadDeer : SwarmSummonBase
    {
        public OverloadDeer() : base("Deer Amalgamation", "Deerclops", NPCID.Deerclops, "The Constant takes over!", "DeerThing2")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.DeerclopsBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.DeerclopsTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerDeer>();
        }
    }
}