using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public interface IScoringParameters
    { }

    public interface IListedScoringMethod
    {
        bool Parse(XmlDbRow row, XmlDbTable table);

        string Name
        {
            get;
        }

        int IScore(IScoringParameters parameters);

        bool Contains<SCORING>() where SCORING : IScoring;

        bool UnloadCaches(bool final);

        bool Cachable
        {
            get;
        }

        void Validate();
    }

    public interface IScoringMethod<T,SP> : IListedScoringMethod
        where T : class
        where SP : ListedScoringParameters<T>
    {
        int Score(SP parameters);
    }

    public class ScoreCachingHelper<T,SP>
        where T : class
        where SP : ListedScoringParameters<T>
    {
        bool mCanCache = true;
        bool mCacheTested = false;

        List<IScoring<T,SP>> mRawScoring = null;

        List<IScoring<T,SP>> mNonCachableScoring = null;
        List<IScoring<T,SP>> mCachableScoring = null;

        public ScoreCachingHelper()
        {
            mRawScoring = new List<IScoring<T,SP>>();
        }

        public ICollection<IScoring<T, SP>> RawScoring
        {
            get { return mRawScoring; }
        }

        public ICollection<IScoring<T, SP>> NonCachableScoring
        {
            get { return mNonCachableScoring; }
        }

        public ICollection<IScoring<T,SP>> CachableScoring
        {
            get { return mCachableScoring; }
        }

        public void Validate()
        {
            foreach (IScoring<T, SP> scoring in mRawScoring)
            {
                scoring.Validate();
            }
        }

        public bool Cachable
        {
            get
            {
                if (!mCacheTested)
                {
                    mCacheTested = true;

                    mCanCache = true;

                    foreach (IScoring<T, SP> scoring in mRawScoring)
                    {
                        if (!scoring.Cachable)
                        {
                            mCanCache = false;
                            break;
                        }
                    }
                }

                return mCanCache;
            }
        }

        public void SetRawScoring(List<IScoring<T, SP>> scoring)
        {
            mRawScoring = scoring;

            mCachableScoring = null;
            mNonCachableScoring = null;
        }

        public void Prime()
        {
            if (mCachableScoring == null)
            {
                mNonCachableScoring = new List<IScoring<T, SP>>();
                mCachableScoring = new List<IScoring<T, SP>>();

                foreach (IScoring<T, SP> scoring in mRawScoring)
                {
                    if (scoring.Cachable)
                    {
                        mCachableScoring.Add(scoring);
                    }
                    else
                    {
                        mNonCachableScoring.Add(scoring);
                    }
                }
            }
        }

        public override string ToString()
        {
            string result = null;

            if (mRawScoring != null)
            {
                foreach (IScoring<T, SP> score in mRawScoring)
                {
                    result += Common.NewLine + score.ToString();
                }
            }
            else
            {
                result = "<No Scoring>";
            }

            return result;
        }
    }

    public abstract class ListedScoringParameters<T> : IScoringParameters
        where T : class
    {
        T mActor;

        protected bool mAbsolute = false;

        public ListedScoringParameters(T obj)
        {
            mActor = obj;
        }
        public ListedScoringParameters(T obj, bool absolute)
        {
            mActor = obj;
            mAbsolute = absolute;
        }

        public T Actor
        {
            get { return mActor; }
        }

        public bool IsAbsolute
        {
            get { return mAbsolute; }
        }

        public override string ToString()
        {
            if (IsAbsolute)
            {
                return "Absolute";
            }
            else
            {
                return "";
            }
        }

        public int Score(string name)
        {
            IListedScoringMethod scoring = ScoringLookup.GetScoring(name);
            if (scoring == null) return 0;

            return scoring.IScore(this);
        }
    }

    public abstract class ListedScoringMethod<T,SP,SubSP> : Scoring<T,SP>, IScoringMethod<T,SP>
        where T : class
        where SP : SubSP
        where SubSP : ListedScoringParameters<T>
    {
        ScoreCachingHelper<T, SubSP> mHelper = new ScoreCachingHelper<T, SubSP>();

        string mName = null;

        int mDivisor = 1;
        int mLowerBound = int.MinValue;
        int mUpperBound = int.MaxValue;

        bool mPercentChance = false;

        public ListedScoringMethod()
        { }

        public override void Validate()
        {
            foreach (IScoring<T, SubSP> scoring in mHelper.RawScoring)
            {
                scoring.Validate();
            }
        }

        public override void Collect(T obj)
        {
            foreach (IScoring<T, SubSP> scoring in mHelper.RawScoring)
            {
                scoring.Collect(obj);
            }
        }

        bool IListedScoringMethod.Contains<SCORING>()
        {
            foreach (IScoring<T, SubSP> score in mHelper.RawScoring)
            {
                if (score is SCORING)
                {
                    return true;
                }
            }

            return false;
        }
        public override bool UnloadCaches(bool final)
        {
            foreach (IScoring<T, SubSP> score in mHelper.RawScoring)
            {
                score.UnloadCaches(final);
            }

            return base.UnloadCaches(final);
        }

        public int IScore(IScoringParameters parameters)
        {
            SP param = parameters as SP;
            if (param == null) return 0;

            return ScoringLookup.AddStat(Name + param.ToString(), Score(param));
        }

        protected int PrivateScore(SP parameters, ICollection<IScoring<T, SubSP>> scoringList, string suffix)
        {
            Common.IStatGenerator manager = ScoringLookup.Stats;

            bool lowDebugging = false;

            if (manager != null)
            {
                lowDebugging = (manager.DebuggingLevel > Common.DebugLevel.Stats);
            }

            int totalScore = 0;
            foreach (IScoring<T, SubSP> scoring in scoringList)
            {
                string scoringName = null, fullScoringName = null;

                if (lowDebugging)
                {
                    scoringName = scoring.ToString() + suffix;

                    fullScoringName = "Duration " + Name + " " + scoringName;

                    scoringName = "Duration " + scoringName;
                }

                using (Common.TestSpan scoringTime2 = new Common.TestSpan(manager, scoringName))
                {
                    using (Common.TestSpan scoringTime = new Common.TestSpan(manager, fullScoringName, Common.DebugLevel.High))
                    {
                        int score = scoring.Score(parameters);

                        ScoringLookup.AddStat(scoring.ToString() + suffix, score);

                        if (score <= -1000)
                        {
                            return score;
                        }

                        totalScore += score;
                    }
                }
            }

            return totalScore;
        }

        protected virtual int GetCachedScore(SP parameters, ICollection<IScoring<T, SubSP>> scoring)
        {
            return PrivateScore(parameters, scoring, " Cached");
        }

        protected abstract float GetRandomValue(SP parameters);

        public override int Score(SP parameters)
        {
            //return ScoreTask.Wait(this, parameters);

            mHelper.Prime();

            int totalScore = 0;
            
            int score = GetCachedScore(parameters, mHelper.CachableScoring);
            if (score <= -1000)
            {
                return score;
            }

            totalScore += score;

            score = PrivateScore(parameters, mHelper.NonCachableScoring, " NotCached");
            if (score <= -1000)
            {
                return score;
            }

            totalScore += score;

            if (!parameters.IsAbsolute)
            {
                totalScore /= mDivisor;

                if (totalScore > mUpperBound)
                {
                    totalScore = mUpperBound;
                }
                else if (totalScore < mLowerBound)
                {
                    totalScore = mLowerBound;
                }
            }

            if (parameters.IsAbsolute)
            {
                ScoringLookup.AddStat("Final Absolute " + Name, totalScore);
            }
            else
            {
                ScoringLookup.AddStat("Raw " + Name, totalScore);
            }

            if ((!parameters.IsAbsolute) && (mPercentChance))
            {
                float randomValue = GetRandomValue(parameters);

                if (totalScore <= randomValue)
                {
                    totalScore = 0;
                }

                ScoringLookup.AddStat("Random " + Name, randomValue);
                ScoringLookup.AddStat("Final " + Name, totalScore);
            }

            return totalScore;
        }

        public string Name
        {
            get { return mName; }
        }

        public override bool Cachable
        {
            get
            {
                return mHelper.Cachable;
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mDivisor = row.GetInt("Divisor", 1);

            mLowerBound = row.GetInt("LowerBound", int.MinValue);

            mUpperBound = row.GetInt("UpperBound", int.MaxValue);

            mPercentChance = row.GetBool("PercentChance");

            return true;
        }

        public virtual bool Parse(XmlDbRow myRow, XmlDbTable table)
        {
            mName = myRow.GetString("Name");

            string error = null;
            if (!Parse(myRow, ref error))
            {
                BooterLogger.AddError(Name + " : " + error);
                return false;
            }

            if ((table == null) || (table.Rows == null) || (table.Rows.Count == 0))
            {
                BooterLogger.AddError(Name + ": Missing Table");
                return false;
            }
            else
            {
                List<IScoring<T, SubSP>> rawScoring = new List<IScoring<T, SubSP>>();

                int index = 1;
                foreach (XmlDbRow row in table.Rows)
                {
                    Type classType = row.GetClassType("FullClassName");
                    if (classType == null)
                    {
                        BooterLogger.AddError(Name + ": Unknown FullClassName " + row.GetString("FullClassName"));
                        continue;
                    }

                    IScoring<T, SubSP> scoring = null;
                    try
                    {
                        scoring = classType.GetConstructor(new Type[0]).Invoke(new object[0]) as IScoring<T, SubSP>;
                    }
                    catch
                    { }

                    if (scoring == null)
                    {
                        BooterLogger.AddError(Name + " (" + index + "): Constructor Fail " + row.GetString("FullClassName") + " as " + typeof(IScoring<T, SubSP>));
                    }
                    else
                    {
                        error = null;
                        if (scoring.Parse(row, ref error))
                        {
                            rawScoring.Add(scoring);
                        }
                        else 
                        {
                            if (!string.IsNullOrEmpty(error))
                            {
                                BooterLogger.AddError(Name + " index " + index + " : " + error);
                            }
                            else
                            {
                                BooterLogger.AddTrace(Name + " index " + index + " : <Warning>");
                            }
                        }
                    }

                    index++;
                }

                List<ICombinedScoring<T, SubSP>> combinedList = Common.DerivativeSearch.Find<ICombinedScoring<T, SubSP>>(Common.DerivativeSearch.Caching.NoCache);

                foreach (ICombinedScoring<T, SubSP> combined in combinedList)
                {
                    IScoring<T, SubSP> scoring = combined as IScoring<T, SubSP>;
                    if (scoring == null) continue;

                    if (combined.Combine(rawScoring))
                    {
                        rawScoring.Add(scoring);
                    }
                }

                List<IScoring<T, SubSP>> scoringList = new List<IScoring<T, SubSP>>(rawScoring);
                rawScoring.Clear();

                foreach (IScoring<T, SubSP> scoring in scoringList)
                {
                    if (scoring.IsConsumed) continue;

                    rawScoring.Add(scoring);
                }

                if (rawScoring.Count > 0)
                {
                    mHelper.SetRawScoring(rawScoring);
                    return true;
                }
                else
                {
                    BooterLogger.AddError(Name + ": No valid scoring");
                    return false;
                }
            }
        }

        public override string ToString()
        {
            string result = "Name: " + Name;

            result += Common.NewLine + mHelper.ToString();

            return result;
        }
        /*
        public class ScoreTask : Common.WaitTask
        {
            ListedScoringMethod<T, SP, SubSP> mMethod;

            SP mParameters;

            int mResult;

            protected ScoreTask(ListedScoringMethod<T, SP, SubSP> method, SP parameters)
            {
                mMethod = method;
                mParameters = parameters;
            }

            public static int Wait(ListedScoringMethod<T, SP, SubSP> method, SP parameters)
            {
                return Wait(new ScoreTask(method, parameters)).mResult;
            }

            protected override void OnWaitPerform()
            {
                mMethod.mHelper.Prime();

                int totalScore = 0;

                int score = mMethod.GetCachedScore(mParameters, mMethod.mHelper.CachableScoring);
                if (score <= -1000)
                {
                    mResult = score;
                    return;
                }

                totalScore += score;

                score = mMethod.PrivateScore(mParameters, mMethod.mHelper.NonCachableScoring, " NotCached");
                if (score <= -1000)
                {
                    mResult = score;
                    return;
                }

                totalScore += score;

                if (!mParameters.IsAbsolute)
                {
                    totalScore /= mMethod.mDivisor;

                    if (totalScore > mMethod.mUpperBound)
                    {
                        totalScore = mMethod.mUpperBound;
                    }
                    else if (totalScore < mMethod.mLowerBound)
                    {
                        totalScore = mMethod.mLowerBound;
                    }
                }

                if (mParameters.IsAbsolute)
                {
                    ScoringLookup.AddStat("Final Absolute " + mMethod.Name, totalScore);
                }
                else
                {
                    ScoringLookup.AddStat("Raw " + mMethod.Name, totalScore);
                }

                if ((!mParameters.IsAbsolute) && (mMethod.mPercentChance))
                {
                    if (totalScore <= 0)
                    {
                        totalScore = 0;
                    }
                    else if (!RandomUtil.RandomChance(totalScore))
                    {
                        totalScore = 0;
                    }

                    ScoringLookup.AddStat("Final " + mMethod.Name, totalScore);
                }

                mResult = totalScore;
            }
        }*/
    }
}

