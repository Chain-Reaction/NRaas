using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class OtherSimTraitScoring : TraitBaseScoring<DualSimScoringParameters>
    {
        public OtherSimTraitScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            // Actors Reversed
            return base.IsHit(new DualSimScoringParameters(parameters.Other, parameters.Actor));
        }
    }
}

