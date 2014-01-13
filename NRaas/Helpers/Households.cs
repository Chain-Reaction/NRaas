using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Households
    {
        public static string IsValidResidentialLot(Lot lot)
        {
            if (lot.Household != null)
            {
                return "Occupied";
            }
            else if (lot.IsCommunityLot)
            {
                return "Community lot";
            }
            else if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP1_PlayerOwnable)
            {
                return "Vacation Home";
            }
            else if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP10_PrivateLot)
            {
                return "Private Lot";
            }
            else if (lot.IsWorldLot)
            {
                return "World lot";
            }

            return null;
        }

        public static int NumSims(Household house)
        {
            if (house == null) return 0;

            return house.AllSimDescriptions.Count;
        }

        public static bool IsRole(Household home)
        {
            foreach (SimDescription sim in Households.All(home))
            {
                if (sim.AssignedRole != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLunarCycleZombie(Household home)
        {
            if (LunarCycleManager.sFullMoonZombies == null) return false;

            if (LunarCycleManager.sFullMoonZombies.Count == 0) return false;

            foreach (SimDescription sim in Households.All(home))
            {
                if (sim.IsZombie)
                {
                    if (LunarCycleManager.sFullMoonZombies.Contains(sim))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsPassport(Household home)
        {
            foreach (SimDescription sim in Households.All(home))
            {
                if (Passport.IsHostedPassportSim(sim.CreatedSim))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsActiveCoworker(Household home)
        {
            if (Household.ActiveHousehold == null) return false;

            foreach (SimDescription sim in Households.All(home))
            {
                foreach (SimDescription active in Households.All(Household.ActiveHousehold))
                {
                    Occupation occupation = active.Occupation;
                    if (occupation == null) continue;

                    if (occupation.Coworkers != null)
                    {
                        if (occupation.Coworkers.Contains(sim)) return true;
                    }
                }
            }

            return false;
        }

        public static bool IsActiveDaycare(Household home)
        {
            if (Household.ActiveHousehold == null) return false;

            foreach (SimDescription actor in Households.All(Household.ActiveHousehold))
            {
                Daycare dayCare = actor.Occupation as Daycare;
                if (dayCare == null) continue;

                foreach (SimDescription sim in Households.All(home))
                {
                    if (dayCare.IsDaycareChild(sim)) return true;
                }
            }

            return false;
        }

        public static bool IsFull(Household house, bool isPet, int maximum)
        {
            if (house == null) return true;

            if (isPet)
            {
                if (NumPetsIncludingPregnancy(house) >= maximum) return true;
            }
            else
            {
                if (NumHumansIncludingPregnancy(house) >= maximum) return true;
            }

            return false;
        }

        public static int NumSimsIncludingPregnancy(IEnumerable<SimDescription> sims)
        {
            int humanCount = 0, petCount = 0;
            return NumSimsIncludingPregnancy(sims, ref humanCount, ref petCount);
        }
        public static int NumSimsIncludingPregnancy(IEnumerable<SimDescription> sims, ref int humanCount, ref int petCount)
        {
            foreach (SimDescription sim in sims)
            {
                if (sim.IsPet)
                {
                    if (sim.IsPregnant)
                    {
                        petCount += 2;
                    }
                    else
                    {
                        petCount++;
                    }
                }
                else
                {
                    if (sim.IsPregnant)
                    {
                        humanCount += 2;
                    }
                    else
                    {
                        humanCount++;
                    }
                }
            }

            return (humanCount + petCount);
        }
        public static int NumSimsIncludingPregnancy(Household house)
        {
            int humanCount = 0, petCount = 0;
            return NumSimsIncludingPregnancy(house, ref humanCount, ref petCount);
        }
        public static int NumSimsIncludingPregnancy(Household house, ref int humanCount, ref int petCount)
        {
            if (house == null) return 0;

            int result = NumSimsIncludingPregnancy(house.AllSimDescriptions, ref humanCount, ref petCount);

            if (house.IsTravelHousehold)
            {
                int outworldSimCount, outworldPetCount;
                GameStates.GetNumHouseholdMembersSimsAndPetsNotInWorldCountingPregnant(out outworldSimCount, out outworldPetCount);

                humanCount += outworldSimCount;
                petCount += outworldPetCount;
            }

            return result;
        }

        public static int NumHumans(IEnumerable<ISimDescription> sims)
        {
            int count = 0;
            foreach (ISimDescription sim in sims)
            {
                if (sim.IsHuman)
                {
                    count++;
                }
            }

            return count;
        }
        public static int NumHumans(Household house)
        {
            if (house == null) return 0;

            return house.SimDescriptions.Count; // Humans
        }

        public static int NumHumansIncludingPregnancy(Household house)
        {
            if (house == null) return 0;

            int humanCount = 0, petCount = 0;
            return NumSimsIncludingPregnancy(house.SimDescriptions, ref humanCount, ref petCount); // Humans
        }

        public static int NumPets(Household house)
        {
            if (house == null) return 0;

            return house.PetSimDescriptions.Count;
        }

        public static int NumPetsIncludingPregnancy(Household house)
        {
            if (house == null) return 0;

            int humanCount = 0, petCount = 0;
            return NumSimsIncludingPregnancy(house.PetSimDescriptions, ref humanCount, ref petCount);
        }

        public static ICollection<SimDescription> All(Household house)
        {
            if (house == null) return new List<SimDescription>();

            return house.AllSimDescriptions;
        }

        public static ICollection<SimDescription> Humans(Household house)
        {
            if (house == null) return new List<SimDescription>();

            return house.SimDescriptions; // Humans
        }

        public static ICollection<SimDescription> Pets(Household house)
        {
            if (house == null) return new List<SimDescription>();

            return house.PetSimDescriptions;
        }

        public static List<Sim> AllSims(Household house)
        {
            List<Sim> sims = new List<Sim>();

            if (house != null)
            {
                foreach (SimDescription sim in house.AllSimDescriptions)
                {
                    if (sim.CreatedSim == null) continue;

                    sims.Add(sim.CreatedSim);
                }
            }

            return sims;
        }

        public static List<Sim> AllHumans(Household house)
        {
            List<Sim> sims = new List<Sim>();

            if (house != null)
            {
                foreach (SimDescription sim in house.SimDescriptions) // Humans
                {
                    if (sim.CreatedSim == null) continue;

                    sims.Add(sim.CreatedSim);
                }
            }

            return sims;
        }

        public static List<Sim> AllPets(Household house)
        {
            List<Sim> sims = new List<Sim>();

            if (house != null)
            {
                foreach (SimDescription sim in house.PetSimDescriptions)
                {
                    if (sim.CreatedSim == null) continue;

                    sims.Add(sim.CreatedSim);
                }
            }

            return sims;
        }

        public static void TransferData(Household newHouse, Household oldHouse)
        {
            if (oldHouse.LifetimeHappinessNotificationShown)
            {
                newHouse.LifetimeHappinessNotificationShown = oldHouse.LifetimeHappinessNotificationShown;
            }

            for (int i = 0; i < newHouse.mMoneySaved.Length; i++)
            {
                newHouse.mMoneySaved[i] += oldHouse.mMoneySaved[i];
            }

            newHouse.mAncientCoinCount += oldHouse.mAncientCoinCount;
            newHouse.UniqueObjectsObtained |= oldHouse.UniqueObjectsObtained;

            if (oldHouse.mKeystonePanelsUsed != null)
            {
                if (newHouse.mKeystonePanelsUsed == null)
                {
                    newHouse.mKeystonePanelsUsed = new PairedListDictionary<WorldName, List<string>>();
                }

                foreach (KeyValuePair<WorldName, List<string>> pair in oldHouse.mKeystonePanelsUsed)
                {
                    if (pair.Value == null) continue;

                    List<string> list;
                    if (!newHouse.mKeystonePanelsUsed.TryGetValue(pair.Key, out list))
                    {
                        list = new List<string>();
                        newHouse.mKeystonePanelsUsed.Add(pair.Key, list);
                    }

                    foreach (string value in pair.Value)
                    {
                        if (list.Contains(value)) continue;

                        list.Add(value);
                    }
                }
            }

            if (oldHouse.mCompletedHouseholdOpportunities != null)
            {
                if (newHouse.mCompletedHouseholdOpportunities == null)
                {
                    newHouse.mCompletedHouseholdOpportunities = new Dictionary<ulong, bool>();
                }

                foreach (KeyValuePair<ulong, bool> pair in oldHouse.mCompletedHouseholdOpportunities)
                {
                    newHouse.mCompletedHouseholdOpportunities[pair.Key] = pair.Value;
                }
            }

            TransferRealEstate(newHouse, oldHouse);
        }

        public static void TransferRealEstate(Household newHouse, Household oldHouse)
        {
            Common.StringBuilder msg = new Common.StringBuilder("TransferRealEstate");

            if (oldHouse.RealEstateManager != null)
            {
                for (int i=oldHouse.RealEstateManager.mAllProperties.Count-1; i>=0; i--)
                {
                    PropertyData data = oldHouse.RealEstateManager.mAllProperties[i] as PropertyData;
                    if (data == null) continue;

                    msg += Common.NewLine + "Data: " + data.PropertyType;

                    PropertyData newData = null;

                    switch (data.PropertyType)
                    {
                        case RealEstatePropertyType.RabbitHole:
                            PropertyData.RabbitHole oldData1 = data as PropertyData.RabbitHole;

                            PropertyData.RabbitHole newData1 = newHouse.RealEstateManager.FindProperty(oldData1.GetRabbitHole()) as PropertyData.RabbitHole;
                            
                            newData = newData1;

                            if ((newData1 != null) && (!newData1.IsFullOwner))
                            {
                                newData1.mIsFullOwner = oldData1.mIsFullOwner;
                            }

                            break;
                        case RealEstatePropertyType.VacationHome:
                        case RealEstatePropertyType.Venue:
                            PropertyData oldData2 = data as PropertyData;

                            foreach (IPropertyData newData2 in newHouse.RealEstateManager.AllProperties)
                            {
                                if (newData2.World != oldData2.World) continue;

                                if (newData2.LotId != oldData2.LotId) continue;

                                newData = newData2 as PropertyData;
                                break;
                            }

                            break;
                    }

                    PropertyData transferData = null;
                    if (newData == null)
                    {
                        msg += Common.NewLine + "Transfer";

                        transferData = data;
                    }
                    else
                    {
                        msg += Common.NewLine + "Money: " + data.TotalValue;

                        newData.mCurrentCollectibleFunds += data.TotalValue;
                        data.mCurrentCollectibleFunds = 0;
                        data.mStoredValue = 0;
                    }

                    if (transferData != null)
                    {
                        transferData.mOwner = newHouse.RealEstateManager;

                        newHouse.RealEstateManager.mAllProperties.Add(transferData);
                    }

                    oldHouse.RealEstateManager.mAllProperties.RemoveAt(i);
                }
            }

            Common.DebugNotify(msg);
        }

        public static int SortByCost(Lot a, Lot b)
        {
            if (a.Cost > b.Cost)
            {
                return 1;
            }
            else if (a.Cost < b.Cost)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}

