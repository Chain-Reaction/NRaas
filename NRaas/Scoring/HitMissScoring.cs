using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    // Cannot be added as part of generic class or ParserFunctions.TryParseEnum will crash the engine
    public enum HitMissExpansionFail
    {
        None,
        Hit,
        Miss
    }

    public class HitMissResult<T, SP>
        where T : class
        where SP : ListedScoringParameters<T>
    {
        int mMinimum;
        int mMaximum;

        string mScoringName;
        IScoringMethod<T,SP> mScoring;

        public HitMissResult(int score)
        {
            mMinimum = score;
            mMaximum = score;
        }
        public HitMissResult(int minimum, int maximum)
        {
            if (minimum > maximum)
            {
                mMinimum = maximum;
                mMaximum = minimum;
            }
            else
            {
                mMinimum = minimum;
                mMaximum = maximum;
            }
        }
        public HitMissResult(XmlDbRow row, string prefix, ref string error)
        {
            if (!row.Exists(prefix + "Scoring"))
            {
                if (!row.Exists(prefix))
                {
                    if (!row.Exists(prefix + "Minimum"))
                    {
                        error = prefix + "Minimum missing";
                        return;
                    }
                    else if (!row.Exists(prefix + "Maximum"))
                    {
                        error = prefix + "Maximum missing";
                        return;
                    }
                }

                mMinimum = row.GetInt(prefix + "Minimum", row.GetInt(prefix, 0));
                mMaximum = row.GetInt(prefix + "Maximum", row.GetInt(prefix, 0));

                if (mMinimum > mMaximum)
                {
                    int temp = mMaximum;
                    mMaximum = mMinimum;
                    mMinimum = temp;
                }
            }
            else
            {
                mScoringName = row.GetString(prefix + "Scoring");
                mScoring = null;
            }
        }

        public void Validate()
        {
            // We don't care about the result, only the Scoring function itself
            IScoringMethod<T, SP> scoring = Scoring;
        }

        protected IScoringMethod<T, SP> Scoring
        {
            get
            {
                if (mScoring == null)
                {
                    if (string.IsNullOrEmpty(mScoringName)) return null;

                    IListedScoringMethod scoring = ScoringLookup.GetScoring(mScoringName);
                    if (scoring != null)
                    {
                        mScoring = scoring as IScoringMethod<T, SP>;
                        if (mScoring == null)
                        {
                            Common.StackLog(new Common.StringBuilder("Scoring Mismatch: " + mScoringName + ", Type: " + typeof(IScoringMethod<T, SP>)));
                        }
                    }
                    mScoringName = null;
                }

                return mScoring;
            }
        }

        public bool Cachable
        {
            get
            {
                IScoringMethod<T, SP> scoring = Scoring;
                if (scoring != null)
                {
                    return scoring.Cachable;
                }

                return true;
            }
        }

        public bool IsZero
        {
            get
            {
                if (Scoring != null) return false;

                if (mMinimum != 0) return false;
                if (mMaximum != 0) return false;

                return true;
            }
        }

        public bool IsAbsolute
        {
            get
            {
                if (Scoring != null)
                {
                    return false;
                }
                else
                {
                    return (mMinimum == mMaximum);
                }
            }
        }

        public int Score(SP parameters)
        {
            IScoringMethod<T, SP> scoring = Scoring;
            if (scoring != null) 
            {
                return scoring.Score(parameters);
            }
            else if (mMinimum == mMaximum)
            {
                return mMinimum;
            }
            else if (!parameters.IsAbsolute)
            {
                return RandomUtil.GetInt(mMinimum, mMaximum);
            }
            else
            {
                return mMaximum;
            }
        }

        public override string ToString()
        {
            string text = null;

            if (Scoring != null)
            {
                text += Scoring.Name;
            }
            else if (mMinimum == mMaximum)
            {
                text += mMinimum;
            }
            else
            {
                text += mMinimum + "," + mMaximum;
            }
            
            return text;
        }
    }

    public abstract class HitMissScoring<T,SP,HitMissSP> : Scoring<T,SP>, ICombinableScoring 
        where T : class
        where HitMissSP : ListedScoringParameters<T>
        where SP : HitMissSP
    {
        protected HitMissExpansionFail mOnExpansionFail = HitMissExpansionFail.None;

        protected HitMissResult<T, HitMissSP> mHit;
        protected HitMissResult<T, HitMissSP> mMiss;

        Dictionary<T,int> mCollected = null;

        public HitMissScoring()
        { }
        public HitMissScoring(int hit, int miss)
        {
            mHit = new HitMissResult<T, HitMissSP>(hit);
            mMiss = new HitMissResult<T, HitMissSP>(miss);
        }

        public override void Validate()
        {
            mHit.Validate();
            mMiss.Validate();

            base.Validate();
        }

        public virtual bool IsCombinable
        {
            get
            {
                return IsAbsolute;
            }
        }

        public override bool Cachable
        {
            get
            {
                if (!mHit.Cachable) return false;

                if (!mMiss.Cachable) return false;

                return base.Cachable;
            }
        }

        protected bool HasCollection
        {
            get
            {
                return (mCollected != null);
            }
        }

        protected bool GetCollectedScore(T obj, out int value)
        {
            if (mCollected.TryGetValue(obj, out value))
            {
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool IsAbsolute
        {
            get
            {
                return ((mHit.IsAbsolute) || (mMiss.IsAbsolute));
            }
        }

        public override string ToString()
        {
            return base.ToString() + ",Hit " + mHit + ",Miss " + mMiss;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mHit = new HitMissResult<T, HitMissSP>(row, "Hit", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            mMiss = new HitMissResult<T, HitMissSP>(row, "Miss", ref error);
            if (!string.IsNullOrEmpty(error)) return false;

            if (row.Exists("OnExpansionFail"))
            {
                if (!ParserFunctions.TryParseEnum<HitMissExpansionFail>(row.GetString("OnExpansionFail"), out mOnExpansionFail, HitMissExpansionFail.None))
                {
                    error = "OnExpansionFail unknown";
                    return false;
                }
            }

            return true;
        }

        protected virtual Dictionary<T,int> HitCollect(T obj)
        {
            return null;
        }

        public override void Collect(T obj)
        {
            mCollected = HitCollect(obj);
     
            base.Collect(obj);
        }

        protected virtual bool HasRequiredVersion()
        {
            return true;
        }

        public abstract bool IsHit(SP parameters);

        public override int Score(SP parameters)
        {
            bool hit = false;
            if (!HasRequiredVersion())
            {
                hit = (mOnExpansionFail == HitMissExpansionFail.Hit);
            }
            else if (mCollected != null)
            {
                hit = mCollected.ContainsKey(parameters.Actor);
            }
            else
            {
                hit = IsHit(parameters);
            }

            return Score(hit, parameters);
        }

        public int Score(bool hit, SP parameters)
        {
            if (hit)
            {
                return mHit.Score(parameters);
            }
            else
            {
                return mMiss.Score(parameters);
            }
        }
    }
}

