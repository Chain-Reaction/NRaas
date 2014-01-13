using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class TraitScoring : TraitBaseScoring<SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public TraitScoring()
        { }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
    }
}

