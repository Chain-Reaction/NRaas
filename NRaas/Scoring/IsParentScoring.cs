using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class IsParentScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public IsParentScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return Helpers.Relationships.GetParents(parameters.Other).Contains(parameters.Actor);
        }
    }
}

