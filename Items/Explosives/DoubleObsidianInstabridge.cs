﻿using Fargowiltas.Projectiles.Explosives;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Explosives
{
    public class DoubleObsidianInstabridge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Double Obsidian Instabridge");
            Tooltip.SetDefault("Creates 2 bridges of obsidian platforms across the whole world" +
                               "\nAlso clears the area between and above the platforms" +
                               "\nDo not use if any important building is nearby");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 32;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.value = Item.buyPrice(0, 0, 3);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<DoubleObsInstaBridgeProj>();
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;

            Projectile.NewProjectile(player.GetProjectileSource_Item(source.Item), mouse, Vector2.Zero, type, 0, 0, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ObsidianInstaBridge>(), 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}