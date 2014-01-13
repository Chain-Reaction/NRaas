using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class MarriedScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public MarriedScoring()
        { }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            DualSimScoringParameters dualParameters = parameters as DualSimScoringParameters;
            if (dualParameters != null)
            {
                Relationship relation = Relationship.Get(dualParameters.Other, dualParameters.Actor, false);
                if (relation == null) return false;

                if (dualParameters.Actor.Partner != dualParameters.Other) return false;
            }

            return parameters.Actor.IsMarried;
        }
    }
}

