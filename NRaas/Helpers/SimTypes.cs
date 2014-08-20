using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public enum SimType : int
    {
        None = 0,
        Service,
        Resident,
        Townie,
        Dead,
        Tourist,
        Mummy,
        SimBot,
        Frankenstein,
        PlayableGhost,
        Pregnant,
        Human,
        Vampire,
        Occult,
        ImaginaryFriend,
        Genie,
        Fairy,
        Werewolf,
        Witch,
        Zombie,
        BoneHilda,
        Unicorn,
        Alien,
        Animal,
        ActiveFamily,
        NonActiveFamily,
        Selectable,
        NonSelectable,
        Hybrid,
        Partnered,
        Steady,
        Married,
        Single,
        Straight,
        Gay,
        Bisexual,
        Plantsim,
        Alive,
        Foreign,
        Local,
        StrandedCouple,
        MiniSim,
        Mermaid,
        Plumbot,
        TimeTraveler,
        Dog,
        Cat,
        Horse,
        Deer,
        Raccoon,
        Stray,
        WildHorse,
        Role,
        Employed,
        Unemployed,
        HasDegree,
        Degreeless
    }

    public class SimTypes
    {
        public static string GetLocalizedName(SimType type)
        {
            switch (type)
            {
                case SimType.Vampire:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Vampire);
                case SimType.Mummy:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Mummy);
                case SimType.SimBot:
                case SimType.Frankenstein:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Frankenstein);
                case SimType.ImaginaryFriend:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.ImaginaryFriend);
                case SimType.Unicorn:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Unicorn);
                case SimType.Genie:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Genie);
                case SimType.Werewolf:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Werewolf);
                case SimType.Fairy:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Fairy);
                case SimType.Witch:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Witch);
                case SimType.Mermaid:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Mermaid);
                case SimType.Plantsim:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.PlantSim);
                case SimType.Plumbot:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.Robot);
                case SimType.TimeTraveler:
                    return OccultTypeHelper.GetLocalizedName(OccultTypes.TimeTraveler);
                default:
                    return Common.Localize("SimType:" + type);
            }
        }

        public static bool IsEquivalentSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            if ((a == null) || (b == null)) return false;

            return ((a.Species == b.Species) || (a.IsADogSpecies && b.IsADogSpecies));
        }

        public static PetPoolType GetPetPool(SimDescription sim)
        {
            bool noPool;
            return GetPetPool(sim, out noPool);
        }
        public static PetPoolType GetPetPool(SimDescription sim, out bool noPool)
        {
            noPool = false;

            if (!GameUtils.IsInstalled(ProductVersion.EP5)) return PetPoolType.None;

            if (sim.IsHuman) return PetPoolType.None;

            List<PetPoolType> check = new List<PetPoolType>();

            switch (sim.Species)
            {
                case CASAgeGenderFlags.Horse:
                    check.Add(PetPoolType.AdoptHorse);
                    check.Add(PetPoolType.BuySellHorse);
                    check.Add(PetPoolType.WildHorse);
                    check.Add(PetPoolType.Stallion);
                    check.Add(PetPoolType.Unicorn);
                    break;
                case CASAgeGenderFlags.Cat:
                    check.Add(PetPoolType.AdoptCat);
                    check.Add(PetPoolType.StrayCat);
                    break;
                case CASAgeGenderFlags.LittleDog:
                case CASAgeGenderFlags.Dog:
                    check.Add(PetPoolType.AdoptDog);
                    check.Add(PetPoolType.StrayDog);
                    break;
            }

            if (check.Count == 0)
            {
                noPool = true;
            }
            else
            {
                foreach (PetPoolType type in check)
                {
                    if (PetPoolManager.IsPetInPoolType(sim, type))
                    {
                        return type;
                    }
                }
            }

            return PetPoolType.None;
        }

        public static bool IsPassporter(SimDescription sim)
        {
            if (IsAwayOnSimPort(sim)) return true;

            if (IsVisitingSimPort(sim)) return true;

            return false;
        }

        public static bool IsAwayOnSimPort(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.HasFlags(SimDescription.FlagField.IsTravelingForPassport)) return true;

            if (sim.HasFlags(SimDescription.FlagField.IsAwayForPassport)) return true;

            return false;
        }

        public static bool IsVisitingSimPort(SimDescription sim)
        {
            if (sim == null) return false;

            if (Passport.Instance != null)
            {
                if (Passport.Instance.IsHostedSim(sim)) return true;
            }

            if (Passport.IsPassportSim(sim)) return true;

            return false;
        }

        public static bool IsLampGenie(SimDescription sim)
        {
            if (!sim.IsGenie) return false;

            OccultGenie occultType = sim.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
            if (occultType == null) return false;

            return occultType.IsTiedToLamp;
        }

        public static bool MatchesAll(SimDescription sim, IEnumerable<SimType> types)
        {
            if (types != null)
            {
                foreach (SimType type in types)
                {
                    if (!Matches(sim, type)) return false;
                }
            }

            return true;
        }

        public static bool MatchesAny(SimDescription sim, IEnumerable<SimType> types, bool resultOnEmpty)
        {
            bool found = false;
            if (types != null)
            {
                foreach (SimType type in types)
                {
                    found = true;
                    if (Matches(sim, type))
                    {
                        return true;
                    }
                }
            }

            if (found) return false;

            return resultOnEmpty;
        }

        public static bool Matches(MiniSimDescription sim, SimType type)
        {
            switch(type)
            {
                case SimType.MiniSim:
                    return true;
                case SimType.Dead:
                    return IsDead(sim);
                case SimType.Alive:
                    return !IsDead(sim);
                case SimType.Service:
                    return sim.IsServicePerson;
                case SimType.Mummy:
                    return sim.IsMummy;
                case SimType.Alien:
                    return sim.IsAlien;
                case SimType.Animal:
                    return !sim.IsHuman;
                case SimType.Fairy:
                    return sim.IsFairy;
                case SimType.Frankenstein:
                case SimType.SimBot:
                    return sim.IsFrankenstein;
                case SimType.PlayableGhost:
                    return sim.IsPlayableGhost;
                case SimType.Unicorn:
                    return sim.IsUnicorn;
                case SimType.Vampire:
                    return sim.IsVampire;
                case SimType.Werewolf:
                    return sim.IsWerewolf;
                case SimType.Witch:
                    return sim.IsWitch;
                case SimType.Genie:
                    return sim.IsGenie;
                case SimType.Mermaid:
                    return sim.IsMermaid;
                case SimType.Human:
                    return sim.IsHuman;
                case SimType.Married:
                    return sim.IsMarried;
                case SimType.Partnered:
                    return sim.HasPartner;
                case SimType.Foreign:
                    return (sim.HomeWorld != GameUtils.GetCurrentWorld());
                case SimType.Local:
                    return (sim.HomeWorld == GameUtils.GetCurrentWorld());
                case SimType.Plumbot:
                    return sim.IsEP11Bot;
                case SimType.Dog:
                    return sim.IsADogSpecies;
                case SimType.Cat:
                    return sim.IsCat;
                case SimType.Horse:
                    return sim.IsHorse;
                case SimType.Deer:
                    return sim.IsDeer;
                case SimType.Raccoon:
                    return sim.IsRaccoon;
                case SimType.Role:
                    return sim.HasAssignedRole;
                case SimType.Employed:
                    return sim.JobIcon != string.Empty;
                case SimType.Unemployed:
                    return sim.JobIcon == string.Empty;
                case SimType.HasDegree:
                    return sim.mDegrees.Count > 0;
                case SimType.Degreeless:
                    return sim.mDegrees.Count == 0;
            }

            return false;
        }
        public static bool Matches(SimDescription sim, SimType type)
        {
            switch (type)
            {
                case SimType.MiniSim:
                    return false;
                case SimType.Dead:
                    return IsDead(sim);
                case SimType.Alive:
                    return !IsDead(sim);
                case SimType.Resident:
                    return (sim.LotHome != null);
                case SimType.Service:
                    return IsService(sim);
                case SimType.Tourist:
                    return IsTourist(sim);
                case SimType.Townie:
                    return ((sim.LotHome == null) && (!IsSpecial(sim)));
                case SimType.Mummy:
                    return sim.IsMummy;
                case SimType.SimBot:
                case SimType.Frankenstein:
                    return sim.IsFrankenstein;
                case SimType.Plumbot:
                    return sim.IsEP11Bot;
                case SimType.TimeTraveler:
                    return sim.IsTimeTraveler;
                case SimType.PlayableGhost:
                    return sim.IsPlayableGhost;
                case SimType.Pregnant:
                    return sim.IsPregnant;
                case SimType.Vampire:
                    return sim.IsVampire;
                case SimType.ImaginaryFriend:
                    return sim.IsImaginaryFriend;
                case SimType.Genie:
                    return sim.IsGenie;
                case SimType.Mermaid:
                    return sim.IsMermaid;
                case SimType.Human:
                    if (!sim.IsHuman) return false;

                    if (sim.IsPlayableGhost) return false;

                    if (sim.OccultManager == null) return true;

                    return !sim.OccultManager.HasAnyOccultType();
                case SimType.Animal:
                    return !sim.IsHuman;
                case SimType.Occult:
                    if (sim.OccultManager == null) return false;

                    return sim.OccultManager.HasAnyOccultType();
                case SimType.Werewolf:
                    return sim.IsWerewolf;
                case SimType.Fairy:
                    return sim.IsFairy;
                case SimType.Witch:
                    return sim.IsWitch;
                case SimType.Zombie:
                    return sim.IsZombie;
                case SimType.BoneHilda:
                    return sim.IsBonehilda;
                case SimType.Unicorn:
                    return sim.IsUnicorn;
                case SimType.Alien:
                    return sim.IsAlien;
                case SimType.Plantsim:
                    return sim.IsPlantSim;
                case SimType.Selectable:
                    return IsSelectable(sim);
                case SimType.NonSelectable:
                    return !IsSelectable(sim);
                case SimType.ActiveFamily:
                    return (Household.ActiveHousehold == sim.Household);
                case SimType.NonActiveFamily:
                    return (Household.ActiveHousehold != sim.Household);
                case SimType.Hybrid:
                    if (sim.OccultManager == null) return false;

                    if (sim.OccultManager.mOccultList == null) return false;

                    return (sim.OccultManager.mOccultList.Count > 1);
                case SimType.Single:
                    return (sim.Partner == null);
                case SimType.Partnered:
                    return (sim.Partner != null);
                case SimType.Steady:
                    if (sim.Partner == null) return false;

                    return !sim.IsMarried;
                case SimType.Married:
                    if (sim.Partner == null) return false;

                    return sim.IsMarried;
                case SimType.Straight:
                    if (sim.IsMale)
                    {
                        if (sim.mGenderPreferenceMale > 0) return false;

                        return (sim.mGenderPreferenceFemale > 0);
                    }
                    else
                    {
                        if (sim.mGenderPreferenceFemale > 0) return false;

                        return (sim.mGenderPreferenceMale > 0);
                    }
                case SimType.Gay:
                    if (sim.IsMale)
                    {
                        if (sim.mGenderPreferenceFemale > 0) return false;

                        return (sim.mGenderPreferenceMale > 0);
                    }
                    else
                    {
                        if (sim.mGenderPreferenceMale > 0) return false;

                        return (sim.mGenderPreferenceFemale > 0);
                    }
                case SimType.Bisexual:
                    if (sim.mGenderPreferenceMale < 0) return false;

                    return (sim.mGenderPreferenceFemale > 0);
                case SimType.Foreign:
                    return (sim.HomeWorld != GameUtils.GetCurrentWorld());
                case SimType.Local:
                    return (sim.HomeWorld == GameUtils.GetCurrentWorld());
                case SimType.StrandedCouple:
                    if (sim.Partner == null) return false;

                    return (sim.Household != sim.Partner.Household);
                case SimType.Dog:
                    return sim.IsADogSpecies;
                case SimType.Cat:
                    return sim.IsCat;
                case SimType.Horse:
                    return sim.IsHorse;
                case SimType.Deer:
                    return sim.IsDeer;
                case SimType.Raccoon:
                    return sim.IsRaccoon;
                case SimType.Role:
                    return sim.HasActiveRole;
                case SimType.WildHorse:
                    return (sim.IsHorse && sim.IsWildAnimal);
                case SimType.Stray:
                    return (!sim.IsHorse && sim.IsWildAnimal);
                case SimType.Employed:
                    return (sim.CareerManager != null && sim.CareerManager.mJob != null);
                case SimType.Unemployed:
                    return (sim.CareerManager != null && sim.CareerManager.mJob == null);
                case SimType.HasDegree:
                    return (sim.CareerManager != null && sim.CareerManager.DegreeManager != null && sim.CareerManager.DegreeManager.GetCompletedDegreeEntries().Count > 0);
                case SimType.Degreeless:
                    return (sim.CareerManager != null && sim.CareerManager.DegreeManager != null && sim.CareerManager.DegreeManager.GetCompletedDegreeEntries().Count == 0);
            }

            return false;
        }

        public static bool IsOccult(IMiniSimDescription sim, OccultTypes type)
        {
            if (sim is MiniSimDescription)
            {
                return IsOccult(sim as MiniSimDescription, type);
            }
            else
            {
                return IsOccult(sim as SimDescription, type);
            }
        }

        public static bool IsOccult(MiniSimDescription sim, OccultTypes type)
        {
            switch (type)
            {
                case OccultTypes.Vampire:
                    return sim.IsVampire;
                case OccultTypes.Frankenstein:
                    return sim.IsFrankenstein;
                case OccultTypes.Mummy:
                    return sim.IsMummy;
                case OccultTypes.Unicorn:
                    return sim.IsUnicorn;
                case OccultTypes.Genie:
                    return sim.IsGenie;
                case OccultTypes.Fairy:
                    return sim.IsFairy;
                case OccultTypes.Ghost:
                    return sim.IsDead || sim.IsPlayableGhost;
                case OccultTypes.Werewolf:
                    return sim.IsWerewolf;
                case OccultTypes.Witch:
                    return sim.IsWitch;
                case OccultTypes.Mermaid:
                    return sim.IsMermaid;
                case OccultTypes.Robot:
                    return sim.IsEP11Bot;
            }

            return false;
        }

        public static bool IsOccult(SimDescription sim)
        {
            return IsOccult(sim, OccultTypes.None);
        }
        public static bool IsOccult(SimDescription sim, OccultTypes type)
        {
            if (sim == null) return false;

            if (sim.OccultManager == null) return false;

            if (type == OccultTypes.None)
            {
                return sim.OccultManager.HasAnyOccultType();
            }
            else
            {
                return sim.OccultManager.HasOccultType(type);
            }
        }

        public static bool IsDead(MiniSimDescription sim)
        {
            if (sim == null) return true;

            if (sim.IsPlayableGhost) return false;

            return sim.IsDead;
        }
        public static bool IsDead(SimDescription sim)
        {
            if (sim == null) return true;

            if (sim.Household == null) return true;

            if (sim.IsPlayableGhost) return false;

            if (sim.IsGhost) return true;

            return sim.IsDead;
        }

        public static bool IsSelectable(Sim sim)
        {
            if (sim == null) return false;

            return IsSelectable(sim.SimDescription);
        }
        public static bool IsSelectable(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.IsNeverSelectable) return false;

            return (sim.Household == Household.ActiveHousehold);
        }

        public static bool IsServiceOrRole(SimDescription sim, bool ignoreResident)
        {
            if (sim == null) return false;

            if (!ignoreResident)
            {
                if (sim.LotHome != null) return false;
            }

            if (sim.AssignedRole != null) return true;

            if (IsService(sim)) return true;

            return false;
        }

        public static bool IsService(SimDescription sim)
        {
            if (sim == null) return false;

            if (IsDead(sim)) return false;

            return IsService(sim.Household);
        }
        public static bool IsService(Household house)
        {
            if (house == null) return true;

            if (house.IsAlienHousehold) return true;

            if (house.IsMermaidHousehold) return true;

            if (house.IsServiceNpcHousehold) return true;

            if (house.IsServobotHousehold) return true;

            return house.IsPetHousehold;
        }

        public static bool InServicePool(SimDescription sim, ServiceType type)
        {
            if (!InServicePool(sim)) return false;

            return (sim.CreatedByService.ServiceType == type);
        }

        public static bool InCarPool(SimDescription sim)
        {
            NpcDriversManager.NpcDrivers type;
            return InCarPool(sim, out type);
        }
        public static bool InCarPool(SimDescription sim, out NpcDriversManager.NpcDrivers type)
        {
            if ((CarNpcManager.Singleton != null) && (CarNpcManager.Singleton.NpcDriversManager != null))
            {
                if (CarNpcManager.Singleton.NpcDriversManager.mDescPools != null)
                {
                    for (int i = 0; i < CarNpcManager.Singleton.NpcDriversManager.mDescPools.Length; i++)
                    {
                        Stack<SimDescription> stack = CarNpcManager.Singleton.NpcDriversManager.mDescPools[i];
                        if (stack == null) continue;

                        foreach (SimDescription stackSim in stack)
                        {
                            if (stackSim == sim)
                            {
                                type = (NpcDriversManager.NpcDrivers)(i + 0x95d01441);
                                return true;
                            }
                        }
                    }
                }

                if ((sim.CreatedSim != null) && (CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers != null))
                {
                    if (CarNpcManager.Singleton.NpcDriversManager.mNpcDrivers.TryGetValue(sim.CreatedSim, out type))
                    {
                        return true;
                    }
                }
            }

            type = NpcDriversManager.NpcDrivers.Taxi;
            return false;
        }

        public static bool IsServiceAlien(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.Household == null) return false;

            return sim.Household.IsAlienHousehold;
        }

        public static bool InServicePool(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.CreatedByService == null) return false;

            return sim.CreatedByService.IsSimDescriptionInPool(sim);
        }

        public static bool IsSpecial(SimDescription sim)
        {
            if (sim == null) return false;

            return IsSpecial(sim.Household);
        }
        public static bool IsSpecial(Household house)
        {
            if (house == null) return true;

            return house.IsSpecialHousehold;
        }

        public static bool IsTourist(SimDescription sim)
        {
            if (sim == null) return false;

            return IsTourist(sim.Household);
        }
        public static bool IsTourist(Household house)
        {
            if (house == null) return false;

            return house.IsTouristHousehold;
        }

        public static bool IsOlderThan(SimDescription description, SimDescription test)
        {
            return (AgingManager.Singleton.GetCurrentAgeInDays(description) > AgingManager.Singleton.GetCurrentAgeInDays(test));
        }

        public static bool IsYoungerThan(SimDescription description, SimDescription test)
        {
            return (AgingManager.Singleton.GetCurrentAgeInDays(description) < AgingManager.Singleton.GetCurrentAgeInDays(test));
        }

        public static SimDescription HeadOfFamily(Household house)
        {
            return HeadOfFamily(house, true);
        }
        public static SimDescription HeadOfFamily(Household house, bool lotFail)
        {
            if (house == null) return null;

            if ((lotFail) && (house.LotHome == null)) return null;

            SimDescription sim = null;
            foreach (SimDescription member in house.SimDescriptions) // All Humans
            {
                if (Household.RoommateManager.IsNPCRoommate(member)) continue;

                if ((sim == null) || (IsOlderThan(member, sim)))
                {
                    sim = member;
                }
            }

            if (sim == null)
            {
                foreach (SimDescription member in house.PetSimDescriptions)
                {
                    if (Household.RoommateManager.IsNPCRoommate(member)) continue;

                    if ((sim == null) || (IsOlderThan(member, sim)))
                    {
                        sim = member;
                    }
                }
            }

            return sim;
        }

        public static bool IsSkinJob(SimDescription sim)
        {
            if ((sim.CreatedByService != null) && (sim.CreatedByService.ServiceType == Sims3.Gameplay.Services.ServiceType.GrimReaper))
            {
                return true;
            }
            else if (sim.IsMummy)
            {
                return true;
            }
            else if (sim.IsRobot)
            {
                return true;
            }
            else if (sim.IsBonehilda)
            {
                return true;
            }

            return false;
        }
    }
}

