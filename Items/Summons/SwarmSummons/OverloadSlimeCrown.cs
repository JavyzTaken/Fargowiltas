using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadSlimeCrown : SwarmSummonBase
    {
        public OverloadSlimeCrown() : base("Swarm Crown", "King Slime", NPCID.KingSlime, "Welcome to the true slime rain!", "SlimyCrown")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 50;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.KingSlimeBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.KingSlimeTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerSlime>();
        }
    }
}