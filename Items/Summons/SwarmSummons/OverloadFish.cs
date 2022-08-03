using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class OverloadFish : SwarmSummonBase
    {
        public OverloadFish() : base("Truffle Worm Clump", "Duke Fishron", NPCID.DukeFishron, "The ocean swells with ferocious pigs!", "TruffleWorm2")
        {
        }

        protected override void setSwarmStats()
        {
            FargoGlobalNPC.SwarmHpMultiplier = 10;
            FargoGlobalNPC.SwarmMinionSpawnCount = 25;
            FargoGlobalNPC.SwarmBagId = ItemID.FishronBossBag;
            FargoGlobalNPC.SwarmTrophyId = ItemID.DukeFishronTrophy;
            FargoGlobalNPC.SwarmEnergizerId = ModContent.ItemType<Energizers.EnergizerFish>();
        }
    }
}