using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Door;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class DoorPortalComponentEx : Door.DoorPortalComponent
    {
        public static List<ulong> mRegistered = new List<ulong>();

        public DoorPortalComponentEx()
        { }
        public DoorPortalComponentEx(GameObject obj)
            : base(obj)
        {
            this.OwnerDoor = obj as Door;
        }

        public override bool OnApproachingPortalObject(Sim sim)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(this.OwnerDoor.ObjectId, false);
            bool allowed = true;
            if (settings != null)
            {
                allowed = settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);

                if (settings.mIsOneWayDoor)
                {
                    CommonDoor.tSide sideInRoom = CommonDoor.tSide.Front;
                    this.OwnerDoor.GetSideOfDoorInRoom(sim.RoomId, out sideInRoom);
                    if (sideInRoom != CommonDoor.tSide.Back)
                    {
                        return false;
                    }
                }
            }

            return (!allowed ? allowed : base.OnApproachingPortalObject(sim));
        }

        public override void OnLaneLocked(Sim sim, LaneInfo info)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(this.OwnerDoor.ObjectId);

            bool allowed = true;
            if (settings != null)
            {
                if (settings.mIsOneWayDoor && (info.mLaneSlots[0] != Door.RoutingSlots.Door1_Rear && info.mLaneSlots[0] != Door.RoutingSlots.Door0_Rear))
                {
                    allowed = false;
                }

                allowed = allowed && settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
            }

            if (!allowed)
            {
                if (sim.SimRoutingComponent != null && sim.SimRoutingComponent.IsRouting)
                {
                    sim.SimRoutingComponent.GetCurrentRoute().DoRouteFail = false;
                }
                return;
            }

            if ((info.mLaneSlots[0] == Door.RoutingSlots.Door0_Rear || info.mLaneSlots[0] == Door.RoutingSlots.Door1_Rear) && settings.mDoorCost > 0 && !settings.WasSimRecentlyLetThrough(sim.SimDescription.SimDescriptionId))
            {
                settings.AddRecentSim(sim.SimDescription.SimDescriptionId);
                GoHere.Settings.AddOrUpdateDoorSettings(this.OwnerDoor.ObjectId, settings, false);

                settings.HandleCost(sim);
            }

            base.OnLaneLocked(sim, info);
        }

        public static string SceneWindow_Hover(IScriptProxy o, ScenePickArgs args)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(o.ObjectId, false);

            if (settings == null) return string.Empty;

            string tooltip = string.Empty;

            if (settings.ActiveFilters.Count > 0)
            {
                string critera = FilterHelper.GetFilterAsLocalizedCriteria(settings.ActiveFilters);
                if (critera.Length > 3)
                {
                    tooltip = GoHere.Localize("DoorCriteria:ToolTip") + " (" + GoHere.Localize("DoorFilterType:" + settings.mType.ToString()) + ")" + Common.NewLine + critera;
                }
            }

            if (settings.mDoorOpen > -1 && settings.mDoorClose > -1)
            {
                if (tooltip != string.Empty)
                {
                    tooltip = tooltip + Common.NewLine;
                }

                tooltip = tooltip + GoHere.Localize("OpenCloseTime:MenuName") + ": " + settings.mDoorOpen.ToString() + " - " + settings.mDoorClose.ToString();
            }

            if (settings.mDoorCost > 0)
            {
                if (tooltip != string.Empty)
                {
                    tooltip = tooltip + Common.NewLine;
                }

                tooltip = tooltip + GoHere.Localize("DoorCost:MenuName") + ": " + settings.mDoorCost.ToString();
            }

            if (settings.mIsOneWayDoor)
            {
                if (tooltip != string.Empty)
                {
                    tooltip = tooltip + Common.NewLine;
                }

                tooltip = tooltip + GoHere.Localize("OneWayDoor:MenuName");
            }

            return tooltip;
        }
        
        public static void AboutToPlanRouteCallback(Route r, string routeType, Vector3 point)
        {
            if (r == null || r.Follower == null) return;

            Sim router = r.Follower.Target as Sim;
            if (router == null) return;

            Lot lot = router.LotCurrent;
            if (lot == null) return;

            InteractionInstance instance = router.CurrentInteraction;

            int destRoom = -1;
            if (instance != null && instance.Target != null)
            {
                destRoom = instance.Target.RoomId;
            }

            if (destRoom == -1) return;

            if (instance.Autonomous) return;

            //Common.Notify(router.FullName + " trying to route to " + destRoom);

            /*
            int depth = 5;
            int currentRoom = 0;            
            while (depth < 5)
            {
                if (currentRoom == 0) currentRoom = destRoom;
                bool allow = false;
                foreach (CommonDoor door in lot.GetObjectsInRoom<CommonDoor>(currentRoom))
            {
                if (door != null)
                {
                        CommonDoor.tSide side;
                        door.GetSideOfDoorInRoom(currentRoom, out side);
                        if (side != CommonDoor.tSide.Front) continue;

                    DoorSettings settings = GoHere.Settings.GetDoorSettings(door.ObjectId, false);
                    if (settings != null)
                    {
                            if (!DoorSettings.TestPortalObject(router, currentRoom, door))
                            {
                               // r.AddObjectToForbiddenPortalList(door.ObjectId);
                                //r.DoRouteFail = false;
                                continue;
                            }
                        }

                        allow = true;
                        break;
                    }
                }

                if (!allow) router.AddExitReason(ExitReason.CancelExternal);
                depth++;
            }
             */

            //bool allow = false;
            foreach (CommonDoor door in lot.GetObjects<CommonDoor>()) // lot.GetObjectsInRoom<CommonDoor>(destRoom)
            {
                if (door != null)
                {
                    DoorSettings settings = GoHere.Settings.GetDoorSettings(door.ObjectId, false);
                    if (settings != null)
                    {
                        if (!DoorSettings.TestPortalObject(router, door.RoomId, door))
                        {
                            if (r == null) return;

                            r.AddObjectToForbiddenPortalList(door.ObjectId);
                            r.DoRouteFail = false; // this seems to be ignored...

                            // for minor zerbu compatability, stops route fail
                            /*
                            if (destRoom == door.RoomId)
                            {
                                Common.Notify(router.FullName + ": denied");
                                router.AddExitReason(ExitReason.CancelExternal);
                                router.InteractionQueue.CancelAllInteractions();
                        }
                             */ 
                            //router.AddExitReason(ExitReason.CancelExternal);
                            continue;
                    }
                }

                    //allow = true;
                    //break;
                }
            }

            //if (allow) { }

            /*

            if (instance != null && Sims3.SimIFace.Route.IsRouteToObjectUnobstructed(router.ObjectId, instance.Target.Position, router.GetRouteOptions(), instance.Target.ObjectId, router.SimDescription.AgeGenderSpecies) != BlockedStatus.Unobstructed)
            {
                //Common.Notify("Route obstructructed: " + router.SimDescription.FullName + " going to " + instance.Target.GetLocalizedName() + " in " + destRoom);
                router.AddExitReason(ExitReason.CanceledByScript);
                if (!router.IsInActiveHousehold)
                {
                    router.InteractionQueue.CancelAllInteractions();
            }
        }    
         */
        }


        [Persistable]
        public class DoorSettings
        {
            public DoorSettings()
            {
            }

            public DoorSettings(ObjectGuid guid)
            {
                mGUID = guid;

                Door door = GameObject.GetObject(guid) as Door;
                if (door != null)
                {
                    RegisterRoomListeners(door.LotCurrent);
                }
            }

            public enum SettingType : uint
            {
                Unset = 0,
                Allow = 1,
                Deny = 2
            }

            private List<string> mActiveFilters = new List<string>();
            private Dictionary<ulong, long> mSimsRecentlyLetThrough = new Dictionary<ulong, long>();
            public bool mMatchAllFilters = false;
            public bool mIsOneWayDoor = false;
            public SettingType mType = SettingType.Deny;
            ObjectGuid mGUID;            
            public int mDoorOpen = -1;
            public int mDoorClose = -1;
            public int mDoorCost = 0;
            public int mDoorTicketDuration = 120;

            public static void ValidateAndSetupDoors()
            {                
                List<ulong> removeSim = new List<ulong>();

                foreach (ObjectGuid guid in new List<ObjectGuid>(GoHere.Settings.mDoorSettings.Keys))
                {
                    DoorSettings settings = GoHere.Settings.mDoorSettings[guid];

                    if (Simulator.GetProxy(guid) != null && settings.SettingsValid)
                    {
                        foreach (KeyValuePair<ulong, long> sims in settings.mSimsRecentlyLetThrough)
                        {
                            if (!settings.WasSimRecentlyLetThrough(sims.Key))
                            {
                                removeSim.Add(sims.Key);
                            }
                            else if (MiniSimDescription.Find(sims.Key) == null)
                            {
                                removeSim.Add(sims.Key);
                            }
                        }

                        foreach (ulong sim in removeSim)
                        {
                            settings.mSimsRecentlyLetThrough.Remove(sim);
                        }

                        GoHere.Settings.AddOrUpdateDoorSettings(guid, settings, false);

                        Door door = GameObject.GetObject(guid) as Door;

                        if (door != null)
                        {
                            RegisterRoomListeners(door.LotCurrent);
                        }

                        continue;
                    }

                    GoHere.Settings.mDoorSettings.Remove(guid);
                }

                TooltipHelper.AddListener(typeof(Door), Type.GetType("NRaas.GoHereSpace.Helpers.DoorPortalComponentEx,NRaasGoHere").GetMethod("SceneWindow_Hover"));
            }            

            public bool IsSimAllowedThrough(ulong descId)
            {
                IMiniSimDescription desc = SimDescription.Find(descId);

                if (desc == null)
                {
                    desc = MiniSimDescription.Find(descId) as MiniSimDescription;
                }

                Door door = GameObject.GetObject(mGUID) as Door;

                if (door != null && door.LotCurrent != null && desc != null)
                {
                    SimDescription desc2 = desc as SimDescription;
                    if (desc2 != null && desc2.CreatedSim != null)
                    {
                        if (desc2.Service != null)
                        {
                            List<Sim> sims = desc2.Service.GetSimsAssignedToLot(door.LotCurrent);
                            if (sims.Contains(desc2.CreatedSim) && GoHere.Settings.mServiceSimsIgnoreAllDoorOptions)
                            {
                                return true;
                            }
                        }

                        if (desc2.HasActiveRole)
                        {
                            if (desc2.AssignedRole != null && desc2.AssignedRole.RoleGivingObject != null)
                            {
                                if (desc2.AssignedRole.RoleGivingObject.InWorld && desc2.AssignedRole.RoleGivingObject.LotCurrent != null)
                                {
                                    if (desc2.AssignedRole.RoleGivingObject.LotCurrent == door.LotCurrent && GoHere.Settings.mRoleSimsIgnoreAllDoorOptions)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    if (GoHere.Settings.mGlobalIgnoreAllDoorOptionsFilterOption.Count > 0)
                    {
                        if (FilterHelper.DoesSimMatchFilters(descId, GoHere.Settings.mGlobalIgnoreAllDoorOptionsFilterOption, false))
                        {
                            return true;
                        }
                    }

                    if (mDoorOpen != -1 && mDoorClose != -1)
                    {
                        bool allowed = true;
                        if (!SimClock.IsTimeBetweenTimes(mDoorOpen, mDoorClose))
                        {
                            allowed = false;

                            if (GoHere.Settings.mGlobalIgnoreDoorTimeLocksFilterOption.Count > 0)
                            {
                                allowed = FilterHelper.DoesSimMatchFilters(descId, GoHere.Settings.mGlobalIgnoreDoorTimeLocksFilterOption, false);
                            }

                            /*
                             * Needs more thought, crashes game due to routing system being confused as to whether sim can route despite it being closed.
                            if (!allowed && desc2 != null && desc2.CreatedSim != null && desc2.CreatedSim.LotCurrent != null && !desc2.CreatedSim.IsRouting)
                            {
                                if (VenueFlowUtility.IsSimInsideVenue(desc2.CreatedSim, desc2.CreatedSim.LotCurrent))
                                {
                                    if (desc2.CreatedSim.LotCurrent == door.LotCurrent && desc2.CreatedSim.RoomId == door.RoomId)
                                    {
                                        //Sims3.Gameplay.Situations.PrivacySituation.RouteToAdjacentRoom(desc2.CreatedSim);
                                        CommonDoor.tSide side;
                                        door.GetSideOfDoorInRoom(desc2.CreatedSim.RoomId, out side);
                                        if (side == CommonDoor.tSide.Front)
                                        {
                                            allowed = true;
                                            door.RouteThroughDoor(desc2.CreatedSim);
                                        }                                        
                                    }
                                }
                                     * Nice to have but need to think about performance implications first
                                else
                                {
                                    Sim.MakeSimGoHome(desc2.CreatedSim, false);
                            }
                            }
                        */

                            /*
                            if (PlumbBob.SelectedActor != null && desc.HasSameHomeLot(PlumbBob.SelectedActor.SimDescription))
                            {
                                allowed = true;
                            }
                            
                            if (desc2 != null && desc2.CreatedSim != null)
                            {
                                if (desc2.CareerManager != null)
                                {
                                    if (desc2.CareerManager.OccupationAsActiveCareer != null)
                                    {
                                        if (desc2.CareerManager.OccupationAsActiveCareer.HasJobAtLot(door.LotCurrent))
                                        {
                                            allowed = true;
                                        }
                                    }

                                    if (desc2.CreatedSim.CareerLocation != null)
                                    {
                                        if (desc2.CreatedSim.CareerLocation.LotCurrent == door.LotCurrent)
                                        {
                                            allowed = true;
                                        }
                                    }
                                }
                            }
                             */
                        }

                        if (!allowed) return allowed;
                    }

                    if (FiltersEnabled > 0)
                    {
                        if (GoHere.Settings.mGlobalIgnoreDoorFiltersFilterOption.Count > 0)
                        {
                            if (FilterHelper.DoesSimMatchFilters(descId, GoHere.Settings.mGlobalIgnoreDoorFiltersFilterOption, false))
                            {
                                return true;
                            }
                        }

                        if (mMatchAllFilters)
                        {
                            if (mType == SettingType.Allow)
                            {
                                return FilterHelper.DoesSimMatchFilters(descId, mActiveFilters, true);
                            }
                            else
                            {
                                return !FilterHelper.DoesSimMatchFilters(descId, mActiveFilters, true);
                            }
                        }
                        else
                        {
                            if (mType == SettingType.Allow)
                            {
                                return FilterHelper.DoesSimMatchFilters(descId, mActiveFilters, false);
                            }
                            else
                            {
                                return !FilterHelper.DoesSimMatchFilters(descId, mActiveFilters, false);
                            }
                        }
                    }
                }

                return (FiltersEnabled == 0);
            }

            public DoorSettings AddFilter(string filter)
            {
                if (!IsFilterActive(filter))
                {
                    mActiveFilters.Add(filter);
                }

                return this;
            }

            public DoorSettings RemoveFilter(string filter)
            {
                mActiveFilters.Remove(filter);

                return this;
            }

            public bool IsFilterActive(string filter)
            {
                return mActiveFilters.Contains(filter);
            }

            public void ClearFilters()
            {
                mActiveFilters.Clear();
            }

            public List<string> ActiveFilters
            {
                get { return mActiveFilters; }
            }

            public int FiltersEnabled
            {
                get { return mActiveFilters.Count; }
            }

            public bool SettingsValid
            {
                get { return (FiltersEnabled > 0 || mDoorCost > 0 || mDoorOpen > -1 || mDoorClose > -1 || mIsOneWayDoor); }
            }

            public void AddRecentSim(ulong sim)
            {
                if (sim != 0 && !mSimsRecentlyLetThrough.ContainsKey(sim))
                {
                    mSimsRecentlyLetThrough.Add(sim, SimClock.CurrentTicks);
                }
            }

            public bool WasSimRecentlyLetThrough(ulong sim)
            {
                long time;
                if (mSimsRecentlyLetThrough.TryGetValue(sim, out time) && ((time + (SimClock.kSimulatorTicksPerSimMinute * mDoorTicketDuration)) > SimClock.CurrentTicks))
                {
                    return true;
                }
                
                return false;
            }

            public bool HandleCost(Sim sim)
            {
                if (sim != null && sim.SimDescription.ChildOrAbove && mDoorCost > 0)
                {
                    if (GoHere.Settings.mGlobalIgnoreDoorCostFilterOption.Count > 0)
                    {
                        if (FilterHelper.DoesSimMatchFilters(sim.SimDescription.SimDescriptionId, GoHere.Settings.mGlobalIgnoreDoorCostFilterOption, false))
                        {
                            return false;
                        }
                    }

                    Household owningHousehold = sim.LotCurrent.Household;
                    if (owningHousehold == null)
                    {
                        List<PropertyData> list = RealEstateManager.AllPropertiesFromAllHouseholds();
                        for (int k = 0; k < list.Count; k++)
                        {
                            if (((list[k] != null) && (sim.LotCurrent.LotId == list[k].LotId)) && ((list[k].Owner != null) && (list[k].Owner.OwningHousehold != null)))
                            {
                                owningHousehold = list[k].Owner.OwningHousehold;
                                break;
                            }
                        }
                    }

                    if (sim.Household == null || (sim.Household != null && owningHousehold != sim.Household))
                    {
                        if (sim.FamilyFunds > mDoorCost)
                        {
                            sim.ModifyFunds(-mDoorCost);
                        }
                        else
                        {
                            sim.UnpaidBills += mDoorCost;
                        }

                        if (owningHousehold != null)
                        {
                            owningHousehold.ModifyFamilyFunds(mDoorCost);
                        }

                        return true;
                    }                    
                }

                return false;
            }

            public static void RegisterRoomListeners(Lot lot)
            {
                if (lot != null && !mRegistered.Contains(lot.LotId) && !lot.IsWorldLot)
                {
                    LotDisplayLevelInfo info = World.LotGetDisplayLevelInfo(lot.LotId);

                    for (int i = info.mMin; i <= info.mMax; i++)
                    {
                        foreach (int room in World.GetInsideRoomsAtLevel(lot.LotId, i, eRoomDefinition.LightBlocking))
                        {
                            List<Lot.OnAllowedInRoomCheck> list;
                            if (!lot.mRoomRestrictionCheckCallbacks.TryGetValue(room, out list))
                            {
                                list = new List<Lot.OnAllowedInRoomCheck>();
                                lot.mRoomRestrictionCheckCallbacks.Add(room, list);
                            }
                            if (!list.Contains(new Lot.OnAllowedInRoomCheck(OnAllowedInRoomCheck)))
                            {
                                list.Add(new Lot.OnAllowedInRoomCheck(OnAllowedInRoomCheck));
                            }
                        }
                    }


                    if (lot.mOnPortalPathPlanHandler == null)
                    {
                        Route.SetPortalLockStatus(lot.mLotId, true);
                    }

                    lot.mOnPortalPathPlanHandler = (PortalPathPlanHandler)Delegate.Combine(lot.mOnPortalPathPlanHandler, new PortalPathPlanHandler(OnPortalPathPlan));


                    foreach (Sim sim in lot.GetAllActors())
                    {
                        if (sim != null)
                        {
                            sim.mAllowedRooms.Remove(lot.LotId);
                        }
                    }

                    mRegistered.Add(lot.LotId);
                }                             
            }

            public static void OnPortalPathPlan(PortalPlanEventArgs args)
            {
                if (args.Actor != null && args.PortalObject != null)
                {
                    CommonDoor portalObject = args.PortalObject as CommonDoor;
                    if (portalObject != null)
                    {                        
                        if (!TestPortalObject(args.Actor, args.RoomTo, portalObject))
                        {
                            if (portalObject.LotCurrent == LotManager.ActiveLot)
                            {
                                //Common.Notify(args.Actor.FirstName + " currently in " + args.Actor.RoomId + " routing from " + args.RoomFrom + " to " + args.RoomTo + " not allowed. Locking " + portalObject.CatalogName);
                            }
                            args.Locked = true;
                            args.Actor.SimRoutingComponent.LockedDoorsDuringPlan.Add(args.PortalObject as Door);
                        }
                    }
                }
            }

            public static bool TestPortalObject(Sim sim, int dest, CommonDoor obj)
            {
                if (obj == null || sim == null) return true;

                CommonDoor.tSide sideInRoom = CommonDoor.tSide.Front;
                obj.GetSideOfDoorInRoom(dest, out sideInRoom);

                DoorSettings settings = GoHere.Settings.GetDoorSettings(obj.ObjectId, false);

                if (settings != null)
                {
                    if (settings.mIsOneWayDoor && sideInRoom != CommonDoor.tSide.Front)
                    {
                        return false;
                    }

                    return settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
                }

                return true;
            }

            public static bool CheckRoomDoors(int room, Sim sim, out int level)
            {
                foreach (CommonDoor obj in sim.LotCurrent.GetObjectsInRoom<CommonDoor>(room))
                {
                    level = obj.Level;
                    if (!TestPortalObject(sim, obj.RoomId, obj))
                    {
                        return false;
                    }
                }

                level = -1;

                return true;
            }

            public static bool OnAllowedInRoomCheck(int srcRoom, Sim sim)
            {
                //if (sim.LotCurrent != LotManager.ActiveLot) return true;

                if (sim != null && sim.LotCurrent != null)
                {
                    if (sim.RoomId == srcRoom) return true;
                    //bool allowed = false;
                    //bool adjoiningAllowed = false;
                    //bool allNull = true;

                    // lets try the simple methods first... is sim allowed to exit the room they are in?
                    int level;
                    if (!CheckRoomDoors(sim.RoomId, sim, out level))
                    {
                        return false;
                    }

                    // is sim allowed in room they are going to?
                    level = 0;
                    if (!CheckRoomDoors(srcRoom, sim, out level))
                    {
                        return false;
                    }

                    /*
                    // not doing the transversal level dance, let the portal plan code deal with it
                    if(level != sim.Level) return true;

                    // good so far, now try to find the rooms in between this one
                    int room = sim.RoomId;
                    List<int> roomLayers = new List<int>();
                    List<int> doneRooms = new List<int>();
                    int loops = 0;
                    while(true)
                    {
                        if(loops >= 10) break;
                        loops++;
                        foreach (CommonDoor obj in sim.LotCurrent.GetObjectsInRoom<CommonDoor>(room))
                    {
                        if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                        {
                                Common.Notify("Testing " + obj.CatalogName + " in " + srcRoom);
                            }

                            int adjRoom = obj.GetAdjoiningRoom(room);

                            if (adjRoom != 0 && !doneRooms.Contains(room))
                            {
                                roomLayers.Add(room);
                                room = adjRoom;
                                continue;
                            }
                        }



                        
                        if (room != sim.RoomId && !CheckRoomDoors(room, sim))
                        {

                        }

                        CommonDoor.tSide sideInRoom = CommonDoor.tSide.Front;
                        obj.GetSideOfDoorInRoom(srcRoom, out sideInRoom);

                        DoorSettings settings = GoHere.Settings.GetDoorSettings(obj.ObjectId, false);

                        if (settings != null && settings.mIsOneWayDoor)
                        {
                            // if it's a one way door, you can only pass through the front so a back isn't in room anywhere, you're not allowed in
                            if (sideInRoom != CommonDoor.tSide.Front)
                            {
                                allNull = false;
                                continue;
                            }
                        }
                     */

                    /*
                        // if front of door isn't in room, we don't care about it's filters
                        if (sideInRoom != CommonDoor.tSide.Front)
                        {
                        Common.Notify("Skipping " + obj.CatalogName + " because front tile is in other room");
                            continue;
                        }
                     */

                    /*
                        if (settings != null)
                        {
                            allNull = false;
                            allowed = settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
                            if (allowed)
                            {
                                if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                                {
                                    Common.Notify(sim.FullName + " initially allowed in " + srcRoom);
                                }
                            }
                            else
                            {
                                if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                                {
                                    Common.Notify(sim.FullName + " NOT initially allowed in " + srcRoom);
                                }
                            }
                        }
                        else
                        {
                            Common.Notify(sim.FullName + " allowed in " + srcRoom + " due to null settings on " + obj.CatalogName);
                            allowed = true;
                        }

                        if (allowed && !adjoiningAllowed)
                        {
                            //Common.Notify("Testing adjoining");
                            // eh...
                            int adjoining = obj.GetAdjoiningRoom(srcRoom);
                            //Common.Notify("Testing adjoining... adjoining to " + srcRoom + " is " + adjoining);
                            foreach (CommonDoor obj2 in obj.LotCurrent.GetObjectsInRoom<CommonDoor>(adjoining))
                            {
                                if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                                {
                                    //Common.Notify("Testing adjoining " + obj2.CatalogName + " in " + adjoining);
                                }

                                sideInRoom = CommonDoor.tSide.Front;
                                obj2.GetSideOfDoorInRoom(adjoining, out sideInRoom);
                                if (sideInRoom != CommonDoor.tSide.Front)
                                {
                                    //Common.Notify("Skipping " + obj2.CatalogName + " because front tile is in other room");
                                    continue;
                                }

                                DoorSettings settings2 = GoHere.Settings.GetDoorSettings(obj.ObjectId, false);
                                if (settings != null)
                                {
                                    allNull = false;
                                    adjoiningAllowed = settings2.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
                                    if (adjoiningAllowed)
                                    {
                                        if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                                        {
                                            //Common.Notify(sim.FullName + " allowed in adjoining " + adjoining);
                                        }
                                        break;
                                    }
                                }
                            }
                        }


                        if (allowed && adjoiningAllowed) break;
                        //if (allowed) break;
                    }

                    if (allNull)
                    {
                        allowed = true;
                    }

                    if (sim.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male)
                    {
                        Common.Notify(sim.FullName + " is in " + sim.RoomId + " allowed: " + allowed.ToString() + " to room " + srcRoom);
                    }

                    return allowed;
                }
                */
                return true;
            }       
                return true;
        }        

        public class Loader : Common.IWorldLoadFinished, Common.IExitBuildBuy
        {
            protected static void ReplaceComponent(Door door)
            {
                if (door.GetComponent<DoorPortalComponentEx>() != null) return;

                if (door.GetComponent<Turnstile.TurnstileDoorPortalComponent>() != null) return;
                if (door.GetComponent<MysteriousDeviceDoor.MysteriousDoorPortalComponent>() != null) return;

                Door.DoorPortalComponent oldComponent = door.PortalComponent as Door.DoorPortalComponent;
                
                door.RemoveComponent<Door.DoorPortalComponent>();

                ObjectComponents.AddComponent<DoorPortalComponentEx>(door, new object[0]);
                
                if (oldComponent != null)
                {
                    Door.DoorPortalComponent newComponent = door.PortalComponent as Door.DoorPortalComponent;
                    if (newComponent != null)
                    {
                        newComponent.OwnerDoor = oldComponent.OwnerDoor;
                    }
                }               
            }

            public void OnWorldLoadFinished()
            {
                foreach (Door door in Sims3.Gameplay.Queries.GetObjects<Door>())
                {
                    ReplaceComponent(door);
                }
            }     
       
            public void OnExitBuildBuy(Lot lot)
            {
                Door[] doors = lot.GetObjects<Door>();
                foreach (Door door in doors)
                {
                    ReplaceComponent(door);
                }

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim != null && sim.mAllowedRooms != null)
                    {
                        sim.mAllowedRooms.Remove(lot.mLotId);
                    }
                }
            }
        }
    }
    }
}