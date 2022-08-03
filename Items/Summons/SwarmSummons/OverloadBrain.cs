using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadBrain : SwarmSummonBase
    {
        public OverloadBrain() : base("Brain Storm", "Brain of Cthulhu", NPCID.BrainofCthulhu, "You feel dumb among so many brains!", "GoreySpine")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 25;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.BrainOfCthulhuBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.BrainofCthulhuTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerBrain>();
        }
    }
}