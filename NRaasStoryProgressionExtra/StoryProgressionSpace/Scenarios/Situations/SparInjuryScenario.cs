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
using Sims3.Gameplay.Skills;
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
    public class SparInjuryScenario : SimEventScenario<MartialArts.SparEvent>
    {
        Sim mWinner;

        public SparInjuryScenario()
        { }
        protected SparInjuryScenario(SparInjuryScenario scenario)
            : base (scenario)
        {
            mWinner = scenario.mWinner;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SparResults";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSparred);
        }

        protected override Scenario Handle(Event paramE, ref ListenerAction result)
        {
            MartialArts.SparEvent e = paramE as MartialArts.SparEvent;
            if (e == null) return null;

            if (e.HasWon) return null;

            mWinner = e.Actor as Sim;

            return base.Handle(paramE, ref result);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Situations.Allow(this, sim))
            {
                IncStat("Scoring Fail");
                return false;
            }
            else if (mWinner == null)
            {
                IncStat("No Winner");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription.DeathType type = SimDescription.DeathType.Starve;
            if (mWinner.OccultManager.HasOccultType(OccultTypes.Mummy))
            {
                type = SimDescription.DeathType.MummyCurse;
            }

            Add(frame, new GoToHospitalScenario(Sim, null, "InjuredFight", type), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new SparInjuryScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSituation, SparInjuryScenario>
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "SparInjury";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP1);
            }
        }
    }
}
