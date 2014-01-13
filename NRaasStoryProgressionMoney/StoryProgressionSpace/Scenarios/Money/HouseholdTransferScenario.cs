using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
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
    public class HouseholdTransferScenario : HouseholdScenario
    {
        public HouseholdTransferScenario()
        { }
        protected HouseholdTransferScenario(HouseholdTransferScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "HouseTransfer";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int PushChance
        {
            get { return 20; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            SimDescription head = SimTypes.HeadOfFamily(house);
            if (head == null)
            {
                IncStat("No Head");
                return false;
            }
            else if (!Money.Allow(this, head))
            {
                IncStat("Money Denied");
                return false;
            }
            else if ((GetValue<TransferAmountOption, int>(house) == 0) && (GetValue<TransferPercentOption, int>(house) == 0))
            {
                IncStat("No Transfer");
                return false;
            }
            else if (GetValue<TransferHouseholdOption,ulong>(house) == house.HouseholdId)
            {
                IncStat("Same House");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household transfer = Household.Find(GetValue<TransferHouseholdOption, ulong>(House));
            if (transfer == null) return false;

            int percent = GetValue<TransferPercentOption, int>(House);

            int amount = 0;
            if (percent > 0)
            {
                int worth = GetValue<NetWorthOption,int>(House);
                if (worth > 0)
                {
                    amount = (int)(worth * (percent / 100f));
                }
            }
            else
            {
                int worth = GetValue<NetWorthOption,int>(transfer);
                if (worth > 0)
                {
                    amount = -(int)(worth * (percent / 100f));
                }
            }

            amount += GetValue<TransferAmountOption, int>(House);

            if (amount != 0)
            {
                Money.AdjustFunds(House, "HouseTransfer", -amount);

                Money.AdjustFunds(transfer, "HouseTransfer", amount);
            }

            return true;
        }

        protected override bool Push()
        {
            if (House.IsActive) return false;

            SimDescription head = SimTypes.HeadOfFamily(House);
            if (head == null)
            {
                return false;
            }
            else
            {
                Household transfer = Household.Find(GetValue<TransferHouseholdOption,ulong>(House));
                if (transfer.LotHome == null) return false;

                return Situations.PushVisit(this, head, transfer.LotHome);
            }
        }

        public override Scenario Clone()
        {
            return new HouseholdTransferScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, HouseholdTransferScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HouseholdTransfer";
            }
        }
    }
}
