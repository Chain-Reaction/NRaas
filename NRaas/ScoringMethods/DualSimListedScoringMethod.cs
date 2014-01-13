using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public class DualSimScoringParameters : SimScoringParameters
    {
        readonly SimDescription mOther;

        public DualSimScoringParameters(SimDescription scoreAgainst, SimDescription other)
            : this(scoreAgainst, other, false)
        { }
        public DualSimScoringParameters(SimDescription scoreAgainst, SimDescription other, bool absolute)
            : base(scoreAgainst, absolute)
        {
            mOther = other;
        }

        public SimDescription Other
        {
            get { return mOther; }
        }

        public override string ToString()
        {
            string text = base.ToString();

            if (mOther != null)
            {
                text += " : " + mOther.FullName;
            }

            return text;
        }
    }

    public class DualSimListedScoringMethod : GenericSimListedScoringMethod<DualSimScoringParameters, DualSimScoringParameters>
    {
        Dictionary<ulong, Dictionary<ulong, int>> mFactorScores = new Dictionary<ulong, Dictionary<ulong, int>>();
        Dictionary<ulong, Dictionary<ulong, int>> mAbsoluteScores = new Dictionary<ulong, Dictionary<ulong, int>>();
        Dictionary<ulong, Dictionary<ulong, float>> mRandomValues = new Dictionary<ulong, Dictionary<ulong, float>>();

        public DualSimListedScoringMethod()
            : base (false)
        { }

        public override bool UnloadCaches(bool final)
        {
            if (!base.UnloadCaches(final)) return false;

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

            return true;
        }

        protected override float GetRandomValue(DualSimScoringParameters parameters)
        {
            Dictionary<ulong, float> targets = null;
            if (!mRandomValues.TryGetValue(parameters.Actor.SimDescriptionId, out targets))
            {
                targets = new Dictionary<ulong, float>();
                mRandomValues[parameters.Actor.SimDescriptionId] = targets;
            }

            float value;
            if (!targets.TryGetValue(parameters.Other.SimDescriptionId, out value))
            {
                value = RandomUtil.GetFloat(0, 100f);
                targets[parameters.Other.SimDescriptionId] = value;
            }

            return value;
        }

        protected override int GetCachedScore(DualSimScoringParameters parameters, ICollection<Scoring.IScoring<SimDescription, DualSimScoringParameters>> scoring)
        {
            Dictionary<ulong, Dictionary<ulong, int>> scores = null;
            if (Cachable)
            {
                if (parameters.IsAbsolute)
                {
                    scores = mAbsoluteScores;
                }
                else
                {
                    scores = mFactorScores;
                }
            }
            else
            {
                ScoringLookup.IncStat(ToString() + " Not Cachable");
            }

            Dictionary<ulong, int> targets = null;

            // Intentionally allow null Actors to bounce the scoring, so we can track the error
            if ((scores != null) /*&& (parameters.Actor != null)*/)
            {
                if (!scores.TryGetValue(parameters.Actor.SimDescriptionId, out targets))
                {
                    targets = new Dictionary<ulong, int>();
                    scores.Add(parameters.Actor.SimDescriptionId, targets);
                }
            }

            ulong other = 0;
            if (parameters.Other != null)
            {
                other = parameters.Other.SimDescriptionId;
            }

            int score;
            if (targets != null)
            {
                if (targets.TryGetValue(other, out score))
                {
                    return score;
                }
            }

            score = base.GetCachedScore(parameters, scoring);

            if (targets != null)
            {
                targets[other] = score;
            }

            return score;
        }
    }
}

