using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class FitnessScaledScoring : SimScaledScoring<SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public FitnessScaledScoring()
        { }

        protected override int GetScaler(SimScoringParameters parameters)
        {
            return (int)(parameters.Actor.Fitness * 100);
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
    }
}

