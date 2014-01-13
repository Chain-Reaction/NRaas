using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class EndParty : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "EndParty";
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

            foreach (Situation sit in Situation.sAllSituations)
            {
                if (sit.Lot != lot) continue;

                HostedSituation hosted = sit as HostedSituation;
                if (hosted == null) continue;

                return true;
            }

            return false;
        }

        public static OptionResult Perform(Lot lot)
        {
            if (lot == null) return OptionResult.Failure;

            foreach (Situation sit in Situation.sAllSituations)
            {
                if (sit.Lot != lot) continue;

                HostedSituation hosted = sit as HostedSituation;
                if (hosted == null) continue;

                hosted.Exit();
            }

            return OptionResult.SuccessClose;
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            return Perform(lot);
        }
    }
}
