using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
using System.Diagnostics;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class DiedScenario : SimEventScenario<Event>, IFormattedStoryScenario
    {
        public DiedScenario()
        { }
        protected DiedScenario(DiedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "Death";
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
            return events.AddListener(this, EventTypeId.kSimDied);
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
            if (sim.Household != null)
            {
                IncStat("Immigrant");
                return false;
            }

            Household house = GetData<StoredNetWorthSimData>(sim).Household;

            if ((house != null) && (house.IsSpecialHousehold))
            {
                IncStat("Service");
                return false;
            }
            else if (Deaths.WasCleansed(sim))
            {
                IncStat("Cleansed");
                return false;
            }

            return base.Allow(sim);
        }

        public static event UpdateDelegate OnInheritCashScenario;
        public static event UpdateDelegate OnPropagateMournScenario;
        public static event UpdateDelegate OnFuneralScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnInheritCashScenario != null)
            {
                OnInheritCashScenario(this, frame);
            }

            if (OnPropagateMournScenario != null)
            {
                OnPropagateMournScenario(this, frame);
            }

            if (OnFuneralScenario != null)
            {
                OnFuneralScenario(this, frame);
            }

            ManagerSim.ForceRecount();

            Manager.AddAlarm(new SimDisposedScenario(Sim));
            return true;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Deaths;
            }

            if (GetData<ManagerDeath.DyingSimData>(Sim).Notified) return null;

            if (Sim.DeathStyle == SimDescription.DeathType.None) return null;

            if (Notifications.HasSignificantRelationship(Household.ActiveHousehold, Sim))
            {
                Stories.AddSummary(manager, Sim, UnlocalizedName, null, logging);
                return null;
            }

            SimDescription.DeathType deathStyle = Sim.DeathStyle;

            // Temporary until a story can be written
            switch (Sim.DeathStyle)
            {
                case SimDescription.DeathType.PetOldAgeBad:
                case SimDescription.DeathType.PetOldAgeGood:
                    deathStyle = SimDescription.DeathType.OldAge;
                    break;
            }

            text = "SimDied:" + deathStyle;

            ManagerStory.Story story = base.PrintFormattedStory(manager, ManagerDeath.ParseNotification(text, Sim.Household, Sim), summaryKey, parameters, extended, logging);

            if (story != null)
            {
                story.mShowNoImage = true;
            }

            return story;
        }

        public override Scenario Clone()
        {
            return new DiedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerDeath, DiedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Died";
            }
        }
    }
}
