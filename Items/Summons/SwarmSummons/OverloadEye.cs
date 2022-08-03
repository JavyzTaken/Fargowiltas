using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadEye : SwarmSummonBase
    {
        public OverloadEye() : base("Eyemalgamation", "Eye of Cthulhu", NPCID.EyeofCthulhu, "Countless eyes pierce the veil staring in your direction!", "SuspiciousEye")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 50;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.EyeOfCthulhuBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.EyeofCthulhuTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerEye>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && !Main.dayTime;
        }
    }
}