using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public interface ICombinableScoring
    {
        bool IsCombinable
        {
            get;
        }
    }

    public interface ICombinedScoring<T,SP>
        where T : class
        where SP : ListedScoringParameters<T>
    {
        bool Combine(List<IScoring<T,SP>> scoring);
    }

    public abstract class CombinedScoring<T,U,SP> : Scoring<T,SP>
        where T : class
        where U : class, IScoring<T,SP>, ICombinableScoring
        where SP : ListedScoringParameters<T>
    {
        protected ScoreCachingHelper<T,SP> mHelper = new ScoreCachingHelper<T,SP>();

        public CombinedScoring()
        { }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            error = "Combined invalid";
            return false;
        }

        public override bool Cachable
        {
            get
            {
                return mHelper.Cachable;
            }
        }

        public override void Validate()
        {
            mHelper.Validate();
        }

        public virtual bool Combine(List<IScoring<T,SP>> origScoring)
        {
            List<IScoring<T,SP>> rawScoring = new List<IScoring<T,SP>>();

            foreach (IScoring<T,SP> score in origScoring)
            {
                U conScore = score as U;
                if (conScore == null) continue;

                if (!conScore.IsCombinable) continue;

                rawScoring.Add(conScore);
            }

            if (rawScoring.Count <= 1)
            {
                return false;
            }

            foreach (U score in rawScoring)
            {
                score.SetToConsumed();
            }

            mHelper.SetRawScoring(rawScoring);

            return true;
        }

        public delegate int ScoringDelegate(U scoring, SP parameters);

        protected virtual int GetCachedScore(ScoringDelegate func, SP parameters, ICollection<IScoring<T,SP>> scoringList)
        {
            return PrivateScore(func, parameters, scoringList, " Cached");
        }

        protected int PrivateScore(ScoringDelegate func, SP parameters, ICollection<IScoring<T,SP>> scoringList, string suffix)
        {
            int totalScore = 0;

            foreach (U scoring in scoringList)
            {
                int score = func(scoring, parameters);

                ScoringLookup.AddStat("Combined " + scoring.ToString() + suffix, score);

                if (score <= -1000)
                {
                    return score;
                }

                totalScore += score;
            }

            return totalScore;
        }

        protected int CalculateScore(ScoringDelegate func, SP parameters)
        {
            mHelper.Prime();

            int totalScore = 0;

            int score = 0;
            
            score = GetCachedScore(func, parameters, mHelper.CachableScoring);
            if (score <= -1000)
            {
                return score;
            }

            totalScore += score;

            score = PrivateScore(func, parameters, mHelper.NonCachableScoring, " NotCached");
            if (score <= -1000)
            {
                return score;
            }

            totalScore += score;

            return totalScore;
        }

        public override bool UnloadCaches(bool final)
        {
            foreach (U score in mHelper.RawScoring)
            {
                score.UnloadCaches(final);
            }

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + Common.NewLine + mHelper;
        }
    }
}

