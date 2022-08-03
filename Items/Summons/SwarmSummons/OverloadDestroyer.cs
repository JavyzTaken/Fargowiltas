using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadDestroyer : SwarmSummonBase
    {
        public OverloadDestroyer() : base("Seismic Actuator", "Destroyer", NPCID.TheDestroyer, "The planet trembles from the core!", "MechWorm")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 10;
            FargoGlobalNPC.SwarmBagId = ItemID.DestroyerBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.DestroyerTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerDestroy>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && !Main.dayTime;
        }
    }
}