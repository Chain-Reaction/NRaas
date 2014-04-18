using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class OccultTypeHelper
    {
        public static List<OccultTypes> CreateList(SimDescription sim)
        {
            return CreateList(sim, false);
        }
        public static List<OccultTypes> CreateList(SimDescription sim, bool onlyTransferable)
        {
            if (sim == null) return new List<OccultTypes>();

            if (sim.OccultManager == null) return new List<OccultTypes>();

            return CreateList(sim.OccultManager.CurrentOccultTypes, onlyTransferable);
        }
        public static List<OccultTypes> CreateList(OccultTypes types)
        {
            return CreateList(types, false);
        }
        public static List<OccultTypes> CreateList(OccultTypes types, bool onlyTransferable)
        {
            List<OccultTypes> results = new List<OccultTypes>();
            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                if (type == OccultTypes.None) continue;

                if ((types & type) != type) continue;

                if (onlyTransferable)
                {
                    if (!OccultManager.DoesOccultTransferToOffspring(type)) continue;
                }

                results.Add(type);
            }

            return results;
        }
        public static List<OccultTypes> CreateListOfAllOccults(bool onlyTransferable)
        {
            List<OccultTypes> results = new List<OccultTypes>();
            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                if (type == OccultTypes.None) continue;                

                if (onlyTransferable)
                {
                    if (!OccultManager.DoesOccultTransferToOffspring(type)) continue;
                }

                results.Add(type);
            }

            return results;
        }
        public static List<OccultTypes> CreateListOfMissingOccults(List<OccultTypes> types, bool onlyTransferable)
        {
            List<OccultTypes> results = new List<OccultTypes>();

            List<OccultTypes> possibleOccults = CreateListOfAllOccults(onlyTransferable);

            foreach (OccultTypes type in possibleOccults)
            {
                if (!types.Contains(type))
                {
                    results.Add(type);
                }
            }

            return results;
        }

        public static bool HasType(Sim sim, OccultTypes type)
        {
            if (sim == null) return false;

            return HasType(sim.SimDescription, type);
        }
        public static bool HasType(SimDescription sim, OccultTypes type)
        {
            if (sim == null) return false;

            if (sim.OccultManager == null) return false;

            return sim.OccultManager.HasOccultType(type);
        }

        public static string GetLocalizedName(OccultTypes type)
        {
            switch (type)
            {
                case OccultTypes.Vampire:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Vampire);
                case OccultTypes.Mummy:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Mummy);
                case OccultTypes.Frankenstein:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Frankenstein);
                case OccultTypes.ImaginaryFriend:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.ImaginaryFriend);
                case OccultTypes.Unicorn:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Unicorn);
                case OccultTypes.Genie:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Genie);
                case OccultTypes.Werewolf:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Werewolf);
                case OccultTypes.Fairy:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Fairy);
                case OccultTypes.Witch:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Witch);
                case OccultTypes.Ghost:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Ghost);
                case OccultTypes.Mermaid:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Mermaid);
                case OccultTypes.PlantSim:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.PlantSim);
                case OccultTypes.TimeTraveler:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.TimeTraveler);
                case OccultTypes.Robot:
                    return OccultManager.GetSingularOccultName(OccultTypesAll.Robot);
                default:
                    return Common.Localize("OccultTypes:None");
            }
        }

        public static bool IsInstalled(OccultTypes type)
        {
            switch (type)
            {
                case OccultTypes.Vampire:
                    return GameUtils.IsInstalled(ProductVersion.EP3 | ProductVersion.EP7);
                case OccultTypes.Mummy:
                    return GameUtils.IsInstalled(ProductVersion.EP1);
                case OccultTypes.Frankenstein:
                    return GameUtils.IsInstalled(ProductVersion.EP2);
                case OccultTypes.ImaginaryFriend:
                    return GameUtils.IsInstalled(ProductVersion.EP4);
                case OccultTypes.Unicorn:
                    return GameUtils.IsInstalled(ProductVersion.EP5);
                case OccultTypes.Genie:
                    return GameUtils.IsInstalled(ProductVersion.EP6);
                case OccultTypes.Werewolf:
                case OccultTypes.Fairy:
                case OccultTypes.Witch:
                    return GameUtils.IsInstalled(ProductVersion.EP7);
                case OccultTypes.PlantSim:
                    return GameUtils.IsInstalled(ProductVersion.EP9);
                case OccultTypes.Mermaid:
                    return GameUtils.IsInstalled(ProductVersion.EP10);
                case OccultTypes.TimeTraveler:
                case OccultTypes.Robot:
                    return GameUtils.IsInstalled(ProductVersion.EP11);
                case OccultTypes.Ghost:
                case OccultTypes.None:
                    return true;
            }

            return false;
        }

        public static bool IsInstalled(SimDescription.DeathType type)
        {
            switch (type)
            {
                case SimDescription.DeathType.OldAge:
                case SimDescription.DeathType.Burn:
                case SimDescription.DeathType.Starve:
                case SimDescription.DeathType.Electrocution:
                case SimDescription.DeathType.Drown:
                case SimDescription.DeathType.None:
                    return GameUtils.IsInstalled(ProductVersion.BaseGame);
                case SimDescription.DeathType.MummyCurse:
                    return GameUtils.IsInstalled(ProductVersion.EP1);
                case SimDescription.DeathType.Meteor:
                    return GameUtils.IsInstalled(ProductVersion.EP2);
                case SimDescription.DeathType.Thirst:
                    return GameUtils.IsInstalled(ProductVersion.EP3) || GameUtils.IsInstalled(ProductVersion.EP7);
                case SimDescription.DeathType.PetOldAgeGood:
                case SimDescription.DeathType.PetOldAgeBad:
                    return GameUtils.IsInstalled(ProductVersion.EP5);
                case SimDescription.DeathType.WateryGrave:
                case SimDescription.DeathType.HumanStatue:
                    return GameUtils.IsInstalled(ProductVersion.EP6);
                case SimDescription.DeathType.Transmuted:
                case SimDescription.DeathType.InvisibleSim:
                case SimDescription.DeathType.HauntingCurse:
                case SimDescription.DeathType.JellyBeanDeath:
                    return GameUtils.IsInstalled(ProductVersion.EP7);
                case SimDescription.DeathType.Freeze:
                    return GameUtils.IsInstalled(ProductVersion.EP8);
                case SimDescription.DeathType.Ranting:
                case SimDescription.DeathType.BluntForceTrauma:
                    return GameUtils.IsInstalled(ProductVersion.EP9);
                case SimDescription.DeathType.MermaidDehydrated:
                case SimDescription.DeathType.ScubaDrown:
                case SimDescription.DeathType.Shark:
                    return GameUtils.IsInstalled(ProductVersion.EP10);
                case SimDescription.DeathType.Causality:
                case SimDescription.DeathType.FutureUrnstoneHologram:
                case SimDescription.DeathType.Jetpack:
                case SimDescription.DeathType.Robot:
                    return GameUtils.IsInstalled(ProductVersion.EP11);
            }

            return false;
        }

        public delegate void Logger(string value);

        public static void ValidateOccult(SimDescription sim, Logger log)
        {
            OccultManager manager = sim.OccultManager;
            if (manager == null)
            {
                manager = new OccultManager(sim);

                if (log != null)
                {
                    log(" Missing Occult Added: " + sim.FullName);
                }
            }

            if (manager.mOccultList == null)
            {
                manager.mOccultList = new List<OccultBaseClass>();

                if (log != null)
                {
                    log(" Missing OccultList Added: " + sim.FullName);
                }
            }

            List<OccultTypes> toAdd = new List<OccultTypes>();

            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                if (type == OccultTypes.None) continue;

                bool found = false;
                foreach (OccultBaseClass occult in manager.mOccultList)
                {
                    if (occult.ClassOccultType == type)
                    {
                        found = true;
                        break;
                    }
                }

                if ((manager.CurrentOccultTypes & type) == type)
                {
                    ApplyTrait(sim, type);

                    if (!found)
                    {
                        toAdd.Add(type);

                        manager.mCurrentOccultTypes &= ~type;
                    }
                }
                else
                {
                    if (found)
                    {
                        manager.mCurrentOccultTypes |= type;

                        if (log != null)
                        {
                            log(" Occult Restored A: " + type.ToString() + " (" + sim.FullName + ")");
                        }
                    }
                }
            }

            foreach (OccultTypes type in toAdd)
            {
                if (!Add(sim, type, false, false)) continue;

                if (log != null)
                {
                    log(" Occult Restored B: " + type.ToString() + " (" + sim.FullName + ")");
                }
            }
        }

        public static void SetupForInstantiatedSim(OccultManager ths)
        {
            if (ths == null) return;

            if (ths.mOccultList == null) return;

            foreach (OccultBaseClass occult in ths.mOccultList)
            {
                try
                {
                    // If the trait is missing, the special motives for Fairies is missing
                    ApplyTrait(ths.mOwnerDescription, occult.ClassOccultType);

                    occult.FixUpOccult(ths.mOwnerDescription);
                    occult.SetupInstantiatedSim(ths.mOwnerDescription.CreatedSim);
                }
                catch (Exception e)
                {
                    Common.Exception(ths.mOwnerDescription, null, "Occult: " + occult.GetType(), e);
                }
            }
        }

        public static bool Add(SimDescription sim, OccultTypes type, bool isReward, bool applyOutfit)
        {            
            try
            {
                if (sim.IsPregnant) return false;

                if (!OccultTypeHelper.IsInstalled(type)) return false;

                if (sim.OccultManager.HasOccultType(type))
                {
                    bool found = false;

                    if (sim.OccultManager.mOccultList != null)
                    {
                        foreach (OccultBaseClass occult in sim.OccultManager.mOccultList)
                        {
                            if (occult.ClassOccultType == type)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found) return false;
                }                

                return AddOccultType(sim.OccultManager, type, applyOutfit, isReward, false, null);
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
        }

        public static void ApplyTrait(SimDescription sim, OccultTypes type)
        {
            TraitNames trait = TraitFromOccult(type);
            if (trait == TraitNames.Unknown) return;

            // Apply the trait using both functions, just in case the user has a tuning mod that mod the trait visible
            sim.TraitManager.AddHiddenElement(trait);
            sim.TraitManager.AddElement(trait);
        }

        // From OccultManager
        protected static bool AddOccultType(OccultManager ths, OccultTypes type, bool addOutfit, bool isReward, bool fromRestore, OccultBaseClass overrideOccultToAdd)
        {            
            OccultBaseClass newOccult = null;
            OccultBaseClass oldOccult = ths.VerifyOccultList(type);
            if (overrideOccultToAdd != null)
            {
                newOccult = overrideOccultToAdd;
            }
            else
            {
                switch (type)
                {
                    case OccultTypes.Mummy:
                        newOccult = new OccultMummy();
                        break;
                    case OccultTypes.Frankenstein:
                        newOccult = new OccultFrankenstein();
                        break;
                    case OccultTypes.Vampire:
                        newOccult = new OccultVampire();
                        break;
                    case OccultTypes.ImaginaryFriend:
                        OccultImaginaryFriend oldImFr = oldOccult as OccultImaginaryFriend;
                        if (oldImFr == null)
                        {
                            newOccult = new OccultImaginaryFriend();
                        }
                        else
                        {
                            newOccult = new OccultImaginaryFriend(oldImFr);
                        }
                        break;
                    case OccultTypes.Unicorn:
                        newOccult = new OccultUnicorn();
                        break;
                    case OccultTypes.Fairy:
                        newOccult = new OccultFairy();
                        break;
                    case OccultTypes.Witch:
                        newOccult = new OccultWitch();
                        break;
                    case OccultTypes.Genie:
                        newOccult = new OccultGenie();
                        break;
                    case OccultTypes.Werewolf:
                        newOccult = new OccultWerewolf();
                        break;
                    case OccultTypes.PlantSim:
                        newOccult = new OccultPlantSim();
                        break;
                    case OccultTypes.Mermaid:
                        newOccult = new OccultMermaid();
                        break;
                    case OccultTypes.TimeTraveler:
                        newOccult = new OccultTimeTraveler();
                        break;
                    case OccultTypes.Robot:
                        newOccult = new OccultRobot();
                        break;
                }
            }

            if (newOccult == null)
            {
                return false;
            }

            OccultTypes originalTypes = ths.mCurrentOccultTypes;
            Role assignedRole = ths.mOwnerDescription.AssignedRole;
            float alienDNAPercentage = ths.mOwnerDescription.AlienDNAPercentage;

            try
            {
                ths.mCurrentOccultTypes = OccultTypes.None;
                ths.mOwnerDescription.AssignedRole = null;
                ths.mOwnerDescription.mAlienDNAPercentage = 0f;

                if (!newOccult.CanAdd(ths.mOwnerDescription, fromRestore))
                {
                    return false;
                }
            }
            finally
            {
                ths.mCurrentOccultTypes = originalTypes;
                ths.mOwnerDescription.AssignedRole = assignedRole;
                ths.mOwnerDescription.mAlienDNAPercentage = alienDNAPercentage;
            }

            if ((ths.mOwnerDescription.SupernaturalData == null) ||
                ((type == OccultTypes.Fairy) && (ths.mOwnerDescription.SupernaturalData.OccultType != OccultTypes.Fairy)) ||
                ((type == OccultTypes.Robot) && (ths.mOwnerDescription.SupernaturalData.OccultType != OccultTypes.Robot)) ||
                ((type == OccultTypes.PlantSim) && (ths.mOwnerDescription.SupernaturalData.OccultType != OccultTypes.PlantSim)))
            {
                ths.mOwnerDescription.AddSupernaturalData(type);
            }

            ths.mIsLifetimeReward = isReward;

            if (type == OccultTypes.Genie)
            {
                // Corrections for improper handling of the special outfits by OccultGenie
                if (ths.mOwnerDescription.mSpecialOutfitIndices == null)
                {
                    ths.mOwnerDescription.mSpecialOutfitIndices = new Dictionary<uint, int>();
                }

                addOutfit = false;
            }

            if (type == OccultTypes.Unicorn)
            {
                OccultUnicornEx.OnAddition(newOccult as OccultUnicorn, ths.mOwnerDescription, addOutfit);
            }

            ApplyTrait(ths.mOwnerDescription, type);

            MidlifeCrisisManager midlifeCrisisManager = ths.mOwnerDescription.MidlifeCrisisManager;

            try
            {
                // Inactive mummies don't agree with mid-life crisis managers
                ths.mOwnerDescription.MidlifeCrisisManager = null;

                newOccult.OnAddition(ths.mOwnerDescription, addOutfit, ths.mIsLifetimeReward, fromRestore);
            }
            finally
            {
                ths.mOwnerDescription.MidlifeCrisisManager = midlifeCrisisManager;
            }

            ths.mOccultList.Add(newOccult);
            ths.mCurrentOccultTypes |= type;
            EventTracker.SendEvent(new BeAnOccultEvent(EventTypeId.kBeAnOccult, ths.mOwnerDescription.CreatedSim, (uint)type));
            if (ths.mOwnerDescription.CreatedSim != null)
            {
                if (!Cane.IsAllowedToUseCane(ths.mOwnerDescription.CreatedSim))
                {
                    Cane.StopUsingAnyActiveCanes(ths.mOwnerDescription.CreatedSim);
                }
                if (!Backpack.IsAllowedToUseBackpack(ths.mOwnerDescription.CreatedSim))
                {
                    Backpack.StopUsingAnyActiveBackpacks(ths.mOwnerDescription.CreatedSim);
                }
                if (!Jetpack.IsAllowedToUseJetpack(ths.mOwnerDescription.CreatedSim))
                {
                    Jetpack.StopUsingAnyActiveJetpacks(ths.mOwnerDescription.CreatedSim);
                }
            }

            (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimDaysPerAgingYearChanged();
            ths.ClearOneShot();
            ths.UpdateOccultUI();            
            if (!fromRestore)
            {
                EventTracker.SendEvent(EventTypeId.kBecameOccult, ths.mOwnerDescription.CreatedSim);                
            }

            if (oldOccult != null)
            {
                newOccult.MergeOccultData(oldOccult);
            }

            if (ths.mOwnerDescription.CreatedSim != null)
            {
                Sim.StandingPosture standing = ths.mOwnerDescription.CreatedSim.Standing as Sim.StandingPosture;
                if (standing != null)
                {
                    standing.SetDefaultIdleAnim();
                }
            }

            return true;
        }

        public static OccultTypes OccultFromTrait(TraitNames trait)
        {
            switch (trait)
            {
                case TraitNames.VampireHiddenTrait:
                    return OccultTypes.Vampire;
                case TraitNames.MummyHiddenTrait:
                    return OccultTypes.Mummy;
                case TraitNames.FrankensteinHiddenTrait:
                    return OccultTypes.Frankenstein;
                case TraitNames.ImaginaryFriendHiddenTrait:
                    return OccultTypes.ImaginaryFriend;
                case TraitNames.UnicornHiddenTrait:
                    return OccultTypes.Unicorn;
                case TraitNames.GenieHiddenTrait:
                    return OccultTypes.Genie;
                case TraitNames.WitchHiddenTrait:
                    return OccultTypes.Witch;
                case TraitNames.LycanthropyHuman:
                    return OccultTypes.Werewolf;
                case TraitNames.FairyHiddenTrait:
                    return OccultTypes.Fairy;
                case TraitNames.PlantSim:
                    return OccultTypes.PlantSim;
                case TraitNames.MermaidHiddenTrait:
                    return OccultTypes.Mermaid;
                case TraitNames.TimeTravelerHiddenTrait:
                    return OccultTypes.TimeTraveler;
                case TraitNames.RobotHiddenTrait:
                    return OccultTypes.Robot;
                default:
                    return OccultTypes.None;
            }
        }

        public static TraitNames TraitFromOccult(OccultTypes occult)
        {
            switch (occult)
            {
                case OccultTypes.Vampire:
                    return TraitNames.VampireHiddenTrait;
                case OccultTypes.Mummy:
                    return TraitNames.MummyHiddenTrait;
                case OccultTypes.Frankenstein:
                    return TraitNames.FrankensteinHiddenTrait;
                case OccultTypes.ImaginaryFriend:
                    return TraitNames.ImaginaryFriendHiddenTrait;
                case OccultTypes.Unicorn:
                    return TraitNames.UnicornHiddenTrait;
                case OccultTypes.Genie:
                    return TraitNames.GenieHiddenTrait;
                case OccultTypes.Witch:
                    return TraitNames.WitchHiddenTrait;
                case OccultTypes.Werewolf:
                    return TraitNames.LycanthropyHuman;
                case OccultTypes.Fairy:
                    return TraitNames.FairyHiddenTrait;
                case OccultTypes.PlantSim:
                    return TraitNames.PlantSim;
                case OccultTypes.Mermaid:
                    return TraitNames.MermaidHiddenTrait;
                case OccultTypes.Robot:
                    return TraitNames.RobotHiddenTrait;
                case OccultTypes.TimeTraveler:
                    return TraitNames.TimeTravelerHiddenTrait;
                default:
                    return TraitNames.Unknown;
            }
        }

        public static bool Remove(SimDescription sim, OccultTypes type, bool alterOutfit)
        {
            if (sim.OccultManager == null) return false;

            if (sim.CreatedSim != null)
            {
                try
                {
                    if (sim.CreatedSim.CurrentOutfitCategory == OutfitCategories.Special)
                    {
                        sim.CreatedSim.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                    }
                }
                catch
                { }
            }

            OccultVampire vampire = sim.OccultManager.GetOccultType(type) as OccultVampire;
            if (vampire != null)
            {
                if ((vampire.mOwningSim == null) || (vampire.mOwningSim.MapTagManager == null))
                {
                    vampire.mPreyMapTag = null;
                }
            }

            if (sim.GetOutfitCount(OutfitCategories.Everyday) == 1)
            {
                SimOutfit outfit = sim.GetOutfit(OutfitCategories.Everyday, 0);

                using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(OutfitCategories.Everyday, 1), outfit))
                { }
            }

            if (type == OccultTypes.Unicorn)
            {
                OccultUnicornEx.OnRemoval(sim);
            }

            bool success = false;

            OccultManager occultManager = sim.OccultManager;

            try
            {
                if (occultManager.HasOccultType(type))
                {
                    occultManager.mCurrentOccultTypes ^= type;
                    foreach (OccultBaseClass occultClass in occultManager.mOccultList)
                    {
                        if (type == occultClass.ClassOccultType)
                        {
                            if ((occultManager.mOwnerDescription.SupernaturalData != null) && (occultManager.mOwnerDescription.SupernaturalData.OccultType == type))
                            {
                                occultManager.mOwnerDescription.RemoveSupernaturalData();
                            }

                            OccultGenie genie = occultClass as OccultGenie;
                            if (genie != null)
                            {
                                OccultGenieEx.OnRemoval(genie, occultManager.mOwnerDescription, alterOutfit);
                            }
                            else
                            {
                                OccultPlantSim plantSim = occultClass as OccultPlantSim;
                                if (plantSim != null)
                                {
                                    OccultPlantSimEx.OnRemoval(plantSim, occultManager.mOwnerDescription, alterOutfit);
                                }
                                else
                                {
                                    occultClass.OnRemoval(occultManager.mOwnerDescription);
                                }
                            }

                            occultManager.mOccultList.Remove(occultClass);
                            occultManager.mIsLifetimeReward = false;
                            if (occultManager.mOccultList.Count == 0x0)
                            {
                                occultManager.mOccultList = null;
                            }
                            break;
                        }
                    }

                    (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimDaysPerAgingYearChanged();

                    occultManager.UpdateOccultUI();

                    EventTracker.SendEvent(EventTypeId.kLostOccult, occultManager.mOwnerDescription.CreatedSim);
                }

                //occultManager.RemoveOccultType(type);
                success = true;
            }
            catch (Exception e)
            {
                bool showError = true;
                switch (type)
                {
                    case OccultTypes.Genie:
                    case OccultTypes.ImaginaryFriend:
                        if (sim.CreatedSim == null)
                        {
                            showError = false;
                        }
                        break;
                }

                if (showError)
                {
                    Common.Exception(sim, e);
                }
            }

            if (!success)
            {
                for (int i = occultManager.mOccultList.Count - 1; i >= 0; i--)
                {
                    if (type == occultManager.mOccultList[i].ClassOccultType)
                    {
                        occultManager.mOccultList.RemoveAt(i);
                    }

                    (Sims3.UI.Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimDaysPerAgingYearChanged();
                    sim.OccultManager.UpdateOccultUI();
                }
            }

            TraitNames trait = TraitFromOccult(type);
            if (trait != TraitNames.Unknown)
            {
                sim.TraitManager.RemoveElement(trait);
            }

            return success;
        }

        public static void TestAndRebuildWerewolfOutfit(SimDescription sim)
        {
            if (!sim.IsWerewolf) return;

            if (sim.GetOutfitCount(OutfitCategories.Supernatural) == 0) return;

            SimOutfit outfit = sim.GetOutfit(OutfitCategories.Supernatural, 0);
            if ((outfit != null) && (outfit.SkinToneKey.InstanceId == 0x0))
            {
                RebuildWerewolfOutfit(sim);
            }
        }

        public static void RebuildWerewolfOutfit(SimDescription sim)
        {
            if (!sim.IsWerewolf) return;

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(OutfitCategories.Supernatural, 0)))
            {
                if (builder.OutfitValid)
                {
                    if (builder.Outfit.SkinToneKey.InstanceId == 0x0)
                    {
                        builder.Builder.SkinTone = sim.SkinToneKey;
                        builder.Builder.SkinToneIndex = sim.SkinToneIndex;
                    }

                    // 0xbfffffffe7ffffffL : CASLogic.sWerewolfPreserveComponents
                    // 0x4000000000000000L : Skin Tone
                    builder.Components = CASLogic.sWerewolfPreserveComponents | 0x4000000000000000L;
                }
            }
        }
    }
}

