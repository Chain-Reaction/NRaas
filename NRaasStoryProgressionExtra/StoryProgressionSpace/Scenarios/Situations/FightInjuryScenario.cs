using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class FightInjuryScenario : SocialEventScenario
    {
        public FightInjuryScenario()
        { }
        protected FightInjuryScenario(FightInjuryScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "FightStory";
        }

        protected override bool ShouldReport
        {
            get
            {
                return GetValue<StoryOption,bool>();
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SocialEvent e)
        {
            if (!e.WasAccepted) return false;

            if (!e.ActorWonFight) return false;

            return base.Allow(e);
        }

        protected override bool Allow(string socialName)
        {
            return (socialName == "Fight!");
        }

        protected static bool ReduceMotives(SimDescription sim)
        {
            Sim createdSim = sim.CreatedSim;
            if (createdSim == null) return false;

            createdSim.Motives.SetExhausted();
            createdSim.Motives.SetValue(CommodityKind.Hygiene, float.MinValue);

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription.DeathType type = SimDescription.DeathType.Starve;
            if (Sim.OccultManager.HasOccultType(OccultTypes.Mummy))
            {
                type = SimDescription.DeathType.MummyCurse;
            }

            if ((Target != null) && (Situations.Allow(this, Target, Managers.Manager.AllowCheck.None)))
            {
                Add(frame, new GoToHospitalScenario(Target, Sim, "InjuredFight", type), ScenarioResult.Start);
            }

            ReduceMotives(Sim);
            ReduceMotives(Target);
            return true;
        }

        public override Scenario Clone()
        {
            return new FightInjuryScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, FightInjuryScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "FightInjury";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class StoryOption : BooleanManagerOptionItem<ManagerSituation>
        {
            public StoryOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "FightStory";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
