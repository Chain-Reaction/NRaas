using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
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
    public class UnifiedBillingScenario : HouseholdScenario, IAlarmScenario
    {
        public UnifiedBillingScenario()
            : base ()
        { }
        protected UnifiedBillingScenario(UnifiedBillingScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UnifiedBilling";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 0.01f, GetValue<TaxDaysOption,DaysOfTheWeek>());
        }

        protected override bool Allow()
        {
            if (Common.IsOnTrueVacation()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            //if (!base.Allow(house)) return false;

            if (house != Household.ActiveHousehold)
            {
                if (!Money.ProgressionEnabled) return false;

                if (!GetValue<UnifiedBillingOption, bool>()) return false;

                if (!Money.Allow(this, SimTypes.HeadOfFamily(house))) return false;
            }

            return true;
        }

        protected static int ComputeNetWorthOfObjectsInHousehold(Household house, bool includeObjectsRemovedOnEvict)
        {
            int num = 0;

            foreach(Lot lot in ManagerLot.GetOwnedLots(house))
            {
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    if ((!(obj is Sim) && !(obj is PlumbBob)) && (includeObjectsRemovedOnEvict || obj.StaysAfterEvict()))
                    {
                        num += obj.Value;
                    }
                }
            }
            return num;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription head = SimTypes.HeadOfFamily(House);

            Household lotOwner = Money.GetDeedOwner(House.LotHome);
            if ((lotOwner != null) && (lotOwner != House))
            {
                if (Money.Allow(this, SimTypes.HeadOfFamily(lotOwner)))
                {
                    int rent = (int)(ManagerLot.GetUnfurnishedLotCost(House.LotHome, 0) * GetValue<RentMultipleOption, int>(head) / 100f);
                    if (rent != 0)
                    {
                        Money.AdjustFunds(House, "Rent", -rent);
                        Money.AdjustFunds(lotOwner, "Rent", rent);
                    }
                }
            }

            int owed = 0;

            bool shouldBill = Money.ShouldBillHouse(House);

            if (shouldBill)
            {
                if (House.LotHome == null) return false;

                if (SimTypes.IsSpecial(House)) return false;

                float standardTaxRate = GetValue<TaxRateOption,int>(head);
                if (!House.LotHome.IsApartmentLot)
                {
                    owed += (int)(ComputeNetWorthOfObjectsInHousehold(House, true) * standardTaxRate / 100f);
                }

                int valueOfAllVacationHomes = House.RealEstateManager.GetValueOfAllVacationHomes();
                owed += (int)Math.Round(valueOfAllVacationHomes * RealEstateManager.kPercentageOfVacationHomeValueBilled);

                Dictionary<int, List<float>> dictionary = new Dictionary<int, List<float>>();
                foreach (IReduceBills bills in House.LotHome.GetObjects<IReduceBills>())
                {
                    List<float> list;
                    int key = bills.ReductionArrayIndex();
                    float item = bills.PercentageReduction();
                    if (dictionary.TryGetValue(key, out list))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        List<float> list2 = new List<float>();
                        list2.Add((float)bills.MaxNumberContributions());
                        list2.Add(item);
                        dictionary.Add(key, list2);
                    }
                }

                foreach (KeyValuePair<int, List<float>> pair in dictionary)
                {
                    int num5 = (int)pair.Value[0];
                    pair.Value.RemoveAt(0);
                    pair.Value.Sort();
                    int count = pair.Value.Count;
                    num5 = Math.Min(num5, count);
                    float num7 = 0f;
                    for (int i = 1; i <= num5; i++)
                    {
                        num7 += pair.Value[count - i];
                    }
                    int amount = (int)(owed * num7);

                    try
                    {
                        House.MoneySaved(pair.Key, (uint)amount);
                    }
                    catch
                    {
                        if ((House.mMoneySaved == null) || (House.mMoneySaved.Length != 3))
                        {
                            House.mMoneySaved = new uint[3];
                        }
                    }

                    owed -= amount;
                }
            }

            bool debtRepayed = false;

            int loanPayment = 0;

            if ((House == Household.ActiveHousehold) && (GetValue<UnifiedBillingOption, bool>()))
            {
                int debt = GetValue<DebtOption, int>(House);

                if (debt > 0)
                {
                    int debtPaydown = 0;

                    float percent = GetValue<DebtPayDownScenario.ActivePercentOption,int>();
                    if (percent > 0)
                    {
                        debtPaydown = (int)(debt * (percent / 100f));
                        if (debtPaydown == 0)
                        {
                            debtPaydown = debt;
                        }
                    }
                    else
                    {
                        debtPaydown = GetValue<DebtPayDownScenario.ActiveOption, int>();
                    }

                    if (debt > debtPaydown)
                    {
                        AddStat("Paydown", debtPaydown);

                        loanPayment = debtPaydown;

                        owed += debtPaydown;

                        AddValue<DebtOption, int>(House, -debtPaydown);
                    }
                    else
                    {
                        AddStat("Paydown", debt);

                        loanPayment = debt;

                        owed += debt;

                        SetValue<DebtOption, int>(House, 0);

                        debtRepayed = true;
                    }
                }
            }

            AddStat("Owed", owed);

            if ((House != Household.ActiveHousehold) || ((GetValue<AutoActiveBillingOption, bool>()) && (Money.ProgressionEnabled)))
            {
                Money.AdjustFunds(House, "PayLoan", -loanPayment);

                Money.AdjustFunds(House, "Bills", -(owed - loanPayment));

                if ((debtRepayed) && (GetValue<DebtOption,int>(House) == 0))
                {
                    Stories.PrintStory(Households, "NoMoreDebt", House, null, ManagerStory.StoryLogging.Full);
                }
            }
            else
            {
                if (shouldBill)
                {
                    owed += (int)House.UnpaidBills;
                }

                if (owed != 0x0)
                {
                    if (Sims3.SimIFace.Environment.HasEditInGameModeSwitch)
                    {
                        AddValue<DebtOption, int>(House, owed);
                        House.UnpaidBills = 0;
                    }
                    else if (HouseholdsEx.NumSims(House) > 0x0)
                    {
                        Bill bill = GlobalFunctions.CreateObjectOutOfWorld("Bill") as Bill;
                        if (bill != null)
                        {
                            bill.Amount = (uint)owed;
                            House.UnpaidBills = 0;
                            Mailbox mailboxOnLot = Mailbox.GetMailboxOnLot(House.LotHome);
                            if (mailboxOnLot != null)
                            {
                                mailboxOnLot.AddMail(bill, false);
                                bill.OriginatingHousehold = House;
                                Mailbox.AddJunkMail(mailboxOnLot);
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new UnifiedBillingScenario(this);
        }

        public class AutoActiveBillingOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public AutoActiveBillingOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AutomaticActiveBilling";
            }
        }

        public class TaxDaysOption : DaysManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public TaxDaysOption()
                : base(ConvertFromList(Mailbox.kBillingDaysOfWeek))
            { }

            public override string GetTitlePrefix()
            {
                return "TaxDays";
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                Manager.GetOption<UnifiedBillingOption>().RestartAlarm();

                return true;
            }
        }

        public class UnifiedBillingOption : BooleanAlarmOptionItem<ManagerMoney, UnifiedBillingScenario>, ManagerMoney.IDebtOption
        {
            public UnifiedBillingOption()
                : base(true)
            { }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                if (Mailbox.mRegularHouseholdBillingAlarm != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(Mailbox.mRegularHouseholdBillingAlarm);
                    Mailbox.mRegularHouseholdBillingAlarm = AlarmHandle.kInvalidHandle;

                    Manager.IncStat("EA Billing Disabled");
                }

                base.PrivateUpdate(fullUpdate, initialPass);
            }

            public override string GetTitlePrefix()
            {
                return "UnifiedBilling";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
