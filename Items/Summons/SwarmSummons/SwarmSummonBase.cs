using Fargowiltas.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Items.Summons.SwarmSummons
{
    public abstract class SwarmSummonBase : ModItem
    {
        //wof only
        private int counter = 0;

        private readonly string itemName;
        private readonly string bossName;
        protected int npcType;
        private readonly string spawnMessage;
        private readonly string material;

        protected SwarmSummonBase(string itemName, string bossName, int npcType, string spawnMessage, string material)
        {
            this.itemName = itemName;
            this.bossName = bossName;
            this.npcType = npcType;
            this.spawnMessage = spawnMessage;
            this.material = material;
        }

        protected abstract void setSwarmStats();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(itemName);
            Tooltip.SetDefault("Summons a " + bossName + " swarm\nBeating the swarm rewards 100 Treasure Bags, 10 Trophies, and an Energizer");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 100;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.consumable = true;

            //if (npcType == NPCID.WallofFlesh)
            //{
            //    Item.useAnimation = 20;
            //    Item.useTime = 2;
            //    Item.consumable = false;
            //}
        }

        public override bool CanUseItem(Player player)
        {
            return !FargoWorld.SwarmActive;
        }

        public override bool? UseItem(Player player)
        {
            FargoWorld.SwarmActive = true;
            setSwarmStats();

            //DG special case
            if (npcType == NPCID.SkeletronHead && Main.dayTime)
            {
                npcType = NPCID.DungeonGuardian;
            }
            //twins special case
            else if (npcType == NPCID.Retinazer)
            {
                //Fargowiltas.SwarmTotal *= 2;
            }

            //wof mega special case
            //if (npcType == NPCID.WallofFlesh)
            //{
                //    FargoGlobalNPC.SpawnWalls(player);
                //    counter++;

                //    if (counter < 10)
                //    {
                //        return true;
                //    }
            //}
            //else
            //{
                int boss = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)player.position.X + Main.rand.Next(-1000, 1000), (int)player.position.Y + Main.rand.Next(-600, -400), npcType);
                Main.npc[boss].GetGlobalNPC<FargoGlobalNPC>().SwarmMaster = true;

                //spawn the other twin as well
                if (npcType == NPCID.Retinazer)
                {
                    int twin = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)player.position.X + Main.rand.Next(-1000, 1000), (int)player.position.Y + Main.rand.Next(-1000, -400), NPCID.Spazmatism);
                    Main.npc[twin].GetGlobalNPC<FargoGlobalNPC>().SwarmMaster = true;
                }

            //}
        //}

            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(spawnMessage), new Color(175, 75, 255));
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(spawnMessage, 175, 75, 255);
            }

            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, material)
                .AddIngredient(null, "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}