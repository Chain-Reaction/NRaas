using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public interface ISimScoringCache
    {
        void UnloadCaches(SimDescription sim);
    }

    public abstract class CombinedSimScoring<T,U,SP> : CombinedScoring<SimDescription,T,SP>, ISimScoringCache, ICombinedScoring<SimDescription,SP>
        where T : class, IScoring<SimDescription,SP>, ICombinableScoring
        where SP : SimScoringParameters
    {
        Dictionary<ulong, int> mScores = new Dictionary<ulong, int>();

        static List<Dictionary<ulong, int>> sCache = new List<Dictionary<ulong, int>>();

        static Dictionary<ulong, U> sOldValues = new Dictionary<ulong, U>();

        public CombinedSimScoring()
        {
            sCache.Add(mScores);
        }

        protected abstract U GetValue(SimDescription sim);

        public void UnloadCaches(SimDescription sim)
        {
            try
            {
                U newValue = GetValue(sim);

                U oldValue = default(U);
                if (sOldValues.TryGetValue(sim.SimDescriptionId, out oldValue))
                {
                    if (!newValue.Equals(oldValue))
                    {
                        foreach (Dictionary<ulong, int> score in sCache)
                        {
                            score.Remove(sim.SimDescriptionId);
                        }
                    }
                }

                sOldValues[sim.SimDescriptionId] = newValue;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }

        public override bool UnloadCaches(bool final)
        {
            if (final)
            {
                mScores.Clear();
            }

            return base.UnloadCaches(final);
        }

        protected abstract ScoringDelegate GetScoringDelegate(SimDescription sim);

        protected override int GetCachedScore(CombinedScoring<SimDescription, T,SP>.ScoringDelegate func, SP parameters, ICollection<IScoring<SimDescription,SP>> scoringList)
        {
            SimDescription actor = parameters.Actor;
            if (actor == null) return 0;

            int totalScore = 0;
            if (mScores.TryGetValue(actor.SimDescriptionId, out totalScore))
            {
                return totalScore;
            }

            // Cut down the stack by one by not calling the base class
            //totalScore = base.GetCachedScore(func, parameters, scoringList);
            totalScore = PrivateScore(func, parameters, scoringList, " Cached");

            mScores.Add(actor.SimDescriptionId, totalScore);

            return totalScore;
        }

        public override int Score(SP parameters)
        {
            SimDescription actor = parameters.Actor;
            if (actor == null) return 0;

            return CalculateScore(GetScoringDelegate(actor), parameters);
        }
    }
}

