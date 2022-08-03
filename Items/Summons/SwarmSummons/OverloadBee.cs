using Fargowiltas.Items.Summons.SwarmSummons.Energizers;
using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadBee : SwarmSummonBase
    {
        public OverloadBee() : base("Overstuffed Larva", "Queen Bee", NPCID.QueenBee, "A deafening buzz pierces through you!", "Abeemination2")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 20;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.QueenBeeBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.QueenBeeTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<EnergizerBee>();
        }
    }
}