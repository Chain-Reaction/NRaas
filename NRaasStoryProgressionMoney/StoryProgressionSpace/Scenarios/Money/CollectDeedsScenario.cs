using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class CollectDeedsScenario : SimScenario
    {
        public CollectDeedsScenario()
            : base ()
        { }
        protected CollectDeedsScenario(CollectDeedsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InactiveDeeds";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int ContinueReportChance
        {
            get { return 25; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (AddScoring("CollectDeeds", sim) <= 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        public static int Perform(Sim sim)
        {
            using (ManagerMoney.SetAccountingKey setKey = new ManagerMoney.SetAccountingKey(sim.Household, "Investments"))
            {
                return Investments.Collect(sim);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int totalFunds = Perform(Sim.CreatedSim);

            if (totalFunds <= 0) return false;

            Manager.AddAlarm(new PostScenario(Sim, totalFunds));
            return true;
        }

        public override Scenario Clone()
        {
            return new CollectDeedsScenario(this);
        }

        public class PostScenario : SimScenario, IAlarmScenario
        {
            int mTotalFunds = 0;

            public PostScenario(SimDescription sim, int totalFunds)
                : base(sim)
            {
                mTotalFunds = totalFunds;
            }
            protected PostScenario(PostScenario scenario)
                : base(scenario)
            {
                mTotalFunds = scenario.mTotalFunds;
            }

            public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
            {
                return alarms.AddAlarm(this, 2);
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "InactiveDeedPost";
                }
                else
                {
                    return "InactiveDeeds";
                }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override int ReportDelay
            {
                get
                {
                    return RandomUtil.GetInt(120, 240);
                }
            }

            protected override int ContinueReportChance
            {
                get { return 25; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override bool Allow()
            {
                if (mTotalFunds == 0)
                {
                    IncStat("No Funds");
                    return false;
                }

                return base.Allow();
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (manager == null)
                {
                    manager = Money;
                }

                if (parameters == null)
                {
                    parameters = new object[] { Sim, mTotalFunds };
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, CollectDeedsScenario>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerMoney main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    Common.RemoveInteraction<RabbitHole.InvestInRabbithole.Definition>(hole);

                    hole.AddInteraction(InvestInRabbithole.BuyoutSingleton);
                    hole.AddInteraction(InvestInRabbithole.InvestSingleton);
                }

                foreach (Lot lot in LotManager.AllLots)
                {
                    Common.RemoveInteraction<PurchaseVenue.Definition>(lot);

                    lot.AddInteraction(PurchaseVenueEx.Singleton);
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "HandleInactiveDeeds";
            }
        }
    }
}
