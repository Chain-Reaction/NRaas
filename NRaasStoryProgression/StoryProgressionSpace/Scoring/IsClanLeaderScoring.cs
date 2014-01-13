using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class IsClanLeaderScoring : IsClanBaseScoring
    {
        public IsClanLeaderScoring()
        { }

        public override bool IsHit(SimScoringParameters parameters)
        {
            SimPersonality clan = StoryProgression.Main.Personalities.GetPersonality(Clan);
            if (clan == null) return false;

            return (clan.Me == parameters.Actor);
        }
    }
}

