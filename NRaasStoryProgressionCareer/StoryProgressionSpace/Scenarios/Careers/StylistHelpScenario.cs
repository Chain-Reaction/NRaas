using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
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
    public class StylistHelpScenario : MoneyTransferScenario
    {
        public StylistHelpScenario(SimDescription sim)
            : base (sim, 25)
        { }
        protected StylistHelpScenario(StylistHelpScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "StylistHelp";
        }

        protected override string AccountingKey
        {
            get { return "StylistHelp"; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int Maximum
        {
            get { return 250; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Careers.GetCareerSims(OccupationNames.Stylist);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situation Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }
            else if (ManagerFriendship.AreEnemies(Sim, sim))
            {
                IncStat("Enemy");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Stylist job = Target.Occupation as Stylist;
            if (job == null)
            {
                IncStat("No Job");
                return false;
            }

            Occupation.SimCompletedTask(Target.CreatedSim, TaskId.Everyday, Sim.CreatedSim);

            return true;
        }

        public override Scenario Clone()
        {
            return new StylistHelpScenario(this);
        }

        public class Installer : ExpansionInstaller<ManagerCareer>
        {
            protected override bool PrivateInstall(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    ManagerCareer.sStylistHelp += OnPerform;
                }

                return true;
            }

            protected static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario simScenario = scenario as SimScenario;
                if (simScenario == null) return;

                frame.Add(scenario.Manager, new StylistHelpScenario(simScenario.Sim), ScenarioResult.Start);
            }
        }
    }
}
