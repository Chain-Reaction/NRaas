using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
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
    public abstract class MoveInLotScenario : Scenario
    {
        Household mHouse = null;

        ICollection<SimDescription> mMovers = null;

        HouseholdBreakdown mBreakdown = null;

        bool mNewHouse = false;

        protected MoveInLotScenario(HouseholdBreakdown breakdown)
        {
            mBreakdown = breakdown;
            mMovers = new List<SimDescription>(mBreakdown.Going);
        }
        protected MoveInLotScenario(ICollection<SimDescription> movers)
        {
            mMovers = movers;
        }
        protected MoveInLotScenario(MoveInLotScenario scenario)
            : base (scenario)
        {
            mMovers = scenario.mMovers;
            mBreakdown = scenario.mBreakdown;
            mHouse = scenario.mHouse;
            mNewHouse = scenario.mNewHouse;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected virtual ManagerLot.FindLotFlags Inspect
        {
            get { return ManagerLot.FindLotFlags.Inspect | ManagerLot.FindLotFlags.InspectPets; }
        }

        protected abstract bool CheapestHome
        {
            get;
        }

        protected virtual int MaximumLoan
        {
            get { return 0; }
        }

        protected virtual bool TestDead
        {
            get { return true; }
        }

        private Household House
        {
            get { return mHouse; }
            set { mHouse = value; }
        }

        public ICollection<SimDescription> Movers
        {
            get { return mMovers; }
        }

        protected int NewHomeStarterPercent
        {
            get { return GetValue<NewHomeStarterPercentOption,int>(); }
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            return GatherResult.Update;
        }

        protected override bool Allow()
        {
            if (!base.Allow()) return false;

            if ((mMovers == null) || (mMovers.Count == 0))
            {
                IncStat("No Movers");
                return false;
            }
            else if (mBreakdown != null)
            {
                if ((mBreakdown.NoneStaying) && (mBreakdown.SimLot != null))
                {
                    int humans = 0, pets = 0, plumbots = 0;
                    AddStat("Going", mBreakdown.GetGoingCount(ref humans, ref pets, ref plumbots));
                    AddStat("Going Humans", humans);
                    AddStat("Going Pets", pets);
                    AddStat("Going Plumbots", plumbots);
                    return false;
                }
                else if (!mBreakdown.SimGoing)
                {
                    IncStat("Not Both");
                    return false;
                }
            }
            
            if (!TestMoveInLot())
            {
                return false;
            }

            return true;
        }

        protected override bool UsesSim(ulong sim)
        {
            if (mMovers != null)
            {
                foreach (SimDescription mover in mMovers)
                {
                    if (mover.SimDescriptionId == sim) return true;
                }
            }

            return false;
        }

        protected bool TestMoveInLot()
        {
            bool nonRoomie = false;

            bool hasAdult = false;
            foreach (SimDescription sim in mMovers)
            {
                if (!Households.Allow(this, sim, GetValue<ManagerHousehold.MinTimeBetweenMovesOption, int>()))
                {
                    IncStat("User Denied");
                    return false;
                }
                else if ((TestDead) && (SimTypes.IsDead (sim)))
                {
                    IncStat("Dead");
                    return false;
                }
                else if (!sim.Marryable)
                {
                    IncStat("Not Marryable");
                    return false;
                }
                else if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
                {
                    IncStat("Reaper Denied");
                    return false;
                }

                if (!Household.RoommateManager.IsNPCRoommate(sim))
                {
                    nonRoomie = true;
                }

                if (Households.AllowGuardian(sim))
                {
                    hasAdult = true;
                }

                Lots.PackupVehicles(sim.CreatedSim, true);
            }

            if (!nonRoomie)
            {
                IncStat("All Roomies");
                return false;
            }

            if (!hasAdult)
            {
                IncStat("No adult");
                return false;
            }

            return true;
        }

        protected void PayForMoveInLot(Dictionary<Household, bool> oldHouses, int lotCost)
        {
            if (lotCost <= House.FamilyFunds)
            {
                AddStat("Self Purchase", lotCost);

                Money.AdjustFunds(House, "BuyLot", -lotCost);

                lotCost = 0;
            }
            else
            {
                lotCost -= House.FamilyFunds;

                AddStat("Remainder", lotCost);

                Money.AdjustFunds(House, "BuyLot", -House.FamilyFunds);
            }

            int maximumLoan = MaximumLoan;

            int oldHouseFunds = 0;
            foreach (Household oldHouse in oldHouses.Keys)
            {
                oldHouseFunds += oldHouse.FamilyFunds;
            }

            int totalFunds = maximumLoan + oldHouseFunds;

            if (lotCost > totalFunds)
            {
                Money.AdjustFunds(House, "BuyLot", totalFunds - lotCost);

                lotCost = totalFunds;

                IncStat("Miscalculated Lot Cost");
            }
            else
            {
                int dowry = (int)((totalFunds - lotCost) * (NewHomeStarterPercent / 100f));

                dowry -= House.FamilyFunds;
                if (dowry > 0)
                {
                    AddStat("Dowry", dowry);

                    Money.AdjustFunds(House, "Dowry", dowry);

                    lotCost += dowry;
                }

                if (maximumLoan > lotCost)
                {
                    maximumLoan = lotCost;
                }

                if (maximumLoan > 0)
                {
                    if ((GetValue<NetWorthOption, int>(House) / 2) < maximumLoan)
                    {
                        maximumLoan /= 2;

                        AddStat("Loan Reduced", GetValue<NetWorthOption, int>(House));
                    }

                    Money.AdjustFunds(House, "BuyLot", -maximumLoan);

                    lotCost -= maximumLoan;

                    AddStat("Loan", maximumLoan);
                }
            }

            if (lotCost > 0)
            {
                AddStat("Final Cost", lotCost);

                if (oldHouseFunds > lotCost)
                {
                    foreach (Household oldHouse in oldHouses.Keys)
                    {
                        int part = -lotCost * (oldHouse.FamilyFunds / oldHouseFunds);

                        Money.AdjustFunds(oldHouse, "BuyLot", part);

                        AddScoring("Old Home Final (A)", part);
                    }
                }
                else
                {
                    foreach (Household oldHouse in oldHouses.Keys)
                    {
                        int part = -lotCost / oldHouses.Count;

                        Money.AdjustFunds(oldHouse, "BuyLot", part);

                        AddScoring("Old Home Final (B)", part);
                    }
                }
            }
        }

        protected void AdjustFundsMoveInLot(SimDescription sim, Dictionary<Household, bool> oldHouses)
        {
            if (!SimTypes.IsSpecial(sim))
            {
                if (!oldHouses.ContainsKey(sim.Household))
                {
                    oldHouses.Add(sim.Household, true);

                    AddStat("Old Home Funds", sim.FamilyFunds);
                }

                if (HouseholdsEx.NumHumans(sim.Household) > 0)
                {
                    int funds = sim.Household.FamilyFunds / HouseholdsEx.NumHumans(sim.Household);

                    AddStat("Transfer", funds);

                    Money.AdjustFunds(House, "MoveIn", funds);

                    Money.AdjustFunds(sim, "MoveOut", -funds);

                    AddStat("Transfer Remainder", sim.FamilyFunds);

                    if (HouseholdsEx.NumHumans(sim.Household) == 1)
                    {
                        Lots.PackupVehicles(sim.CreatedSim, false);

                        int debt = GetValue<DebtOption, int>(sim.Household);

                        AddValue<DebtOption, int>(House, debt);

                        SetValue<DebtOption, int>(sim.Household, 0);

                        AddStat("Debt Conversion", debt);

                        funds = Households.Assets(sim);

                        Money.AdjustFunds(House, "SellLot", funds);

                        AddStat("Total Conversion", funds);
                    }
                }
            }
        }

        protected bool DuoSameHouse(SimDescription a, SimDescription b)
        {
            if (Flirts.IsCloselyRelated(a, b)) return true;

            return (!mNewHouse);
        }

        protected abstract ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds);

        public delegate void OnPresetLotHomeFunc(Lot lot, Household house);

        public static event OnPresetLotHomeFunc OnPresetLotHome;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ManagerLot.FindLotFlags flags = Inspect;
            if (CheapestHome)
            {
                flags |= ManagerLot.FindLotFlags.CheapestHome;
            }

            Lot lot = Lots.FindLot(this, mMovers, MaximumLoan, flags, OnLotPriceCheck);
            if (lot == null)
            {
                IncStat("No Lot");
                return false;
            }
            else if (lot.Household != null)
            {
                IncStat("Occupied");
                return false;
            }

            int lotCost = Lots.GetLotCost(lot);
            AddStat("Lot Cost", lotCost);

            List<SimDescription> sims = new List<SimDescription>(mMovers);

            Dictionary<Household, bool> oldHouses = new Dictionary<Household, bool>();

            House = null;

            foreach (SimDescription sim in sims)
            {
                if (House == null)
                {
                    if (SimTypes.IsSpecial(sim))
                    {
                        break;
                    }
                    else if (HouseholdsEx.NumSims(sim.Household) == sims.Count)
                    {
                        House = sim.Household;
                    }
                }
                else if (House != sim.Household)
                {
                    House = null;
                    break;
                }
            }

            int newFunds = 0;

            Lot oldLot = null;

            mNewHouse = false;
            if (House == null)
            {
                House = Household.Create();
                House.ModifyFamilyFunds(-House.FamilyFunds);

                House.SetName(sims[0].LastName);

                SetValue<AcountingOption, AccountingData>(House, new AccountingData());
                
                mNewHouse = true;
            }
            else if (House.LotHome != null)
            {
                oldLot = House.LotHome;

                newFunds = Lots.GetLotCost(House.LotHome);

                Lots.ProcessAbandonLot(oldLot);

                House.MoveOut();
            }

            if (OnPresetLotHome != null)
            {
                OnPresetLotHome(lot, House);
            }
            lot.MoveIn(House);

            ManagerSim.ForceRecount();

            Money.AdjustFunds(House, "SellLot", newFunds);

            AddStat("New Home Funds", House.FamilyFunds);

            SetValue<InspectedOption, bool>(House, false);

            if (mNewHouse)
            {
                foreach (SimDescription sim in sims)
                {
                    if (sim.Household != null)
                    {
                        AdjustFundsMoveInLot(sim, oldHouses);
                    }

                    Households.MoveSim(sim, House);

                    if (sim.IsMale)
                    {
                        House.Name = sim.LastName;
                    }
                }
            }

            if (OnLotPriceCheck(this, lot, newFunds, newFunds) != ManagerLot.CheckResult.IgnoreCost)
            {
                PayForMoveInLot(oldHouses, lotCost);
            }

            AddStat("Remaining Funds", House.FamilyFunds);

            foreach (SimDescription sim in sims)
            {
                Sims.Instantiate(sim, lot, true);
            }

            EventTracker.SendEvent(new HouseholdUpdateEvent(EventTypeId.kFamilyMovedInToNewHouse, House));

            if ((oldLot != null) && (GetValue<NotifyOnMoveOption, bool>(House)))
            {
                if (AcceptCancelDialog.Show(Common.Localize("NotifyOnMove:Prompt", false, new object[] { oldLot.Name, oldLot.Address })))
                {
                    if (CameraController.IsMapViewModeEnabled())
                    {
                        Sims3.Gameplay.Core.Camera.ToggleMapView();
                    }

                    Camera.FocusOnLot(oldLot.LotId, 0f);
                }
            }

            SetValue<NotifyOnMoveOption, bool>(House, false);

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (name == null) return null;

            if (manager == null)
            {
                manager = Lots;
            }

            if ((House != null) && (parameters == null))
            {
                List<SimDescription> houseSims = new List<SimDescription>(HouseholdsEx.Humans(House));

                for (int i = houseSims.Count - 1; i >= 0; i--)
                {
                    if (houseSims[i].ChildOrAbove) continue;

                    houseSims.RemoveAt(i);
                }

                if (houseSims.Count == 1)
                {
                    name += "Single";
                    parameters = new object[] { houseSims[0], House.LotHome };
                }
                else if (houseSims.Count == 2)
                {
                    if (DuoSameHouse(houseSims[0], houseSims[1]))
                    {
                        name += "DuoSameHome";
                    }
                    else
                    {
                        name += "Duo";
                    }
                    parameters = new object[] { houseSims[0], houseSims[1], House.LotHome };
                }
                else
                {
                    SimDescription head = SimTypes.HeadOfFamily(House);

                    string names = ManagerStory.CreateNameString(houseSims, head).ToString();

                    parameters = new object[] { head, names, House.LotHome };
                }
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public class NewHomeStarterPercentOption : IntegerManagerOptionItem<ManagerLot>
        {
            public NewHomeStarterPercentOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "MoveInStartingFundPercent";
            }

            protected override bool PrivatePerform()
            {
                if (!base.PrivatePerform()) return false;

                if (Value > 100)
                {
                    SetValue (100);
                }
                else if (Value < 0)
                {
                    SetValue (0);
                }

                return true;
            }
        }
    }
}
