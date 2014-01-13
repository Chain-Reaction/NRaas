using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public abstract class GenericSimListedScoringMethod<SP, SubSP> : ListedScoringMethod<SimDescription, SP, SubSP>
        where SP : SubSP
        where SubSP : SimScoringParameters
    {
        Dictionary<ulong, int> mFactorScores = null;
        Dictionary<ulong, int> mAbsoluteScores = null;
        Dictionary<ulong, float> mRandomValues = null;

        int mCycleRetention = 3;

        int mCycleCount = 0;

        public GenericSimListedScoringMethod()
        {
            mFactorScores = new Dictionary<ulong, int>();
            mAbsoluteScores = new Dictionary<ulong, int>();
            mRandomValues = new Dictionary<ulong, float>();
        }
        protected GenericSimListedScoringMethod(bool hasCache)
        {
            if (hasCache)
            {
                mFactorScores = new Dictionary<ulong, int>();
                mAbsoluteScores = new Dictionary<ulong, int>();
                mRandomValues = new Dictionary<ulong, float>();
            }
        }

        public override bool UnloadCaches(bool final)
        {
            mCycleCount++;

            if ((final) || (mCycleCount >= mCycleRetention))
            {
                if (mFactorScores != null)
                {
                    mFactorScores.Clear();
                }
                if (mAbsoluteScores != null)
                {
                    mAbsoluteScores.Clear();
                }
                if (mRandomValues != null)
                {
                    mRandomValues.Clear();
                }

                mCycleCount = 0;
                base.UnloadCaches(final);

                return true;
            }

            return false;
        }

        protected override float GetRandomValue(SP parameters)
        {
            float value;
            if (!mRandomValues.TryGetValue(parameters.Actor.SimDescriptionId, out value))
            {
                value = RandomUtil.GetFloat(0, 100f);
                mRandomValues[parameters.Actor.SimDescriptionId] = value;
            }

            return value;
        }

        protected override int GetCachedScore(SP parameters, ICollection<IScoring<SimDescription, SubSP>> scoring)
        {
            Dictionary<ulong, int> scores = null;
            if (parameters.IsAbsolute)
            {
                scores = mAbsoluteScores;
            }
            else
            {
                scores = mFactorScores;
            }

            int score = 0;
            // Intentionally allow null Actors to bounce the scoring, so we can track the error
            if ((scores != null) /*&& (parameters.Actor != null)*/)
            {
                if (scores.TryGetValue(parameters.Actor.SimDescriptionId, out score))
                {
                    return score;
                }
            }

            // Cut down the stack by one by not calling the base class
            //score = base.GetCachedScore(parameters, scoring);
            score = PrivateScore(parameters, scoring, " Cached");

            if (scores != null)
            {
                scores[parameters.Actor.SimDescriptionId] = score;
            }

            return score;
        }

        public override bool Parse(XmlDbRow row, XmlDbTable table)
        {
            mCycleRetention = row.GetInt("CycleRetention", mCycleRetention);

            return base.Parse(row, table);
        }
    }
}

