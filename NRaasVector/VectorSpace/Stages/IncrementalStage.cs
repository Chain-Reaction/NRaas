using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public class IncrementalStage : CounterStage
    {
        int mMaximum;

        string mMaxMutation;

        public IncrementalStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Maximum", Name))
            {
                mMaximum = row.GetInt("Maximum", 0);
            }

            mMaxMutation = row.GetString("MaxMutation");
        }

        protected override bool IsSuccess(SimDescription sim, DiseaseVector vector)
        {
            if (vector.Increment(Counter) < mMaximum) return true;

            vector.Erase(Counter);
            return false;
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Maximum: " + mMaximum;
            result += Common.NewLine + " MaxMutation: " + mMaxMutation;

            return result;
        }
    }
}
