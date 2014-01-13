using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
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
    public class ManageCareerScenario : SimScenario
    {
        bool mIncreasePressure = true;

        public ManageCareerScenario()
        { }
        public ManageCareerScenario(SimDescription sim)
            : base (sim)
        {
            mIncreasePressure = false;
        }
        protected ManageCareerScenario(ManageCareerScenario scenario)
            : base (scenario)
        {
            mIncreasePressure = scenario.mIncreasePressure;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ManageCareer";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if ((!GetValue<NewBossScenario.Option,bool>()) && (!GetValue<NewCoworkerScenario.Option,bool>()) && (!GetValue<NewClassmateScenario.Option,bool>())) return false;

            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.Employed;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (!ManagerCareer.ValidCareer(sim.Occupation))
            {
                IncStat("No Career");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new NewBossScenario(Sim), ScenarioResult.Start);
            Add(frame, new NewCoworkerScenario(Sim), ScenarioResult.Start);
            Add(frame, new NewClassmateScenario(Sim), ScenarioResult.Start);
            return false;
        }

        public override Scenario Clone()
        {
            return new ManageCareerScenario(this);
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    CareerChangedScenario.OnManageCareerScenario += OnInstall;
                }

                return true;
            }

            public static void OnInstall(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;

                scenario.Add(frame, new ManageCareerScenario(s.Sim), ScenarioResult.Start);
            }
        }
    }
}
