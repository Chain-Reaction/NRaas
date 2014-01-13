using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class LongTermPartnerScoring : LongTermRelationshipBaseScoring<SimScoringParameters>
    {
        public LongTermPartnerScoring()
        { }

        public override bool IsHit(SimScoringParameters parameters)
        {
            return BaseIsHit(parameters as DualSimScoringParameters);
        }

        public override int Score(SimScoringParameters parameters)
        {
            if (parameters.Actor.Partner == null)
            {
                return mNotKnown.Score(parameters);
            }
            else
            {
                bool success = false;
                int score = BaseScore(new DualSimScoringParameters(parameters.Actor, parameters.Actor.Partner), parameters.Actor.Partner, out success);

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
}

