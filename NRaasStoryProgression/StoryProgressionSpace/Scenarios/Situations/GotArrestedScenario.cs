using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class GotArrestedScenario : DisgracefulActionEventScenario
    {
        static Dictionary<ulong, Pair<string, int>> sRepository = new Dictionary<ulong, Pair<string, int>>();

        string mStoryName;

        public GotArrestedScenario()
        { }
        protected GotArrestedScenario(GotArrestedScenario scenario)
            : base (scenario)
        {
            mStoryName = scenario.mStoryName;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "GoToJail";
            }
            else
            {
                return mStoryName;
            }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimCommittedDisgracefulAction);
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Handles(DisgracefulActionType type)
        {
            return (type == DisgracefulActionType.Arrested);
        }

        protected override bool Allow()
        {
            if (!GetValue<OptionV2,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public static void AddToRepository(SimDescription sim, string storyName, int bail)
        {
            sRepository[sim.SimDescriptionId] = new Pair<string, int>(storyName, bail);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int bail = 0;

            ManagerSim.AddBuff(this, Sim, BuffNames.Nauseous, Origin.FromJailFood);

            Pair<string,int> value;
            if (sRepository.TryGetValue(Sim.SimDescriptionId, out value))
            {
                mStoryName = value.First;
                bail = value.Second;

                sRepository.Remove(Sim.SimDescriptionId);
            }

            if (Sim.Occupation != null)
            {
                if (AddScoring("JailDemotion", GetValue<DemoteChanceOption, int>(), ScoringLookup.OptionType.Chance, Sim) > 0)
                {
                    Sim.Occupation.DemoteSim();
                }
            }

            bail += GetValue<BailOption, int>();
            if (bail > 0)
            {
                Money.AdjustFunds(Sim, "CourtFees", -bail);
            }

            return true;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new GotArrestedScenario(this);
        }

        public class OptionV2 : BooleanEventOptionItem<ManagerSituation, GotArrestedScenario>, IDebuggingOption
        {
            public OptionV2()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HandleArrest";
            }
        }

        public class DemoteChanceOption : IntegerManagerOptionItem<ManagerCareer>, ManagerCareer.IPerformanceOption
        {
            public DemoteChanceOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "JailDemoteChance";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<OptionV2, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class BailOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public BailOption()
                : base(500)
            { }

            public override string GetTitlePrefix()
            {
                return "JailBail";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<OptionV2, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
