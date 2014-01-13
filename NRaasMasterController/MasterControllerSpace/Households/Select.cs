using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class Select : HouseholdFromList, IHouseholdOption
    {
        public override string Name
        {
            get
            {
                if (NRaas.MasterController.Settings.mDreamCatcher)
                {
                    return Common.Localize("SelectHouseDreamCatcher:MenuName");
                }

                return Common.Localize("SelectHouse:MenuName");
            }
        }

        public override string GetTitlePrefix()
        {
            return "SelectHouse";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            if (me == null) return false;

            if (me == Household.ActiveHousehold) return false;

            //if (SimTypes.IsSpecial(me)) return false;

            if (CommonSpace.Helpers.Households.NumSims(me) == 0) return false;

            return (true);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (me == null) return OptionResult.Failure;

            if (CommonSpace.Helpers.Households.NumSims(me) == 0) return OptionResult.Failure;

            foreach (SimDescription sim in CommonSpace.Helpers.Households.All(me))
            {
                if (Sims.Select.Perform(sim))
                {
                    return OptionResult.SuccessClose;
                }
            }

            return OptionResult.Failure;
        }
    }
}
