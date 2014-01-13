using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Scoring
{
    public class RelationshipCircleScoring : Scoring<SimDescription,DualSimScoringParameters>
    {
        int mScore = 0;
        int mScale = 25;
        int mFriendBound = 0;
        int mEnemyBound = 0;

        public RelationshipCircleScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Score"))
            {
                error = "Score missing";
                return false;
            }
            else if (!row.Exists("Scale"))
            {
                error = "Scale missing";
                return false;
            }
            else if (!row.Exists("FriendBound"))
            {
                error = "FriendBound missing";
                return false;
            }
            else if (!row.Exists("EnemyBound"))
            {
                error = "EnemyBound missing";
                return false;
            }

            mScore = row.GetInt("Score");
            mScale = row.GetInt("Scale");
            mFriendBound = row.GetInt("FriendBound");
            mEnemyBound = row.GetInt("EnemyBound");

            return true;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            int score = 0;

            foreach (Relationship relation in Relationship.GetRelationships(parameters.Other))
            {
                LongTermRelationshipScaledScoring scoring = null;
                if (relation.LTR.Liking >= mFriendBound)
                {
                    scoring = new LongTermRelationshipScaledScoring(mScore, mScale);
                }
                else if (relation.LTR.Liking <= mEnemyBound)
                {
                    scoring = new LongTermRelationshipScaledScoring(-mScore, mScale);
                }

                if (scoring != null)
                {
                    score += scoring.Score(new DualSimScoringParameters(parameters.Actor, relation.GetOtherSimDescription(parameters.Other)));
                }
            }

            return score;
        }
    }
}
