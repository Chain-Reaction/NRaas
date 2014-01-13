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
    public class PreferenceMale : SelectionOption
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PreferenceMale";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (!me.TeenOrAbove) return false;

            if (me.mGenderPreferenceMale <= 0) return false;

            return true;
        }
    }
}
