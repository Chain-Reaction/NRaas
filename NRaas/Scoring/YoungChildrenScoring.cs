using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class YoungChildrenScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public YoungChildrenScoring()
        { }
        public YoungChildrenScoring(int hit, int miss)
            : base (hit, miss)
        { }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            if (parameters.Actor.Genealogy == null) return false;

            SimDescription other = null;

            DualSimScoringParameters dualParam = parameters as DualSimScoringParameters;
            if (dualParam != null)
            {
                other = dualParam.Other;

                if ((other != null) && (other.Genealogy == null)) return false;
            }

            foreach (SimDescription child in Helpers.Relationships.GetChildren(parameters.Actor))
            {
                if (!child.ChildOrBelow) continue;

                if (child.Genealogy == null) continue;

                if ((other == null) || (child.Genealogy.IsParentOrStepParent(other.Genealogy)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

