using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class RetiredScenario : SimEventScenario<Event>
    {


        public RetiredScenario()
        { }
        protected RetiredScenario(RetiredScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Retired";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kCareerRetired);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (ManagerCareer.RetiredLocation != null)
            {
                GetData<CareerSimData>(Sim).Retire();
            }

            Add(frame, new CareerChangedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (SimTypes.IsSelectable(Sim)) return null;

            if (Sim.CareerManager.RetiredCareer == null)
            {
                IncStat("Not Retired");
                return null;
            }

            if (manager == null)
            {
                manager = Careers;
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, Sim.CareerManager.RetiredCareer.PensionAmount() };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new RetiredScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, RetiredScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Retired";
            }
        }
    }
}
