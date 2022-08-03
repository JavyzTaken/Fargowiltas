using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadPlant : SwarmSummonBase
    {
        public OverloadPlant() : base("Heart of the Jungle", "Plantera", NPCID.Plantera, "The jungle beats as one!", "PlanterasFruit")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.PlanteraBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.PlanteraTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerPlant>();
        }
    }
}