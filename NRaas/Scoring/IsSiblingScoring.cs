using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class IsSiblingScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public IsSiblingScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return Helpers.Relationships.GetSiblings(parameters.Other).Contains(parameters.Actor);
        }
    }
}

