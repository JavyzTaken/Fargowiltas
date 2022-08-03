using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons.Energizers
{
    public class EnergizerMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Energizer");
            Tooltip.SetDefault("Reward of the Runic Conscription\n'You enjoy cheese'");
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