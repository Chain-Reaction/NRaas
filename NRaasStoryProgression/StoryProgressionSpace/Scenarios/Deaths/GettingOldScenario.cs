using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class GettingOldScenario : SimEventScenario<Event>, IFormattedStoryScenario
    {
        public GettingOldScenario()
        { }
        protected GettingOldScenario(GettingOldScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "GettingOld";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Deaths;
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimGettingOld);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (SimTypes.IsService(sim))
            {
                IncStat("Service");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Already Dead");
                return false;
            }
            else if (!sim.Elder)
            {
                IncStat("Invalid");
                return false;
            }

            return base.Allow(sim);
        }

        public static event UpdateDelegate OnInheritStuffScenario;

        public static void AddDeathScenarios(SimScenario scenario, ScenarioFrame frame)
        {
            scenario.GetData<ManagerDeath.DyingSimData>(scenario.Sim).Testing = true;

            scenario.Add(frame, new PregnancySaveScenario(scenario.Sim), ScenarioResult.Start);
            scenario.Add(frame, new AncestralSaveScenario(scenario.Sim), ScenarioResult.Failure);

            scenario.Add(frame, new ChildrenToSafetyScenario(scenario.Sim.Household), ScenarioResult.Start);

            if (OnInheritStuffScenario != null)
            {
                OnInheritStuffScenario(scenario, frame);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription partner = Sim.Partner;
            if (partner != null)
            {
                SetElapsedTime<DayOfLastPartnerOption>(Sim);
                SetElapsedTime<DayOfLastPartnerOption>(partner);
            }

            if (Sim.CreatedSim != null)
            {
                Sim.CreatedSim.SetReservedVehicle(null);
            }

            AddDeathScenarios(this, frame);

            Add(frame, new LastOneStandingScenario(Sim), ScenarioResult.Start);
            Add(frame, new DyingScenario(Sim), ScenarioResult.Failure);

            Add(frame, new ResetDyingSimDataScenario(Sim), ScenarioResult.Start);

            return true;
        }

        public override Scenario Clone()
        {
            return new GettingOldScenario(this);
        }

        protected class DyingScenario : SimScenario
        {
            public DyingScenario(SimDescription sim)
                : base (sim)
            { }
            protected DyingScenario(DyingScenario scenario)
                : base (scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "Dying";
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return Sims.All;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (!Deaths.IsDying(sim))
                {
                    IncStat("Saved");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (manager == null)
                {
                    manager = Deaths;
                }

                if (parameters == null)
                {
                    parameters = new object[] { Sim };
                }

                if (!Notifications.HasSignificantRelationship(Household.ActiveHousehold, Sim))
                {
                    return base.PrintFormattedStory(manager, ManagerDeath.ParseNotification("SimGettingOld:TNS", Sim.Household, Sim), summaryKey, parameters, extended, logging);
                }
                else
                {
                    Stories.AddSummary(manager, Sim, UnlocalizedName, null, logging);
                }

                return null;
            }

            public override Scenario Clone()
            {
                return new DyingScenario(this);
            }
        }

        public class ResetDyingSimDataScenario : ScheduledSoloScenario
        {
            SimDescription mSim;

            public ResetDyingSimDataScenario(SimDescription sim)
            {
                mSim = sim;
            }
            public ResetDyingSimDataScenario(ResetDyingSimDataScenario scenario)
            {
                mSim = scenario.mSim;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "ResetDyingSimData";
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override bool Allow()
            {
                if (mSim == null)
                {
                    IncStat("No Sim");
                    return false;
                }

                return base.Allow();
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                GetData<ManagerDeath.DyingSimData>(mSim).Testing = false;
                return true;
            }

            public override Scenario Clone()
            {
                return new ResetDyingSimDataScenario(this);
            }
        }

        public class Option : BooleanEventOptionItem<ManagerDeath, GettingOldScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "GettingOld";
            }
        }
    }
}
