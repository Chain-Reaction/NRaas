using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class CancelPosture : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "CancelPosture";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }
        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.Posture == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (me != null && me.CreatedSim != null && me.CreatedSim.Posture != null)
            {
                me.CreatedSim.Posture.CancelPosture(me.CreatedSim);
            }

            return true;
        }
    }
}
