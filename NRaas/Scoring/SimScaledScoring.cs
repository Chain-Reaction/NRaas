using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public abstract class SimScaledScoring<SP> : Scoring<SimDescription, SP>
        where SP : SimScoringParameters
    {
        int mScore = 0;
        int mScale = 25;

        public SimScaledScoring()
        { }
        protected SimScaledScoring(int score, int scale)
        {
            mScore = score;
            mScale = scale;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mScore + "," + mScale;
        }

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

            mScore = row.GetInt("Score", 0);
            mScale = row.GetInt("Scale", 25);

            return true;
        }

        protected abstract int GetScaler(SP parameters);

        public override int Score(SP parameters)
        {
            int value = GetScaler(parameters);

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

            return score * mScore;
        }
    }
}

