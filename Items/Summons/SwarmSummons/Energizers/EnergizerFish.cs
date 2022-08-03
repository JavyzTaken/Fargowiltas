using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons.Energizers
{
    public class EnergizerFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fishy Energizer");
            Tooltip.SetDefault("Reward of the Truffle Worm Clump\n'You feel content'");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.rare = 1;
            Item.value = 100000;
        }
    }
}