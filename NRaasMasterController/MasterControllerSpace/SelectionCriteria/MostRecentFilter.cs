using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class MostRecentFilter : SelectionOption
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.MostRecent";
        }

        protected override bool Allow(CommonSpace.Options.MiniSimDescriptionParameters parameters)
        {
            if (MasterController.Settings.mMostRecentFilter == null) return false;

            return base.Allow(parameters);
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            return true;
        }
    }
}
