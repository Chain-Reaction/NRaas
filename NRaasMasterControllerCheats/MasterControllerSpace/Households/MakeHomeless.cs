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
    public class MakeHomeless : HouseholdFromList, IHouseholdOption
    {
        protected override int GetMaxSelection()
        {
            return 0;
        }

        public override string GetTitlePrefix()
        {
            return "MakeHomeless";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            if (me == Household.ActiveHousehold) return false;

            return (me != null);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (me == null) return OptionResult.Failure;

            if (!AcceptCancelDialog.Show(Common.Localize("MakeHomeless:Prompt", false, new object[] { me.Name })))
            {
                return OptionResult.Failure;
            }

            me.MoveOut();
            return OptionResult.SuccessClose;
        }
    }
}
