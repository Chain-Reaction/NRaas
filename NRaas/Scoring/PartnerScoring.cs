using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class PartnerScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public PartnerScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            if (parameters.Other == null) return false;

            if (parameters.Other.Partner == parameters.Actor) return true;

            if (parameters.Actor.Partner == parameters.Other) return true;

            return false;
        }
    }
}

