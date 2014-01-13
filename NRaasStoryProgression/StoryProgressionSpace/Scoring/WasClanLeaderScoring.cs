using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class WasClanLeaderScoring : IsClanLeaderScoring
    {
        public WasClanLeaderScoring()
        { }

        public override bool IsHit(SimScoringParameters parameters)
        {
            // If we are currently the leader return false
            if (base.IsHit(parameters)) return false;
            
            string lastLeadership = StoryProgression.Main.GetValue<LastLeadershipOption,string>(parameters.Actor);
            if (string.IsNullOrEmpty(lastLeadership)) return false;

            return (lastLeadership.ToLower() == Clan);
        }
    }
}

