using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadBetsy : SwarmSummonBase
    {
        public OverloadBetsy() : base("Dragon Egg Tray", "Betsy", NPCID.DD2Betsy, "The real Old One's Army is attacking!", "BetsyEgg")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.BossBagBetsy;
            FargoGlobalNPC.SwarmTrophyId = ItemID.BossTrophyBetsy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerBetsy>();
        }
    }
}