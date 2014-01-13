using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
    public class RerollCareerOnTopLevelScenario : SimScenario
    {
        public RerollCareerOnTopLevelScenario (SimDescription sim)
            : base(sim)
        { }
        protected RerollCareerOnTopLevelScenario(RerollCareerOnTopLevelScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RerollCareerOnTopLevel";
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

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected static bool IsTopLevel(Career career)
        {
            if (career == null) return false;

            if (career.CurLevel == null) return false;

            if (career.CurLevel.NextLevels == null) return true;

            if (career.CurLevel.NextLevels.Count == 0) return true;

            return false;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.Occupation == null)
            {
                IncStat("No Job");
                return false;
            }
            else if (!IsTopLevel(sim.Occupation as Career))
            {
                IncStat("Too Low");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim.Occupation.LeaveJobNow(Career.LeaveJobReason.kQuit);

            Add(frame, new FindJobScenario(Sim, true, true), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new RerollCareerOnTopLevelScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>
        {
            public Option()
                : base(false)
            { }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    PromotedScenario.OnJackOfAllTradesScenario += OnPerform;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "RerollCareerOnTopLevel";
            }

            protected static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new RerollCareerOnTopLevelScenario(s.Sim), ScenarioResult.Start);
            }
        }
    }
}
