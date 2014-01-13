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
    public class SameHousehold : SelectionOption, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.SameHousehold";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (actor is SimDescription)
            {
                return (me.Household == (actor as SimDescription).Household);
            }
            else if (actor is MiniSimDescription)
            {
                return (actor as MiniSimDescription).HouseholdMembers.Contains(me.SimDescriptionId);
            }
            else
            {
                return false;
            }
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            return me.HouseholdMembers.Contains (actor.SimDescriptionId);
        }
    }
}
