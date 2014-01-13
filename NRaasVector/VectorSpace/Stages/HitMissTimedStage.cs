using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Stages
{
    public abstract class HitMissTimedStage : TimedStage
    {
        Pair<string, int> mSuccessStage = new Pair<string, int>();
        Pair<string, int> mFailureStage = new Pair<string, int>();

        public HitMissTimedStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "SuccessStage", Name))
            {
                mSuccessStage.First = row.GetString("SuccessStage");
            }

            if (BooterLogger.Exists(row, "FailureStage", Name))
            {
                mFailureStage.First = row.GetString("FailureStage");
            }
        }

        public override bool ValidateStages(Dictionary<string,int> stages)
        {
            int stage;
            if (!stages.TryGetValue(mSuccessStage.First, out stage))
            {
                BooterLogger.AddError(Name + " SuccessStage Missing: " + mSuccessStage.First);
                return false;
            }

            mSuccessStage.Second = stage;

            if (!stages.TryGetValue(mFailureStage.First, out stage))
            {
                BooterLogger.AddError(Name + " FailureStage Missing: " + mFailureStage.First);
                return false;
            }

            mFailureStage.Second = stage;

            return true;
        }

        protected abstract bool IsSuccess(SimDescription sim, DiseaseVector vector);

        public override int GetNextStage(SimDescription sim, DiseaseVector vector)
        {
            if (IsSuccess(sim, vector))
            {
                return mSuccessStage.Second;
            }
            else
            {
                return mFailureStage.Second;
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Success: " + mSuccessStage;
            result += Common.NewLine + " Failure: " + mFailureStage;

            return result;
        }
    }
}
