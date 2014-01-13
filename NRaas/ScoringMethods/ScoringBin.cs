using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.ScoringMethods
{
    public class ScoringBin : StatBin, IScoringGenerator
    {
        public ScoringBin(string name)
            : base(name)
        { }

        public int AddScoring(string scoring, SimDescription sim)
        {
            return AddScoring(scoring, sim, Common.DebugLevel.Stats);
        }
        public int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim), minLevel);
        }

        public int AddScoring(string scoring, SimDescription sim, SimDescription other)
        {
            return AddScoring(scoring, sim, other, Common.DebugLevel.Stats);
        }
        public int AddScoring(string scoring, SimDescription sim, SimDescription other, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim, other), minLevel);
        }

        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim)
        {
            return AddScoring(scoring, option, type, sim, Common.DebugLevel.Stats);
        }
        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, option, type, sim), minLevel);
        }

        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other)
        {
            return AddScoring(scoring, option, type, sim, other, Common.DebugLevel.Stats);
        }
        public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, option, type, sim, other), minLevel);
        }
    }
}

