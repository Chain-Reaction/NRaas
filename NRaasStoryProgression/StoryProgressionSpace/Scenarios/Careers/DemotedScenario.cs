using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scoring;
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
    public class DemotedScenario : CareerEventScenario, IFormattedStoryScenario
    {
        public DemotedScenario()
        { }
        protected DemotedScenario(DemotedScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "Demotion";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Careers;
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kCareerDemotion);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        public static event UpdateDelegate OnOnlyOneMayLeadScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (OnOnlyOneMayLeadScenario != null)
            {
                OnOnlyOneMayLeadScenario(this, frame);
            }

            Add(frame, new CareerChangedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            if ((extended == null) && (Sim.Occupation != null))
            {
                extended = new string[] { Sim.Occupation.GetLocalizedCareerName(Sim.IsFemale), EAText.GetNumberString(Sim.Occupation.Level) };
            }

            text = Common.LocalizeEAString(Sim.IsFemale, "Gameplay/Careers/Career:Demotion", new object[] { Sim, Event.Career.CurLevelJobTitle });

            ManagerStory.Story story = base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
                
            if (story != null)
            {
                story.mOverrideImage = Event.Career.CareerIconColored;
            }

            return story;
        }

        public override Scenario Clone()
        {
            return new DemotedScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCareer, DemotedScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Demoted";
            }
        }
    }
}
