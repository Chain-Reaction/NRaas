using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class RichScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>
    {
        public RichScoring()
        { }

        public override bool IsHit(SimScoringParameters parameters)
        {            
            return parameters.Actor.IsRich;
        }
    }
}

