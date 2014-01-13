using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public class SimScoringList : ScoringList<SimDescription>
    {
        IListedScoringMethod mMethod = null;

        public SimScoringList(Common.IStatGenerator stats, string scoring, ICollection<SimDescription> actors, bool allowSpecial)
            : this(stats, scoring, actors, allowSpecial, null)
        { }
        public SimScoringList(Common.IStatGenerator stats, string scoring, ICollection<SimDescription> actors, bool allowSpecial, SimDescription other)
            : this(scoring)
        {
            Score(stats, scoring, actors, allowSpecial, other);
        }
        public SimScoringList(string scoring)
        {
            mMethod = ScoringLookup.GetScoring(scoring);
        }

        protected void Score(Common.IStatGenerator stats, string scoring, ICollection<SimDescription> actors, bool allowSpecial, SimDescription other)
        {
            bool dual = (mMethod is DualSimListedScoringMethod);

            foreach (SimDescription actor in actors)
            {
                if (!allowSpecial)
                {
                    if (SimTypes.IsSpecial(actor)) continue;
                }

                if (dual)
                {
                    Add(stats, scoring, actor);
                }
                else
                {
                    Add(stats, scoring, actor, other);
                }
            }
        }

        public int Add(Common.IStatGenerator stats, string scoring, SimDescription scoreAgainst)
        {
            return Add(stats, scoring, new SimScoringParameters(scoreAgainst));
        }
        public int Add(Common.IStatGenerator stats, string scoring, SimDescription scoreAgainst, SimDescription other)
        {
            return Add(stats, scoring, new DualSimScoringParameters(scoreAgainst, other));
        }

        protected int Add(Common.IStatGenerator stats, string scoring, SimScoringParameters parameters)
        {
            if (mMethod == null) return 0;

            int score = stats.AddScoring(scoring, mMethod.IScore(parameters));

            mInternalList.Add(new ScorePair(parameters.Actor, score));
            return score;
        }
    }
}