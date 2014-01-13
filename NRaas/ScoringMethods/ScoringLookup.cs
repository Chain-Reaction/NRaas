using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.CommonSpace.ScoringMethods
{
    public interface IScoringCache
    {
        bool UnloadCaches(bool final);
    }

    public interface IScoringGenerator : Common.IStatGenerator
    {
        int AddScoring(string scoring, SimDescription sim);
        int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel);

        int AddScoring(string scoring, SimDescription sim, SimDescription other);
        int AddScoring(string scoring, SimDescription sim, SimDescription other, Common.DebugLevel minLevel);

        int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim);
        int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, Common.DebugLevel minLevel);

        int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other);
        int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other, Common.DebugLevel minLevel);
    }

    public class ScoringLookup : Common.IWorldLoadFinished
    {
        public enum OptionType
        {
            Chance,
            Bounded,
            Unbounded,
        }

        static Dictionary<string, IListedScoringMethod> sScoring = new Dictionary<string, IListedScoringMethod>();

        static Dictionary<Type, List<IListedScoringMethod>> sTypedScoring = new Dictionary<Type, List<IListedScoringMethod>>();

        static Common.IStatGenerator sStats;

        public static int Count
        {
            get
            {
                return sScoring.Count;
            }
        }

        public static Common.IStatGenerator Stats
        {
            get { return sStats; }
            set { sStats = value; }
        }

        public static int AddStat(string stat, int val)
        {
            if (sStats == null) return val;

            return sStats.AddStat(stat, val);
        }
        public static int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            if (sStats == null) return val;

            return sStats.AddStat(stat, val, minLevel);
        }

        public static float AddStat(string stat, float val)
        {
            if (sStats == null) return val;

            return sStats.AddStat(stat, val);
        }
        public static float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (sStats == null) return val;

            return sStats.AddStat(stat, val, minLevel);
        }

        public static void IncStat(string stat)
        {
            if (sStats == null) return;

            sStats.IncStat(stat);
        }
        public static void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (sStats == null) return;

            sStats.IncStat(stat, minLevel);
        }

        protected static int HandleOption(int option, OptionType type, out bool end)
        {
            end = false;
            switch (type)
            {
                case OptionType.Chance:
                    if (option == 0)
                    {
                        end = true;
                        return 0;
                    }

                    if (option == 100)
                    {
                        end = true;
                        return 100;
                    }

                    return option + RandomUtil.GetInt(-100, 0);
                case OptionType.Unbounded:
                    return option;
                default:
                    if (option == 0)
                    {
                        end = true;
                        return 0;
                    }

                    if (option == 100)
                    {
                        end = true;
                        return 100;
                    }

                    return option;
            }
        }

        public static int GetScore(string name, int option, OptionType type, SimDescription scoreAgainst)
        {
            bool end;
            int score = HandleOption(option, type, out end);
            if (end) return score;

            return GetScore(name, scoreAgainst) + score;
        }
        public static int GetScore(string name, SimDescription scoreAgainst)
        {
            IListedScoringMethod scoring = GetScoring(name);
            if (scoring == null) return 0;

            return scoring.IScore(new SimScoringParameters(scoreAgainst));
        }
        public static int GetScore(string name, int option, OptionType type, SimDescription scoreAgainst, SimDescription other)
        {
            bool end;
            int score = HandleOption(option, type, out end);
            if (end) return score;

            return GetScore(name, scoreAgainst, other) + score;
        }
        public static int GetScore(string name, SimDescription scoreAgainst, SimDescription other)
        {
            IListedScoringMethod scoring = GetScoring(name);
            if (scoring == null) return 0;

            return scoring.IScore(new DualSimScoringParameters(scoreAgainst, other));
        }

        public static IEnumerable<KeyValuePair<string, IListedScoringMethod>> AllScoring
        {
            get { return sScoring; }
        }

        public static Dictionary<string, IListedScoringMethod>.KeyCollection ScoringKeys
        {
            get
            {
                return sScoring.Keys;
            }
        }

        public static void Validate()
        {
            foreach (IListedScoringMethod scoring in sScoring.Values)
            {
                scoring.Validate();
            }
        }

        public static void UnloadCaches<T>()
            where T : IScoring
        {
            List<IListedScoringMethod> list;
            if (!sTypedScoring.TryGetValue(typeof(T), out list))
            {
                list = new List<IListedScoringMethod>();
                sTypedScoring.Add(typeof(T), list);

                foreach (IListedScoringMethod scoring in sScoring.Values)
                {
                    if (scoring.Contains<T>())
                    {
                        list.Add(scoring);
                    }
                } 
            }

            foreach (IListedScoringMethod scoring in list)
            {
                scoring.UnloadCaches(false);
            } 
        }
        public static void UnloadCaches(string name, bool final)
        {
            IListedScoringMethod scoring;
            if (sScoring.TryGetValue(name, out scoring))
            {
                scoring.UnloadCaches(final);
            }
        }
        public static void UnloadCaches(bool final)
        {
            foreach (IListedScoringMethod scoring in sScoring.Values)
            {
                scoring.UnloadCaches(final);
            }

            List<IScoringCache> caches = Common.DerivativeSearch.Find<IScoringCache>();

            foreach (IScoringCache cache in caches)
            {
                cache.UnloadCaches(final);
            }
        }

        public static bool HasScoring(string name)
        {
            return (GetScoring(name, false) != null);
        }

        public static IListedScoringMethod GetScoring(string name)
        {
            return GetScoring(name, true);
        }
        public static IListedScoringMethod GetScoring(string name, bool fireError)
        {
            IListedScoringMethod scoring;
            if (sScoring.TryGetValue(name, out scoring))
            {
                return scoring;
            }
            else
            {
                if (fireError)
                {
                    Common.Notify("Missing Scoring : " + name);
                }
                return null;
            }
        }

        public static bool AddScoring(string name, IListedScoringMethod scoring)
        {
            if (sScoring.ContainsKey(name)) return false;

            sScoring.Add(name, scoring);
            return true;
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                UnloadCaches(true);
            }
            catch (Exception e)
            {
                Common.Exception("ScoringLookup:OnWorldLoadFinished", e);
            }
        }
    }
}