using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class IsChildScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public IsChildScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return Helpers.Relationships.GetChildren(parameters.Other).Contains(parameters.Actor);
        }
    }
}

