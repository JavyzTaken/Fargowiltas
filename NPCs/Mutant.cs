using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using Fargowiltas.Items.Summons.SwarmSummons;
using Fargowiltas.Items.Misc;
using Fargowiltas.Items.Summons.Mutant;
using Fargowiltas.Localization;
using Fargowiltas.Projectiles;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Fargowiltas.ShoppingBiomes;

namespace Fargowiltas.NPCs
{
    [AutoloadHead]
    public class Mutant : ModNPC
    {
        private static bool prehardmodeShop;
        private static bool hardmodeShop;
        private static int shopNum = 1;

        internal bool spawned;
        private bool canSayDefeatQuote = true;
        private int defeatQuoteTimer = 900;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = -1f,
                Direction = -1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPC.Happiness.SetBiomeAffection<SkyBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike);

            NPC.Happiness.SetNPCAffection<Abominationn>(AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection<Deviantt>(AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection<LumberJack>(AffectionLevel.Dislike);

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Suffocation
                }
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.Fargowiltas.Bestiary.Mutant")
            });
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = NPC.downedMoonlord ? 50 : 15;
            NPC.lifeMax = NPC.downedMoonlord ? 5000 : 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            if (GetInstance<FargoConfig>().CatchNPCs)
            {
                Main.npcCatchable[NPC.type] = true;
            //    NPC.catchItem = (short)Mod.ItemType("Mutant");
            }

            NPC.buffImmune[BuffID.Suffocation] = true;

            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
            {
                NPC.lifeMax = 77000;
                NPC.defense = 360;
            }
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void AI()
        {
            NPC.breath = 200;
            if (defeatQuoteTimer > 0)
                defeatQuoteTimer--;
            else
                canSayDefeatQuote = false;

            if (!spawned)
            {
                spawned = true;
                if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
                {
                    NPC.lifeMax = 77000;
                    NPC.life = NPC.lifeMax;
                    NPC.defense = 360;
                }
            }
        }

        public override bool CanTownNPCSpawn(int numTownnpcs, int money)
        {
            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("MutantAlive"))
            {
                return false;
            }

            return GetInstance<FargoConfig>().Mutant && FargoWorld.DownedBools["boss"] && !FargoGlobalNPC.AnyBossAlive();
        }

        public override List<string> SetNPCNameList() {
            string[] names =
            {
                "Mods.Fargowiltas.NPCs.Mutant.Names.Flacken", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Dorf", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Bingo",
                "Mods.Fargowiltas.NPCs.Mutant.Names.ans",
                "Mods.Fargowiltas.NPCs.Mutant.Names.Fargo", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Grim",
                "Mods.Fargowiltas.NPCs.Mutant.Names.Mike", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Fargu", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Terrance",
                "Mods.Fargowiltas.NPCs.Mutant.Names.CattyNPem", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Tom", 
                "Mods.Fargowiltas.NPCs.Mutant.Names.Weirdus",
                "Mods.Fargowiltas.NPCs.Mutant.Names.Polly"
            };

            return new List<string>(names.Select(x => Language.GetTextValue(x)));
        }

        public override string GetChat()
        {
            if (NPC.homeless && canSayDefeatQuote && Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
            {
                canSayDefeatQuote = false;

                if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("EternityMode"))
                    return MutantLangEntry.NPCs("Mutant.Dialog.EmbraceEternity");
                
                return MutantLangEntry.NPCs("Mutant.Dialog.DownedMutant");
            }

            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && Main.rand.NextBool(4))
            {
                if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("MutantArmor"))
                    return MutantLangEntry.NPCs("Mutant.Dialog.MutantArmor");
            }

            List<LangEntry> dialogue = new List<string>
            {
                "Savagery",
                "StrongerProgression",
                "DeathPerception",
                "SelfSummon",
                "BuyInBulk",
                "Apple",
                "JustMonika",
                "Lucky",
                "HamAndSwiss",
                "Clothes",
                "GoldSpending",
                "ViolenceForViolence",
                "DesertThemedBossesAreFunnyThoriumSpiritCalamityHolyCrapTrelamiumReference",
                "BrotherlyLove",
                "CalamityModReference",
                "BowBeforeMe",
                "ZombiesForBreakfast",
                "SpacebarJump",
                "GotYourNose",
                "Terry",
                "BlueDoll",
                "ImpendingDoomApproaches",
                "ThirdDimension",
                "FabsolCalamity",
                "FewerFriends",
                "DivermanuelSamuel",
                "Eearth",
                "Apotheosis",
                "NoPockets",
                "CatPerson",
                "GreenDragon",
                "Ohayou",
                "IAmAGreedyBastard",
                "SoulOfSouls",
                "EXBoxLive",
                "ComedicCritiqueOnModernArt",
                "HowManyGuidesDoesItTakeToOfferGoodAdvice",
                "Bedless",
                "UpdateSoonTm",
                "Slacking",
                "Fargo",
                "Ech",
                "SmoothJazzAndOtherFunBeatsToJamTo",
                "Cthulhu",
                "InfiniteUseBossSummons"
            }.Select<string, LangEntry>(x => MutantLangEntry.NPCs("Mutant.Dialog." + x)).ToList();

            if (Fargowiltas.ModLoaded["FargowiltasSouls"])
            {
                dialogue.AddWithCondition(MutantLangEntry.NPCs("Collecting"), NPC.downedMoonlord);

                if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
                {
                    dialogue.Add(MutantLangEntry.NPCs("RematchMe"));
                }
                else if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedFishronEX") || (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedAbom"))
                {
                    dialogue.Add(MutantLangEntry.NPCs("FightMe"));
                }
            }
            else
            {
                dialogue.Add(MutantLangEntry.NPCs("BadRatsIsAPhysicsPuzzleGameWhereRatsFinallyGetTheirBloodyRevengeOnTheirNewPrisonersTheCats"));
            }

            //dialogue.AddWithCondition(MutantLangEntry.NPCs("CalamitiModNaTelefonie"), Fargowiltas.ModLoaded["CalamityMod"]);
            //dialogue.AddWithCondition(MutantLangEntry.NPCs("DontMixContentModsItsKindOfIronicThoughBecauseSoulsIsAContentModAndSoulsDLCExists"), Fargowiltas.ModLoaded["CalamityMod"] && Fargowiltas.ModLoaded["ThoriumMod"]);
            //dialogue.AddWithCondition(MutantLangEntry.NPCs("Trelamium2"), Fargowiltas.ModLoaded["ThoriumMod"]);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("PumpkinMoon"), Main.pumpkinMoon);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("Coat"), Main.snowMoon);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("SlimeRain"), Main.slimeRain);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("BloodMoon"), Main.bloodMoon);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("WhyDidRedigitAddAPeriodJokeIntoTheGame"), Main.bloodMoon);
            dialogue.AddWithCondition(MutantLangEntry.NPCs("Sleepy"), !Main.dayTime);

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (BirthdayParty.PartyIsUp)
            {
                if (partyGirl >= 0)
                {
                    dialogue.Add($"{Main.npc[partyGirl].GivenName} is the one who invited me, I don't understand why though.");
                }
                
                dialogue.Add("I don't know what everyone's so happy about, but as long as nobody mistakes me for a Pigronata, I'm happy too.");
            }

            int nurse = NPC.FindFirstNPC(NPCID.Nurse);
            if (nurse >= 0)
            {
                dialogue.Add($"Whenever we're alone, {Main.npc[nurse].GivenName} keeps throwing syringes at me, no matter how many times I tell her to stop!");
            }

            int witchDoctor = NPC.FindFirstNPC(NPCID.WitchDoctor);
            if (witchDoctor >= 0)
            {
                dialogue.Add($"Please go tell {Main.npc[witchDoctor].GivenName} to drop the 'mystical' shtick, I mean, come on! I get it, you make tainted water or something.");
            }

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0)
            {
                dialogue.Add($"Why does {Main.npc[dryad].GivenName}'s outfit make my wings flutter?");
            }

            int stylist = NPC.FindFirstNPC(NPCID.Stylist);
            if (stylist >= 0)
            {
                dialogue.Add($"{Main.npc[stylist].GivenName} once gave me a wig... I look hideous with long hair.");
            }

            int truffle = NPC.FindFirstNPC(NPCID.Truffle);
            if (truffle >= 0)
            {
                dialogue.Add("That mutated mushroom seems like my type of fella.");
            }

            int tax = NPC.FindFirstNPC(NPCID.TaxCollector);
            if (tax >= 0)
            {
                dialogue.Add($"{Main.npc[tax].GivenName} keeps asking me for money, but he won't accept my spawners!");
            }

            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if (guide >= 0)
            {
                dialogue.Add($"Any idea why {Main.npc[guide].GivenName} is always cowering in fear when I get near him?");
            }

            int cyborg = NPC.FindFirstNPC(NPCID.Cyborg);
            if (truffle >= 0 && witchDoctor >= 0 && cyborg >= 0 && Main.rand.NextBool(52))
            {
                dialogue.Add($"If any of us could play instruments, I'd totally start a band with {Main.npc[witchDoctor].GivenName}, {Main.npc[truffle].GivenName}, and {Main.npc[cyborg].GivenName}.");
            }

            if (partyGirl >= 0)
            {
                dialogue.Add($"Man, {Main.npc[partyGirl].GivenName}'s confetti keeps getting stuck to my wings");
            }

            int demoman = NPC.FindFirstNPC(NPCID.Demolitionist);
            if (demoman >= 0)
            {
                dialogue.Add($"I'm surprised {Main.npc[demoman].GivenName} hasn't blown a hole in the floor yet, on second thought that sounds fun.");
            }

            int tavernkeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernkeep >= 0)
            {
                dialogue.Add($"{Main.npc[tavernkeep].GivenName} keeps suggesting I drink some beer, something tells me he wouldn't like me when I'm drunk though.");
            }

            int dyeTrader = NPC.FindFirstNPC(NPCID.DyeTrader);
            if (dyeTrader >= 0)
            {
                dialogue.Add($"{Main.npc[dyeTrader].GivenName} wants to see what I would look like in blue... I don't know how to feel.");
            }

            return Main.rand.Next(dialogue);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            switch (shopNum)
            {
                case 1:
                    button = MutantLangEntry.NPCs("Mutant.Buttons.PreHM");
                    break;

                case 2:
                    button = MutantLangEntry.NPCs("Mutant.Buttons.Hardmode");
                    break;

                default:
                    button = MutantLangEntry.NPCs("Mutant.Buttons.PostML");
                    break;
            }

            if (Main.hardMode)
            {
                button2 = MutantLangEntry.NPCs("CycleShop");
            }

            if (NPC.downedMoonlord)
            {
                if (shopNum >= 4)
                {
                    shopNum = 1;
                }
            }
            else
            {
                if (shopNum >= 3)
                {
                    shopNum = 1;
                }
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;

                switch (shopNum)
                {
                    case 1:
                        prehardmodeShop = true;
                        hardmodeShop = false;
                        break;
                    case 2:
                        hardmodeShop = true;
                        prehardmodeShop = false;
                        break;
                    default:
                        prehardmodeShop = false;
                        hardmodeShop = false;
                        break;
                }
            }
            else if (!firstButton && Main.hardMode)
            {
                shopNum++;
            }
        }

        public static void AddItem(bool check, int itemType, int price, ref Chest shop, ref int nextSlot)
        {
            if (!check || shop is null)
            {
                return;
            }

            shop.item[nextSlot].SetDefaults(itemType);
            shop.item[nextSlot].shopCustomPrice = price > 0 ? price : shop.item[nextSlot].value;

            // Lowered prices with discount card and pact
            if (Fargowiltas.ModLoaded["FargowiltasSouls"])
            {
                float modifier = 1f;
                if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("MutantDiscountCard"))
                {
                    modifier -= 0.2f;
                }

                if ((bool)ModLoader.GetMod("FargowiltasSouls").Call("MutantPact"))
                {
                    modifier -= 0.3f;
                }

                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * modifier);
            }

            nextSlot++;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            AddItem(Main.expertMode, ModContent.ItemType<Overloader>(), 400000, ref shop, ref nextSlot);

            if (prehardmodeShop)
            {
                AddItem(true, ModContent.ItemType<ModeToggle>(), -1, ref shop, ref nextSlot);

                if (Fargowiltas.ModLoaded["FargowiltasSouls"] && TryFind("FargowiltasSouls", "Masochist", out ModItem masochist))
                {
                    AddItem(true, masochist.Type, 10000, ref shop, ref nextSlot); // mutants gift, dam meme namer
                }

                foreach (MutantSummonInfo summon in Fargowiltas.summonTracker.SortedSummons)
                {
                    //phm
                    if (summon.progression <= MutantSummonTracker.WallOfFlesh)
                    {
                        AddItem(summon.downed(), summon.itemId, summon.price, ref shop, ref nextSlot);
                    }
                }
            }
            else if (hardmodeShop)
            {
                foreach (MutantSummonInfo summon in Fargowiltas.summonTracker.SortedSummons)
                {
                    //hm
                    if (summon.progression > MutantSummonTracker.WallOfFlesh && summon.progression <= MutantSummonTracker.Moonlord)
                    {
                        AddItem(summon.downed(), summon.itemId, summon.price, ref shop, ref nextSlot);
                    }
                }
            }
            else
            {
                foreach (MutantSummonInfo summon in Fargowiltas.summonTracker.SortedSummons)
                {
                    //post ml
                    if (summon.progression > MutantSummonTracker.Moonlord)
                    {
                        AddItem(summon.downed(), summon.itemId, summon.price, ref shop, ref nextSlot);
                    }
                }

                AddItem(true, ModContent.ItemType<AncientSeal>(), 100000000, ref shop, ref nextSlot);
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
            {
                damage = 700;
                knockback = 7f;
            }
            else if (NPC.downedMoonlord)
            {
                damage = 250;
                knockback = 6f;
            }
            else if (Main.hardMode)
            {
                damage = 60;
                knockback = 5f;
            }
            else
            {
                damage = 20;
                knockback = 4f;
            }
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            if (NPC.downedMoonlord)
            {
                cooldown = 1;
            }
            else if (Main.hardMode)
            {
                cooldown = 20;
                randExtraCooldown = 25;
            }
            else
            {
                cooldown = 30;
                randExtraCooldown = 30;
            }
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant") && TryFind("FargowiltasSouls", "MutantSpearThrownFriendly", out ModProjectile penetrator))
            {
                projType = penetrator.Type;
            }
            else if (NPC.downedMoonlord)
            {
                projType = ProjectileType<PhantasmalEyeProjectile>();
            }
            else if (Main.hardMode)
            {
                projType = ProjectileType<MechEyeProjectile>();
            }
            else
            {
                projType = ProjectileType<EyeProjectile>();
            }

            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            if (Fargowiltas.ModLoaded["FargowiltasSouls"] && (bool)ModLoader.GetMod("FargowiltasSouls").Call("DownedMutant"))
            {
                multiplier = 25f;
                randomOffset = 0f;
            }
            else
            {
                multiplier = 12f;
                randomOffset = 2f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 8; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, 2.5f * hitDirection, -2.5f, Scale: 0.8f);
                }

                if (!Main.dedServ)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                    Gore.NewGore(NPC.GetSource_Death(), pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/MutantGore3").Type);

                    pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                    Gore.NewGore(NPC.GetSource_Death(), pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/MutantGore2").Type);

                    pos = NPC.position + new Vector2(Main.rand.Next(NPC.width - 8), Main.rand.Next(NPC.height / 2));
                    Gore.NewGore(NPC.GetSource_Death(), pos, NPC.velocity, ModContent.Find<ModGore>("Fargowiltas/MutantGore1").Type);
                }
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
