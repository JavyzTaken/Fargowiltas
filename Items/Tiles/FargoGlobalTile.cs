using Fargowiltas.Items.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Fargowiltas.Tiles
{
    public class FargoGlobalTile : GlobalTile
    {
        public override int[] AdjTiles(int type)
        {
            if (type == TileID.HeavyWorkBench)
            {
                int[] adjTiles = new int[] { TileID.WorkBenches, TileID.HeavyWorkBench };

                return adjTiles;
            }

            //if (type == ModContent.TileType<CrucibleCosmosSheet>())
            //{
            //    Main.LocalPlayer.adjHoney = true;
            //    Main.LocalPlayer.adjLava = true;
            //}

            return base.AdjTiles(type);
        }

        public override void MouseOver(int i, int j, int type)
        {
            if (type == TileID.Extractinator)
            {
                Main.player[Main.myPlayer].GetModPlayer<FargoPlayer>().extractSpeed = true;
            }
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
<<<<<<< HEAD
            if (WorldGen.gen)
            {
                return;
            }

            if (type == TileID.Trees && !FargoWorld.DownedBools["lumberjack"] && !fail)
=======
            if (type == TileID.Trees && !fail && !(FargoWorld.DownedBools.TryGetValue("lumberjack", out bool down) && down))
>>>>>>> 2dcfed5c271e143cd83cf1584b8324a1806beaab
            {
                FargoWorld.WoodChopped++;

                if (FargoWorld.WoodChopped > 500)
                {
                    FargoWorld.DownedBools["lumberjack"] = true;
                }
            }
        }

        internal static void DestroyChest(int x, int y)
        {
            int chestType = 1;

            int chest = Chest.FindChest(x, y);
            if (chest != -1)
            {
                for (int i = 0; i < 40; i++)
                {
                    Main.chest[chest].item[i] = new Item();
                }

                Main.chest[chest] = null;

                if (Main.tile[x, y].type == TileID.Containers2)
                {
                    chestType = 5;
                }

                if (Main.tile[x, y].type >= TileID.Count)
                {
                    chestType = 101;
                }
            }

            for (int i = x; i < x + 2; i++)
            {
                for (int j = y; j < y + 2; j++)
                {
                    Main.tile[i, j].type = 0;
                    Main.tile[i, j].sTileHeader = 0;
                    Main.tile[i, j].frameX = 0;
                    Main.tile[i, j].frameY = 0;
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (chest != -1)
                {
                    NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, chestType, x, y, 0f, chest, Main.tile[x, y].type);
                }

                NetMessage.SendTileSquare(-1, x, y, 3);
            }
        }

        internal static Point16 FindChestTopLeft(int x, int y, bool destroy)
        {
            Tile tile = Main.tile[x, y];
            if (TileID.Sets.BasicChest[tile.type])
            {
                TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
                x -= tile.frameX / 18 % data.Width;
                y -= tile.frameY / 18 % data.Height;

                if (destroy)
                {
                    DestroyChest(x, y);
                }

                return new Point16(x, y);
            }

            return Point16.NegativeOne;
        }

        internal static void ClearEverything(int x, int y)
        {
            FindChestTopLeft(x, y, true);

            Tile tile = Main.tile[x, y];
            WorldGen.KillTile(x, y, noItem: true);
            tile.ClearEverything();

            //tile.lava(false);
            //tile.honey(false);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.sendWater(x, y);
            }

            NetMessage.SendTileSquare(-1, x, y, 1);
        }
    }
}