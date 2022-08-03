using System;
using System.Linq;
using System.Collections.Generic;
using Fargowiltas.Buffs;
using Fargowiltas.Items.Summons.SwarmSummons.Energizers;
using Fargowiltas.Items.Tiles;
////using Fargowiltas.Items.Vanity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;

namespace Fargowiltas.NPCs
{
    public class FargoGlobalNPC : GlobalNPC
    {
        internal static int[] Bosses = { 
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            //NPCID.EaterofWorldsHead,
            NPCID.BrainofCthulhu,
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.QueenSlimeBoss,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            NPCID.Retinazer,
            NPCID.Spazmatism,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.DukeFishron,
            NPCID.HallowBoss,
            NPCID.CultistBoss,
            NPCID.MoonLordCore,
            NPCID.MartianSaucerCore,
            NPCID.Pumpking,
            NPCID.IceQueen,
            NPCID.DD2Betsy,
            NPCID.DD2OgreT3,
            NPCID.IceGolem,
            NPCID.SandElemental,
            NPCID.Paladin,
            NPCID.Everscream,
            NPCID.MourningWood,
            NPCID.SantaNK1,
            NPCID.HeadlessHorseman,
            NPCID.PirateShip 
        };

        public static int LastWoFIndex = -1;
        public static int WoFDirection = 0;

        internal bool PillarSpawn = true;

        //swarm things
        public static float SwarmHpMultiplier;
        public static int SwarmMinionSpawnCount;
        public static int SwarmBagId;
        public static int SwarmTrophyId;
        public static int SwarmEnergizerId;

        public bool SwarmMaster = false;
        public bool SwarmMasterMinion = false; //ech real. (skeeltron arms etc)
        public bool SwarmMinion = false;
        //internal bool SwarmActive;
        //internal bool PandoraActive;
        internal bool NoLoot = false;

        public static int eaterBoss = -1;
        public static int brainBoss = -1;
        public static int plantBoss = -1;

        public override bool InstancePerEntity => true;

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            if (target.dontTakeDamage && target.type == NPCType<Squirrel>())
                return false;
            
            if (target.friendly && GetInstance<FargoConfig>().SaferBoundNPCs && (target.type == NPCID.BoundGoblin || target.type == NPCID.BoundMechanic || target.type == NPCID.BoundWizard || target.type == NPCID.BartenderUnconscious || target.type == NPCID.GolferRescue))
                return false;
            
            return base.CanHitNPC(npc, target);
        }

        private bool firstTick = true;
        private bool sluggishAI = true;
        private int swarmMinionAI;

        private void modifyStats(NPC npc, float healthMultiplier, float damageMultiplier, float defenseMultiplier, float sizeMultiplier)
        {
            npc.lifeMax = (int)(npc.lifeMax * healthMultiplier);
            npc.life = npc.lifeMax;
            npc.defDamage = (int)(npc.defDamage * damageMultiplier);
            npc.damage = (int)(npc.damage * damageMultiplier);
            npc.defDefense = (int)(npc.defDefense * defenseMultiplier);
            npc.defense = (int)(npc.defense * defenseMultiplier);

            //vanilla rescale code
            float originalScale = npc.scale;

            int scaledWidth = (int)(npc.width * npc.scale);
            int scaledHeight = (int)(npc.height * npc.scale);

            npc.position.X += scaledWidth / 2f;
            npc.position.Y += scaledHeight;
            npc.scale = originalScale * sizeMultiplier;
            npc.width = (int)(npc.width * npc.scale);
            npc.height = (int)(npc.height * npc.scale);

            if (npc.height == 16 || npc.height == 32)
                npc.height++;

            npc.position.X -= npc.width / 2f;
            npc.position.Y -= npc.height;
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.boss)
            {
                boss = npc.whoAmI;
            }

            switch (npc.type)
            {
                case NPCID.EaterofWorldsHead:

                    eaterBoss = npc.whoAmI;

                    if (FargoWorld.SwarmActive && firstTick)
                    {
                        modifyStats(npc, SwarmHpMultiplier, 2f, 2f, 2f);
                    }
                    

                    break;

                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    if (FargoWorld.SwarmActive && firstTick)
                    {
                        npc.scale = 2f;
                    //    modifyStats(npc, SwarmHpMultiplier, 2f, 2f, 4f);
                    }

                    break;

                case NPCID.BrainofCthulhu:
                    brainBoss = npc.whoAmI;
                    break;

                case NPCID.Plantera:
                    plantBoss = npc.whoAmI;
                    break;

                case NPCID.TheDestroyer:
                    if (FargoWorld.SwarmActive)
                    {
                        if (npc.ai[0] == 0)
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                return false;

                            for (int i = 0; i < Main.maxNPCs; i++) //purge segments i shouldn't have
                            {
                                if (Main.npc[i].active && (Main.npc[i].type == NPCID.TheDestroyerBody || Main.npc[i].type == NPCID.TheDestroyerTail) && Main.npc[i].realLife == npc.whoAmI)
                                {
                                    npc.life = 0;
                                    npc.HitEffect();
                                    npc.active = false;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                                }
                            }
                            
                            npc.ai[3] = npc.whoAmI;
                            npc.realLife = npc.whoAmI;
                            int prev = npc.whoAmI;
                            int bodySegments = 9;
                            for (int j = 0; j < bodySegments; j++)
                            {
                                int type = NPCID.TheDestroyerBody;
                                if (j == bodySegments - 1)
                                {
                                    type = NPCID.TheDestroyerTail;
                                }

                                int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + npc.height), type, npc.whoAmI);
                                NPC npc2 = Main.npc[n];
                                npc2.ai[3] = npc.whoAmI;
                                npc2.realLife = npc.whoAmI;
                                npc2.ai[1] = prev;
                                Main.npc[prev].ai[0] = n;

                                if (SwarmMaster)
                                {
                                    npc2.position.X += npc2.width / 2;
                                    npc2.position.Y += npc2.height;
                                    npc2.scale = 5f;
                                    npc2.width = (int)(98f * npc2.scale);
                                    npc2.height = (int)(92f * npc2.scale);
                                    npc2.position.X -= npc2.width / 2;
                                    npc2.position.Y -= npc2.height;
                                }


                                //Main.npc[n].GetGlobalNPC<FargoGlobalNPC>().SwarmActive = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, number: n);
                                prev = n;
                            }
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                                var netMessage = Mod.GetPacket();
                                netMessage.Write((byte)4);
                                netMessage.Write(npc.whoAmI);
                                netMessage.Write(npc.lifeMax);
                                netMessage.Send();
                            }
                            return false;
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int count = 0;
                            for (int i = 0; i < Main.maxNPCs; i++) //confirm i have exactly the right number of segments behind me
                            {
                                if (Main.npc[i].active && (Main.npc[i].type == NPCID.TheDestroyerBody || Main.npc[i].type == NPCID.TheDestroyerTail) && Main.npc[i].realLife == npc.whoAmI)
                                {
                                    count++;
                                    if (count > 9)
                                        break;
                                }
                            }

                            if (count != 9) //if not exactly the right pieces, die
                            {
                                npc.life = 0;
                                npc.HitEffect();
                                npc.active = false;
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    //NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("head killed by wrong count, " + count.ToString()), Color.White);
                                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                                }
                                else
                                {
                                    //Main.NewText("head killed by wrong count, " + count.ToString());
                                }
                            }
                        }
                    }
                    break;

                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    if (FargoWorld.SwarmActive)// && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //kill if real life is invalid
                        if (!(npc.realLife > -1 && npc.realLife < Main.maxNPCs && Main.npc[npc.realLife].active && Main.npc[npc.realLife].type == NPCID.TheDestroyer))
                        {
                            //Main.NewText("body realLife invalid, die");
                            npc.life = 0;
                            npc.HitEffect();
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                            return false;
                        }

                        int prev = npc.whoAmI;
                        int segment = (int)npc.ai[1];
                        int i = 0;
                        const int maxLength = 9;
                        for (; i < maxLength; i++) //iterate upwards along destroyer's body
                        {
                            if (segment > -1 && segment < Main.maxNPCs && Main.npc[segment].active && Main.npc[segment].type == NPCID.TheDestroyerBody
                                && Main.npc[segment].ai[3] == npc.ai[3] && Main.npc[segment].ai[0] == Main.npc[prev].whoAmI)
                            {
                                prev = segment;
                                segment = (int)Main.npc[segment].ai[1]; //continue if next is a valid BODY segment
                            }
                            else
                            {
                                break; //stop otherwise (this includes if head is found early, which is okay!)
                            }
                        }

                        //if last segment seen is indeed destroyer head
                        if (segment > -1 && segment < Main.maxNPCs && Main.npc[segment].active && Main.npc[segment].type == NPCID.TheDestroyer)
                        {
                            if (i == maxLength && npc.type != NPCID.TheDestroyerTail) //i am the furthest possible segment, become tail
                            {
                                //Main.NewText("body: become tail");
                                npc.type = NPCID.TheDestroyerTail;
                                npc.ai[0] = 0f;
                                npc.ai[2] = 0f;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                                npc.localAI[2] = 0f;
                                npc.localAI[3] = 0f;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                            }
                        }
                        else //last segment seen isn't destroyer head, die
                        {
                            //Main.NewText("body killed by wrong lead");
                            npc.life = 0;
                            npc.HitEffect();
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                            return false;
                        }
                    }
                    break;

                

                //                case NPCID.BlueSlime:
                //                    if (FargoWorld.OverloadedSlimeRain && npc.netID == NPCID.GreenSlime)
                //                    {
                //                        int[] slimes = { NPCID.BlueSlime, NPCID.RedSlime, NPCID.PurpleSlime, NPCID.YellowSlime, NPCID.BlackSlime, NPCID.JungleSlime };

                //                        npc.SetDefaults(slimes[Main.rand.Next(slimes.Length)]);

                //                        if (Main.netMode == NetmodeID.Server)
                //                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                //                    }

                //                    break;

                default:
                    break;
            }

            if (SwarmMaster)
            {
                if (firstTick)
                {
                    if (npc.type != NPCID.EaterofWorldsHead)
                    {
                        modifyStats(npc, SwarmHpMultiplier, 2f, 2f, 4f);
                    }

                    npc.GivenName = "Swarm Master " + npc.FullName;

                    if (npc.type != NPCID.BrainofCthulhu)
                    {
                        for (int i = 0; i < SwarmMinionSpawnCount; i++)
                        {
                            SpawnSwarmMinion(npc, npc.type);
                        }
                    }

                    firstTick = false;
                }

                if (swarmMinionAI++ >= 60 && NPC.CountNPCS(npc.type) < SwarmMinionSpawnCount)
                {
                    int minionID = npc.type;

                    SpawnSwarmMinion(npc, minionID);
                    swarmMinionAI = 0;
                }

                if (npc.type != NPCID.EaterofWorldsHead)
                {
                    npc.position -= npc.velocity * 0.5f;
                    sluggishAI = !sluggishAI;
                }


                

                switch (npc.type)
                {
                    case NPCID.KingSlime:
                    case NPCID.QueenSlimeBoss:
                    case NPCID.DD2Betsy:
                    case NPCID.MoonLordCore:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordHand:

                        npc.position.X += npc.width / 2;
                        npc.position.Y += npc.height;
                        npc.scale = 5f;
                        npc.width = (int)(98f * npc.scale);
                        npc.height = (int)(92f * npc.scale);
                        npc.position.X -= npc.width / 2;
                        npc.position.Y -= npc.height;

                        //return false;
                        break;

                }

                return sluggishAI;
            }

            firstTick = false;

            return true;
        }

        private void SpawnSwarmMinion(NPC npc, int boss)
        {
            int spawn = NPC.NewNPC(NPC.GetBossSpawnSource(Main.myPlayer), (int)npc.position.X + Main.rand.Next(-1000, 1000), (int)npc.position.Y + Main.rand.Next(-400, -100), boss);

            if (spawn != Main.maxNPCs)
            {
                NPC minion = Main.npc[spawn];
                minion.GetGlobalNPC<FargoGlobalNPC>().SwarmMinion = true;
                minion.boss = false;

                modifyStats(minion, SwarmHpMultiplier / 10, 1f, 0.5f, 0.75f);
                
                NetMessage.SendData(MessageID.SyncNPC, number: spawn);
            }


            //if (npc.type == NPCID.WallofFlesh)
            //{
            //    NPC currentWoF = Main.npc[LastWoFIndex];
            //    int startingPos = (int)currentWoF.position.X;
            //    spawn = NPC.NewNPC(NPC.GetBossSpawnSource(Main.myPlayer), startingPos + (400 * WoFDirection), (int)currentWoF.position.Y, NPCID.WallofFlesh, 0);
            //    if (spawn != Main.maxNPCs)
            //    {
            //        Main.npc[spawn].GetGlobalNPC<FargoGlobalNPC>().SwarmActive = true;
            //        LastWoFIndex = spawn;
            //    }
            //}
        }

        

        public override void AI(NPC npc)
        {
            // Wack ghost saucers begone
            if (FargoWorld.OverloadMartians && npc.type == NPCID.MartianSaucerCore && npc.dontTakeDamage)
            {
                npc.dontTakeDamage = false;
            }

            if (SwarmMaster)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                    case NPCID.QueenSlimeBoss:
                    case NPCID.DD2Betsy:
                    case NPCID.MoonLordCore:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordHand:
                        npc.position.X += npc.width / 2;
                        npc.position.Y += npc.height;
                        npc.scale = 5f;
                        npc.width = (int)(98f * npc.scale);
                        npc.height = (int)(92f * npc.scale);
                        npc.position.X -= npc.width / 2;
                        npc.position.Y -= npc.height;
                        break;

                }
            }

        }

        public override void PostAI(NPC npc)
        {
            if (FargoWorld.SwarmActive && (npc.type == NPCID.Golem || npc.type == NPCID.Deerclops || npc.type == NPCID.MoonLordCore))
                npc.dontTakeDamage = false; //always vulnerable in swarm

            if (SwarmMaster)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                    case NPCID.QueenSlimeBoss:
                    case NPCID.DD2Betsy:
                    case NPCID.MoonLordCore:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordHand:
                        npc.position.X += npc.width / 2;
                        npc.position.Y += npc.height;
                        npc.scale = 5f;
                        npc.width = (int)(98f * npc.scale);
                        npc.height = (int)(92f * npc.scale);
                        npc.position.X -= npc.width / 2;
                        npc.position.Y -= npc.height;
                        break;

                }
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNPC)
            {
                if (parentNPC.GetGlobalNPC<FargoGlobalNPC>().SwarmMaster)
                {
                    if (npc.type != NPCID.EaterofWorldsBody)
                    {
                        modifyStats(npc, SwarmHpMultiplier, 2f, 2f, 4f);
                    }

                    
                    //npc.scale = parentNPC.scale;
                    //npc.GetGlobalNPC<FargoGlobalNPC>().SwarmMasterMinion = true;
                }

                if (parentNPC.GetGlobalNPC<FargoGlobalNPC>().SwarmMinion)
                {
                    npc.scale = parentNPC.scale;
                }
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            Player player = Main.LocalPlayer;

            if (GetInstance<FargoConfig>().NPCSales)
            {
                void AddItem(ref int next, int itemID, int customPrice = -1)
                {
                    if (next >= 40)
                        return;

                    shop.item[next].SetDefaults(itemID);
                    if (customPrice != -1)
                        shop.item[next].shopCustomPrice = customPrice;

                    next++;
                }

                switch (type)
                {
                    case NPCID.PartyGirl:
                        if (BirthdayParty.PartyIsUp)
                        {
                            AddItem(ref nextSlot, ItemID.SliceOfCake);
                        }
                        break;

                    case NPCID.Clothier:
                        AddItem(ref nextSlot, ItemID.PharaohsMask, Item.buyPrice(gold: 1));
                        AddItem(ref nextSlot, ItemID.PharaohsRobe, Item.buyPrice(gold: 1));

                        if (player.anglerQuestsFinished >= 10)
                        {
                            AddItem(ref nextSlot, ItemID.AnglerHat);

                            if (player.anglerQuestsFinished >= 15)
                            {
                                AddItem(ref nextSlot, ItemID.AnglerVest);

                                if (player.anglerQuestsFinished >= 20)
                                {
                                    AddItem(ref nextSlot, ItemID.AnglerPants);
                                }
                            }
                        }

                        AddItem(ref nextSlot, ItemID.BlueBrick, Item.buyPrice(silver: 1));
                        AddItem(ref nextSlot, ItemType<UnsafeBlueBrickWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafeBlueSlabWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafeBlueTileWall>(), Item.buyPrice(copper: 25));

                        AddItem(ref nextSlot, ItemID.GreenBrick, Item.buyPrice(silver: 1));
                        AddItem(ref nextSlot, ItemType<UnsafeGreenBrickWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafeGreenSlabWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafeGreenTileWall>(), Item.buyPrice(copper: 25));

                        AddItem(ref nextSlot, ItemID.PinkBrick, Item.buyPrice(silver: 1));
                        AddItem(ref nextSlot, ItemType<UnsafePinkBrickWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafePinkSlabWall>(), Item.buyPrice(copper: 25));
                        AddItem(ref nextSlot, ItemType<UnsafePinkTileWall>(), Item.buyPrice(copper: 25));

                        if (Main.LocalPlayer.inventory.Any(i => !i.IsAir && i.useAmmo == ItemID.Bone))
                        {
                            AddItem(ref nextSlot, ItemType<Items.Ammos.BrittleBone>());
                        }
                        break;

                    case NPCID.Merchant:
                        if (player.anglerQuestsFinished >= 5)
                        {
                            AddItem(ref nextSlot, ItemID.FuzzyCarrot);

                            if (player.anglerQuestsFinished >= 10)
                            {
                                AddItem(ref nextSlot, ItemID.AnglerEarring);
                                AddItem(ref nextSlot, ItemID.HighTestFishingLine);
                                AddItem(ref nextSlot, ItemID.TackleBox);
                                AddItem(ref nextSlot, ItemID.GoldenBugNet);
                                AddItem(ref nextSlot, ItemID.FishHook);

                                if (Main.hardMode)
                                {
                                    AddItem(ref nextSlot, ItemID.FinWings);
                                    AddItem(ref nextSlot, ItemID.SuperAbsorbantSponge);
                                    AddItem(ref nextSlot, ItemID.BottomlessBucket);

                                    if (player.anglerQuestsFinished >= 25)
                                    {
                                        AddItem(ref nextSlot, ItemID.HotlineFishingHook);

                                        if (player.anglerQuestsFinished >= 30)
                                        {
                                            AddItem(ref nextSlot, ItemID.GoldenFishingRod);
                                        }
                                    }
                                }
                            }
                        }

                        if (Main.LocalPlayer.inventory.Any(i => !i.IsAir && i.useAmmo == AmmoID.Dart))
                        {
                            AddItem(ref nextSlot, ItemID.Seed, 3);
                        }
                        break;

                    case NPCID.Painter:

                        if (player.ZoneDungeon)
                        {
                            nextSlot = 15;

                            AddItem(ref nextSlot, ItemID.BloodMoonRising);
                            AddItem(ref nextSlot, ItemID.BoneWarp);
                            AddItem(ref nextSlot, ItemID.TheCreationoftheGuide);
                            AddItem(ref nextSlot, ItemID.TheCursedMan);
                            AddItem(ref nextSlot, ItemID.TheDestroyer);
                            AddItem(ref nextSlot, ItemID.Dryadisque);
                            AddItem(ref nextSlot, ItemID.TheEyeSeestheEnd);
                            AddItem(ref nextSlot, ItemID.FacingtheCerebralMastermind);
                            AddItem(ref nextSlot, ItemID.GloryoftheFire);
                            AddItem(ref nextSlot, ItemID.GoblinsPlayingPoker);
                            AddItem(ref nextSlot, ItemID.GreatWave);
                            AddItem(ref nextSlot, ItemID.TheGuardiansGaze);
                            AddItem(ref nextSlot, ItemID.TheHangedMan);
                            AddItem(ref nextSlot, ItemID.Impact);
                            AddItem(ref nextSlot, ItemID.ThePersistencyofEyes);
                            AddItem(ref nextSlot, ItemID.PoweredbyBirds);
                            AddItem(ref nextSlot, ItemID.TheScreamer);
                            AddItem(ref nextSlot, ItemID.SkellingtonJSkellingsworth);
                            AddItem(ref nextSlot, ItemID.SparkyPainting);
                            AddItem(ref nextSlot, ItemID.SomethingEvilisWatchingYou);
                            AddItem(ref nextSlot, ItemID.StarryNight);
                            AddItem(ref nextSlot, ItemID.TrioSuperHeroes);
                            AddItem(ref nextSlot, ItemID.TheTwinsHaveAwoken);
                            AddItem(ref nextSlot, ItemID.UnicornCrossingtheHallows);
                        }
                        else if (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight)
                        {
                            nextSlot = 19;

                            AddItem(ref nextSlot, ItemID.AmericanExplosive);
                            AddItem(ref nextSlot, ItemID.CrownoDevoursHisLunch);
                            AddItem(ref nextSlot, ItemID.Discover);
                            AddItem(ref nextSlot, ItemID.FatherofSomeone);
                            AddItem(ref nextSlot, ItemID.FindingGold);
                            AddItem(ref nextSlot, ItemID.GloriousNight);
                            AddItem(ref nextSlot, ItemID.GuidePicasso);
                            AddItem(ref nextSlot, ItemID.Land);
                            AddItem(ref nextSlot, ItemID.TheMerchant);
                            AddItem(ref nextSlot, ItemID.NurseLisa);
                            AddItem(ref nextSlot, ItemID.OldMiner);
                            AddItem(ref nextSlot, ItemID.RareEnchantment);
                            AddItem(ref nextSlot, ItemID.Sunflowers);
                            AddItem(ref nextSlot, ItemID.TerrarianGothic);
                            AddItem(ref nextSlot, ItemID.Waldo);
                        }
                        else if (player.ZoneUnderworldHeight)
                        {
                            nextSlot = 19;

                            AddItem(ref nextSlot, ItemID.DarkSoulReaper);
                            AddItem(ref nextSlot, ItemID.Darkness);
                            AddItem(ref nextSlot, ItemID.DemonsEye);
                            AddItem(ref nextSlot, ItemID.FlowingMagma);
                            AddItem(ref nextSlot, ItemID.HandEarth);
                            AddItem(ref nextSlot, ItemID.ImpFace);
                            AddItem(ref nextSlot, ItemID.LakeofFire);
                            AddItem(ref nextSlot, ItemID.LivingGore);
                            AddItem(ref nextSlot, ItemID.OminousPresence);
                            AddItem(ref nextSlot, ItemID.ShiningMoon);
                            AddItem(ref nextSlot, ItemID.Skelehead);
                            AddItem(ref nextSlot, ItemID.TrappedGhost);
                        }
                        //deserttt

                        break;

                    case NPCID.Demolitionist:
                        if (Main.hardMode)
                        {
                            AddItem(ref nextSlot, ItemID.CopperOre);
                            AddItem(ref nextSlot, ItemID.TinOre);
                            AddItem(ref nextSlot, ItemID.IronOre);
                            AddItem(ref nextSlot, ItemID.LeadOre);
                            AddItem(ref nextSlot, ItemID.SilverOre);
                            AddItem(ref nextSlot, ItemID.TungstenOre);
                            AddItem(ref nextSlot, ItemID.GoldOre);
                            AddItem(ref nextSlot, ItemID.PlatinumOre);
                        }
                        if (NPC.downedPlantBoss)
                        {
                            AddItem(ref nextSlot, ItemID.Meteorite);
                            AddItem(ref nextSlot, ItemID.DemoniteOre);
                            AddItem(ref nextSlot, ItemID.CrimtaneOre);
                            AddItem(ref nextSlot, ItemID.Hellstone);
                        }
                        if (NPC.downedMoonlord)
                        {
                            AddItem(ref nextSlot, ItemID.CobaltOre);
                            AddItem(ref nextSlot, ItemID.PalladiumOre);
                            AddItem(ref nextSlot, ItemID.MythrilOre);
                            AddItem(ref nextSlot, ItemID.OrichalcumOre);
                            AddItem(ref nextSlot, ItemID.AdamantiteOre);
                            AddItem(ref nextSlot, ItemID.TitaniumOre);
                            AddItem(ref nextSlot, ItemID.ChlorophyteOre);
                        }

                        break;

                    case NPCID.Steampunker:
                        AddItem(ref nextSlot, WorldGen.crimson ? ItemID.PurpleSolution : ItemID.RedSolution);
                        break;

                    case NPCID.DyeTrader:
                        FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
                        if (modPlayer.FirstDyeIngredients["RedHusk"])
                        {
                            AddItem(ref nextSlot, ItemID.RedHusk);
                        }
                        if (modPlayer.FirstDyeIngredients["OrangeBloodroot"])
                        {
                            AddItem(ref nextSlot, ItemID.OrangeBloodroot);
                        }
                        if (modPlayer.FirstDyeIngredients["YellowMarigold"])
                        {
                            AddItem(ref nextSlot, ItemID.YellowMarigold);
                        }
                        if (modPlayer.FirstDyeIngredients["LimeKelp"])
                        {
                            AddItem(ref nextSlot, ItemID.LimeKelp);
                        }
                        if (modPlayer.FirstDyeIngredients["GreenMushroom"])
                        {
                            AddItem(ref nextSlot, ItemID.GreenMushroom);
                        }
                        if (modPlayer.FirstDyeIngredients["TealMushroom"])
                        {
                            AddItem(ref nextSlot, ItemID.TealMushroom);
                        }
                        if (modPlayer.FirstDyeIngredients["CyanHusk"])
                        {
                            AddItem(ref nextSlot, ItemID.CyanHusk);
                        }
                        if (modPlayer.FirstDyeIngredients["SkyBlueFlower"])
                        {
                            AddItem(ref nextSlot, ItemID.SkyBlueFlower);
                        }
                        if (modPlayer.FirstDyeIngredients["BlueBerries"])
                        {
                            AddItem(ref nextSlot, ItemID.BlueBerries);
                        }
                        if (modPlayer.FirstDyeIngredients["PurpleMucos"])
                        {
                            AddItem(ref nextSlot, ItemID.PurpleMucos);
                        }
                        if (modPlayer.FirstDyeIngredients["VioletHusk"])
                        {
                            AddItem(ref nextSlot, ItemID.VioletHusk);
                        }
                        if (modPlayer.FirstDyeIngredients["PinkPricklyPear"])
                        {
                            AddItem(ref nextSlot, ItemID.PinkPricklyPear);
                        }
                        if (modPlayer.FirstDyeIngredients["BlackInk"])
                        {
                            AddItem(ref nextSlot, ItemID.BlackInk);
                        }

                        break;

                    case NPCID.Dryad:
                        if (Main.hardMode)
                        {
                            AddItem(ref nextSlot, ItemID.NaturesGift, Item.buyPrice(gold: 20));
                            AddItem(ref nextSlot, ItemID.JungleRose, Item.buyPrice(gold: 10));

                            AddItem(ref nextSlot, ItemID.StrangePlant1, Item.buyPrice(gold: 5));
                            AddItem(ref nextSlot, ItemID.StrangePlant2, Item.buyPrice(gold: 5));
                            AddItem(ref nextSlot, ItemID.StrangePlant3, Item.buyPrice(gold: 5));
                            AddItem(ref nextSlot, ItemID.StrangePlant4, Item.buyPrice(gold: 5));
                        }
                        break;

                    case NPCID.Wizard:
                        if (NPC.downedGolemBoss)
                            AddItem(ref nextSlot, ItemID.SuperManaPotion);
                        break;
                }
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            FargoPlayer fargoPlayer = player.GetFargoPlayer();

            if (fargoPlayer.BattleCry)
            {
                spawnRate = (int)(spawnRate * 0.1);
                maxSpawns = (int)(maxSpawns * 10f);
            }

            if (fargoPlayer.CalmingCry)
            {
                spawnRate = (int)(spawnRate * 10f);
                maxSpawns = (int)(maxSpawns * 0.1);
            }

            if ((FargoWorld.OverloadGoblins || FargoWorld.OverloadPirates) && player.position.X > Main.invasionX * 16.0 - 3000 && player.position.X < Main.invasionX * 16.0 + 3000)
            {
                if (FargoWorld.OverloadGoblins)
                {
                    spawnRate = (int)(spawnRate * 0.2);
                    maxSpawns = (int)(maxSpawns * 10f);
                }
                else if (FargoWorld.OverloadPirates)
                {
                    spawnRate = (int)(spawnRate * 0.2);
                    maxSpawns = (int)(maxSpawns * 30f);
                }
            }

            if (FargoWorld.OverloadPumpkinMoon || FargoWorld.OverloadFrostMoon)
            {
                spawnRate = (int)(spawnRate * 0.2);
                maxSpawns = (int)(maxSpawns * 10f);
            }
            else if (FargoWorld.OverloadMartians)
            {
                spawnRate = (int)(spawnRate * 0.2);
                maxSpawns = (int)(maxSpawns * 30f);
            }

            if (AnyBossAlive() && GetInstance<FargoConfig>().BossZen && player.Distance(Main.npc[boss].Center) < 6000)
            {
                maxSpawns = 0;
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = Main.LocalPlayer;

            if (FargoWorld.OverloadGoblins && player.position.X > Main.invasionX * 16.0 - 3000 && player.position.X < Main.invasionX * 16.0 + 3000)
            {
                // Literally nothing in the pool in the invasion so set everything to custom
                pool[NPCID.GoblinSummoner] = 1f;
                pool[NPCID.GoblinArcher] = 3f;
                pool[NPCID.GoblinPeon] = 5f;
                pool[NPCID.GoblinSorcerer] = 3f;
                pool[NPCID.GoblinWarrior] = 5f;
                pool[NPCID.GoblinThief] = 5f;
                pool[NPCID.GoblinScout] = 3f;
            }
            else if (FargoWorld.OverloadPirates && player.position.X > Main.invasionX * 16.0 - 3000 && player.position.X < Main.invasionX * 16.0 + 3000)
            {
                // Literally nothing in the pool in the invasion so set everything to custom
                if (NPC.CountNPCS(NPCID.PirateShip) < 4)
                {
                    pool[NPCID.PirateShip] = .5f;
                }

                pool[NPCID.Parrot] = 2f;
                pool[NPCID.PirateCaptain] = 1f;
                pool[NPCID.PirateCrossbower] = 3f;
                pool[NPCID.PirateCorsair] = 5f;
                pool[NPCID.PirateDeadeye] = 4f;
                pool[NPCID.PirateDeckhand] = 5f;
            }

            else if (FargoWorld.OverloadPumpkinMoon)
            {
                pool[NPCID.Pumpking] = 4f;
                pool[NPCID.MourningWood] = 4f;
                pool[NPCID.HeadlessHorseman] = 3f;
                pool[NPCID.Scarecrow1] = .5f;
                pool[NPCID.Scarecrow2] = .5f;
                pool[NPCID.Scarecrow3] = .5f;
                pool[NPCID.Scarecrow4] = .5f;
                pool[NPCID.Scarecrow5] = .5f;
                pool[NPCID.Scarecrow6] = .5f;
                pool[NPCID.Scarecrow7] = .5f;
                pool[NPCID.Scarecrow8] = .5f;
                pool[NPCID.Scarecrow9] = .5f;
                pool[NPCID.Scarecrow10] = .5f;
                pool[NPCID.Hellhound] = 3f;
                pool[NPCID.Poltergeist] = 3f;
                pool[NPCID.Splinterling] = 3f;
            }
            else if (FargoWorld.OverloadFrostMoon)
            {
                pool[NPCID.IceQueen] = 5f;
                pool[NPCID.Everscream] = 5f;
                pool[NPCID.SantaNK1] = 5f;
                pool[NPCID.ZombieElf] = 1f;
                pool[NPCID.ZombieElfBeard] = 1f;
                pool[NPCID.ZombieElfGirl] = 1f;
                pool[NPCID.GingerbreadMan] = 2f;
                pool[NPCID.ElfArcher] = 2f;
                pool[NPCID.Nutcracker] = 3f;
                pool[NPCID.ElfCopter] = 3f;
                pool[NPCID.Flocko] = 2f;
                pool[NPCID.Yeti] = 4f;
                pool[NPCID.PresentMimic] = 2f;
                pool[NPCID.Krampus] = 4f;
            }
            else if (FargoWorld.OverloadMartians)
            {
                pool[NPCID.MartianSaucerCore] = 1f;
                pool[NPCID.Scutlix] = 3f;
                pool[NPCID.ScutlixRider] = 2f;
                pool[NPCID.MartianWalker] = 3f;
                pool[NPCID.MartianDrone] = 2f;
                pool[NPCID.GigaZapper] = 1f;
                pool[NPCID.MartianEngineer] = 2f;
                pool[NPCID.MartianOfficer] = 2f;
                pool[NPCID.RayGunner] = 1f;
                pool[NPCID.GrayGrunt] = 1f;
                pool[NPCID.BrainScrambler] = 1f;
            }
        }

        public override bool PreKill(NPC npc)
        {
            if (NoLoot || FargoWorld.SwarmActive && !SwarmMaster || SwarmMinion)
            {
                return false;
            }

            //if (FargoWorld.SwarmActive && (npc.type == NPCID.BlueSlime || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.Creeper || (npc.type >= NPCID.PirateCorsair && npc.type <= NPCID.PirateCrossbower)))
            //{
            //    return false;
            //}

            if (FargoWorld.SwarmActive && Main.netMode != NetmodeID.MultiplayerClient && SwarmMaster)
            {
                switch (npc.type)
                {
                    case NPCID.EaterofWorldsHead:

                        bool flag = true;
                        for (int i = 0; i < 200; i++)
                        {
                            if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == 13 || Main.npc[i].type == 14 || Main.npc[i].type == 15))
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (!flag)
                        {
                            return false;
                        }
                        
                        break;


                    case NPCID.Spazmatism:
                    case NPCID.Retinazer:

                        break;
                }


                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                        Main.NewText("The swarm has been defeated!", new Color(206, 12, 15));
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The swarm has been defeated!"), new Color(206, 12, 15));
                    NetMessage.SendData(MessageID.WorldData); //sync world
                }

                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, SwarmEnergizerId, 1);
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, SwarmTrophyId, 10);
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, SwarmBagId, 100);

                FargoWorld.SwarmActive = false;

                return false;
            }

            return true;
        }

        public override void OnKill(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Painter:
                    if (NPC.AnyNPCs(NPCID.MoonLordCore))
                        Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemType<EchPainting>());
                    break;

                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    if (!DD2Event.Ongoing)
                    {
                        if (Main.rand.NextBool(14))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.BossMaskOgre);

                        Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] { ItemID.ApprenticeScarf, ItemID.SquireShield, ItemID.HuntressBuckler, ItemID.MonkBelt, ItemID.DD2SquireDemonSword, ItemID.MonkStaffT1, ItemID.MonkStaffT2, ItemID.BookStaff, ItemID.DD2PhoenixBow, ItemID.DD2PetGhost }));

                        Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.GoldCoin, Main.rand.Next(4, 7));
                    }
                    break;

                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    if (!DD2Event.Ongoing)
                    {
                        if (Main.rand.NextBool(14))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.BossMaskDarkMage);

                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.NextBool() ? ItemID.WarTable : ItemID.WarTableBanner);

                        if (Main.rand.NextBool(6))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] { ItemID.DD2PetGato, ItemID.DD2PetDragon }));
                    }
                    break;

                case NPCID.HeadlessHorseman:
                    if (!Main.dayTime && !Main.pumpkinMoon)
                    {
                        if (Main.rand.NextBool(20))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.JackOLanternMask);
                    }
                    break;

                case NPCID.MourningWood:
                    if (!Main.dayTime && !Main.pumpkinMoon)
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.SpookyWood, 30);

                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] {
                                ItemID.SpookyHook,
                                ItemID.SpookyTwig,
                                ItemID.StakeLauncher,
                                ItemID.CursedSapling,
                                ItemID.NecromanticScroll,
                                Main.expertMode ? ItemID.WitchBroom : ItemID.SpookyWood
                            }));
                    }
                    break;

                case NPCID.Pumpking:
                    if (!Main.dayTime && !Main.pumpkinMoon)
                    {
                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] {
                                ItemID.TheHorsemansBlade,
                                ItemID.BatScepter,
                                ItemID.BlackFairyDust,
                                ItemID.SpiderEgg,
                                ItemID.RavenStaff,
                                ItemID.CandyCornRifle,
                                ItemID.JackOLanternLauncher,
                                ItemID.ScytheWhip
                            }));
                    }
                    break;

                case NPCID.Everscream:
                    if (!Main.dayTime && !Main.snowMoon)
                    {
                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] {
                                ItemID.ChristmasTreeSword,
                                ItemID.ChristmasHook,
                                ItemID.Razorpine,
                                ItemID.FestiveWings
                            }));
                    }
                    break;

                case NPCID.SantaNK1:
                    if (!Main.dayTime && !Main.snowMoon)
                    {
                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] {
                                ItemID.EldMelter,
                                ItemID.ChainGun
                            }));
                    }
                    break;

                case NPCID.IceQueen:
                    if (!Main.dayTime && !Main.snowMoon)
                    {
                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, Main.rand.Next(new int[] {
                                ItemID.BlizzardStaff,
                                ItemID.SnowmanCannon,
                                ItemID.NorthPole,
                                ItemID.BabyGrinchMischiefWhistle,
                                ItemID.ReindeerBells
                            }));
                    }
                    break;

                default:
                    break;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.ZombieEskimo:
                case NPCID.ArmedZombieEskimo:
                case NPCID.Penguin:
                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                    npcLoot.Add(ItemDropRule.OneFromOptions(10, ItemID.EskimoHood, ItemID.EskimoCoat, ItemID.EskimoPants));
                    break;

                case NPCID.GreekSkeleton:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && (drop.itemId == ItemID.GladiatorHelmet || drop.itemId == ItemID.GladiatorBreastplate || drop.itemId == ItemID.GladiatorLeggings));
                    npcLoot.Add(ItemDropRule.OneFromOptions(10, ItemID.GladiatorHelmet, ItemID.GladiatorBreastplate, ItemID.GladiatorLeggings));
                    break;

                case NPCID.Merchant:
                    npcLoot.Add(ItemDropRule.Common(ItemID.MiningShirt, 8));
                    npcLoot.Add(ItemDropRule.Common(ItemID.MiningPants, 8));
                    break;

                case NPCID.Nurse:
                    npcLoot.Add(ItemDropRule.Common(ItemID.LifeCrystal, 5));
                    break;

                case NPCID.Demolitionist:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Dynamite, 2, 5, 5));
                    break;

                case NPCID.Dryad:
                    npcLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 3));
                    break;

                case NPCID.DD2Bartender:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Ale, 2, 4, 4));
                    break;

                case NPCID.ArmsDealer:
                    npcLoot.Add(ItemDropRule.Common(ItemID.NanoBullet, 4, 30, 30));
                    break;

                case NPCID.Clothier:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Skull, 20));
                    break;

                case NPCID.Mechanic:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Wire, 5, 40, 40));
                    break;

                case NPCID.Wizard:
                    npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 5, 5, 5));
                    break;

                case NPCID.TaxCollector:
                    npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 8, 10, 10));
                    break;

                case NPCID.Truffle:
                    npcLoot.Add(ItemDropRule.Common(ItemID.MushroomStatue, 8));
                    break;

                case NPCID.Angler:
                    npcLoot.Add(ItemDropRule.OneFromOptions(2, ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed));
                    break;


                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    npcLoot.Add(ItemDropRule.Common(ItemID.DefenderMedal, 1, 20, 20));
                    break;

                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    npcLoot.Add(ItemDropRule.Common(ItemID.DefenderMedal, 1, 5, 5));
                    break;

                case NPCID.Raven:
                    npcLoot.Add(ItemDropRule.Common(ItemID.GoodieBag));
                    break;

                case NPCID.SlimeRibbonRed:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Present));
                    break;

                case NPCID.BloodZombie:
                    npcLoot.Add(ItemDropRule.OneFromOptions(200, ItemID.BladedGlove, ItemID.BloodyMachete));
                    break;

                case NPCID.Clown:
                    npcLoot.Add(ItemDropRule.Common(ItemID.Bananarang));
                    break;

                case NPCID.MoonLordCore:
                    npcLoot.Add(ItemDropRule.Common(ItemID.MoonLordLegs, 100));
                    break;
            }

            base.ModifyNPCLoot(npc, npcLoot);
        }

        public override bool CheckDead(NPC npc)
        {
            // Lumber Jaxe
            if (npc.FindBuffIndex(ModContent.BuffType<WoodDrop>()) != -1)
            {
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ItemID.Wood, Main.rand.Next(10, 30));
            }

            switch (npc.type)
            {
                // Avoid lunar event with cultist summon
                case NPCID.CultistBoss:
                    if (!PillarSpawn)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc2 = Main.npc[i];
                            NPC.LunarApocalypseIsUp = false;

                            if (npc2.type == NPCID.LunarTowerNebula || npc2.type == NPCID.LunarTowerSolar || npc2.type == NPCID.LunarTowerStardust || npc2.type == NPCID.LunarTowerVortex)
                            {
                                NPC.TowerActiveSolar = true;
                                npc2.active = false;
                            }

                            NPC.TowerActiveSolar = false;
                        }
                    }

                    break;

                case NPCID.GiantWormHead:
                case NPCID.DiggerHead:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "worm");
                    break;

                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    FargoUtils.TryDowned(npc, "Abominationn", Color.Orange, "ogre");
                    break;

                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    FargoUtils.TryDowned(npc, "Abominationn", Color.Orange, "darkMage");
                    break;

                case NPCID.Clown:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "clown");

                    break;

                case NPCID.BlueSlime:
                    if (npc.netID == NPCID.Pinky)
                    {

                        FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "pinky");

                    }
                    break;

                case NPCID.UndeadMiner:

                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "undeadMiner");
                    break;

                case NPCID.Tim:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "tim");
                    break;

                case NPCID.DoctorBones:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "doctorBones");
                    break;

                case NPCID.Mimic:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "mimic");
                    break;

                case NPCID.WyvernHead:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "wyvern");
                    break;

                case NPCID.RuneWizard:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "runeWizard");
                    break;

                case NPCID.Nymph:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "nymph");
                    break;

                case NPCID.Moth:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "moth");
                    break;

                case NPCID.RainbowSlime:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "rainbowSlime");
                    break;

                case NPCID.Paladin:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedPlantBoss, "rareEnemy", "paladin");
                    break;

                case NPCID.Medusa:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "medusa");
                    break;

                case NPCID.IceGolem:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "iceGolem");
                    break;

                case NPCID.SandElemental:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "sandElemental");
                    break;

                case NPCID.Nailhead:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedPlantBoss, "rareEnemy", "nailhead");
                    break;

                case NPCID.Mothron:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "rareEnemy", "mothron");
                    break;

                case NPCID.BigMimicCorruption:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "mimicCorrupt");
                    break;

                case NPCID.BigMimicHallow:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "mimicHallow");
                    break;

                case NPCID.BigMimicCrimson:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "mimicCrimson");
                    break;

                case NPCID.BigMimicJungle:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "mimicJungle");
                    break;

                case NPCID.GoblinSummoner:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode && NPC.downedGoblins, "rareEnemy", "goblinSummoner");
                    break;

                case NPCID.PirateShip:
                    FargoUtils.TryDowned(npc, "Abominationn", Color.Orange, NPC.downedPirates, "flyingDutchman");
                    break;

                case NPCID.DungeonSlime:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedBoss3, "rareEnemy", "dungeonSlime");
                    break;

                case NPCID.PirateCaptain:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode && NPC.downedPirates, "rareEnemy", "pirateCaptain");
                    break;

                case NPCID.SkeletonSniper:
                case NPCID.TacticalSkeleton:
                case NPCID.SkeletonCommando:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedPlantBoss, "rareEnemy", "skeletonGun");
                    break;

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedPlantBoss, "rareEnemy", "skeletonMage");
                    break;

                case NPCID.BoneLee:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, NPC.downedPlantBoss, "rareEnemy", "boneLee");
                    break;

                case NPCID.HeadlessHorseman:
                    FargoUtils.TryDowned(npc, "Abominationn", Color.Orange, "headlessHorseman");
                    break;

                case NPCID.ZombieMerman:
                case NPCID.EyeballFlyingFish:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "zombieMerman", "eyeFish");
                    break;

                case NPCID.BloodEelHead:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "bloodEel");
                    break;

                case NPCID.GoblinShark:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, Main.hardMode, "rareEnemy", "goblinShark");
                    break;

                case NPCID.BloodNautilus:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "dreadnautilus");
                    break;

                case NPCID.Gnome:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "gnome");
                    break;

                case NPCID.RedDevil:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "redDevil");
                    break;

                case NPCID.GoldenSlime:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "goldenSlime");
                    break;

                case NPCID.GoblinScout:
                    FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", "goblinScout");
                    break;

                default:
                    break;
            }

            if (Fargowiltas.ModRareEnemies.ContainsKey(npc.type))
            {
                FargoUtils.TryDowned(npc, "Deviantt", Color.HotPink, "rareEnemy", Fargowiltas.ModRareEnemies[npc.type]);

            }

            if (npc.type == NPCID.DD2Betsy && !SwarmMinion)
            {
                FargoUtils.PrintText("Betsy has been defeated!", new Color(175, 75, 0));
                FargoWorld.DownedBools["betsy"] = true;
            }

            if (npc.boss)
            {
                FargoWorld.DownedBools["boss"] = true;
            }

            return true;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (GetInstance<FargoConfig>().RottenEggs && projectile.type == ProjectileID.RottenEgg && npc.townNPC)
            {
                damage *= 20;
            }
        }

        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            // No angler check enables luiafk compatibility
            if (GetInstance<FargoConfig>().AnglerQuestInstantReset && Main.anglerQuestFinished)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.AnglerQuestSwap();
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    // Broadcast swap request to server
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)3);
                    netMessage.Send();
                }
            }
        }

        

        

        //public static void SpawnWalls(Player player)
        //{
        //    int startingPos;

        //    if (LastWoFIndex == -1)
        //    {
        //        startingPos = (int)player.position.X;
        //    }
        //    else
        //    {
        //        startingPos = (int)Main.npc[LastWoFIndex].position.X;
        //    }

        //    Vector2 pos = player.position;

        //    if (WoFDirection == 0)
        //    {
        //        //1 is to the right, -1 is left
        //        WoFDirection = ((player.position.X / 16) > (Main.maxTilesX / 2)) ? 1 : -1;
        //    }

        //    int wof = NPC.NewNPC(NPC.GetBossSpawnSource(Main.myPlayer), startingPos + (400 * WoFDirection), (int)pos.Y, NPCID.WallofFlesh, 0);
        //    Main.npc[wof].GetGlobalNPC<FargoGlobalNPC>().SwarmActive = true;

        //    LastWoFIndex = wof;
        //}

        public static bool SpecificBossIsAlive(ref int bossID, int bossType)
        {
            if (bossID != -1)
            {
                if (Main.npc[bossID].active && Main.npc[bossID].type == bossType)
                {
                    return true;
                }
                else
                {
                    bossID = -1;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static int boss = -1;

        public static bool AnyBossAlive()
        {
            if (boss == -1)
                return false;

            NPC npc = Main.npc[boss];

            if (npc.active && npc.type != NPCID.MartianSaucerCore && (npc.boss || npc.type == NPCID.EaterofWorldsHead))
                return true;
            boss = -1;
            return false;
        }
    }
}