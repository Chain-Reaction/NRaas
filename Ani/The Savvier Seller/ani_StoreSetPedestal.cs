using Sims3.SimIFace;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Interfaces;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase
{
    //[RuntimeExport]
    public class ani_StoreSetPedestal : StoreSetBase
    {
        public override string GetSwipeVfxName()
        {
            return "store_pedestalPurchase";
        }

        public override bool HandToolAllowPlacementInSlot(IGameObject objectToPlaceInSlot, Slot slot, AdditionalSlotPlacementCheckResults checks)
        {
            checks.IgnoreSlotPlacementFlags = true;
            return true;
        }
    }

}
