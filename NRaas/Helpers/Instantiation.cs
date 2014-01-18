using NRaas.CommonSpace.Stores;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Objects;
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
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Instantiation
    {
        public delegate Sim OnReset(Sim sim, bool fadeOut);

        public static bool EnsureInstantiate(SimDescription sim, Lot lot)
        {
            if (sim.CreatedSim == null)
            {
                if (sim.Household == null)
                {
                    if (!sim.IsValidDescription)
                    {
                        sim.Fixup();
                    }

                    Urnstone urnstone = Urnstones.CreateGrave(sim, SimDescription.DeathType.OldAge, false, true);
                    if (urnstone != null)
                    {
                        Common.Sleep();

                        if (!Urnstones.GhostSpawn(urnstone, lot))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Instantiation.Perform(sim, null);
                }
            }

            return (sim.CreatedSim != null);
        }

        public static Vector3 FindRoutableLocation(IGameObject ths, Vector3 startPoint, out Vector3 fwd)
        {
            Vector3 result = startPoint;

            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(startPoint);
            fglParams.BooleanConstraints = FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.PreferEmptyTiles;
            if (!GlobalFunctions.FindGoodLocation(ths, fglParams, out result, out fwd))
            {
                fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                GlobalFunctions.FindGoodLocation(ths, fglParams, out result, out fwd);
            }

            return result;
        }

        public static bool AttemptToPutInSafeLocation(Sim ths, bool allowHibernate)
        {
            return AttemptToPutInSafeLocation(ths, Vector3.Invalid, allowHibernate);
        }
        public static bool AttemptToPutInSafeLocation(Sim ths, Vector3 destination, bool allowHibernate)
        {
            if ((ths == null) || (ths.HasBeenDestroyed)) return false;

            if ((ths.LotHome == null) || (destination != Vector3.Invalid))
            {
                Lot lot = ths.LotHome;
                if (lot == null)
                {
                    lot = ths.VirtualLotHome;
                }

                Vector3 pos = destination;

                if (pos == Vector3.Invalid)
                {
                    pos = AttemptToFindSafeLocation(lot, ths.IsHorse);
                }

                if (pos != Vector3.Invalid)
                {
                    try
                    {
                        Vector3 fwd;
                        ths.SetPosition(FindRoutableLocation(ths, pos, out fwd));
                        ths.SetForward(fwd);
                        ths.RemoveFromWorld();
                        ths.AddToWorld();
                        ths.SetHiddenFlags(HiddenFlags.Nothing);
                        ths.SetOpacity(1f, 0f);
                        ths.SimRoutingComponent.ForceUpdateDynamicFootprint();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(ths, e);
                    }

                    return true;
                }
                else
                {
                    if (allowHibernate)
                    {
                        ths.FadeOut(false, true);
                    }
                    return false;
                }
            }

            Vector3 originalPos = ths.Position;

            try
            {
                if (!ths.AttemptToPutInSafeLocation(true)) return false;
            }
            catch (Exception e)
            {
                Common.DebugException(ths, e);
                return false;
            }

            return (ths.Position != originalPos);
        }

        public static Vector3 AttemptToFindSafeLocation(Lot lot, bool isHorse)
        {
            if (lot == null) return Vector3.Invalid;

            if (isHorse)
            {
                Mailbox mailbox = lot.FindMailbox();
                if (mailbox != null)
                {
                    return mailbox.Position;
                }
                else
                {
                    Door frontDoor = lot.FindFrontDoor();
                    if (frontDoor != null)
                    {
                        int roomId = frontDoor.GetRoomIdOfDoorSide(CommonDoor.tSide.Front);
                        if (roomId != 0)
                        {
                            roomId = frontDoor.GetRoomIdOfDoorSide(CommonDoor.tSide.Back);
                        }

                        if (roomId == 0)
                        {
                            List<GameObject> objects = lot.GetObjectsInRoom<GameObject>(roomId);
                            if (objects.Count > 0)
                            {
                                return RandomUtil.GetRandomObjectFromList(objects).Position;
                            }
                        }
                    }
                }
            }

            return lot.EntryPoint();
        }

        public static Vector3 GetPositionInRandomLot(Lot lot)
        {
            List<Lot> lots = new List<Lot>(LotManager.sLots.Values);
            lots.Remove(lot);

            if (lots.Count == 0)
            {
                lots.Add(lot);
            }

            return Service.GetPositionInRandomLot(RandomUtil.GetRandomObjectFromList(lots));
        }

        public static Sim PerformOffLot(SimDescription ths, Lot lot, OnReset reset)
        {
            return PerformOffLot(ths, lot, null, reset);
        }
        public static Sim PerformOffLot(SimDescription ths, Lot lot, SimOutfit outfit, OnReset reset)
        {
            if ((ths.CreatedSim != null) && (reset == null)) return ths.CreatedSim;

            return Perform(ths, GetPositionInRandomLot(lot), outfit, reset);
        }

        public static Sim Perform(SimDescription ths, OnReset reset)
        {
            if (ths == null) return null;

            Lot lot = ths.LotHome;
            if (lot == null)
            {
                lot = ths.VirtualLotHome;
            }

            return Perform(ths, lot, reset);
        }
        public static Sim Perform(SimDescription ths, Lot lot, OnReset reset)
        {
            Vector3 result = Vector3.Invalid;
            if (lot != null)
            {
                result = AttemptToFindSafeLocation(lot, ths.IsHorse);
                if (result == Vector3.Invalid)
                {
                    result = lot.EntryPoint();
                }
            }
            else
            {
                result = GetPositionInRandomLot(lot);
            }

            return Perform(ths, result, null, reset);
        }
        public static Sim Perform(SimDescription ths, Vector3 position, SimOutfit outfit, OnReset reset)
        {
            try
            {
                ResourceKey outfitKey = ths.mDefaultOutfitKey;

                if (outfit == null)
                {
                    if (ths.IsHorse)
                    {
                        outfit = ths.GetOutfit(OutfitCategories.Naked, 0);
                    }
                    
                    if (outfit == null)
                    {
                        outfit = ths.GetOutfit(OutfitCategories.Everyday, 0);
                    }

                    if ((outfit == null) || (!outfit.IsValid))
                    {
                        return null;
                    }
                }

                if (outfit != null)
                {
                    outfitKey = outfit.Key;
                }

                return Perform(ths, position, outfitKey, true, reset);
            }
            catch (Exception e)
            {
                ths.mSim = null;

                ths.mDefaultOutfitKey = ResourceKey.kInvalidResourceKey;

                Common.Exception(ths, e);
            }

            return null;
        }

        // From SimDescription.Instantiate
        private static Sim Perform(SimDescription ths, Vector3 position, ResourceKey outfitKey, bool forceAlwaysAnimate, OnReset reset)
        {
            Household.HouseholdSimsChangedCallback changedCallback = null;
            Household changedHousehold = null;

            bool isChangingWorlds = GameStates.sIsChangingWorlds;

            bool isLifeEventManagerEnabled = LifeEventManager.sIsLifeEventManagerEnabled;

            Corrections.RemoveFreeStuffAlarm(ths);

            using (SafeStore store = new SafeStore(ths, SafeStore.Flag.LoadFixup | SafeStore.Flag.Selectable | SafeStore.Flag.Unselectable))
            {
                try
                {
                    // Stops the memories system from interfering
                    LifeEventManager.sIsLifeEventManagerEnabled = false;

                    // Stops UpdateInformationKnownAboutRelationships()
                    GameStates.sIsChangingWorlds = true;

                    if (ths.Household != null)
                    {
                        changedCallback = ths.Household.HouseholdSimsChanged;
                        changedHousehold = ths.Household;

                        ths.Household.HouseholdSimsChanged = null;
                    }

                    if (ths.CreatedSim != null)
                    {
                        AttemptToPutInSafeLocation(ths.CreatedSim, false);

                        if (reset != null)
                        {
                            ths.CreatedSim.SetObjectToReset();

                            reset(ths.CreatedSim, false);
                        }

                        return ths.CreatedSim;
                    }

                    if (ths.AgingState != null)
                    {
                        bool flag = outfitKey == ths.mDefaultOutfitKey;

                        ths.AgingState.SimBuilderTaskDeferred = false;

                        ths.AgingState.PreInstantiateSim(ref outfitKey);
                        if (flag)
                        {
                            ths.mDefaultOutfitKey = outfitKey;
                        }
                    }

                    int capacity = forceAlwaysAnimate ? 0x4 : 0x2;
                    Hashtable overrides = new Hashtable(capacity);
                    overrides["simOutfitKey"] = outfitKey;
                    overrides["rigKey"] = CASUtils.GetRigKeyForAgeGenderSpecies((ths.Age | ths.Gender) | ths.Species);
                    if (forceAlwaysAnimate)
                    {
                        overrides["enableSimPoseProcessing"] = 0x1;
                        overrides["animationRunsInRealtime"] = 0x1;
                    }

                    string instanceName = "GameSim";
                    ProductVersion version = ProductVersion.BaseGame;
                    if (ths.Species != CASAgeGenderFlags.Human)
                    {
                        instanceName = "Game" + ths.Species;
                        version = ProductVersion.EP5;
                    }

                    SimInitParameters initData = new SimInitParameters(ths);
                    Sim target = GlobalFunctions.CreateObjectWithOverrides(instanceName, version, position, 0x0, Vector3.UnitZ, overrides, initData) as Sim;
                    if (target != null)
                    {
                        if (target.SimRoutingComponent == null)
                        {
                            // Performed to ensure that a useful error message is produced when the Sim construction fails
                            target.OnCreation();
                            target.OnStartup();
                        }

                        target.SimRoutingComponent.EnableDynamicFootprint();
                        target.SimRoutingComponent.ForceUpdateDynamicFootprint();

                        ths.PushAgingEnabledToAgingManager();

                        /* This code is idiotic
                        if ((ths.Teen) && (target.SkillManager != null))
                        {
                            Skill skill = target.SkillManager.AddElement(SkillNames.Homework);
                            while (skill.SkillLevel < SimDescription.kTeenHomeworkSkillStartLevel)
                            {
                                skill.ForceGainPointsForLevelUp();
                            }
                        }
                        */

                        // Custom
                        OccultTypeHelper.SetupForInstantiatedSim(ths.OccultManager);

                        if (ths.IsAlien)
                        {
                            World.ObjectSetVisualOverride(target.ObjectId, eVisualOverrideTypes.Alien, null);
                        }

                        AttemptToPutInSafeLocation(target, false);

                        EventTracker.SendEvent(EventTypeId.kSimInstantiated, null, target);

                        /*
                        MiniSimDescription description = MiniSimDescription.Find(ths.SimDescriptionId);
                        if ((description == null) || (!GameStates.IsTravelling && (ths.mHomeWorld == GameUtils.GetCurrentWorld())))
                        {
                            return target;
                        }
                        description.UpdateInWorldRelationships(ths);
                        */

                        if (ths.HealthManager != null)
                        {
                            ths.HealthManager.Startup();
                        }

                        if (((ths.SkinToneKey.InstanceId == 15475186560318337848L) && !ths.OccultManager.HasOccultType(OccultTypes.Vampire)) && (!ths.OccultManager.HasOccultType(OccultTypes.Werewolf) && !ths.IsGhost))
                        {
                            World.ObjectSetVisualOverride(ths.CreatedSim.ObjectId, eVisualOverrideTypes.Genie, null);
                        }

                        if (ths.Household.IsAlienHousehold)
                        {
                            (Sims3.UI.Responder.Instance.HudModel as HudModel).OnSimCurrentWorldChanged(true, ths);
                        }

                        if (Household.RoommateManager.IsNPCRoommate(ths.SimDescriptionId))
                        {
                            Household.RoommateManager.AddRoommateInteractions(target);
                        }
                    }

                    return target;
                }
                finally
                {
                    LifeEventManager.sIsLifeEventManagerEnabled = isLifeEventManagerEnabled;

                    GameStates.sIsChangingWorlds = isChangingWorlds;

                    if ((changedHousehold != null) && (changedCallback != null))
                    {
                        changedHousehold.HouseholdSimsChanged = changedCallback;

                        if (changedHousehold.HouseholdSimsChanged != null)
                        {
                            changedHousehold.HouseholdSimsChanged(Sims3.Gameplay.CAS.HouseholdEvent.kSimAdded, ths.CreatedSim, null);
                        }
                    }
                }
            }
        }        
    }
}

