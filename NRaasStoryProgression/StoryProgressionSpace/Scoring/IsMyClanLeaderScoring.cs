using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsMyClanLeaderScoring : HitMissScoring<SimDescription, DualSimScoringParameters, DualSimScoringParameters>, IClanScoring
    {
        public IsMyClanLeaderScoring()
        { }
        public IsMyClanLeaderScoring(int hit, int miss)
            : base(hit, miss)
        { }

        public override bool IsHit(DualSimScoringParameters parameters)
        {
            List<SimPersonality> clans = StoryProgression.Main.Personalities.GetClanMembership(parameters.Actor, false);

            foreach (SimPersonality clan in clans)
            {
                if (clan.Me == parameters.Other) return true;
            }

            return false;
        }
    }
}

