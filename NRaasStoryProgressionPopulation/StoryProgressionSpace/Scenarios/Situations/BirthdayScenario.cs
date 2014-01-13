using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class BirthdayScenario : GatheringScenario
    {
        SimDescription mBirthdaySim = null;

        protected BirthdayScenario(SimDescription host, SimDescription birthdaySim)
            : base(host, host.LotHome)
        {
            mBirthdaySim = birthdaySim;
        }
        protected BirthdayScenario(BirthdayScenario scenario)
            : base (scenario)
        {
            mBirthdaySim = scenario.mBirthdaySim;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "BirthdayGathering";
        }

        protected override OutfitCategories PartyAttire
        {
            get { return OutfitCategories.Everyday; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (AddScoring("HostParty", sim) < 0)
            {
                IncStat("Scoring Fail");
                return false;
            }

            return base.Allow(sim);
        }

        public static BirthdayScenario Create(ManagerSituation manager, SimDescription birthdaySim)
        {
            SimDescription host = null;

            List<SimDescription> hosts = new List<SimDescription>();
            if (!manager.Households.AllowGuardian(birthdaySim))
            {
                foreach (SimDescription parent in Relationships.GetParents(birthdaySim))
                {
                    hosts.Add(parent);
                }
            }
            else
            {
                hosts.Add(birthdaySim);
            }

            foreach (SimDescription sim in hosts)
            {
                if (sim == null) continue;

                if (sim.LotHome == null) continue;

                if (manager.IsBusy(manager, sim, true)) continue;

                host = sim;
                break;
            }

            if (host == null) return null;

            return new BirthdayScenario(host, birthdaySim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((!Relationships.IsCloselyRelated(mBirthdaySim, Target, false)) || (ManagerSim.GetLTR(mBirthdaySim, Target) < 50))
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Sim == mBirthdaySim)
                {
                    name += "SelfHost";
                }
                else
                {
                    parameters = new object[] { Sim, mBirthdaySim, Lot };
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new BirthdayScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BirthdayGathering";
            }
        }
    }
}
