using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class OpenSesame : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "OpenSesame";
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            if (me == Household.ActiveHousehold) return false;

            if (lot.IsCommunityLot)
            {
                if (Camera.sActiveLotOverride == lot) return false;
            }
            else
            {
                Household active = Household.ActiveHousehold;
                if (active == null) return false;

                if (active.mGreetedLots.Contains(lot)) return false;
            }

            return true;
        }

        public static OptionResult Perform(Lot lot)
        {
            if (lot == null) return OptionResult.Failure;

            if (lot.IsCommunityLot)
            {
                Camera.sActiveLotOverride = lot;

                CameraController.TetherToLot(lot.LotId);
            }
            else
            {
                Household active = Household.ActiveHousehold;
                if (active == null) return OptionResult.Failure;

                active.AddGreetedLotToHousehold(lot, ObjectGuid.InvalidObjectGuid);
            }

            return OptionResult.SuccessClose;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            return Perform(lot);
        }
    }
}
