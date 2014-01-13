using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class StatBin : Common.IStatGenerator, IDisposable
    {
        string mName;

        Dictionary<string, StatEntry> mStatEntries = new Dictionary<string, StatEntry>();

        List<Pair<string, float>> mStore = new List<Pair<string,float>>();

        public StatBin(string name)
        {
            mName = name;
        }

        public void Dispose()
        {
            Dump(true, "", int.MaxValue, false);
        }

        public int Count
        {
            get { return mStatEntries.Count; }
        }

        public Common.DebugLevel DebuggingLevel
        {
            get
            {
                if (Common.kDebugging)
                {
                    return Common.DebugLevel.Stats;
                }
                else
                {
                    return Common.DebugLevel.Quiet;
                }
            }
        }

        public virtual void IncStat(string stat)
        {           
            if (!mStatEntries.ContainsKey(stat))
            {
                mStatEntries.Add(stat, new StatEntry(stat));
            }

            mStatEntries[stat].Inc();
        }
        public void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (DebuggingLevel >= minLevel)
            {
                IncStat(stat);
            }
        }

        public int AddStat(string stat, int val)
        {
            AddStat(stat, (float)val);
            return val;
        }
        public int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            if (DebuggingLevel >= minLevel)
            {
                AddStat(stat, val);
            }

            return val;
        }

        private void ConvertStore()
        {
            foreach (Pair<string, float> pair in mStore)
            {
                if (!mStatEntries.ContainsKey(pair.First))
                {
                    mStatEntries.Add(pair.First, new StatEntry(pair.First));
                }
                mStatEntries[pair.First].Add(pair.Second);
            }

            mStore.Clear();
        }

        public virtual float AddStat(string stat, float val)
        {
            mStore.Add(new Pair<string,float>(stat, val));
            return val;
        }
        public float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (DebuggingLevel >= minLevel)
            {
                AddStat(stat, val);
            }

            return val;
        }

        public float AddScoring(string stat, float score)
        {
            if (score > 0)
            {
                AddStat(stat + " Scoring Pos", score);
            }
            else if (score < 0)
            {
                AddStat(stat + " Scoring Neg", score);
            }
            else
            {
                AddStat(stat + " Scoring Zero", score);
            }

            return score;
        }

        public int AddScoring(string stat, int score)
        {
            AddScoring(stat, (float)score);
            return score;
        }
        public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
        {
            if (DebuggingLevel >= minLevel)
            {
                AddScoring(stat, (float)score);
            }
            return score;
        }

        public string GetResults(bool includeZero)
        {
            ConvertStore();

            List<string> results = new List<string>();

            foreach (StatEntry entry in mStatEntries.Values)
            {
                results.Add (entry.GetOverRatio(includeZero));
            }

            results.Sort();

            string result = null;
            foreach (string value in results)
            {
                result += Common.NewLine + value;
            }

            return result;
        }

        public void Log(IStatBinLogger logger)
        {
            ConvertStore();

            List<string> values = new List<string>();
            foreach (StatEntry entry in mStatEntries.Values)
            {
                values.Add(entry.ToString());
            }

            values.Sort();

            foreach (string entry in values)
            {
                logger.Append(entry);
            }
        }

        public void Dump(string suffixTitle, int maximum)
        {
            Dump(false, suffixTitle, maximum, true);
        }
        public void Dump(string suffixTitle, bool empty)
        {
            Dump(false, suffixTitle, int.MaxValue, empty);
        }
        public void Dump(bool notify, string suffixTitle, int maximum, bool empty)
        {
            ConvertStore();

            if (mStatEntries.Count == 0) return;

            List<string> values = new List<string>();
            foreach (StatEntry entry in mStatEntries.Values)
            {
                values.Add(entry.ToString());
            }

            values.Sort();

            StringBuilder message = new StringBuilder();

            string header = "-- " + mName + " " + suffixTitle + " --" + Common.NewLine;

            int count = 0;

            foreach (string entry in values)
            {
                message.Append(Common.NewLine + entry);
                count++;

                if (count > maximum)
                {
                    if (notify)
                    {
                        Common.DebugNotify(header + message);

                        message = null;

                        count = 0;
                    }
                }
            }

            if (message.Length > 0)
            {
                if (notify)
                {
                    Common.DebugNotify(header + message);
                }
                else
                {
                    Common.WriteLog(header + message);
                }
            }

            if (empty)
            {
                mStatEntries.Clear();
            }
        }

        public class StatEntry
        {
            string mName = null;
            Common.StatValueCount mUnder = new Common.StatValueCount();
            Common.StatValueCount mZero = new Common.StatValueCount();
            Common.StatValueCount mOver = new Common.StatValueCount();

            float mMin = float.MaxValue;
            float mMax = float.MinValue;

            public StatEntry(string name)
            {
                mName = name;
            }

            public int Count
            {
                get
                {
                    return (mUnder.Count + mZero.Count + mOver.Count);
                }
            }

            public void Add(float val)
            {
                if (mMin > val)
                {
                    mMin = val;
                }

                if (mMax < val)
                {
                    mMax = val;
                }

                if (val == 0)
                {
                    mZero.Add(1);
                }
                else if (val > 0)
                {
                    mOver.Add(val);
                }
                else
                {
                    mUnder.Add(val);
                }
            }

            public void Inc()
            {
                mZero.Add(1);
            }

            public string GetOverRatio(bool includeZero)
            {
                int total = (mOver.Count + mUnder.Count + mZero.Count);

                if (includeZero)
                {
                    return Name + " : " + ((mOver.Count + mZero.Count) * 100 / total) + "% (" + total + ")";
                }
                else
                {
                    return Name + " : " + (mOver.Count * 100 / total) + "% (" + total + ")";
                }
            }

            public override string ToString()
            {
                string result = mName.Replace(","," ") + ": ";

                if ((mUnder.Count > 0) || (mOver.Count > 0))
                {
                    result += ",Min," + mMin;
                    result += ",Max," + mMax;
                }
                else if (Common.StatValueCount.sFullLog)
                {
                    result += ",Min,";
                    result += ",Max,";
                }

                bool otherValue = Common.StatValueCount.sFullLog;

                string value = mUnder.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    result += ",Under," + value;

                    otherValue = true;
                }

                value = mZero.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    if ((Common.StatValueCount.sFullLog) || ((mUnder.Count + mOver.Count) > 0))
                    {
                        result += ",Zero," + value;
                    }
                    else
                    {
                        result += "," + value;
                    }

                    otherValue = true;
                }

                value = mOver.ToString(mMax);
                if (!string.IsNullOrEmpty(value))
                {
                    if (otherValue)
                    {
                        result += ",Over";
                    }

                    result += "," + value;
                }

                if ((mUnder.Count + mOver.Count) > 0)
                {
                    int total = (mOver.Count + mUnder.Count + mZero.Count);

                    otherValue = false;

                    if (mUnder.Count > 0)
                    {
                        result += ",Under," + (mUnder.Count * 100 / total) + "%";

                        otherValue = true;
                    }

                    if (mZero.Count > 0)
                    {
                        result += ",Zero," + (mZero.Count * 100 / total) + "%";

                        otherValue = true;
                    }

                    if ((otherValue) && (mOver.Count > 0))
                    {
                        result += ",Over," + (mOver.Count * 100 / total) + "%";
                    }
                }

                return result;
            }

            public string Name
            {
                get
                {
                    return mName;
                }
            }
        }

        public interface IStatBinLogger
        {
            void Append(string text);
        }
    }
}
