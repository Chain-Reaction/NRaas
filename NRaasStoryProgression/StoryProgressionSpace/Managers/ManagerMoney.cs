using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Objects.Register;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerMoney : Manager
    {
        Dictionary<Household,string> mAccountKeys = new Dictionary<Household,string>();

        bool mAllowConsignment = false;

        public ManagerMoney(Main manager)
            : base(manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Money";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerMoney>(this).Perform(initial);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (SimTypes.IsSpecial(sim))
            {
                stats.IncStat("Allow: Special");
                return false;
            }

            if (Household.RoommateManager != null)
            {
                if (Household.RoommateManager.IsNPCRoommate(sim))
                {
                    stats.IncStat("Allow: Roomie");
                    return false;
                }
            }

            if (((check & AllowCheck.Scoring) != AllowCheck.Scoring) && !settings.GetValue<AllowMoneyOption, bool>())
            {
                stats.IncStat("Allow: Money Denied");
                return false;
            }

            return true;
        }

        public static event OnAllow OnAllowRent;

        public bool AllowRent(IScoringGenerator stats, SimDescription sim)
        {
            if (OnAllowRent == null) return true;

            return OnAllowRent(stats, GetData(sim), Managers.Manager.AllowCheck.None);
        }

        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        // this was done because the scoring causes the stack depth to be exceeded when a value in the PrivateAllow
        // function calls SimData so I check the value in the scoring instead        
        public bool Allow(IScoringGenerator stats, SimDescription sim, bool fromScoring)
        {
            AllowCheck check = AllowCheck.None;
            check &= AllowCheck.Scoring;
            return PrivateAllow(stats, sim, check);
        }
        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (Household.RoommateManager.IsNPCRoommate(sim))
            {
                IncStat("Roommate");
                return false;
            }

            check &= ~AllowCheck.Active;

            return base.PrivateAllow(stats, sim, check);
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            mAccountKeys.Clear();

            AddListener(EventTypeId.kFamilyFundsChanged, OnFundsChanged);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((fullUpdate) || (initialPass))
            {
                mAllowConsignment = (Sims3.Gameplay.Queries.CountObjects<ConsignmentRegister>() > 0);
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public List<SimDescription> GetDeedOwner(RabbitHole hole)
        {
            List<SimDescription> owners = new List<SimDescription>();

            if (hole != null)
            {
                foreach (Household house in Household.GetHouseholdsLivingInWorld())
                {
                    if (house.RealEstateManager == null) continue;

                    PropertyData data = house.RealEstateManager.FindProperty(hole);
                    if (data == null) continue;

                    foreach (SimDescription sim in HouseholdsEx.Humans(house))
                    {
                        if (Households.AllowGuardian(sim))
                        {
                            owners.Add(sim);
                        }
                    }
                }
            }

            return owners;
        }
        public Household GetDeedOwner(Lot lot)
        {
            if (lot == null) return null;

            foreach (Household house in Household.GetHouseholdsLivingInWorld())
            {
                if (house.RealEstateManager == null) continue;

                PropertyData data = house.RealEstateManager.FindProperty(lot);
                if (data == null) continue;

                return house;
            }

            return null;
        }

        public static void TransferProperty(Household src, Household dst, PropertyData deed)
        {
            src.RealEstateManager.mAllProperties.Remove(deed);

            PropertyData.RabbitHole rabbithole = deed as PropertyData.RabbitHole;
            if (rabbithole != null)
            {
                foreach (PropertyData existing in dst.RealEstateManager.AllProperties)
                {
                    PropertyData.RabbitHole existingHole = existing as PropertyData.RabbitHole;
                    if (existingHole == null) continue;

                    if (existingHole.GetRabbitHole() == rabbithole.GetRabbitHole())
                    {
                        existingHole.mCurrentCollectibleFunds += rabbithole.TotalValue;
                        return;
                    }
                }
            }

            dst.RealEstateManager.mAllProperties.Add(deed);

            deed.mOwner = dst.RealEstateManager;
        }

        public void AdjustAccounting(Household house, string key, int delta)
        {
            if (delta == 0) return;

            AccountingData data = GetValue<AcountingOption, AccountingData>(house);
            if (data == null)
            {
                data = new AccountingData();
            }

            if (Common.kDebugging)
            {
                if (GetValue<AccountingKeyOption,bool>())
                {
                    Common.DebugStackLog("Household: " + house.Name + Common.NewLine + "Key: " + key + Common.NewLine + "Delta: " + delta + Common.NewLine);
                }
            }

            data.Add(key, delta);

            SetValue<AcountingOption, AccountingData>(house, data);
        }

        public delegate void OnFundsChangedFunc(ManagerMoney manager, FamilyFundsChangedEvent e, string key);

        public static event OnFundsChangedFunc OnFamilyFundsChanged;

        protected ListenerAction OnFundsChanged(Event e)
        {
            try
            {
                FamilyFundsChangedEvent fundEvent = e as FamilyFundsChangedEvent;
                if ((fundEvent != null) && (fundEvent.Actor != null))
                {
                    SimDescription head = SimTypes.HeadOfFamily(fundEvent.Actor.Household);
                    if ((head != null) && (head.CreatedSim == fundEvent.Actor))
                    {
                        string key;
                        if (!mAccountKeys.TryGetValue(fundEvent.Actor.Household, out key))
                        {
                            key = null;
                        }

                        // Sending a null into this funtion automatically sets "Income" or "Expense"
                        AdjustAccounting(head.Household, key, (int)fundEvent.Delta);

                        if (OnFamilyFundsChanged != null)
                        {
                            OnFamilyFundsChanged(this, fundEvent, key);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
            }

            return ListenerAction.Keep;
        }

        protected static bool IsConsignable(GameObject obj)
        {
            if (!obj.CanBeSold()) return false;

            if ((!GameUtils.IsInstalled(ProductVersion.EP2)) && (!GameUtils.IsInstalled(ProductVersion.EP7)))
            {
                return false;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                if (ConsignmentRegister.IsObjectConsignable(obj, null)) return true;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP7))
            {
                if (PotionShopConsignmentRegister.IsObjectConsignable(obj, null)) return true;
            }

            if (obj is ICraftedArt) return true;

            if (obj is MinorPet) return true;

            return Consignments.IsCollectable(obj);
        }

        public int Sell(SimDescription sim, GameObject obj)
        {
            if ((mAllowConsignment) && (sim.CreatedSim != null) && (IsConsignable (obj)))
            {
                Inventory inventory = Inventories.ParentInventory(obj);
                if ((inventory == null) || (inventory.TryToRemove(obj)))
                {
                    try
                    {
                        if (GameUtils.IsInstalled(ProductVersion.EP2))
                        {
                            ConsignmentRegister.OnConsignedItem(sim.CreatedSim, obj);
                        }
                        else if (GameUtils.IsInstalled(ProductVersion.EP7))
                        {
                            PotionShopConsignmentRegister.OnConsignedItem(sim.CreatedSim, obj);
                        }
 
                        return 0;
                    }
                    catch(Exception e)
                    {
                        Common.DebugException(obj, e);
                    }
                }
            }

            int value = obj.Value;

            Consignments.NotifySell(sim, obj);

            AdjustFunds(sim, "SellItem", value);

            obj.Destroy();

            return value;
        }

        public bool AdjustFunds(SimDescription sim, string key, int funds)
        {
            if (sim == null) return false;

            if (!Money.Allow(this, sim)) return false;

            return AdjustFunds(sim.Household, key, funds);
        }
        public bool AdjustFunds(Household house, string key, int funds)
        {
            if (house == null) return false;

            if (SimTypes.IsSpecial(house)) return false;

            bool ret = true;

            if (funds >= 0)
            {
                house.mFamilyFunds += funds;
                ret = true;
            }
            else
            {
                int remainder = house.FamilyFunds + funds;

                if (remainder < 0)
                {
                    AddStat("Debt Incurred", remainder);

                    house.mFamilyFunds = 0;

                    if (GetValue<DebtOption, int>(house) == 0)
                    {
                        Stories.PrintStory(this, "NewDebt", house, null, ManagerStory.StoryLogging.Full);
                    };

                    AddValue<DebtOption, int>(house, -remainder);
                    ret = false;
                }
                else
                {
                    house.mFamilyFunds = remainder;

                    ret = true;
                }
            }

            if (house == Household.ActiveHousehold)
            {
                if (house.FamilyFundsChanged != null)
                {
                    house.FamilyFundsChanged(house.mFamilyFunds - funds, house.mFamilyFunds);
                }
            }

            AdjustAccounting(house, key, funds);

            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                GetData<StoredNetWorthSimData>(sim).Reset();
            }
            return ret;
        }

        public bool ShouldBillHouse(Household house)
        {
            if (house == null) return false;

            // Intentionally use GameUtils
            if (GameUtils.IsOnVacation())
            {
                if (house == GameStates.TravelHousehold) return false;
            }

            foreach (SimDescription sim in house.AllSimDescriptions)
            {
                if (sim.TraitManager.HasElement(TraitNames.NoBillsEver))
                {
                    return false;
                }
            }

            return true;
        }

        public class AdjustFundsTask : Common.FunctionTask
        {
            SimDescription mSim;

            string mKey;

            int mValue;

            protected AdjustFundsTask(SimDescription sim, string key, int value)
            {
                mSim = sim;
                mKey = key;
                mValue = value;
            }

            public static void Perform(SimDescription sim, string key, int value)
            {
                new AdjustFundsTask(sim, key, value).AddToSimulator();
            }

            protected override void OnPerform()
            {
                StoryProgression.Main.Money.AdjustFunds(mSim, mKey, mValue);
            }
        }

        public class Updates : AlertLevelOption<ManagerMoney>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerMoney>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerMoney>
        {
            public SpeedOption()
                : base(1000, false)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerMoney>
        {
            public DumpScoringOption()
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerMoney>
        {
            public TicksPassedOption()
            { }
        }

        public class SetAccountingKey : IDisposable
        {
            ManagerMoney mManager;

            Household mHouse;

            string mOriginal;

            public SetAccountingKey(Household household, string key)
            {
                mHouse = household;

                mManager = NRaas.StoryProgression.Main.Money;

                if (!mManager.mAccountKeys.TryGetValue(mHouse, out mOriginal))
                {
                    mOriginal = null;
                }

                mManager.mAccountKeys[mHouse] = key;
            }

            public void Dispose()
            {
                mManager.mAccountKeys[mHouse] = mOriginal;
            }
        }

        public interface IDebtOption : INotRootLevelOption
        { }

        public class DebtListingOption : NestingManagerOptionItem<ManagerMoney, IDebtOption>
        {
            public DebtListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "DebtListing";
            }
        }

        public interface IFeesOption : INotRootLevelOption
        { }

        public class FeesListingOption : NestingManagerOptionItem<ManagerMoney, IFeesOption>
        {
            public FeesListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "FeesListing";
            }
        }

        public interface IPurchaseOption : INotRootLevelOption
        { }

        public class PurchaseListingOption : NestingManagerOptionItem<ManagerMoney, IPurchaseOption>
        {
            public PurchaseListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "PurchaseListing";
            }
        }

        public interface ISalesOption : INotRootLevelOption
        { }

        public class SalesListingOption : NestingManagerOptionItem<ManagerMoney, ISalesOption>
        {
            public SalesListingOption()
            { }

            public override string GetTitlePrefix()
            {
                return "SalesListing";
            }
        }

        public class RichThresholdOption : IntegerManagerOptionItem<ManagerMoney>
        {
            public RichThresholdOption()
                : base(SimDescription.kSimoleonThresholdForBeingRich)
            { }

            public override string GetTitlePrefix()
            {
                return "RichThreshold";
            }

            public override void SetValue(int value, bool persist)
            {
                SimDescription.kSimoleonThresholdForBeingRich = value;

                base.SetValue(value, persist);
            }
        }

        public class AccountingKeyOption : BooleanManagerOptionItem<ManagerMoney>, IDebuggingOption
        {
            public AccountingKeyOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowAccountingKey";
            }
        }
    }
}

