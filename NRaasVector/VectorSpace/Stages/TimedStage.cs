using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public abstract class TimedStage : VectorBooter.Stage
    {
        int mMinDuration;
        int mMaxDuration;

        string mMinMutation;
        string mMaxMutation;

        public TimedStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "MinDuration", Name))
            {
                mMinDuration = row.GetInt("MinDuration", 0);
            }

            mMinMutation = row.GetString("MinMutation");

            if (BooterLogger.Exists(row, "MaxDuration", Name))
            {
                mMaxDuration = row.GetInt("MaxDuration", 0);
            }

            mMaxMutation = row.GetString("MaxMutation");
        }

        public override int GetDuration(DiseaseVector vector)
        {
            int min = mMinDuration + vector.GetCounter(mMinMutation);
            int max = mMaxDuration + vector.GetCounter(mMaxMutation);

            if (max < min)
            {
                max = min;
            }

            if (max <= 0) return 0;

            return RandomUtil.GetInt(min, max);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Duration: " + mMinDuration + " to " + mMaxDuration;
            result += Common.NewLine + " Mutation: " + mMinMutation + " to " + mMaxMutation;

            return result;
        }
    }
}
