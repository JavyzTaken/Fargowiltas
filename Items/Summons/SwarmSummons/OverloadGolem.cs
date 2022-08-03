using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadGolem : SwarmSummonBase
    {
        public OverloadGolem() : base("Runic Power Cell", "Golem", NPCID.Golem, "Ancient automatons come crashing down!", "LihzahrdPowerCell2")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.GolemBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.GolemTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerGolem>();
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive && NPC.downedPlantBoss;
        }
    }
}