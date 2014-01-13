using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class LongTermRelationshipScaledScoring : SimScaledScoring<DualSimScoringParameters>
    {
        int mPosMultiple = 1;
        int mNegMultiple = 1;

        public LongTermRelationshipScaledScoring()
        { }
        public LongTermRelationshipScaledScoring(int score, int scale)
            : base(score, scale)
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mPosMultiple = row.GetInt("PosMultiple", 1);
            mNegMultiple = row.GetInt("NegMultiple", 1);

            return base.Parse(row, ref error);
        }

        public static int GetLTR(SimDescription a, SimDescription b)
        {
            Relationship relation = Relationship.Get(a, b, false);
            if (relation == null) return 0;

            return (int)relation.LTR.Liking;
        }

        protected override int GetScaler(DualSimScoringParameters parameters)
        {
            int score = GetLTR(parameters.Actor, parameters.Other);
            if (score > 0)
            {
                return (score * mPosMultiple);
            }
            else
            {
                return (score * mNegMultiple);
            }
        }
    }
}

