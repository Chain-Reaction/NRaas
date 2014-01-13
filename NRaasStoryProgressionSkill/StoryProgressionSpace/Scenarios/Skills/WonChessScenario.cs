using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
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
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class WonChessScenario : SimEventScenario<PlayedChessEvent>
    {
        SimDescription mOpponent;

        public WonChessScenario()
        { }
        protected WonChessScenario(WonChessScenario scenario)
            : base (scenario)
        {
            mOpponent = scenario.mOpponent;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "WonChess";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int ContinueReportChance
        {
            get { return 20; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kPlayedRankedChessMatch);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            PlayedChessEvent pEvent = e as PlayedChessEvent;
            if (pEvent == null) return null;

            if (pEvent.mOpponent == null) return null;

            if (!pEvent.WonGame) return null;

            Sim sim = e.Actor as Sim;
            if (sim == null) return null;

            mOpponent = pEvent.mOpponent.SimDescription;

            return base.Handle(e, ref result);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mOpponent };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new WonChessScenario(this);
        }

        public class Installer : ExpansionInstaller<ManagerSkill>
        {
            protected override bool PrivateInstall(ManagerSkill manager, bool initial)
            {
                manager.AddListener(new WonChessScenario());

                return true;
            }
        }
    }
}
