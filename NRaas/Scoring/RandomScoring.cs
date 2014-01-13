using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public abstract class RandomScoring<T,SP> : Scoring<T,SP> 
        where T: class
        where SP : ListedScoringParameters<T>
    {
        HitMissResult<T,SP> mResult;

        bool mInvert = false;

        public RandomScoring()
        { }
        public RandomScoring(int score)
        {
            mResult = new HitMissResult<T, SP>(score);
        }
        public RandomScoring(int min, int max)
        {
            mResult = new HitMissResult<T, SP>(min, max);
        }

        public override void Validate()
        {
            if (mResult != null)
            {
                mResult.Validate();
            }

            base.Validate();
        }

        public override bool Cachable
        {
            get
            {
                if (!mResult.Cachable) return false;

                return base.Cachable;
            }
        }

        public override string ToString()
        {
            return base.ToString() + "," + mResult + "," + mInvert;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mResult = new HitMissResult<T, SP>(row, "", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            mInvert = row.GetBool("Invert");

            return true;
        }

        public override int Score(SP parameters)
        {
            if (mInvert)
            {
                return -mResult.Score(parameters);
            }
            else
            {
                return mResult.Score(parameters);
            }
        }
    }
}

