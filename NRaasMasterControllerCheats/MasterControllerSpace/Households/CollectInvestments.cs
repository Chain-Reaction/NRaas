using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class CollectInvestments : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "CollectInvestments";
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

            if (me == null) return false;

            if (me.IsSpecialHousehold) return false;

            if (me.RealEstateManager.AllProperties.Count == 0) return false;

            if (CommonSpace.Helpers.Households.AllSims(me).Count == 0) return false;

            return true;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            Sim sim = CommonSpace.Helpers.Households.AllSims(me)[0];

            int collected = Investments.Collect(sim);

            if (collected > 0)
            {
                Common.Notify(Common.Localize(GetTitlePrefix () + ":Success", false, new object[] { me.Name, collected }));
            }

            return OptionResult.SuccessClose;
        }
    }
}
