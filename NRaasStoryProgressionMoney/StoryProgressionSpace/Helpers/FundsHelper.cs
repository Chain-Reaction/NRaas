using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class FundsHelper : Common.IPreLoad
    {
        static Tracer sTracer = new Tracer();

        public void OnPreLoad()
        {
            ManagerMoney.OnFamilyFundsChanged += OnFundChanged;
        }

        public void OnFundChanged(ManagerMoney manager, FamilyFundsChangedEvent e, string key)
        {
            sTracer.Perform(manager, e.Actor as Sim, (int)e.Delta, key);

            if (sTracer.mReservingSimDisposed)
            {
                Household house = e.Actor.Household;
                if (house != null)
                {
                    house.mFamilyFunds -= (int)e.Delta;
                }
            }
            else if (manager.DebuggingEnabled)
            {
                if (!sTracer.mIgnore)
                {
                    Common.DebugException(e.Actor, null, new Common.StringBuilder(sTracer.ToString()), new Exception());
                }
            }
        }

        public class Tracer : StackTracer
        {
            ManagerMoney mManager;

            Sim mSim;

            string mKey;

            int mDelta;

            public bool mReservingSimDisposed;

            public bool mIgnore;

            public Tracer()
            {
                AddTest(typeof(Sims3.Gameplay.Objects.Vehicles.Bicycle), "Void OnReservingSimDisposed", OnReservingSimDisposed);
                AddTest(typeof(Sims3.Gameplay.Objects.Vehicles.CarOwnable), "Void OnReservingSimDisposed", OnReservingSimDisposed);

                AddTest(typeof(Sims3.Gameplay.UI.LiveDragHelperModel), "Void SellObject", OnGeneral);
                AddTest(typeof(Sims3.Gameplay.Abstracts.GameObject), "Void DepreciateNewPurchasesAndSendBoughtEvent", OnGeneral);
                AddTest(typeof(Sims3.Gameplay.Moving.GameplayMovingModel), "Void Apply", OnGeneral);
                
                // Handled by calling routine
                AddTest("NRaas.StoryProgressionSpace.Helpers.RentalHelper", "Void PurchaseRentalLot", OnGeneral);
                AddTest("NRaas.StoryProgressionSpace.Helpers.RentalHelper", "Void SellRentalLot", OnGeneral);

                AddTest(typeof(Sims3.Gameplay.Careers.Occupation), "Void RetirementAlarmCallback", OnPension);

                AddTest(typeof(Sims3.Gameplay.RealEstate.PropertyData), "Void CollectedMoney", OnInvestments);
                AddTest(typeof(Sims3.Gameplay.RealEstate.RealEstateManager), "IPropertyData PurchaseVacationHome", OnProperty);
                AddTest(typeof(Sims3.Gameplay.RealEstate.RealEstateManager), "IPropertyData PurchasePrivateLot", OnProperty);
                AddTest(typeof(Sims3.Gameplay.RealEstate.RealEstateManager), "Void SellProperty", OnProperty);
                AddTest(typeof(Sims3.Gameplay.EditTownModel), "Void EvictHousehold", OnProperty);

                AddTest(typeof(Sims3.Gameplay.Objects.RabbitHoles.Restaurant), "Void ChargeBill", OnService);
                AddTest(typeof(Sims3.Gameplay.Abstracts.RabbitHole.AttendClassInRabbitHole), "Boolean InRabbitHole", OnTraining);

                AddTest(typeof(Sims3.Gameplay.Careers.XpBasedCareer), "Void GenerateRewardsAndLevelUpTNS", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Careers.Career), "Void FinishWorking", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Careers.Career), "Int32 GivePromotionBonus", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Careers.Occupation), "Void PayPto", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Careers.ProSports), "Void GameDayEndAlarmHandler", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Objects.RabbitHoles.PoliceStation.LowLevelPoliceWork), "Boolean InRabbitHole", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Careers.Criminal.DoASideJob), "Void OnTimePassed", OnCareerPay);
                AddTest("NRaas.RegisterSpace.Tasks.RoleManagerTaskEx", "Void SimulateRoles", OnCareerPay);
                AddTest("NRaas.CareerSpace.SelfEmployment.Repairman", "Void GetPaid", OnCareerPay);
                AddTest(typeof(Sims3.Gameplay.Objects.Electronics.Computer.WorkFromHome), "Void FinishWorking", OnCareerPay);
                AddTest("NRaas.CareerSpace.Careers.Homemaker", "Void StipendAlarmCallback", OnCareerPay);
                
                AddTest(typeof(Sims3.Gameplay.Skills.Writing), "Void PartialWorkPayment", OnWriting);
                AddTest(typeof(Sims3.Gameplay.Skills.Writing.RoyaltyAlarm), "Void AlarmCallBack", OnWriting);
                AddTest(typeof(Sims3.Gameplay.Objects.Electronics.Computer), "Void FinalizeWriting", OnWriting);
                
                AddTest(typeof(Sims3.Gameplay.CelebritySystem.SueForSlander), "Boolean InRabbitHole", OnLawsuit);
                AddTest(typeof(Sims3.Gameplay.Controllers.FireManager), "Void UnregisterFire", OnInsurance);
                AddTest(typeof(Sims3.Gameplay.Objects.Electronics.Computer.Hack), "Int32 GetPaid", OnGeneral);
                AddTest(typeof(Sims3.Gameplay.Objects.Appliances.BaristaBar.BuyDrinkFromBar), "Boolean Run", OnGeneral);
                
                AddTest(typeof(Sims3.Gameplay.Skills.BandSkill), "Void MadeTips", OnTips);
                
                AddTest(typeof(Sims3.Gameplay.Socializing.SocialCallback), "Void AfterMoochMoney", OnMooching);
                AddTest(typeof(Sims3.Gameplay.Socializing.SocialCallback), "Void GiveFlowersBeforeCallback", OnDonation);

                AddTest("NRaas.CareerSpace.Interactions.Shakedown", "Void OnAccepted", OnTheft);

                AddTest("NRaas.MasterControllerSpace.Households.FamilyFunds", "OptionResult Run", OnCheating);

                AddTest("NRaas.CareerSpace.SelfEmployment.Trainer", "Void OnTrained", OnTraining);

                AddTest(typeof(Sims3.Gameplay.Objects.FoodObjects.Recipe), "Boolean UseUpIngredientsForOneServing", OnGroceries);
                AddTest(typeof(Sims3.Gameplay.Abstracts.ShoppingRabbitHole), "Boolean DoSharedItemPurchaseCode", OnGroceries);
                AddTest(typeof(Sims3.Gameplay.Objects.Vehicles.IceCreamTruck.OrderIceCream), "Boolean Run", OnGroceries);

                AddTest(typeof(Sims3.Gameplay.Abstracts.ShowVenue.AttendShow), "Boolean InRabbitHole", OnServices);

                AddTest("NRaas.MoverSpace.Helpers.GameplayMovingModelEx", "Void Apply", OnGeneral);
                AddTest("NRaas.TravelerSpace.Interactions.SitInCarToTriggerTravelEx", "Boolean Run", OnGeneral);
            }

            public bool Perform(ManagerMoney manager, Sim sim, int delta, string key)
            {
                mManager = manager;
                mSim = sim;
                mDelta = delta;
                mKey = key;

                return Perform();
            }

            public override void Reset()
            {
                mReservingSimDisposed = false;
                mIgnore = false;

                base.Reset();
            }

            private bool OnReservingSimDisposed(StackTrace trace, StackFrame frame)
            {
                mReservingSimDisposed = true;

                mManager.IncStat("Funds ReservingSimDisposed");
                return true;
            }

            private bool OnGeneral(StackTrace trace, StackFrame frame)
            {
                mManager.IncStat("Funds Handled");

                mIgnore = true;
                return true;
            }

            private bool OnServices(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Services", "Services", false);
                return true;
            }

            private bool OnGroceries(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Groceries", "Groceries", false);
                return true;
            }

            private bool OnDailyExpenses(StackTrace trace, StackFrame frame)
            {
                AlterFunds("DailyExpenses", "DailyExpenses", false);
                return true;
            }

            private bool OnDonation(StackTrace trace, StackFrame frame)
            {
                AlterFunds("GiveAway", "GiveAway", false);
                return true;
            }

            private bool OnService(StackTrace trace, StackFrame frame)
            {
                AlterFunds("ServiceFees", "ServiceFees", true);
                return true;
            }

            private bool OnPension(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Pension", "Pension", false);
                return true;
            }

            private bool OnInvestments(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Investments", "Investments", true);
                return true;
            }

            private bool OnCheating(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Cheating", "Cheating", false);
                return true;
            }

            private bool OnTheft(StackTrace trace, StackFrame frame)
            {
                AlterFunds("TakeCash", "TakeCash", false);
                return true;
            }

            private bool OnProperty(StackTrace trace, StackFrame frame)
            {
                AlterFunds("PropertySold", "PropertyBought", false);
                return true;
            }

            private bool OnTraining(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Training", "Training", false);
                return true;
            }

            private bool OnCareerPay(StackTrace trace, StackFrame frame)
            {
                AlterFunds("CareerPay", "Services", true);
                return true;
            }

            private bool OnWriting(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Royalties", "Royalties", true);
                return true;
            }

            private bool OnLawsuit(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Lawsuit", "Lawsuit", true);
                return true;
            }

            private bool OnInsurance(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Insurance", "Insurance", false);
                return true;
            }

            private bool OnTips(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Tips", "Tips", false);
                return true;
            }

            private bool OnMooching(StackTrace trace, StackFrame frame)
            {
                AlterFunds("Mooch", "Mooch", false);
                return true;
            }

            private void AlterFunds(string incomeKey, string expenseKey, bool applyTax)
            {
                AlterFundsTask.Perform(this, incomeKey, expenseKey, applyTax);

                mIgnore = true;
            }

            public override string ToString()
            {
                string result = "Sim: " + mSim.FullName;
                result += Common.NewLine + "Key: " + mKey;
                result += Common.NewLine + "Delta: " + mDelta;
                result += Common.NewLine + "ReservingSimDisposed: " + mReservingSimDisposed;
                result += Common.NewLine + "Ignore: " + mIgnore;                
                result += Common.NewLine + base.ToString();

                return result;
            }

            public class AlterFundsTask : Common.FunctionTask
            {
                Tracer mTracer;

                string mIncomeKey;
                string mExpenseKey;
                bool mApplyTax;

                protected AlterFundsTask(Tracer tracer, string incomeKey, string expenseKey, bool applyTax)
                {
                    mTracer = tracer;
                    mIncomeKey = incomeKey;
                    mExpenseKey = expenseKey;
                    mApplyTax = applyTax;
                }

                public static void Perform(Tracer tracer, string incomeKey, string expenseKey, bool applyTax)
                {
                    new AlterFundsTask(tracer, incomeKey, expenseKey, applyTax).AddToSimulator();
                }

                protected override void OnPerform()
                {
                    try
                    {
                        if (mTracer.mDelta > 0)
                        {
                            if (mTracer.mKey == null)
                            {
                                mTracer.mManager.IncStat("Funds " + mIncomeKey);

                                mTracer.mManager.AdjustAccounting(mTracer.mSim.Household, "Income", -mTracer.mDelta);
                                mTracer.mManager.AdjustAccounting(mTracer.mSim.Household, mIncomeKey, mTracer.mDelta);
                            }

                            if (mApplyTax)
                            {
                                float incomeTax = mTracer.mManager.GetValue<IncomeTaxOption, int>(mTracer.mSim);
                                if (incomeTax > 0)
                                {
                                    int tax = (int)(mTracer.mDelta * (incomeTax / 100f));
                                    if (tax > 0)
                                    {
                                        mTracer.mManager.AdjustFunds(mTracer.mSim.SimDescription, "IncomeTax", -tax);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (mTracer.mKey == null)
                            {
                                mTracer.mManager.IncStat("Funds " + mExpenseKey);

                                mTracer.mManager.AdjustAccounting(mTracer.mSim.Household, "Expense", -mTracer.mDelta);
                                mTracer.mManager.AdjustAccounting(mTracer.mSim.Household, mExpenseKey, mTracer.mDelta);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(mTracer.ToString(), e);
                    }
                }
            }
        }
    }
}
