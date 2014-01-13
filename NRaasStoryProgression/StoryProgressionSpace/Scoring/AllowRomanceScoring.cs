using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class AllowRomanceScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>, IScoring<SimDescription, DualSimScoringParameters>
    {
        public AllowRomanceScoring()
        { }
        public AllowRomanceScoring(int hit, int miss)
            : base(hit, miss)
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
            if (SimTypes.IsSelectable(parameters.Actor)) return false;

            ManagerRomance romance = NRaas.StoryProgression.Main.Romances;

            using (NetWorthOption.CacheValue cache = new NetWorthOption.CacheValue(romance, parameters.Actor.Household))
            {
                return romance.Allow(romance, parameters.Actor, Managers.Manager.AllowCheck.None);
            }
        }
    }
}

