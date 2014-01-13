using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class EngagementGatheringScenario : GatheringScenario
    {
        public EngagementGatheringScenario(SimDescription sim)
            : base(sim, sim.LotHome)
        { }
        protected EngagementGatheringScenario(EngagementGatheringScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "EngagementGathering";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override OutfitCategories PartyAttire
        {
            get { return OutfitCategories.Formalwear; }
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

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Friends.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.Partner == null)
            {
                IncStat("No Partner");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if ((sim.Teen) && (!Sim.Teen))
            {
                IncStat("Too Young");
                return false;
            }
            else if (Sim.Gender != Target.Gender)
            {
                IncStat("Wrong Gender");
                return false;
            }
            else if (Sim.Partner == Target)
            {
                IncStat("Partner");
                return false;
            }
            else if ((ManagerSim.GetLTR(Sim, Target) < 50) &&
                     (ManagerSim.GetLTR(Sim.Partner, Target) < 50))
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override Party GetParty(Lot lot, Sim host, List<SimDescription> guests, OutfitCategories partyAttire, DateAndTime startTime)
        {
            if (BachelorParty.CanHaveBachelorParty(host))
            {
                return new BachelorParty(lot, host, guests, partyAttire, startTime);
            }
            else
            {
                return base.GetParty(lot, host, guests, partyAttire, startTime);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!Guests.Contains(Sim.Partner))
            {
                Guests.Add(Sim.Partner);
            }

            if (!base.PrivateUpdate(frame)) return false;

            if (Sim.CreatedSim != null)
            {
                EventTracker.SendEvent(EventTypeId.kThrewBachelorParty, Sim.CreatedSim);
            }

            Sim.SetHadBachelorParty();

            int maximum = GetValue<MaximumGiftOption, int>();

            foreach (SimDescription sim in Guests)
            {
                Add(frame, new ManualCashTransferScenario(Sim, sim, 10, "Engagement", 0, maximum), ScenarioResult.Start);
            }

            return true;
        }

        protected override bool Push()
        {
            if (Target == null)
            {
                if (!Situations.PushGathering(this, Sim.Partner, Lot)) return false;
            }             
                
            return base.Push();
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Romances;
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, Sim.Partner, Lot };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new EngagementGatheringScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSituation main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    EngagementScenario.OnGatheringScenario += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "EngagementGathering";
            }

            public static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                DualSimScenario s = scenario as DualSimScenario;
                if (s == null) return;

                if (RandomUtil.CoinFlip())
                {
                    scenario.Add(frame, new EngagementGatheringScenario(s.Sim), ScenarioResult.Start);
                }
                else
                {
                    scenario.Add(frame, new EngagementGatheringScenario(s.Target), ScenarioResult.Start);
                }
            }
        }

        public class MaximumGiftOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public MaximumGiftOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "EngagementMaximumGift";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
