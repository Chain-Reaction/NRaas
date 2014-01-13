using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class PartnerScenario : DualSimScenario
    {
        public PartnerScenario()
        { }
        protected PartnerScenario(PartnerScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected virtual bool TestPartnered
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {            
            return Romances.Partnered;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Romances.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((TestPartnered) && (sim.Partner != Sim))
            {
                IncStat("Not Partner");
                return false;
            }
            else if (!Romances.Allow(this, Sim, sim))
            {
                return false;
            }

            return base.TargetAllow(sim);
        }
    }
}
