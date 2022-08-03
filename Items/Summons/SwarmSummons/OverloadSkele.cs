using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadSkele : SwarmSummonBase
    {
        public OverloadSkele() : base("", "", NPCID.SkeletronHead, "A great clammering of bones rises from the dungeon!", "SuspiciousSkull")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmMinionSpawnCount = 20;

            if (npcType == NPCID.SkeletronHead)
            {
                FargoGlobalNPC.SwarmHpMultiplier = 20;
                FargoGlobalNPC.SwarmBagId = ItemID.SkeletronBossBag;
                FargoGlobalNPC.SwarmTrophyId = ItemID.SkeletronTrophy;
                FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerSkele>();
            }
            else
            {
                FargoGlobalNPC.SwarmHpMultiplier = 5;
                FargoGlobalNPC.SwarmBagId = ItemID.BoneKey;
                FargoGlobalNPC.SwarmTrophyId = ItemID.SkeletronTrophy;
                FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerDG>();
            }


            
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Chain Necklace");
            Tooltip.SetDefault(
@"Summons a Skeletron swarm during the night
Summons a Dungeon Guardians swarm during the day
Beating the swarm rewards 100 Treasure Bags, 10 Trophies, and an Energizer");
        }
    }
}