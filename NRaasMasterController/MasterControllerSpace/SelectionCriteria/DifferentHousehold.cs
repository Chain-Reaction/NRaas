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
    [Persistable]
    public class DifferentHousehold : SelectionOption
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.DifferentHousehold";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (actor is SimDescription)
            {
                return (me.Household != (actor as SimDescription).Household);
            }
            else if (actor is MiniSimDescription)
            {
                MiniSimDescription miniSim = actor as MiniSimDescription;
                if (miniSim.HouseholdMembers == null) return false;

                return !miniSim.HouseholdMembers.Contains(me.SimDescriptionId);
            }
            else
            {
                return false;
            }
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            if (me.HouseholdMembers == null) return false;

            return !me.HouseholdMembers.Contains (actor.SimDescriptionId);
        }
    }
}
