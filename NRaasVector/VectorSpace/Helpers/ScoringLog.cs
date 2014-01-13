using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;

namespace NRaas.VectorSpace
{
    public sealed class ScoringLog : Common.IStatGenerator
    {
        static StatBin sStats = new StatBin("Vector");

        public static ScoringLog sLog = new ScoringLog();

        static ScoringLog()
        {
            ScoringLookup.Stats = sLog;
        }

        public static void Dump(bool empty)
        {
            sStats.Dump(null, empty);
        }

        public Common.DebugLevel DebuggingLevel
        {
            get { return Common.DebugLevel.Stats; }
        }

        public int AddScoring(string scoring, SimDescription sim)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim));
        }
        public int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim));
        }

        public int AddScoring(string stat, int score)
        {
            return (int)sStats.AddScoring(stat, score);
        }
        public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
        {
            return (int)sStats.AddScoring(stat, score);
        }

        public int AddStat(string stat, int val)
        {
            return (int)sStats.AddStat(stat, val);
        }
        public int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            return (int)sStats.AddStat(stat, val);
        }

        public float AddStat(string stat, float val)
        {
            return sStats.AddStat(stat, val);
        }
        public float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            return sStats.AddStat(stat, val);
        }

        public void IncStat(string stat)
        {
            sStats.IncStat(stat);
        }
        public void IncStat(string stat, Common.DebugLevel minLevel)
        {
            sStats.IncStat(stat);
        }
    }
}
