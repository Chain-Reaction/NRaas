using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
    public class CampaignGatheringScenario : GatheringScenario
    {
        public CampaignGatheringScenario()
        { }
        protected CampaignGatheringScenario(CampaignGatheringScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CampaignGathering";
        }

        protected override OutfitCategories PartyAttire
        {
            get { return OutfitCategories.Formalwear; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (GetValue<Option,int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.TraitManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (AddScoring("CampaignMooch", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }
            else if (!CampaignMoochScenario.IsRequired(this, sim))
            {
                IncStat("Unnecessary");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsService(sim))
            {
                IncStat("Service");
                return false;
            }
            else if (AddScoring("PartyGuest", Sim, sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            int delay = 1;
            if (IsHomeParty)
            {
                delay = 4;
            }

            foreach (SimDescription guest in Guests)
            {
                Manager.AddAlarm(new DelayedMoochScenario(Sim, guest, delay));
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new CampaignGatheringScenario(this);
        }

        protected class DelayedMoochScenario : CampaignMoochScenario, IAlarmScenario
        {
            int mDelay = 0;

            public DelayedMoochScenario(SimDescription sim, SimDescription target, int delay)
                : base(sim, target, false)
            {
                mDelay = delay;
            }
            public DelayedMoochScenario(DelayedMoochScenario scenario)
                : base(scenario)
            {
                mDelay = scenario.mDelay;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "DelayedCampaignMooch";
                }
                else
                {
                    return base.GetTitlePrefix(type);
                }
            }

            public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
            {
                return alarms.AddAlarm(this, mDelay);
            }

            protected override bool TargetAllow(SimDescription sim)
            {
                if ((Sim.CreatedSim == null) || (Target.CreatedSim == null))
                {
                    IncStat("Hibernating");
                    return false;
                }

                return base.TargetAllow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (Sim.CreatedSim.LotCurrent != Target.CreatedSim.LotCurrent)
                {
                    if (!Situations.PushVisit(this, Target, Sim.CreatedSim.LotCurrent)) return false;
                }

                return base.PrivateUpdate(frame);
            }

            public override Scenario Clone()
            {
                return new DelayedMoochScenario(this);
            }
        }

        public class Option : ChanceScenarioOptionItem<ManagerSituation, CampaignGatheringScenario>, ManagerSituation.IGatheringOption
        {
            public Option()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "CampaignGathering";
            }
        }
    }
}
