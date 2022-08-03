using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons.Energizers
{
    public class EnergizerGolem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lihzahrd Energizer");
            Tooltip.SetDefault("Reward of the Runic Power Cell\n'You wish it was spelled \"lizard\"");
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