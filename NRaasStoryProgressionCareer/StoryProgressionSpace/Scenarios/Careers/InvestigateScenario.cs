using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Personalities;
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
    public class InvestigateScenario : MoneyTransferScenario
    {
        string mStoryName;

        SimDescription mInstigator;

        int mMinimum = 250;
        int mMaximum = 1000;

        public InvestigateScenario (SimDescription sim, SimDescription instigator, string story, int minimum, int maximum)
            : base(sim, 20)
        {
            mStoryName = story;
            mInstigator = instigator;
            mMinimum = minimum;
            mMaximum = maximum;
        }
        protected InvestigateScenario(InvestigateScenario scenario)
            : base (scenario)
        {
            mStoryName = scenario.mStoryName;
            mInstigator = scenario.mInstigator;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "Investigate";
            }
            else
            {
                return mStoryName;
            }
        }

        protected override string AccountingKey
        {
            get { return "Investigate"; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool TargetCheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Minimum
        {
            get { return mMinimum; }
        }

        protected override int Maximum
        {
            get { return mMaximum; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Careers.GetCareerSims(OccupationNames.PrivateEye);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (mInstigator == sim)
            {
                IncStat("Instigator");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (ManagerFriendship.AreEnemies(sim, Sim))
            {
                IncStat("Enemy");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            PrivateEye job = Target.Occupation as PrivateEye;
            if (job == null)
            {
                IncStat("No Job");
                return false;
            }

            job.UpdateXp(50);

            job.CaseCompleted();
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override bool Push()
        {
            return Situations.PushVisit(this, Target, Lots.GetCommunityLot(Target.CreatedSim, null, false));
        }

        public override Scenario Clone()
        {
            return new InvestigateScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    List<IInvestigationScenario> scenarios = Common.DerivativeSearch.Find<IInvestigationScenario>();

                    foreach (IInvestigationScenario scenario in scenarios)
                    {
                        scenario.InstallInvestigation(OnRun);
                    }
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "Investigate";
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                IInvestigationScenario s = scenario as IInvestigationScenario;
                if (s == null) return;

                if (!s.AllowGoToJail) return;

                if (string.IsNullOrEmpty(s.InvestigateStoryName)) return;

                scenario.Add(frame, new InvestigateScenario(s.Target, s.Sim, s.InvestigateStoryName, s.InvestigateMinimum, s.InvestigateMaximum), ScenarioResult.Failure);
            }
        }
    }
}
