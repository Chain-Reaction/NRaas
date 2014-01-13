using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public class ScoringList<T> where T: class
    {
        public class ScorePair
        {
            public T mObject = default(T);
            public int mScore;

            public ScorePair(T Obj, int Score)
            {
                mObject = Obj;
                mScore = Score;
            }
        }

        protected List<ScorePair> mInternalList;

        public ScoringList()
        {
            mInternalList = new List<ScorePair>();
        }

        public void Add(T Obj, int Score)
        {
            mInternalList.Add(new ScorePair(Obj, Score));
        }

        public int Count
        {
            get
            {
                return mInternalList.Count;
            }
        }

        public void Clear()
        {
            mInternalList.Clear();
        }

        public void AddScoring(string stat, Common.IStatGenerator manager)
        {
            foreach (ScorePair score in mInternalList)
            {
                manager.AddScoring(stat, score.mScore);
            }
        }

        public T GetBest()
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count == 0)
            {
                return default(T);
            }
            return mInternalList[0].mObject;
        }
        public T GetBest(int minscore)
        {
            mInternalList.Sort(SortListByScore);
            if ((mInternalList.Count == 0) || (mInternalList[0].mScore < minscore))
            {
                return default(T);
            }
            else
            {
                return mInternalList[0].mObject;
            }
        }

        public List<T> GetWorstByMaxScore(int maxscore)
        {
            mInternalList.Sort(SortListByScore);

            List<T> list = new List<T>();
            for (int i = mInternalList.Count - 1; i>=0 ; i--)
            {
                if (mInternalList[i].mScore > maxscore) break;

                list.Add(mInternalList[i].mObject);
            }
            return list;
        }

        public List<T> GetBestByMinScore(int minscore)
        {
            mInternalList.Sort(SortListByScore);

            List<T> list = new List<T>();
            for (int i = 0; i < mInternalList.Count; i++)
            {
                if (mInternalList[i].mScore < minscore) break;

                list.Add(mInternalList[i].mObject);
            }
            return list;
        }

        public List<T> GetBestByPercent(float percent)
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count == 0)
            {
                return new List<T>();
            }
            int num = (int)(mInternalList.Count * (percent / 100f));
            if (num == 0)
            {
                num = 1;
            }
            List<T> list = new List<T>();
            for (int i = 0; i < num; i++)
            {
                list.Add(mInternalList[i].mObject);
            }
            return list;
        }

        public T GetBestCandidateFromTop(float percent)
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count != 0)
            {
                int num = (int)(mInternalList.Count * (percent / 100f));
                if (num == 0)
                {
                    num = 1;
                }
                List<T> randomList = new List<T>();
                for (int i = 0; i < num; i++)
                {
                    randomList.Add(mInternalList[i].mObject);
                }
                if (randomList.Count > 0)
                {
                    return RandomUtil.GetRandomObjectFromList<T>(randomList);
                }
            }
            return default(T);
        }

        public int GetBestScore()
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count == 0)
            {
                return 0;
            }
            return mInternalList[0].mScore;
        }

        public T GetWorst()
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count == 0)
            {
                return default(T);
            }
            return mInternalList[mInternalList.Count - 1].mObject;
        }

        public List<T> GetWorst(float Percent)
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count == 0)
            {
                return new List<T>();
            }
            int num = (int) (mInternalList.Count * (Percent / 100f));
            if (num == 0)
            {
                num = 1;
            }
            List<T> list = new List<T>();
            int num2 = 0;
            for (int i = mInternalList.Count - 1; num2 < num; i++)
            {
                list.Add(mInternalList[i].mObject);
                num2++;
            }
            return list;
        }

        public T GetWorstCandidateFromBottom(float Percent)
        {
            mInternalList.Sort(SortListByScore);
            if (mInternalList.Count != 0)
            {
                int num = (int) (mInternalList.Count * (Percent / 100f));
                if (num == 0)
                {
                    num = 1;
                }
                List<T> randomList = new List<T>();
                int num2 = 0;
                for (int i = mInternalList.Count - 1; num2 < num; i++)
                {
                    randomList.Add(mInternalList[i].mObject);
                    num2++;
                }
                if (randomList.Count > 0)
                {
                    return RandomUtil.GetRandomObjectFromList<T>(randomList);
                }
            }
            return default(T);
        }

        private static int SortListByScore(ScorePair a, ScorePair b)
        {
            if (a.mScore > b.mScore)
            {
                return -1;
            }
            if (a.mScore < b.mScore)
            {
                return 1;
            }
            return 0;
        }
    }
}

