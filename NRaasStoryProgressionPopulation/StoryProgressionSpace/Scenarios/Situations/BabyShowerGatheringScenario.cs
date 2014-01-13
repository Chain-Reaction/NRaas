using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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
    public class BabyShowerGatheringScenario : GatheringScenario
    {
        public BabyShowerGatheringScenario(SimDescription sim)
            : base(sim, sim.LotHome)
        { }
        protected BabyShowerGatheringScenario(BabyShowerGatheringScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "BabyShowerGathering";
        }

        protected override bool Progressed
        {
            get { return true; }
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

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if ((target.Teen) && (!Sim.Teen))
            {
                IncStat("Too Young");
                return false;
            }
            else if (Sim.Gender != target.Gender)
            {
                IncStat("Wrong Gender");
                return false;
            }
            else if (ManagerSim.GetLTR(Sim, target) < 50)
            {
                IncStat("Low LTR");
                return false;
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            int maximum = GetValue<MaximumGiftOption, int>();

            foreach (SimDescription sim in Guests)
            {
                Add(frame, new ManualCashTransferScenario(Sim, sim, 10, "BabyShower", 0, maximum), ScenarioResult.Start);
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
            return new BabyShowerGatheringScenario(this);
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
                    HaveBabyScenario.OnGatheringScenario += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "BabyShowerGathering";
            }

            public static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new BabyShowerGatheringScenario(s.Sim), ScenarioResult.Start);
            }
        }

        public class MaximumGiftOption : IntegerManagerOptionItem<ManagerSituation>, ManagerSituation.IGatheringOption
        {
            public MaximumGiftOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "BabyShowerMaximumGift";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
