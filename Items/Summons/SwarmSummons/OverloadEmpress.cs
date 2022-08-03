using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadEmpress : SwarmSummonBase
    {
        public OverloadEmpress() : base("Jar of Lacewings", "Empress of Light", NPCID.HallowBoss, "Bullet heaven is descending!", "PrismaticPrimrose")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 20;
            FargoGlobalNPC.SwarmBagId = ItemID.FairyQueenBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.FairyQueenTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerEmpress>();
        }
    }
}