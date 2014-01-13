using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class TraitCompareScoring : Scoring<SimDescription, DualSimScoringParameters>
    {
        int mHitScore = 0;
        int mMissScore = 0;

        public TraitCompareScoring()
        { }

        public override string ToString()
        {
            return base.ToString() + "," + mHitScore + "," + mMissScore;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Hit"))
            {
                error = "Hit missing";
                return false;
            }

            mHitScore = row.GetInt("Hit", 0);

            if (!row.Exists("Miss"))
            {
                error = "Miss missing";
                return false;
            }

            mMissScore = row.GetInt("Miss", 0);

            return true;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            int score = 0;
            foreach (Trait traitA in parameters.Other.TraitManager.List)
            {
                if (traitA.IsReward) continue;

                foreach (Trait traitB in parameters.Actor.TraitManager.List)
                {
                    if (traitB.IsReward) continue;

                    if (traitA.TraitGuid == traitB.TraitGuid)
                    {
                        score += mHitScore;
                    }
                    else if (TraitManager.DoTraitsConflict(traitA.Guid, traitB.Guid))
                    {
                        score -= mMissScore;
                    }
                }
            }

            return score;
        }
    }
}

