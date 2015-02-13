using Sims3.SimIFace;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase
{

    //[RuntimeExport]
    public class ani_StoreShelf : StoreSetBase
    {
        public override string GetSwipeAnimationName(GameObject target)
        {
            return "a_genericSwipe_pickUp_counter_x";
        }
        public override string GetSwipeVfxName()
        {
            return "store_shelfPurchase";
        }

        public override bool HandToolAllowPlacementInSlot(IGameObject objectToPlaceInSlot, Slot slot, AdditionalSlotPlacementCheckResults checks)
        {
            checks.IgnoreSlotPlacementFlags = true;
            return true;
        }
    }

}
