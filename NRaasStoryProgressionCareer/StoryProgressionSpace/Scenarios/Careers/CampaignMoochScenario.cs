using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
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
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class CampaignMoochScenario : MoneyTransferScenario
    {
        bool mPush = true;

        int mSteal = 0;

        public CampaignMoochScenario()
            : base (10)
        { }
        public CampaignMoochScenario(SimDescription sim, SimDescription target, bool push)
            : base (sim, target, 10)
        {
            mPush = push;
        }
        protected CampaignMoochScenario(CampaignMoochScenario scenario)
            : base (scenario)
        {
            mPush = scenario.mPush;
            mSteal = scenario.mSteal;
        }

        protected override bool CheckBusy
        {
            get { return mPush; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CampaignMooch";
        }

        protected override string AccountingKey
        {
            get { return "CampaignDonation"; }
        }

        protected override int ContinueReportChance
        {
            get { return 25; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override int Maximum
        {
            get 
            {
                int upper = GetValue<MaxOption, int>();

                if (Sim == null)
                {
                    return upper;
                }
                else
                {
                    int val = GetRemaining(Sim);

                    if (val > upper)
                    {
                        val = upper;
                    }

                    return val;
                }
            }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool TestOpposing
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.GetCareerSims(OccupationNames.Political);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return new SimScoringList(Manager, "MoochVictim", Sims.Adults, false, sim).GetBestByMinScore(1);
        }

        protected static int GetRemaining(SimDescription sim)
        {
            if (sim == null) return 0;

            Political job = sim.Occupation as Political;
            if (job == null) return 0;

            Political.MetricCampaignMoney metric = null;
            foreach (PerfMetric item in job.CurLevel.Metrics)
            {
                metric = item as Political.MetricCampaignMoney;
                if (metric != null)
                {
                    break;
                }
            }
            if (metric == null) return 0;

            return (int)(metric.mMap3 - job.CampaignMoneyRaised);
        }

        public static bool IsRequired (Common.IStatGenerator stats, SimDescription sim)
        {
            Political job = sim.Occupation as Political;
            if (job == null)
            {
                stats.IncStat("No Job");
                return false;
            }
            else if (!job.HasCampaignMoneyMetric())
            {
                stats.IncStat("No Need");
                return false;
            }
            else if (GetRemaining(sim) <= 0)
            {
                stats.IncStat("Unnecessary");
                return false;
            }

            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!IsRequired(this, sim))
            {
                IncStat("Unnecessary");
                return false;
            }
            else if (AddScoring("CampaignMooch", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool AdjustFunds()
        {
            // Overridden to stop the transfer in the base class
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Political job = Sim.Occupation as Political;
            if (job == null)
            {
                IncStat("No Job");
                return false;
            }

            if ((job.ChanceCaught > 0) && (AddScoring("Embezzlement", Sim) > 0))
            {
                if (AddScoring("CaughtEmbezzling", job.ChanceCaught, ScoringLookup.OptionType.Chance, Sim) > 0)
                {
                    Add(frame, new CaughtEmbezzlingScenario(Sim), ScenarioResult.Start);
                }
                else
                {
                    mSteal = (int)(Funds * job.PercentCampaignFundsToSteal);
                    if (mSteal > Political.kEmbezzleMax)
                    {
                        mSteal = Political.kEmbezzleMax;
                    }
                    else if (mSteal < Political.kEmbezzleMin)
                    {
                        mSteal = Political.kEmbezzleMin;
                    }
                }
            }

            Money.AdjustFunds(Sim, AccountingKey, mSteal);

            job.CampaignMoneyRaised += (Funds - mSteal);

            if ((Sim.CreatedSim != null) && (Target.CreatedSim != null))
            {
                // CampaignDonationScenario will handle the adjust funds for the Target
                EventTracker.SendEvent(new IncrementalEvent(EventTypeId.kRaisedTonsOfMoney, Sim.CreatedSim, Target.CreatedSim, (float)Funds));
                EventTracker.SendEvent(EventTypeId.kAskedForDonation, Sim.CreatedSim, Target.CreatedSim);
            }
            else
            {
                Money.AdjustFunds(Target, AccountingKey, -Funds);
            }

            return true;
        }

        protected override bool Push()
        {
            if (!mPush) return true;

            return base.Push();
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (mSteal > 0)
                {
                    name += "AndSteal";
                }

                parameters = new object[] { Sim, Target, Funds + mSteal, mSteal };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new CampaignMoochScenario(this);
        }

        public class MaxOption : IntegerScenarioOptionItem<ManagerCareer, CampaignMoochScenario>
        {
            public MaxOption()
                : base(10000)
            { }

            public override string GetTitlePrefix()
            {
                return "CampaignMoochMax";
            }
        }
    }
}
