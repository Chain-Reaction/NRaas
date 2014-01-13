using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class AutoCleanScenario : ScheduledSoloScenario, IAlarmScenario
    {
        public AutoCleanScenario()
        { }
        protected AutoCleanScenario(AutoCleanScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "AutoClean";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDelayed(this, GetValue<OptionV2, int>(), TimeUnit.Minutes);
        }

        protected override bool Allow()
        {
            if (GetValue<OptionV2,int>() <= 0) return false;

            if (Household.ActiveHousehold == null)
            {
                IncStat("No Active");
                return false;
            }
            else if (Household.ActiveHousehold.LotHome == null)
            {
                IncStat("No Active Lot");
                return false;
            }

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household house = Household.ActiveHousehold;

            Lot lot = house.LotHome;

            foreach (IThrowAwayable awayable in Sims3.Gameplay.Queries.GetObjects<IThrowAwayable>(lot))
            {
                if (awayable == null) continue;

                if (awayable.InUse) continue;

                if (!awayable.HandToolAllowUserPickupBase()) continue;

                if (!awayable.ShouldBeThrownAway()) continue;

                if ((awayable.Parent != null) && (awayable.Parent.InUse)) continue;

                if (awayable is Bar.Glass) continue;

                if (awayable is Bill) continue;

                bool flag = false;
                if (awayable is BarTray)
                {
                    foreach (Slot slot in awayable.GetContainmentSlots())
                    {
                        if (awayable.GetContainedObject(slot) is Bar.Glass)
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                if (!flag)
                {
                    awayable.ThrowAwayImmediately();
                }
            }

            foreach (IDestroyOnMagicalCleanup destroyable in Sims3.Gameplay.Queries.GetObjects<IDestroyOnMagicalCleanup>(lot))
            {
                GameObject obj = destroyable as GameObject;
                if (obj == null) continue;

                obj.FadeOut(false, true);
            }

            List<Fridge> fridges = new List<Fridge>(lot.GetObjects<Fridge>());
            if (fridges.Count > 0)
            {
                Fridge fridge = fridges[0];
                if ((fridge != null) &&
                    (house.SharedFridgeInventory != null) &&
                    (house.SharedFamilyInventory.Inventory != null))
                {
                    foreach (ServingContainer container in Sims3.Gameplay.Queries.GetObjects<ServingContainer>(lot))
                    {
                        if ((!container.InUse) &&
                            (fridge.HandToolAllowDragDrop(container)) &&
                            (container.HasFood && container.HasFoodLeft()) &&
                            (!container.IsSpoiled &&
                            (container.GetQuality() >= Quality.Neutral)))
                        {
                            house.SharedFridgeInventory.Inventory.TryToAdd(container, false);
                        }
                    }
                }
            }

            foreach (Sim sim in HouseholdsEx.AllSims(house))
            {
                if (!Inventories.VerifyInventory(sim.SimDescription)) continue;

                foreach (IThrowAwayable awayable2 in Inventories.QuickDuoFind<IThrowAwayable,GameObject>(sim.Inventory))
                {
                    if (awayable2 == null) continue;

                    if (awayable2.InUse) continue;

                    if (!awayable2.HandToolAllowUserPickupBase()) continue;

                    if (!awayable2.ShouldBeThrownAway()) continue;

                    if (awayable2.InUse) continue;

                    if ((awayable2 is Newspaper) && !(awayable2 as Newspaper).IsOld) continue;

                    if (awayable2 is TrashPileOpportunity) continue;

                    if ((awayable2 is PreparedFood) && !(awayable2 as PreparedFood).IsSpoiled) continue;

                    awayable2.ThrowAwayImmediately();
                }
            }

            LotLocation[] puddles = World.GetPuddles(lot.LotId, LotLocation.Invalid);
            if (puddles.Length > 0x0)
            {
                foreach (LotLocation location in puddles)
                {
                    if ((lot.TombRoomManager == null) || !lot.TombRoomManager.IsObjectInATombRoom(location))
                    {
                        PuddleManager.RemovePuddle(lot.LotId, location);
                    }
                }
            }

            try
            {
                List<Bill> allBills = new List<Bill>();
                List<Bill> allBillsInMailboxes = new List<Bill>();

                uint num = Bill.GetTotalAmountForAllBillsInHousehold(HouseholdsEx.AllSims(lot.Household)[0], allBills, allBillsInMailboxes);
                if (house.FamilyFunds >= num)
                {
                    foreach (Bill bill in allBills)
                    {
                        if (!bill.InUse)
                        {
                            Money.AdjustFunds(house, "Bills", -(int)bill.Amount);

                            bill.DestroyBill(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.DebugException(lot, e);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new AutoCleanScenario(this);
        }

        public class OptionV2 : IntegerManagerOptionItem<ManagerLot>
        {
            private AlarmManagerReference mAutoCleanAlarm = null;

            public OptionV2()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "AutomaticCleaning";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
            {
                ToggleAutoClean(Value);

                base.PrivateUpdate(fullUpdate, initialPass);
            }

            public override void SetValue(int value, bool persist)
            {
                base.SetValue(value, persist);

                ToggleAutoClean(value);
            }

            protected void ToggleAutoClean(int interval)
            {
                if (interval > 0)
                {
                    if ((mAutoCleanAlarm == null) && (Manager != null))
                    {
                        mAutoCleanAlarm = Manager.AddAlarm(new AutoCleanScenario());
                    }
                }
                else
                {
                    if (mAutoCleanAlarm != null)
                    {
                        mAutoCleanAlarm.Dispose();
                        mAutoCleanAlarm = null;
                    }
                }
            }
        }
    }
}
