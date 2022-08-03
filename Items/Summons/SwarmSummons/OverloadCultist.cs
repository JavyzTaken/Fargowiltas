using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadCultist : SwarmSummonBase
    {
        public OverloadCultist() : base("Zealot's Madness", "Lunatic Cultist", NPCID.CultistBoss, "Defeaning chants fill your ears!", "CultistSummon")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.CultistBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.AncientCultistTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerCultist>();
        }
    }
}