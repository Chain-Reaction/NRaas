using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public abstract class MethodScoring<T,SP> : HitMissScoring<T,SP,SP>
        where T : class
        where SP : ListedScoringParameters<T>
    {
        string mMethod = null;

        IListedScoringMethod mScoring = null;

        HitMissResult<T, SP> mGate;

        public MethodScoring()
        { }

        public override void Validate()
        {
            if (mGate != null)
            {
                mGate.Validate();
            }

            base.Validate();
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Method"))
            {
                error = "Method missing";
                return false;
            }
            else if (!row.Exists("Gate"))
            {
                error = "Gate missing";
                return false;
            }

            mMethod = row.GetString("Method");

            if (ScoringLookup.GetScoring(mMethod) == null)
            {
                error = mMethod + " not found";
                return false;
            }

            mGate = new HitMissResult<T, SP>(row, "Gate", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            return base.Parse(row, ref error);
        }

        public override bool IsHit(SP parameters)
        {
            if (mScoring == null)
            {
                if (string.IsNullOrEmpty(mMethod)) return false;

                mScoring = ScoringLookup.GetScoring(mMethod);
                mMethod = null; // Stop the system from checking again
                if (mScoring == null) return false;
            }

            return (mScoring.IScore(parameters) >= mGate.Score(parameters));
        }
    }
}

