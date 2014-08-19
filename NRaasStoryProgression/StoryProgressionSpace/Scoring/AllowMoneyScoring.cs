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

            bool allow = false;
            using (NetWorthOption.CacheValue cache = new NetWorthOption.CacheValue(money, parameters.Actor.Household))
            {
                allow = money.Allow(money, parameters.Actor, true);
                if (allow)
                {
                    SimData data = StoryProgression.Main.GetData(parameters.Actor);
                    if (data != null)
                    {                        
                        if ((!data.GetValue<AllowMoneyOption, bool>()))
                        {
                            return false;
                        }
                    }
                }

                return allow;
            }
        }
    }
}

