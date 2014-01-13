using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public class TestCounterStage : CounterStage
    {
        int mMinimum;
        int mMaximum;

        string mMinMutation;
        string mMaxMutation;

        public TestCounterStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Minimum", Name))
            {
                mMinimum = row.GetInt("Minimum", 0);
            }

            mMinMutation = row.GetString("MinMutation");

            if (BooterLogger.Exists(row, "Maximum", Name))
            {
                mMaximum = row.GetInt("Maximum", 0);
            }

            mMaxMutation = row.GetString("MaxMutation");
        }

        protected override bool IsSuccess(SimDescription sim, DiseaseVector vector)
        {
            int value = vector.GetCounter(Counter);

            if (value > (mMaximum + vector.GetCounter(mMaxMutation))) return false;

            if (value < (mMinimum + vector.GetCounter(mMinMutation))) return false;

            return true;
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Minimum: " + mMinimum;
            result += Common.NewLine + " MinMutation: " + mMinMutation;
            result += Common.NewLine + " Minimum: " + mMinimum;
            result += Common.NewLine + " MaxMutation: " + mMaxMutation;

            return result;
        }
    }
}
