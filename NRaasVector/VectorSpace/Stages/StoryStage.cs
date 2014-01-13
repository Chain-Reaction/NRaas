using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Stages
{
    public class StoryStage : VectorBooter.Stage
    {
        Pair<string, int> mNextStage = new Pair<string, int>();

        public StoryStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "NextStage", Name))
            {
                mNextStage.First = row.GetString("NextStage");
            }
        }

        public override bool ValidateStages(Dictionary<string, int> stages)
        {
            int stage;
            if (!stages.TryGetValue(mNextStage.First, out stage))
            {
                BooterLogger.AddError(Name + " NextStage Missing: " + mNextStage.First);
                return false;
            }

            mNextStage.Second = stage;
            return true;
        }

        public override int GetNextStage(SimDescription sim, DiseaseVector vector)
        {
            return mNextStage.Second;
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " NextStage: " + mNextStage;

            return result;
        }
    }
}
