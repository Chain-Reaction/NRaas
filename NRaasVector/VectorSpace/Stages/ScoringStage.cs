using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public class ScoringStage : HitMissTimedStage
    {
        string mScoring;

        int mMinimum;

        string mMinMutation;

        public ScoringStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Scoring", Name))
            {
                mScoring = row.GetString("Scoring");
                if (string.IsNullOrEmpty(mScoring))
                {
                    BooterLogger.AddError(Name + " Empty Scoring");
                }
                else if (ScoringLookup.GetScoring(mScoring) == null)
                {
                    BooterLogger.AddError(Name + " Invalid Scoring: " + mScoring);
                }
            }

            if (row.GetString("Minimum") == "Strength")
            {
                mMinimum = int.MinValue;
            }
            else
            {
                mMinimum = row.GetInt("Minimum", int.MinValue);
            }

            mMinMutation = row.GetString("MinMutation");
        }

        protected override bool IsSuccess(SimDescription sim, DiseaseVector vector)
        {
            int minimum = mMinimum;
            if (minimum == int.MinValue)
            {
                minimum = vector.NetStrength;
            }

            minimum += vector.GetCounter(mMinMutation);

            return (ScoringLookup.GetScore(mScoring, sim) > minimum);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Scoring: " + mScoring;

            if (mMinimum == int.MinValue)
            {
                result += Common.NewLine + " Minimum: <Strength>";
            }
            else
            {
                result += Common.NewLine + " Minimum: " + mMinimum;
            }

            result += Common.NewLine + " MinMutation: " + mMinMutation;

            return result;
        }
    }
}
