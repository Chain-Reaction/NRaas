using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims.Basic;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class PartnerPollinate : SimFromList, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return "PartnerPollinate";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.Partner == null) return false;

            if (me.CreatedSim == null) return false;

            string reason = null;
            if (!Pollinate.Allow(me.CreatedSim, ref reason))
            {
                Common.DebugNotify("Reason: " + reason);
                return false;
            }

            return base.PrivateAllow(me);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Pollinate.Perform(me, me.Partner, ApplyAll);
        }
    }
}
