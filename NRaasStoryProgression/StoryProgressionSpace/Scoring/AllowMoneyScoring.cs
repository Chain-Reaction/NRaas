using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class AllowMoneyScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public AllowMoneyScoring()
        { }

        public override bool Cachable
        {
            get { return false; }
        }

        public int Score(DualSimScoringParameters parameters)
        {
            return Score(parameters as SimScoringParameters);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            ManagerMoney money = StoryProgression.Main.Money;

            using (NetWorthOption.CacheValue cache = new NetWorthOption.CacheValue(money, parameters.Actor.Household))
            {
                return money.Allow(money, parameters.Actor);
            }
        }
    }
}

