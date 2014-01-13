using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class WeightScaledScoring : Scoring<SimDescription, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        int mUnderScore = 0;
        int mOverScore = 0;
        int mScale = 25;

        public WeightScaledScoring()
        { }
        public WeightScaledScoring(int thinScore, int fatScore, int scale)
        {
            mUnderScore = thinScore;
            mOverScore = fatScore;
            mScale = scale;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mUnderScore + "," + mOverScore + "," + mScale;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("UnderScore"))
            {
                error = "UnderScore missing";
                return false;
            }
            else if (!row.Exists("OverScore"))
            {
                error = "OverScore missing";
                return false;
            }
            else if (!row.Exists("Scale"))
            {
                error = "Scale missing";
                return false;
            }

            mUnderScore = row.GetInt("UnderScore", 0);
            mOverScore = row.GetInt("OverScore", 0);
            mScale = row.GetInt("Scale", 25);

            return true;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
        public override int Score(SimScoringParameters parameters)
        {
            int value = (int)(parameters.Actor.Weight * 100);

            int score = 0;
            if (value > 0)
            {
                score++;
            }
            else if (value < 0)
            {
                score--;
            }

            score += value / mScale;

            if (score > 0)
            {
                return score * mOverScore;
            }
            else
            {
                return -score * mUnderScore;
            }
        }
    }
}

