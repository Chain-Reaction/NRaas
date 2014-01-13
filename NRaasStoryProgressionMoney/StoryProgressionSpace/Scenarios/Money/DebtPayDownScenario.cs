using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
    public class DebtPayDownScenario : HouseholdScenario
    {
        public DebtPayDownScenario()
            : base ()
        { }
        protected DebtPayDownScenario(DebtPayDownScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NoMoreDebt";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            if (house == Household.ActiveHousehold) return false;

            if (GetValue<DebtOption,int>(house) <= 0) return false;

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (SimTypes.IsSpecial(House))
            {
                SetValue<DebtOption,int>(House, 0);
                return false;
            }
            else if (House.LotHome == null)
            {
                SetValue<DebtOption,int>(House, 0);
                return true;
            }

            int debt = GetValue<DebtOption, int>(House);

            if (debt > 0)
            {
                if (House.FamilyFunds < debt)
                {
                    AddStat("Paid off", House.FamilyFunds);

                    SetValue<DebtOption,int>(House, debt -House.FamilyFunds);

                    Money.AdjustFunds(House, "PayDebt", -House.FamilyFunds);
                }
                else
                {
                    AddStat("Paid off", debt);

                    Money.AdjustFunds(House, "PayDebt", -debt);

                    SetValue<DebtOption, int>(House, 0);

                    return true;
                }
            }
            else if (debt < 0)
            {
                SetValue<DebtOption, int>(House, 0);
            }

            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (House.LotHome == null)
            {
                name = "HomelessDebtForgiven";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new DebtPayDownScenario(this);
        }

        public class ActiveOption : IntegerScenarioOptionItem<ManagerMoney, DebtPayDownScenario>, ManagerMoney.IDebtOption
        {
            public ActiveOption()
                : base(1000)
            { }

            public override string GetTitlePrefix()
            {
                return "ActiveDebtPaydown";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ActivePercentOption,int>() > 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ActivePercentOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public ActivePercentOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ActiveDebtPercentPaydown";
            }

            protected override int Validate(int value)
            {
                if (value < 0)
                {
                    value = 0;
                }

                return base.Validate(value);
            }
        }
    }
}
