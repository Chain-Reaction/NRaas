using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsOpposingClanScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>, IClanScoring
    {
        public IsOpposingClanScoring()
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            return StoryProgression.Main.Personalities.IsOpposing(StoryProgression.Main.Personalities, parameters.Actor, parameters.Other, false);
        }
    }
}

