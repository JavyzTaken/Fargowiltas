using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadQueenSlime : SwarmSummonBase
    {
        public OverloadQueenSlime() : base("Swarm Crystal", "Queen Slime", NPCID.QueenSlimeBoss, "Welcome to the truer slime rain!", "JellyCrystal")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.QueenSlimeBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.QueenSlimeTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerQueenSlime>();
        }
    }
}