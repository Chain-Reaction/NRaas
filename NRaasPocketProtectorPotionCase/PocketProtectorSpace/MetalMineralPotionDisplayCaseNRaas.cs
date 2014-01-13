using Sims3.Gameplay;
using Sims3.Gameplay.Objects.HobbiesSkills;

namespace Sims3.Gameplay.Objects.HobbiesSkills
{
    public class MetalMineralPotionDisplayCaseNRaas : MetalMineralPotionDisplayCase
    {
        public MetalMineralPotionDisplayCaseNRaas()
        { }

        public override void DoReset(Gameplay.Abstracts.GameObject.ResetInformation resetInformation)
        {
            Inventory inventory = InventoryComp.Inventory;

            try
            {
                // Stops the case from deleting inventory content on reset
                InventoryComp.Inventory = null;

                base.DoReset(resetInformation);
            }
            finally
            {
                InventoryComp.Inventory = inventory;
            }
        }
    }
}
