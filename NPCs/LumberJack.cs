using System.Collections.Generic;
using System.Linq;
using Fargowiltas.Items.Tiles;
using Fargowiltas.Items.Vanity;
using Fargowiltas.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Fargowiltas.NPCs
{
    [AutoloadHead]
    public class LumberJack : ModNPC
    {
        private bool dayOver;
        private bool nightOver;

        //public override bool Autoload(ref string name)
        //{
        //    name = "LumberJack";
        //    return mod.Properties.Autoload;
        //}

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LumberJack");

            Main.npcFrameCount[NPC.type] = 25;

            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = -1f,
                Direction = -1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("A wholly ordinary lumberjack that loves chopping wood. But could there be more to him than meets the eye? ...Probably not.")
            });
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 40;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            //if (GetInstance<FargoConfig>().CatchNPCs)
            //{
            //    Main.npcCatchable[NPC.type] = true;
            //    NPC.catchItem = (short)mod.ItemType("LumberJack");
            //}
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return GetInstance<FargoConfig>().Lumber && FargoWorld.DownedBools.TryGetValue("lumberjack", out bool down) && down;
        }

        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void AI()
        {
            if (!Main.dayTime)
            {
                nightOver = true;
            }

            if (Main.dayTime)
            {
                dayOver = true;
            }
        }

        public override string TownNPCName()
        {
            string[] names = { "Griff", "Jack", "Bruce", "Larry", "Will", "Jerry", "Liam", "Stan", "Lee", "Woody", "Leif", "Paul" };
            return Main.rand.Next(names);
        }

        public override string GetChat()
        {
            List<string> dialogue = new List<string>
            {
                "Dynasty wood? Between you and me, that stuff ain't real wood!",
                "Sure cactus isn't wood, but I can still chop it with me trusty axe.",
                "You wouldn't by chance have any fantasies about me... right?",
                "I eat a bowl of woodchips for breakfast... without any milk.",
                "TIIIIIIIIIMMMBEEEEEEEERRR!",
                "I'm a lumberjack and I'm okay, I sleep all night and I work all day!",
                "You won't ever need an axe again with me around.",
                "I have heard of people cutting trees with fish, who does that?",
                "You wanna see me work without my shirt on? Maybe in 2030.",
                "You ever seen the world tree?",
                "You want what? ...Sorry that's not the kind of wood I'm selling.",
                "Why don't I sell acorns? ...I replant all the trees I chop, don't you?",
                "What's the best kind of tree? ... Any if I can chop it.",
                "Can I axe you a question?",
                "Might take a nap under a tree later, care to join me?",
                "I'm an expert in all wood types.",
                "I wonder if there'll be more trees to chop in 1.4.",
                "Red is one of my favourite colors, right after wood.",
                "It's always flannel season.",
                "I'm glad my wood put such a big smile on your face."
            };

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0)
            {
                dialogue.Add($"{Main.npc[dryad].GivenName} told me to start hugging trees... I hug trees with my chainsaw.");
            }

            int nurse = NPC.FindFirstNPC(NPCID.Nurse);
            if (nurse >= 0)
            {
                dialogue.Add($"I always see {Main.npc[nurse].GivenName} looking at my biceps when I'm working. Wonder if she wants some of my wood.");
            }

            //if (Fargowiltas.ModLoaded["ThoriumMod"])
            //{
            //    dialogue.Add("Astroturf? Sorry I only grow trees on real grass.");
            //    dialogue.Add("Yew tree? Sakura tree? Nope, haven't found any.");
            //}

            return Main.rand.Next(dialogue);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Tree Treasures";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.LocalPlayer;

            if (firstButton)
            {
                shop = true;
                return;
            }

            if (dayOver && nightOver)
            {
                string quote = "";

                if (player.ZoneDesert && !player.ZoneBeach)
                {
                    quote = "While I was chopping down a cactus these things leaped at me, why don't you take care of them?";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Scorpion, ItemID.BlackScorpion }), 5);
                    player.QuickSpawnItem(ItemID.Cactus, 50);
                }
                else if (player.ZoneJungle)
                {
                    quote = "These mahogany trees are full of life, but a tree only has one purpose: to be chopped. Oh yea these fell out of the last one.";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Buggy, ItemID.Sluggy, ItemID.Grubby, ItemID.Frog }), 5);
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Mango, ItemID.Pineapple }), 5);
                    player.QuickSpawnItem(ItemID.RichMahogany, 50);
                }
                else if (player.ZoneHallow)
                {
                    quote = "This place is a bit fanciful for my tastes, but the wood's as choppable as any. Nighttime has these cool bugs though, take a few.";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.LightningBug, ItemID.FairyCritterBlue, ItemID.FairyCritterGreen, ItemID.FairyCritterPink }), 5);
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Starfruit, ItemID.Dragonfruit }), 5);
                    player.QuickSpawnItem(ItemID.Pearlwood, 50);

                    //add prismatic lacewing if post plantera
                }
                else if (player.ZoneGlowshroom && Main.hardMode)
                {
                    quote = "Whatever causes these to glow is beyond me, you're probably gonna eat them anyway so have this while you're at it.";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.GlowingSnail, ItemID.TruffleWorm }), 5);
                    player.QuickSpawnItem(ItemID.GlowingMushroom, 50);
                    //add mushroom grass seeds

                }
                else if (player.ZoneCorrupt || player.ZoneCrimson)
                {
                    quote = "The trees here are probably the toughest in this branch of reality.. Sorry, just tree puns. I found these for you here.";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Elderberry, ItemID.BlackCurrant, ItemID.BloodOrange, ItemID.Rambutan }), 5);
                }
                else if (player.ZoneSnow)
                {
                    //penguin
                    quote = "This neck of the woods is pretty eh? Here I've got some of my favorite wood for you.";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Cherry, ItemID.Plum }), 5);
                    player.QuickSpawnItem(ItemID.BorealWood, 50);
                }
                else if (player.ZoneBeach)
                {
                    quote = "Even on vacation, I always fit in a little chopping. A couple annoying birds fell out of a palm tree. Take them off my hands.. maybe cook them up?";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Coconut, ItemID.Banana }), 5);
                    player.QuickSpawnItem(ItemID.Seagull, 5);
                    player.QuickSpawnItem(ItemID.PalmWood, 50);
                }
                else if (player.ZoneUnderworldHeight)
                {
                    quote = "I looked around here for a while and didn't find any trees. I did find these little guys though. Maybe you'll want them?";
                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.HellButterfly, ItemID.MagmaSnail, ItemID.Lavafly }), 5);

                }
                else if (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight)
                {
                    quote = "I certainly didn't expect to find such wonderful trees down here. Check this out.";

                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Diamond, ItemID.Ruby, ItemID.Amethyst, ItemID.Emerald, ItemID.Sapphire, ItemID.Topaz, ItemID.Amber }), 15);

                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.GemSquirrelDiamond, ItemID.GemSquirrelAmber, ItemID.GemSquirrelAmethyst, ItemID.GemSquirrelEmerald, ItemID.GemSquirrelRuby, ItemID.GemSquirrelSapphire, ItemID.GemSquirrelTopaz, ItemID.GemBunnyAmber, ItemID.GemBunnyAmethyst, ItemID.GemBunnyDiamond, ItemID.GemBunnyEmerald, ItemID.GemBunnyRuby, ItemID.GemBunnySapphire, ItemID.GemBunnyTopaz }), 5);
                }
                //purity, most common option likely
                else// if (player.position.Y > Main.worldSurface)
                {
                    if (Main.dayTime)
                    {
                        //butterfly
                        if (Main.rand.Next(2) == 0)
                        {
                            quote = "Back in the day, people used to forge butterflies into powerful gear. We try to forget those days... but here have one.";
                            player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly, ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly }), 5);
                        }
                        else if (Main.rand.Next(20) == 0)
                        {
                            quote = "";
                            player.QuickSpawnItem(ItemID.EucaluptusSap);
                        }
                        else
                        {
                            quote = "These little critters are always falling out of the trees I cut down. Maybe you can find a use for them?";
                            player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Grasshopper, ItemID.Squirrel, ItemID.SquirrelRed, ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal }), 5);
                        }

                        
                        //add ladybug if its windy
                        //add rat if in graveyard
                    }
                    else
                    {
                        quote = "Chopping trees at night is always relaxing... well except for the flying eyeballs. Have one of these little guys to keep you company.";
                        player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Firefly }));
                    }

                    player.QuickSpawnItem(Main.rand.Next(new int[] { ItemID.Lemon, ItemID.Peach, ItemID.Apricot, ItemID.Grapefruit }), 5);
                    player.QuickSpawnItem(ItemID.Wood, 50);
                }

                Main.npcChatText = quote;
                dayOver = false;
                nightOver = false;
            }
            else
            {
                Main.npcChatText = "I'm resting after a good day of chopping, come back tomorrow and maybe I'll have something else for you.";
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ItemID.WoodPlatform);
            shop.item[nextSlot].shopCustomPrice = 5;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.Wood);
            shop.item[nextSlot].shopCustomPrice = 10;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.BorealWood);
            shop.item[nextSlot].shopCustomPrice = 10;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.RichMahogany);
            shop.item[nextSlot].shopCustomPrice = 15;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.PalmWood);
            shop.item[nextSlot].shopCustomPrice = 15;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.Ebonwood);
            shop.item[nextSlot].shopCustomPrice = 15;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.Shadewood);
            shop.item[nextSlot].shopCustomPrice = 15;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.Pearlwood);
            shop.item[nextSlot].shopCustomPrice = 20;
            nextSlot++;

            if (NPC.downedHalloweenKing)
            {
                shop.item[nextSlot].SetDefaults(ItemID.SpookyWood);
                shop.item[nextSlot].shopCustomPrice = 50;
                nextSlot++;
            }

            shop.item[nextSlot].SetDefaults(ItemID.Cactus);
            shop.item[nextSlot].shopCustomPrice = 10;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.LivingWoodWand);
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<LumberjackMask>());
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<LumberjackBody>());
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<LumberjackPants>());
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.LumberJaxe>());
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ItemID.SharpeningStation);
            shop.item[nextSlot].shopCustomPrice = 100000;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<WoodenToken>());
            shop.item[nextSlot].shopCustomPrice = 10000;
            nextSlot++;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<LumberJaxe>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void OnKill()
        {
            FargoWorld.DownedBools["lumberjack"] = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 8; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, 2.5f * hitDirection, -2.5f, Scale: 0.8f);
                }

                Vector2 pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                Gore.NewGore(pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/LumberGore3").Type);

                pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                Gore.NewGore(pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/LumberGore2").Type);

                pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                Gore.NewGore(pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/LumberGore1").Type);
            }
            else
            {
                for (int k = 0; k < damage / NPC.lifeMax * 50.0; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hitDirection, -1f, Scale: 0.6f);
                }
            }
        }
    }
}
