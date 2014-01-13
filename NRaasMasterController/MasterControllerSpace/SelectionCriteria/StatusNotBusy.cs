using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class StatusNotBusy : SelectionOption
    {
        public override string GetTitlePrefix()
        {
 	        return "Criteria.StatusNotBusy";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.CareerManager != null)
            {
                DateAndTime TwoHourTime = SimClock.CurrentTime();
                TwoHourTime.Ticks += SimClock.ConvertToTicks(2f, TimeUnit.Hours);

                if (me.Occupation != null)
                {
                    if (me.Occupation.IsWorkHour(TwoHourTime)) return false;
                }
                if (me.CareerManager.School != null)
                {
                    if (me.CareerManager.School.IsWorkHour(TwoHourTime)) return false;
                }
            }

            return true;
        }

        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            //if (!base.Allow(me, actor)) return false;

            return true;
        }
    }
}
