using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class LongTermRelationshipScoring : LongTermRelationshipBaseScoring<DualSimScoringParameters>
    {
        public LongTermRelationshipScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return BaseIsHit(parameters);
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            bool success;
            int score = BaseScore(parameters, parameters.Other, out success);

            if (success)
            {
                return score;
            }
            else
            {
                return base.Score(parameters);
            }
        }
    }
}

