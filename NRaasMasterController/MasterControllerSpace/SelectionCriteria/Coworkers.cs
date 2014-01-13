using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class Coworkers : SelectionOption, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Coworkers";
        }

        protected bool IsCoworker(Occupation career, SimDescription me)
        {
            if (career == null) return false;

            if (career.Coworkers == null) return false;

            if (career.Coworkers.Contains(me)) return true;

            return (career.Boss == me);
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            SimDescription actorDesc = actor as SimDescription;
            if (actorDesc == null) return false;

            if (IsCoworker(actorDesc.Occupation, me))
            {
                return true;
            }

            if ((actorDesc.CareerManager != null) && (IsCoworker(actorDesc.CareerManager.School, me)))
            {
                return true;
            }

            return false;
        }
    }
}
