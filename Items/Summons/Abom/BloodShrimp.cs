using Terraria;
using Terraria.ID;

namespace Fargowiltas.Items.Summons.Abom
{
    public class BloodShrimp : BaseSummon
    {
        public override int NPCType => NPCID.BloodNautilus;

        public override string NPCName => "Dreadnautilus";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Blood Shrimp");
            Tooltip.SetDefault("Summons Dreadnautilus" +
                               "\nOnly usable at night");
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime;
        }
    }
}