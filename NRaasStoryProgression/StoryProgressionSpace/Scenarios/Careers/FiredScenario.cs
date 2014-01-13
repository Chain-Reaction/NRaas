using NRaas.StoryProgressionSpace;
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
    public class FiredScenario : CareerEventScenario, IFormattedStoryScenario
    {
        public FiredScenario()
        { }
        protected FiredScenario(FiredScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "Fired";
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
            return events.AddListener(this, EventTypeId.kCareerFired);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new CareerChangedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            if (Event.Career is School) return null;

            text = Common.LocalizeEAString(Sim.IsFemale, "Gameplay/Careers/Career:FiredText", new object[] { Sim });

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new FiredScenario(this);
        }

        public class FiredOption : BooleanEventOptionItem<ManagerCareer, FiredScenario>, IDebuggingOption
        {
            public FiredOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Fired";
            }
        }
    }
}
