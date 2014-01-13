using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class FocusHomeInspection : Focus
    {
        public override string GetTitlePrefix()
        {
            return "FocusHomeInspection";
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot.IsCommunityLot) return false;

            return (new HomeInspection(lot).Satisfies(me).Count > 0);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            Perform(lot);

            string reason = null;
            foreach (HomeInspection.Result result in new HomeInspection(lot).Satisfies(me))
            {
                switch (result.mReason)
                {
                    case HomeInspection.Reason.NoDouble:
                        if (!MasterController.Settings.mCheckForNoDoubleBed) continue;
                        break;
                    case HomeInspection.Reason.TooFewBeds:
                        continue;
                }

                reason += Common.NewLine + Common.Localize("HomeInspection:" + result.mReason, false, new object[] { result.mExisting, result.mRequired });
            }

            SimpleMessageDialog.Show(Name, Common.Localize("HomeInspection:Header") + reason);
            return OptionResult.SuccessClose;
        }
    }
}
