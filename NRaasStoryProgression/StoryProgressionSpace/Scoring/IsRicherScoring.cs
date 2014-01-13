using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsRicherScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>
    {
        public IsRicherScoring()
        { }

        public override bool Cachable
        {
            get { return false; }
        }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            int actor = StoryProgression.Main.GetValue<NetWorthOption, int>(parameters.Actor.Household);
            int other = StoryProgression.Main.GetValue<NetWorthOption, int>(parameters.Other.Household);

            return (actor > other);
        }
    }
}

