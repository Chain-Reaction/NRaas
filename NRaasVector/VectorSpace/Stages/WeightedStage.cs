using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Stages
{
    public class WeightedStage : TimedStage
    {
        Pair<string,int>[] mNextStages;
        int[] mNextWeights;

        int mTotalWeight;

        public WeightedStage(XmlDbRow row)
            : base(row)
        {
            if (!BooterLogger.Exists(row, "NextStages", Name)) return;
            if (!BooterLogger.Exists(row, "NextWeights", Name)) return;

            List<string> nextStages = row.GetStringList("NextStages", ',');
            List<string> nextWeights = row.GetStringList("NextWeights", ',');

            if (nextStages.Count != nextWeights.Count)
            {
                BooterLogger.AddError(Name + " NextStages/NextWeights Size Mismatch");

                mNextStages = new Pair<string, int>[] { new Pair<string, int> ("", 0) };
                mNextWeights = new int[] { 1 };

                return;
            }

            mNextStages = new Pair<string, int>[nextStages.Count];
            mNextWeights = new int[nextWeights.Count];

            for (int i = 0; i < nextStages.Count; i++)
            {
                mNextStages[i].First = nextStages[i];

                if (!int.TryParse(nextWeights[i], out mNextWeights[i]))
                {
                    BooterLogger.AddError(Name + " NextWeights not numeric: " + nextWeights[i]);
                    return;
                }
            }

            foreach (int weight in mNextWeights)
            {
                mTotalWeight += weight;
            }
        }

        public override bool ValidateStages(Dictionary<string,int> stages)
        {
            for (int i = 0; i < mNextStages.Length; i++)
            {
                int stage;
                if (!stages.TryGetValue(mNextStages[i].First, out stage))
                {
                    BooterLogger.AddError(" NextStages missing: " + mNextStages[i]);
                    return false;
                }

                mNextStages[i].Second = stage;
            }

            return true;
        }

        public override int GetNextStage(SimDescription sim, DiseaseVector vector)
        {
            int result = RandomUtil.GetInt(mTotalWeight);

            int i = 0, current = 0;
            for (i = 0; i < mNextWeights.Length; i++)
            {
                current += mNextWeights[i];
                if (current >= result) break;
            }

            return mNextStages[i].Second;
        }

        public override string ToString()
        {
            string result = base.ToString();

            for (int i = 0; i < mNextStages.Length; i++)
            {
                result += Common.NewLine + " Weights: " + mNextStages[i] + " (" + mNextWeights[i] + ")";
            }

            return result;
        }
    }
}
