using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerLot : Manager, ISimFromBinManager
    {
        public enum CheckResult
        {
            Failure,
            Success,
            IgnoreCost,
        }

        public enum FindLotFlags
        {
            None = 0x0,
            CheapestHome = 0x01,
            Inspect = 0x02,
            InspectPets = 0x04,
            InspectCareerItems = 0x08,
            AllowExistingInfractions = 0x10,
        }

        int mOccupiedResidentialLotCount;
        int mResidentialLotCount;
        int mUnoccupiedResidentialLotCount;

        int mPreviousPressure = 0;

        Dictionary<Manager,int> mImmigrationPressure = new Dictionary<Manager,int> ();

        Dictionary<Lot, int> mWorth = new Dictionary<Lot, int>();

        int mNumServingsOnStartup = 0;

        public ManagerLot(Main manager)
            : base(manager)
        { }

        public int NumServingsOnStartup
        {
            get { return mNumServingsOnStartup; }
        }

        public int PreviousPressure
        {
            get { return mPreviousPressure; }
            set 
            { 
                mPreviousPressure = value;
                mImmigrationPressure.Clear();
            }
        }

        public Dictionary<Manager, int> ImmigrationPressure
        {
            get { return mImmigrationPressure; }
        }
        
        public override string GetTitlePrefix(PrefixType type)
        {
            return "Lots";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerLot>(this).Perform(initial);
        }

        public float FreeLotRatio
        {
            get
            {
                return ((float)mUnoccupiedResidentialLotCount) / ((float)mOccupiedResidentialLotCount);
            }
        }

        public bool RoomForAnotherMoveIn
        {
            get
            {
                return (mUnoccupiedResidentialLotCount > 4);
            }
        }

        public override void Startup(PersistentOptionBase options)
        {
            base.Startup(options);

            mWorth.Clear();

            mNumServingsOnStartup = Household.kNumServingsOnStartup;

            Household.kNumServingsOnStartup = 0;
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            return null;
        }
        
        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((ProgressionEnabled) && (fullUpdate))
            {
                ClearWorth();

                mUnoccupiedResidentialLotCount = 0;
                mOccupiedResidentialLotCount = 0;
                mResidentialLotCount = 0;

                foreach (Lot lot in LotManager.AllLots)
                {
                    if ((lot.IsCommunityLot) || (!lot.IsResidentialLot) || (!lot.CanSupportPopulation(1)))
                    {
                        continue;
                    }

                    if (lot.Household == null)
                    {
                        mUnoccupiedResidentialLotCount++;
                    }
                    else
                    {
                        mOccupiedResidentialLotCount++;
                    }

                    mResidentialLotCount++;
                }

                IncreaseImmigrationPressure(this, RandomUtil.GetInt(GetValue<ImmigrantRequirementScenario.RandomPressureOption,int>()));
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            mWorth.Clear();
        }

        public delegate void OnOptionsUpdatedFunc(Lot lot);

        public static event OnOptionsUpdatedFunc OnOptionsUpdated;

        public static void LotOptionsChanged(Lot lot)
        {
            if (OnOptionsUpdated == null) return;

            OnOptionsUpdated(lot);
        }

        public delegate int GetLotCostFunc(Lot lot);

        public static event GetLotCostFunc OnGetLotCost;

        public static int GetUnfurnishedLotCost(Lot lot, int failureValue)
        {
            return CommonSpace.Helpers.Lots.GetUnfurnishedCost(lot);
        }

        public int GetLotCost(Lot lot)
        {
            if (lot == null) return 0;

            if (OnGetLotCost != null)
            {
                return OnGetLotCost(lot);
            }
            else
            {
                return lot.Cost;
            }
        }

        public bool AllowCastes(Common.IStatGenerator stats, Lot lot, SimDescription sim)
        {
            List<SimDescription> sims = new List<SimDescription>();
            sims.Add(sim);
            return AllowCastes(stats, lot, sims);
        }
        public bool AllowCastes(Common.IStatGenerator stats, Lot lot, IEnumerable<SimDescription> sims)
        {
            return Options.GetLotOptions(lot).AllowCastes(stats, sims);
        }

        public bool Allow(Common.IStatGenerator stats, Lot lot, ICollection<SimDescription> sims, FindLotFlags flags, bool allowRentable)
        {
            LotOptions lotData = Options.GetLotOptions(lot);

            if (UnchartedIslandMarker.IsUnchartedIsland(lot))
            {
                stats.IncStat("Find Lot: Uncharted");
                return false;
            }
            else if (!lotData.GetValue<AllowLotUseOption, bool>())
            {
                stats.IncStat("Find Lot: Lot Allow Denied");
                return false;
            }
            else if (!PassesHomeInspection(stats, lot, sims, flags))
            {
                stats.IncStat("Find Lot: Home Inspection");
                return false;
            }

            if (!allowRentable)
            {
                if (Money.GetDeedOwner(lot) != null) 
                {
                    return false;
                }
            }

            return lotData.AllowCastes(stats, sims);
        }

        public List<SimDescription> GetSimsWith<T>()
            where T : GameObject
        {
            List<SimDescription> sims = new List<SimDescription>();
            foreach (T machine in Sims3.Gameplay.Queries.GetObjects<T>())
            {
                if (machine.LotCurrent == null) continue;

                if (machine.LotCurrent.Household == null) continue;

                sims.AddRange(machine.LotCurrent.Household.AllSimDescriptions);
            }

            return sims;
        }

        public static Computer GetUsableComputer(SimDescription sim)
        {
            List<Computer> computers = new List<Computer>();
            foreach (Computer computer in Inventories.InventoryFindAll<Computer>(sim))
            {
                if (computer.IsComputerUsable(sim.CreatedSim, true, false, true))
                {
                    computers.Add(computer);
                }
            }

            if (computers.Count == 0)
            {
                foreach (Computer computer in sim.LotHome.GetObjects<Computer>())
                {
                    if (computer.IsComputerUsable(sim.CreatedSim, true, false, true))
                    {
                        computers.Add(computer);
                    }
                }
            }

            if (computers.Count == 0)
            {
                return null;
            }

            return RandomUtil.GetRandomObjectFromList(computers);
        }

        public void ProcessAbandonLot(Lot lot)
        {
            if (lot == null) return;

            Household house = lot.Household;
            if (house == null) return;

            SimDescription sim = SimTypes.HeadOfFamily(house);
            if (sim != null)
            {
                if (sim.CreatedSim != null)
                {
                    PackupVehicles(sim.CreatedSim, false);
                }
            }
        }

        public int GetWorth(Household house)
        {
            if (house == null) return 0;

            if (house.LotHome == null) return 0;

            int worth = 0;
            if (!mWorth.TryGetValue(house.LotHome, out worth))
            {
                worth = GetLotCost(house.LotHome);

                mWorth.Add(house.LotHome, worth);
            }

            return worth;
        }

        public void ClearWorth()
        {
            mWorth.Clear();
        }
        public void ClearWorth(Household house)
        {
            if (house == null) return;

            ClearWorth(house.LotHome);
        }
        public void ClearWorth(Lot lot)
        {
            if (lot == null) return;

            mWorth.Remove(lot);
        }

        public override Scenario GetImmigrantRequirement(ImmigrationRequirement requirement)
        {
            return new ImmigrantRequirementScenario(requirement);
        }

        public void IncreaseImmigrationPressure (Manager manager, int count)
        {
            if (count <= 0) return;

            if (mImmigrationPressure.ContainsKey(manager))
            {
                mImmigrationPressure[manager] += count;
            }
            else
            {
                mImmigrationPressure.Add(manager, count);
            }
        }

        public static List<Lot> GetOwnedLots(SimDescription sim)
        {
            if (sim == null) return new List<Lot>();

            return GetOwnedLots(sim.Household);
        }
        public static List<Lot> GetOwnedLots(Household house)
        {
            List<Lot> results = new List<Lot>();

            if (house != null)
            {
                if (house.LotHome != null)
                {
                    results.Add(house.LotHome);
                }

                if (house.RealEstateManager != null)
                {
                    results.AddRange(house.RealEstateManager.GetPrivateLotMoveToLots());
                }
            }

            return results;
        }

        public bool AllowSim(Common.IStatGenerator stats, Sim sim, Lot lot)
        {
            if (sim == null) return false;

            if (!lot.IsOpenVenue()) 
            {
                stats.IncStat("Not Open Venue");
                return false;
            }
            else if (UnchartedIslandMarker.IsUnchartedIsland(lot))
            {
                stats.IncStat("Uncharted");
                return false;
            }

            LotOptions lotOptions = GetLotOptions(lot);

            if (!lotOptions.GetValue<AllowLotPushOption, bool>()) 
            {
                stats.IncStat("Lot Push Denied");
                return false;
            }

            if (!lotOptions.AllowCastes(stats, sim.SimDescription))
            {
                return false;
            }

            switch (lot.CommercialLotSubType)
            {
                case CommercialLotSubType.kEP2_JunkyardNoVisitors:
                case CommercialLotSubType.kMisc_NoVisitors:
                    stats.IncStat("No Visitors");
                    return false;
                case CommercialLotSubType.kArtGallery:
                case CommercialLotSubType.kTheatre:
                    if (sim.TraitManager.HasElement(TraitNames.CantStandArt))
                    {
                        stats.IncStat("CantStandArt");
                        return false;
                    }
                    break;
                case CommercialLotSubType.kBeach:
                case CommercialLotSubType.kEP3_DanceClubPool:
                case CommercialLotSubType.kPool:
                case CommercialLotSubType.kFishingSpot:
                    if (sim.TraitManager.HasElement(TraitNames.CantStandArt))
                    {
                        stats.IncStat("CantStandArt");
                        return false;
                    }
                    break;
                case CommercialLotSubType.kGym:
                    if (sim.TraitManager.HasElement(TraitNames.CouchPotato))
                    {
                        stats.IncStat("CouchPotato");
                        return false;
                    }
                    break;
                case CommercialLotSubType.kGraveyard:
                    if (sim.TraitManager.HasElement(TraitNames.Coward))
                    {
                        stats.IncStat("Coward");
                        return false;
                    }
                    break;
            }

            CarryingChildPosture posture = sim.CarryingChildPosture;
            if (posture != null)
            {
                if (!AllowSim(stats, posture.Child, lot))
                {
                    stats.IncStat("Child Fail");
                    return false;
                }
            }

            return true;
        }

        public Lot GetCommunityLot(Sim a, List<CommercialLotSubType> types, bool mustHaveSims)
        {
            if (a == null) return null;

            // Stops an error in HowMuchItWantsSimToCome()
            if (a.CelebrityManager.Level > CelebrityManager.HighestLevel)
            {
                a.CelebrityManager.mLevel = CelebrityManager.HighestLevel;
            }

            ScoringList<Lot> lots = new ScoringList<Lot>();
            foreach (Lot lot in LotManager.AllLotsWithoutCommonExceptions)
            {
                if (!lot.IsCommunityLot) continue;

                if (!AllowSim(this, a, lot)) continue;

                if (mustHaveSims)
                {
                    if (lot.GetAllActorsCount() == 0) continue;
                }

                if (types != null)
                {
                    if (!types.Contains(lot.CommercialLotSubType)) continue;
                }

                lots.Add(lot, (int)lot.HowMuchItWantsSimToCome(a, SimClock.HoursPassedOfDay));
            }

            if (lots.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(lots.GetBestByPercent(50f));
        }
        public Lot GetCommunityLot(Sim a, Sim b)
        {
            if ((a == null) || (b == null)) return null;

            // Stops an error in HowMuchItWantsSimToCome()
            if (a.CelebrityManager.Level > CelebrityManager.HighestLevel)
            {
                a.CelebrityManager.mLevel = CelebrityManager.HighestLevel;
            }

            // Stops an error in HowMuchItWantsSimToCome()
            if (b.CelebrityManager.Level > CelebrityManager.HighestLevel)
            {
                b.CelebrityManager.mLevel = CelebrityManager.HighestLevel;
            }

            ScoringList<Lot> lots = new ScoringList<Lot> ();
            foreach (Lot lot in LotManager.AllLotsWithoutCommonExceptions)
            {
                if (!lot.IsCommunityLot) continue;

                if (!AllowSim(this, a, lot)) continue;

                if (!AllowSim(this, b, lot)) continue;

                lots.Add (lot, (int)(lot.HowMuchItWantsSimToCome (a, SimClock.HoursPassedOfDay) + lot.HowMuchItWantsSimToCome (b, SimClock.HoursPassedOfDay)));
            }

            if (lots.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(lots.GetBestByPercent(50f));
        }

        public Lot GetHomeLot(Sim a, Sim b, bool testPush)
        {
            Lot result = null;
            if (a.LotHome == null)
            {
                result = b.LotHome;
            }
            else if (b.LotHome == null)
            {
                result = a.LotHome;
            }

            if (result != null)
            {
                if ((testPush) && (!GetValue<AllowLotPushOption, bool>(result))) return null;
            }

            if ((RandomUtil.CoinFlip()) && ((!testPush) || (GetValue<AllowLotPushOption, bool>(a.LotHome))))
            {
                return a.LotHome;
            }
            else if ((!testPush) || (GetValue<AllowLotPushOption, bool>(b.LotHome)))
            {
                return b.LotHome;
            }
            else
            {
                return null;
            }
        }

        public bool PassesHomeInspection(Common.IStatGenerator stats, Lot lot, ICollection<SimDescription> newMembers, ICollection<SimDescription> existing, FindLotFlags flags)
        {
            if (lot == null) return false;

            List<SimDescription> members = new List<SimDescription> ();
            members.AddRange (newMembers);
            members.AddRange (existing);

            return PassesHomeInspection(stats, lot, members, flags);
        }
        public bool PassesHomeInspection(Common.IStatGenerator stats, Lot lot, ICollection<SimDescription> sims, FindLotFlags flags)
        {
            if (lot == null) return false;

            Dictionary<HomeInspection.Reason, bool> existingResults = new Dictionary<HomeInspection.Reason, bool>();

            if (sims != null)
            {
                Dictionary<Household, bool> houses = new Dictionary<Household, bool>();

                foreach (SimDescription sim in sims)
                {
                    if ((sim.Household != null) && (!houses.ContainsKey(sim.Household)))
                    {
                        houses.Add(sim.Household, true);

                        if (sim.LotHome != null)
                        {
                            foreach (HomeInspection.Result result in new HomeInspection(sim.LotHome).Satisfies(HouseholdsEx.All(sim.Household)))
                            {
                                existingResults[result.mReason] = true;
                            }
                        }
                    }

                    if (sim.Occupation != null)
                    {
                        if (((flags & FindLotFlags.InspectCareerItems) == FindLotFlags.InspectCareerItems) && (GetValue<CareerObjectInspectionOption, bool>()))
                        {
                            DreamJob job = ManagerCareer.GetDreamJob(sim.Occupation.Guid);
                            if (job != null)
                            {
                                if (!job.Satisfies(Careers, sim, lot, true))
                                {
                                    stats.IncStat("Career Inspection Fail");
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return PassesHomeInspection(stats, lot, existingResults.Keys, new HomeInspection(lot).Satisfies(sims), flags);
        }
        protected bool PassesHomeInspection(Common.IStatGenerator stats, Lot lot, ICollection<HomeInspection.Reason> existingResults, IEnumerable<HomeInspection.Result> results, FindLotFlags flags)
        {
            if (lot == null) return false;

            Lot.LotMetrics metrics = new Lot.LotMetrics();
            lot.GetLotMetrics (ref metrics);

            if (metrics.FridgeCount == 0)
            {
                stats.IncStat("Home Inspection: No Fridge");
                return false;
            }

            if (((flags & FindLotFlags.Inspect) == FindLotFlags.Inspect) && (GetValue<RigorousNewHomeOption,bool> ()))
            {
                bool failure = false;
                foreach (HomeInspection.Result result in results)
                {
                    switch (result.mReason)
                    {
                        case HomeInspection.Reason.NoDouble:
                            continue;
                        case HomeInspection.Reason.TooFewDouble:
                            if (!GetValue<DoubleBedInspectionOption, bool>())
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Disabled");
                                continue;
                            }
                            break;
                        case HomeInspection.Reason.TooFewCribs:
                            if ((!GetValue<CribInspectionOption, bool>()))
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Disabled");
                                continue;
                            }
                            else if (existingResults.Contains(HomeInspection.Reason.TooFewCribs))
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Existing Infraction");
                                continue;
                            }
                            break;
                        case HomeInspection.Reason.TooFewBeds:
                            if (GetValue<DoubleBedInspectionOption, bool>())
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Disabled");
                                continue;
                            }
                            break;
                        case HomeInspection.Reason.TooFewStalls:
                            if (result.mExisting > 0)
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " At Least One");
                                continue;
                            }
                            // Never move horses to a lot that has no stalls
                            break;
                        case HomeInspection.Reason.TooFewPetBeds:
                            if (((flags & FindLotFlags.InspectPets) != FindLotFlags.InspectPets))
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Not Checking");
                                continue;
                            }
                            else if (!GetValue<PetInspectionOption, bool>())
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Disabled");
                                continue;
                            }
                            else if (existingResults.Contains(HomeInspection.Reason.TooFewPetBeds))
                            {
                                stats.IncStat("Home Inspection: " + result.mReason + " Existing Infraction");
                                continue;
                            }
                            break;
                    }

                    stats.AddStat("Home Inspection: " + result.mReason + " Existing", result.mExisting);
                    stats.AddStat("Home Inspection: " + result.mReason + " Required", result.mRequired);
                    failure = true;
                }

                if (failure) return false;
            }

            return true;
        }

        public static List<Vehicle> GetLotVehicles(Lot lot)
        {
            List<Vehicle> results = new List<Vehicle>();

            if (lot != null)
            {
                if (lot.IsResidentialLot)
                {
                    List<IOwnableVehicle> vehicles = new List<IOwnableVehicle>(lot.GetObjects<IOwnableVehicle>());
                    foreach (IOwnableVehicle vehicle in vehicles)
                    {
                        CarOwnable ownable = vehicle as CarOwnable;
                        if (ownable != null)
                        {
                            if (ownable.LotHome != lot) continue;

                            results.Add(ownable);
                        }
                        else
                        {
                            Bicycle bicycle = vehicle as Bicycle;
                            if (bicycle != null)
                            {
                                if (bicycle.LotHome != lot) continue;

                                results.Add(bicycle);
                            }
                        }
                    }
                }
                else
                {
                    foreach (ParkingSpace space in lot.GetObjects<ParkingSpace>())
                    {
                        CarOwnable ownable = space.ReservedVehicle as CarOwnable;
                        if (ownable == null) continue;

                        if (ownable.GeneratedOwnableForNpc) continue;

                        results.Add(ownable);
                    }
                }
            }

            return results;
        }

        protected int CalculateVehicleCost(Lot lot)
        {
            if (lot == null) return 0;

            int cost = 0;

            List<Vehicle> vehicles = GetLotVehicles (lot);
            foreach (Vehicle vehicle in vehicles)
            {
                cost += vehicle.Cost;
            }

            if (vehicles.Count > 0)
            {
                AddStat("Vehicle Cost", cost);
            }

            return cost;
        }

        public bool HasPersonalVehicle(SimDescription sim)
        {
            if (sim.CreatedSim == null) return false;

            if (Inventories.InventoryDuoFindAll<IOwnableVehicle, Vehicle>(sim).Count > 0) return true;

            return (sim.GetPreferredVehicle() != null);
        }

        public bool PackupVehicles(Sim sim, bool onlyMine)
        {
            if (sim == null) return false;

            AddTry ("Packup Vehicles");

            if (sim.LotHome == null) 
            {
                IncStat ("Packup: Homeless");
                return false;
            }

            List<Vehicle> vehicles = GetLotVehicles(sim.LotHome);
            foreach (Vehicle vehicle in vehicles)
            {
                if (onlyMine)
                {
                    IOwnableVehicle ownable = vehicle as IOwnableVehicle;
                    if (ownable == null) continue;

                    if (ownable != sim.GetPreferredVehicle ()) continue;
                }

                if (Inventories.TryToMove(vehicle, sim))
                {
                    IncStat("Packup: Packed " + vehicle.GetLocalizedName());
                }
                else
                {
                    IncStat("Packup: Fail " + vehicle.GetLocalizedName());
                }
            }

            ClearWorth(sim.Household);
            return true;
        }

        public delegate CheckResult LotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds);

        public Lot FindLot(IScoringGenerator stats, ICollection<SimDescription> sims, int maximumLoan, FindLotFlags flags, LotPriceCheck inPriceRange)
        {
            stats.IncStat("FindLot");

            Dictionary<SimDescription, bool> lookup = new Dictionary<SimDescription,bool> ();

            Dictionary<Household, bool> homes = new Dictionary<Household, bool>();

            Dictionary<ulong,int> castes = new Dictionary<ulong,int>();
            int simCount = 0;

            bool allowRentable = true;

            if (sims != null)
            {
                foreach (SimDescription sim in sims)
                {
                    if (lookup.ContainsKey(sim)) continue;
                    lookup.Add(sim, true);

                    if (!Money.AllowRent(stats, sim))
                    {
                        allowRentable = false;
                    }

                    simCount++;

                    foreach (CasteOptions caste in GetData(sim).Castes)
                    {
                        int count;
                        if (castes.TryGetValue(caste.ID, out count))
                        {
                            castes[caste.ID] = count + 1;
                        }
                        else
                        {
                            castes[caste.ID] = 1;
                        }
                    }

                    if (SimTypes.IsSpecial(sim)) continue;

                    if (sim.Household == null) continue;

                    homes[sim.Household] = true;
                }
            }

            int currentLotCost = 0;

            int availableFunds = maximumLoan;
            foreach (Household home in homes.Keys)
            {
                bool allMoving = true;
                foreach (SimDescription sim in home.AllSimDescriptions)
                {
                    if (!lookup.ContainsKey(sim))
                    {
                        allMoving = false;
                        break;
                    }
                }

                if (allMoving)
                {
                    if (home.LotHome == null)
                    {
                        availableFunds += home.NetWorth();
                    }
                    else if (GetValue<IsAncestralOption, bool>(home))
                    {
                        stats.IncStat("FindLot: Ancestral Fail");
                        return null;
                    }
                    else
                    {
                        currentLotCost += GetLotCost(home.LotHome);

                        availableFunds += home.FamilyFunds + GetLotCost(home.LotHome);

                        availableFunds -= CalculateVehicleCost(home.LotHome);
                    }
                }
                else
                {
                    availableFunds += home.FamilyFunds;
                }
            }

            flags |= FindLotFlags.InspectCareerItems;

            stats.AddStat("Available Funds", availableFunds);

            List<Lot> choices = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                string reason = HouseholdsEx.IsValidResidentialLot(lot);
                if (!string.IsNullOrEmpty(reason))
                {
                    stats.IncStat("Find Lot: " + reason);
                }
                else if (!Allow(stats, lot, sims, flags, allowRentable))
                {
                    continue;
                }
                else
                {
                    stats.AddStat("Lot Cost", GetLotCost(lot));

                    if ((inPriceRange == null) || (inPriceRange(stats, lot, currentLotCost, availableFunds) != CheckResult.Failure))
                    {
                        choices.Add(lot);
                    }
                }
            }

            if (choices.Count == 0)
            {
                stats.IncStat("Find Lot: Failure");
                return null;
            }
            else if ((flags & FindLotFlags.CheapestHome) == FindLotFlags.CheapestHome)
            {
                choices.Sort(new Comparison<Lot>(HouseholdsEx.SortByCost));

                return choices[0];
            }
            else
            {
                return RandomUtil.GetRandomObjectFromList(choices);
            }
        }

        public bool PutAwayBook(Common.IStatGenerator stats, Book book, Lot lot)
        {
            Bookshelf shelf = book.MyShelf;
            if ((shelf == null) || (!shelf.InWorld))
            {
                List<Bookshelf> shelves = new List<Bookshelf>();
                foreach(Bookshelf choice in lot.GetObjects<Bookshelf>())
                {
                    if (choice.Inventory == null) continue;

                    if (!choice.InWorld) continue;

                    if ((choice.Repairable != null) && (choice.Repairable.Broken)) continue;

                    shelves.Add(choice);
                }

                if (shelves.Count > 0)
                {
                    shelf = RandomUtil.GetRandomObjectFromList(shelves);
                }
            }

            if (shelf != null)
            {
                stats.IncStat("Book Shelved");

                shelf.Inventory.TryToMove(book);
                return true;
            }
            else
            {
                stats.IncStat("No Shelf");

                return false;
            }
        }

        public class LotHelper
        {
            private int mNumMembers;

            public LotHelper(int numMembers)
            {
                this.mNumMembers = numMembers;
            }

            public bool SupportsHousehold(Lot lot)
            {
                return (((lot.Household == null) && !lot.IsCommunityLot) && lot.CanSupportPopulation(this.mNumMembers));
            }
        }

        public delegate bool LotPredicate(Lot lot);

        public class Updates : AlertLevelOption<ManagerLot>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerLot>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class RigorousNewHomeOption : BooleanManagerOptionItem<ManagerLot>, IInspectionOption
        {
            public RigorousNewHomeOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RigorousHomeInspection";
            }
        }

        public class DoubleBedInspectionOption : BooleanManagerOptionItem<ManagerLot>, IInspectionOption
        {
            public DoubleBedInspectionOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeInspectionDoubleBed";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<RigorousNewHomeOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class CribInspectionOption : BooleanManagerOptionItem<ManagerLot>, IInspectionOption
        {
            public CribInspectionOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeInspectionCribCount";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<RigorousNewHomeOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class PetInspectionOption : BooleanManagerOptionItem<ManagerLot>, IInspectionOption
        {
            public PetInspectionOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeInspectionPets";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<RigorousNewHomeOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class CareerObjectInspectionOption : BooleanManagerOptionItem<ManagerLot>, IInspectionOption
        {
            public CareerObjectInspectionOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomeInspectionCareerObjects";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<RigorousNewHomeOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class SpeedOption : SpeedBaseOption<ManagerLot>
        {
            public SpeedOption()
                : base(1000, false)
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerLot>
        {
            public TicksPassedOption()
                : base()
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerLot>
        {
            public DumpScoringOption()
            { }
        }

        public interface IImmigrationEmigrationOption : INotRootLevelOption
        { }

        public class ImmigrationEmigrationOption : NestingManagerOptionItem<ManagerLot, IImmigrationEmigrationOption>
        {
            public ImmigrationEmigrationOption()
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrationEmigrationListing";
            }
        }

        public interface IInspectionOption : INotRootLevelOption
        { }

        public class InspectionOption : NestingManagerOptionItem<ManagerLot, IInspectionOption>
        {
            public InspectionOption()
            { }

            public override string GetTitlePrefix()
            {
                return "InspectionListing";
            }
        }

        public class AddInteractionsOption : BooleanManagerOptionItem<ManagerLot>
        {
            public static bool sQuickCheck = true;

            public AddInteractionsOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AddInteractions";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override void SetValue(bool value, bool persist)
            {
                base.SetValue(value, persist);

                sQuickCheck = value;
            }
        }
    }
}

