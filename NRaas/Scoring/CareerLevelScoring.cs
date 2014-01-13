using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class CareerLevelScoring : Scoring<SimDescription, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        int mScore;

        public CareerLevelScoring()
        { }
        public CareerLevelScoring(int score)
        {
            mScore = score;
        }

        public override string ToString()
        {
            return base.ToString() + "," + mScore;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Score"))
            {
                error = "Score missing";
                return false;
            }

            mScore = row.GetInt("Score", 0);

            return true;
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }
        public override int Score(SimScoringParameters parameters)
        {
            if (parameters.Actor.Occupation == null) return 0;

            return parameters.Actor.Occupation.CareerLevel * mScore;
        }
    }
}

