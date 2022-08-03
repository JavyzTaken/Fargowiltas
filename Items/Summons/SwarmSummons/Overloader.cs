using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public class Overloader : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overloader");
            Tooltip.SetDefault("Used to craft swarm summons");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            Terraria.ID.ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.rare = 2;
        }
    }
}