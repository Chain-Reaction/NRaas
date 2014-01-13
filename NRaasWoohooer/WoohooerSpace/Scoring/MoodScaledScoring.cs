using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Scoring
{
    public class MoodScaledScoring : SimScaledScoring<SimScoringParameters>
    {
        public MoodScaledScoring()
        { }

        public override bool Cachable
        {
            get { return false; }
        }

        protected override int GetScaler(SimScoringParameters parameters)
        {
            if (!Woohooer.Settings.mUseMoodInScoring) return 0;

            if (parameters.Actor.CreatedSim == null) return 0;

            if (parameters.Actor.CreatedSim.MoodManager == null) return 0;

            return parameters.Actor.CreatedSim.MoodManager.MoodValue;
        }
    }
}

