using Sims3.SimIFace;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase
{
    [RuntimeExport]
    public class StoreSetParkingSpace : StoreSetBase
    {
        public const int kFillsNumSpaces = 1;
        [TunableComment("Necessary proximity before swiping"), Tunable]
        public new static float kMaxProximityBeforeSwiping = 6f;
        public virtual int FillsNumSpaces
        {
            get
            {
                return 1;
            }
        }
        public override float MaxProximityBeforeSwiping()
        {
            return StoreSetParkingSpace.kMaxProximityBeforeSwiping;
        }
        public override bool HandToolAllowPlacementInSlot(IGameObject objectToPlaceInSlot, Slot slot, AdditionalSlotPlacementCheckResults checks)
        {
            bool flag = false;
            IUsesParkingSpace usesParkingSpace = objectToPlaceInSlot as IUsesParkingSpace;
            if (usesParkingSpace != null && (ulong)usesParkingSpace.NumParkingSpotsOccupied <= (ulong)((long)this.FillsNumSpaces))
            {
                flag = true;
            }
            checks.IgnoreSlotPlacementFlags = flag;
            return flag;
        }
        public override string GetSwipeVfxName()
        {
            return "store_parkingPurchase";
        }
    }
}
