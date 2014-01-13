using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class WeightMatchScoring : SimScaledScoring<DualSimScoringParameters>
    {
        public WeightMatchScoring()
        { }

        protected override int GetScaler(DualSimScoringParameters parameters)
        {
            return (int)Math.Abs((parameters.Actor.Weight - parameters.Other.Weight) * 100);
        }
    }
}

