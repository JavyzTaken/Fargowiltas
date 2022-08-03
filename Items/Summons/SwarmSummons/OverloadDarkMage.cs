using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadDarkMage : SwarmSummonBase
    {
        public OverloadDarkMage() : base("Really Forbidden Tome", "Dark Mage", NPCID.DD2DarkMageT1, "You feel like you're in a library!", "ForbiddenTome")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 50;
            FargoGlobalNPC.SwarmMinionSpawnCount = 20;
            FargoGlobalNPC.SwarmBagId = ItemID.BossBagDarkMage;
            FargoGlobalNPC.SwarmTrophyId = ItemID.BossTrophyDarkmage;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerDarkMage>();
        }
    }
}
