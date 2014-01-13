using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class SameHouseScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public SameHouseScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            if (parameters.Other == null) return false;

            return (parameters.Actor.Household == parameters.Other.Household);
        }
    }
}

