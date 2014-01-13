using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class DebtSplitScenario : DebtMoveScenario
    {
        SimDescription mHead = null;

        List<SimDescription> mMovers = null;

        public DebtSplitScenario()
            : base ()
        { }
        protected DebtSplitScenario(DebtSplitScenario scenario)
            : base (scenario)
        {
            mHead = scenario.mHead;
            mMovers = scenario.mMovers;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "DebtSplit";
        }

        protected override int DangerRatio
        {
            get { return GetValue<RatioOption,int>(); }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Money.AdjustFunds(House, "PayDebt", -House.FamilyFunds);

            mHead = SimTypes.HeadOfFamily(House);
            mMovers = new List<SimDescription>(HouseholdsEx.All(House));

            Add(frame, new DebtSplitFamilyScenario(House), ScenarioResult.Start);
            Add(frame, new DebtForgivenScenario(House), ScenarioResult.Failure);
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Money;
            }

            if (parameters == null)
            {
                List<object> sims = new List<object>();
                foreach (SimDescription sim in mMovers)
                {
                    if (House.Contains(sim)) continue;

                    sims.Add(sim);
                }

                if (sims.Contains(mHead))
                {
                    sims.Remove(mHead);
                    sims.Insert(0, mHead);
                }

                parameters = sims.ToArray();
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new DebtSplitScenario(this);
        }

        public class RatioOption : IntegerScenarioOptionItem<ManagerMoney, DebtSplitScenario>, ManagerMoney.IDebtOption
        {
            public RatioOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "DebtSplitRatio";
            }
        }
    }
}
