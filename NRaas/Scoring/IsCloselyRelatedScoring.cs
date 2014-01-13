using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class IsCloselyRelatedScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public IsCloselyRelatedScoring()
        { }
        public IsCloselyRelatedScoring(int hitScore, int missScore)
            : base(hitScore, missScore)
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return Helpers.Relationships.IsCloselyRelated(parameters.Actor, parameters.Other, false);
        }
    }
}

