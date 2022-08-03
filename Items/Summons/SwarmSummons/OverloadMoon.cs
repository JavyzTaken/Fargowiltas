using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadMoon : SwarmSummonBase
    {
        public OverloadMoon() : base("Runic Inscription", "Moon Lord", NPCID.MoonLordCore, "The wind whispers of death's approach!", "CelestialSigil2")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 20;
            FargoGlobalNPC.SwarmBagId = ItemID.MoonLordBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.MoonLordTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerMoon>();
        }
    }
}