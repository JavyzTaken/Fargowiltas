using Fargowiltas.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Fargowiltas
{
    internal class DevianttDialogueTracker
    {
        public static class HelpDialogueType
        {
            public static readonly byte BossOrEvent = 0;
            public static readonly byte Environment = 1;
            public static readonly byte Misc = 2;
        }

        public readonly record struct HelpDialogue
        {
            public string Key { get; }

            public byte Type { get; }

            public Predicate<string> Predicate { get; }

            public HelpDialogue(string key, byte type, Predicate<string>? predicate) {
                Key = "Mods.Fargowiltas.NPCs.DevianttDialog." + key;
                Type = type;
                Predicate = predicate ?? (_ => true);
            }

            public bool CanDisplay(string deviName) => Predicate(deviName);
        }

        public List<HelpDialogue> PossibleDialogue;
        private int lastDialogueType;

        public DevianttDialogueTracker()
        {
            PossibleDialogue = new List<HelpDialogue>();
        }

        public void AddDialogue(string key, byte type, Predicate<string> predicate = null)
        {
            PossibleDialogue.Add(new HelpDialogue(key, type, predicate));
        }

        public string GetDialogue(string deviName)
        {
            WeightedRandom<string> dialogueChooser = new WeightedRandom<string>();
            (List<HelpDialogue> sortedDialogue, int type) = SortDialogue(deviName);

            foreach (HelpDialogue dialogue in sortedDialogue)
            {
                dialogueChooser.Add(dialogue.Key);
            }

            lastDialogueType = type;
            return Language.GetTextValue(dialogueChooser.Get());
        }

        private (List<HelpDialogue> sortedDialogue, int type) SortDialogue(string deviName)
        {
            List<HelpDialogue> sortedDialogue = new List<HelpDialogue>();
            int typeChoice = 0;
            int attempts = 0;
            while (true)
            {
                attempts++;
                typeChoice = Main.rand.Next(3);
                if (typeChoice != lastDialogueType || typeChoice == HelpDialogueType.Misc) // There's a lot more misc so allow repeats
                {
                    sortedDialogue = PossibleDialogue.Where((dialogue) => dialogue.Type == typeChoice && dialogue.CanDisplay(deviName)).ToList();

                    if (sortedDialogue.Count != 0)
                        break;
                }
                
                if (attempts == 100)
                {
                    typeChoice = HelpDialogueType.BossOrEvent;
                    sortedDialogue = PossibleDialogue.Where((dialogue) => dialogue.Type == typeChoice && dialogue.CanDisplay(deviName)).ToList();
                    break;
                }
            }

            return (sortedDialogue, typeChoice);
        }

        public void AddVanillaDialogue() {
            Mod soulsMod = ModLoader.GetMod("FargowiltasSouls");
            bool CallBoolean(string call) => (bool?) soulsMod.Call(call) ?? false;

            #region Boss/Event Dialog

            byte d = HelpDialogueType.BossOrEvent;
            AddDialogue("PostMutantTeaser", d, _ => CallBoolean("DownedMutant"));
            AddDialogue("MutantTip", d, _ => CallBoolean("DownedAbom") && !CallBoolean("DownedMutant"));
            AddDialogue("SigilOfChampionsTip", d, _ => NPC.downedMoonlord && !CallBoolean("DownedEridanus"));
            AddDialogue("CelestialPillarsTip", d, _ => NPC.downedAncientCultist && !NPC.downedMoonlord);
            AddDialogue("AncientCultistTip", d, _ => NPC.downedFishron && !NPC.downedAncientCultist);
            AddDialogue("DukeFishronTip", d, _ => FargoWorld.DownedBools["betsy"] && !NPC.downedFishron);
            AddDialogue("BetsyTip", d, _ => NPC.downedGolemBoss && !FargoWorld.DownedBools["betsy"]);
            AddDialogue("GolemTip", d, _ => NPC.downedPlantBoss && !NPC.downedGolemBoss);
            AddDialogue("PlanteraTip", d, _ => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && !NPC.downedPlantBoss);
            //AddDialogue("PiratesTip", d, _ => Main.hardMode && !NPC.downedPirates);
            AddDialogue("DestroyerTip", d, _ => Main.hardMode && !NPC.downedMechBoss1);
            AddDialogue("TheTwinsTip", d, _ => Main.hardMode && !NPC.downedMechBoss2);
            AddDialogue("SkeletronPrimeTip", d, _ => Main.hardMode && !NPC.downedMechBoss3);
            AddDialogue("WallOfFleshTip", d, _ => CallBoolean("DownedDevi") && !Main.hardMode);
            AddDialogue("DeviTip", d, _ => NPC.downedBoss3 && !CallBoolean("DownedDevi"));
            AddDialogue("SkeletronTip", d, _ => NPC.downedQueenBee && !NPC.downedBoss3);
            AddDialogue("QueenBeeTip", d, _ => NPC.downedBoss2 && !NPC.downedQueenBee);
            AddDialogue("BrainOfCthulhuTip", d, _ => NPC.downedBoss1 && !NPC.downedBoss2 && WorldGen.crimson);
            AddDialogue("EaterOfWorldsTip", d, _ => NPC.downedBoss1 && !NPC.downedBoss2 && !WorldGen.crimson);
            AddDialogue("GoblinsCrimsonTip", d, _ => !NPC.downedGoblins && WorldGen.crimson);
            AddDialogue("GoblinsCorruptionTip", d, _ => !NPC.downedGoblins && !WorldGen.crimson);
            // I added this because, if there isn't always dialogue available for a boss, the dialogue chooser self destructs
            AddDialogue("EyeOfCthulhuTip", d, _ => NPC.downedSlimeKing && !NPC.downedBoss1);
            AddDialogue("KingSlimeTip", d, _ => !NPC.downedSlimeKing);

            #endregion

            #region Environment Dialog

            d = HelpDialogueType.Environment;
            AddDialogue("WaterTip", d, _ => !Main.LocalPlayer.accFlipper && !Main.LocalPlayer.gills && !CallBoolean("MutantAntibodies"));
            AddDialogue("LavaTip", d, _ => !Main.LocalPlayer.fireWalk && !(Main.LocalPlayer.lavaMax > 0) && !Main.LocalPlayer.buffImmune[BuffID.OnFire] && !CallBoolean("PureHeart"));
            AddDialogue("SpaceTip", d, _ => !Main.LocalPlayer.buffImmune[BuffID.Suffocation] && !CallBoolean("PureHeart"));
            //AddDialogue("OldHardmodeTip", HelpDialogueType.Environment, _ => Main.hardMode && !CallBoolean(PureHeart));
            AddDialogue("HardmodeTip", d, _ => Main.hardMode && !CallBoolean("PureHeart"));

            #endregion

            #region Misc Dialog

            d = HelpDialogueType.Misc;
            AddDialogue("AbomFocusTip", d, _ => CallBoolean("DownedEridanus") && !CallBoolean("DownedAbom"));
            AddDialogue("AuraTip", d);
            AddDialogue("DebuffImmunityTip", d);
            AddDialogue("SoulToggleTip", d);
            //AddDialogue("LessEffectiveAmmoTip", d, _ => Main.LocalPlayer.HeldItem.CountsAsClass(DamageClass.Ranged));
            //AddDialogue("TopHatSquirrelTip", d, _ => !NPC.AnyNPCs(ModContent.NPCType<Squirrel>()));
            AddDialogue("KingSlimeLifeCrystalTip", d, _ => Main.LocalPlayer.statLifeMax < 400);
            AddDialogue("FishGivingMidleFingerTip", d, _ => !Main.hardMode);
            //AddDialogue("ThisIsAnRPGGameAfterAllTip", d, _ => Main.hardMode);
            AddDialogue("ChlorophyteLifeFruitTip", d, _ => Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && Main.LocalPlayer.statLifeMax2 < 500);
            // This is much more possible than before because of how branching works, so I just decided to remove it.
            //AddDialogue("EnchantmentsUsedToBeActuallyHardToObtainTip", d, _ => Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3);

            #endregion
        }
    }
}
