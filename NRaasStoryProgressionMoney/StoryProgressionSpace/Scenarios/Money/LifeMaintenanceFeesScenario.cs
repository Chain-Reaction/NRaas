using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Locations;
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
    public class LifeMaintenanceFeesScenario : HouseholdScenario, IAlarmScenario
    {
        public LifeMaintenanceFeesScenario()
            : base ()
        { }
        protected LifeMaintenanceFeesScenario(LifeMaintenanceFeesScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "LifeMaintenanceFees";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 1);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!Money.Allow(this, SimTypes.HeadOfFamily(house)))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int fees = 0;

            if ((House != Household.ActiveHousehold) || (GetValue<ActiveFeeOption, bool>()))
            {
                fees = HouseholdsEx.NumSims(House) * GetValue<DailyInactivePerSimFeeOption, int>();
            }

            if (Common.IsOnTrueVacation())
            {
                fees += GetValue<VacationFeeOption, int>();
            }

            Money.AdjustFunds(House, "DailyExpenses", -fees);

            if ((GetValue<DebtOption, int>(House) - House.FamilyFunds) > GetValue<MinimumDebtOption, int>())
            {
                int elders = 0, children = 0;

                foreach (SimDescription sim in HouseholdsEx.Humans(House))
                {
                    if (sim.Elder)
                    {
                        elders++;
                    }
                    else if (sim.ChildOrBelow)
                    {
                        children++;
                    }
                }

                Money.AdjustFunds(House, "SocialSecurity", elders * GetValue<SocialSecurityOption, int>());

                Money.AdjustFunds(House, "Welfare", children * GetValue<WelfareOption, int>());
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new LifeMaintenanceFeesScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerMoney, LifeMaintenanceFeesScenario>, ManagerMoney.IFeesOption, IDebuggingOption
        {
            public Option()
                : base (true)
            { }

            public override string GetTitlePrefix()
            {
                return "LifeMaintenanceFees";
            }
        }

        public class VacationFeeOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public VacationFeeOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "VacationFee";
            }

            public override bool ShouldDisplay()
            {
                if (!GameStates.IsOnVacation) return false;

                return base.ShouldDisplay();
            }
        }

        public class MinimumDebtOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public MinimumDebtOption()
                : base(10000)
            { }

            public override string GetTitlePrefix()
            {
                return "MinimumDebtForWelfare";
            }

            public override bool ShouldDisplay()
            {
                if (GameStates.IsOnVacation) return false;

                return base.ShouldDisplay();
            }
        }

        public class WelfareOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public WelfareOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "Welfare";
            }

            public override bool ShouldDisplay()
            {
                if (GameStates.IsOnVacation) return false;

                return base.ShouldDisplay();
            }
        }

        public class SocialSecurityOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public SocialSecurityOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "SocialSecurity";
            }

            public override bool ShouldDisplay()
            {
                if (GameStates.IsOnVacation) return false;

                return base.ShouldDisplay();
            }
        }

        public class DailyInactivePerSimFeeOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public DailyInactivePerSimFeeOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "DailyInactivePerSimFee";
            }
        }

        public class ActiveFeeOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public ActiveFeeOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ActiveDailyFee";
            }
        }
    }
}
