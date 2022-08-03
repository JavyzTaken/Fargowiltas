using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadTwins : SwarmSummonBase
    {
        public OverloadTwins() : base("Omnifocal Lens", "Twins", NPCID.Retinazer, "A legion of glowing iris sing a dreadful song!", "MechEye")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.TwinsBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.SpazmatismTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerTwins>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && !Main.dayTime;
        }
    }
}