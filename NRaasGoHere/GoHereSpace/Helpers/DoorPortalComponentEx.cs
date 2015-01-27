using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Helpers
{
    public class DoorPortalComponentEx : Door.DoorPortalComponent
    {
        public DoorPortalComponentEx()
        { }
        public DoorPortalComponentEx(GameObject obj)
            : base(obj)
        {
            this.OwnerDoor = obj as Door;
        }

        // this is just an extra safety measure
        public override bool OnApproachingPortalObject(Sim sim)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(this.OwnerDoor.ObjectId);
            bool allowed = true;
            if (settings != null)
            {
                allowed = settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
            }

            return (!allowed ? allowed : base.OnApproachingPortalObject(sim));
        }

        public override void OnLaneLocked(Sim sim, LaneInfo info)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(this.OwnerDoor.ObjectId);
            bool allowed = true;
            if (settings != null)
            {
                allowed = settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
            }

            if (!allowed)
            {
                if (sim.SimRoutingComponent != null && sim.SimRoutingComponent.IsRouting)
                {
                    sim.SimRoutingComponent.GetCurrentRoute().DoRouteFail = false;
                }
                return;
            }

            base.OnLaneLocked(sim, info);
        }

        public override bool OnPortalStart(Sim s)
        {
            DoorSettings settings = GoHere.Settings.GetDoorSettings(this.OwnerDoor.ObjectId);

            if (settings != null)
            {
                settings.HandleCost(s);
            }

            return base.OnPortalStart(s);
        }

        [Persistable]
        public class DoorSettings
        {
            public DoorSettings(ObjectGuid guid)
            {
                mGUID = guid;
            }

            public enum SettingType : uint
            {
                Unset = 0,
                Allow = 1,
                Deny = 2
            }

            private List<string> mActiveFilters = new List<string>();
            public bool mMatchAllFilters = false;
            public SettingType mType = SettingType.Deny;
            ObjectGuid mGUID;            
            public int mDoorOpen = -1;
            public int mDoorClose = -1;
            public int mDoorCost = 0;

            public static void ValidateDoorSettings()
            {
                List<ObjectGuid> remove = new List<ObjectGuid>();

                foreach(KeyValuePair<ObjectGuid, DoorSettings> settings in GoHere.Settings.mDoorSettings)
                {
                    if (Simulator.GetProxy(settings.Key) != null) continue;

                    remove.Add(settings.Key);
                }

                foreach (ObjectGuid r in remove)
                {
                    GoHere.Settings.mDoorSettings.Remove(r);
                }
            }

            public bool IsSimAllowedThrough(ulong descId)
            {
                IMiniSimDescription desc = SimDescription.Find(descId);

                if(desc == null)
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
                            if (sims.Contains(desc2.CreatedSim))
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
                                    if (desc2.AssignedRole.RoleGivingObject.LotCurrent == door.LotCurrent)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    if (mDoorOpen != -1 && mDoorClose != -1)
                    {
                        bool allowed = true;
                        if (!SimClock.IsTimeBetweenTimes(mDoorOpen, mDoorClose))
                        {
                            allowed = false;

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
                        }

                        if (!allowed) return allowed;
                    }

                    if (FiltersEnabled > 0)
                    {
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

            public int FiltersEnabled
            {
                get { return mActiveFilters.Count; }
            }

            public void HandleCost(Sim sim)
            {
                if (sim != null && mDoorCost > 0)
                {
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
                    }

                }
            }

            public static void RegisterRoomListeners()
            {
                foreach (Lot lot in LotManager.AllLots)
                {
                    if (lot != null && !lot.IsWorldLot)
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

                        foreach (Sim sim in lot.GetAllActors())
                        {
                            if (sim != null)
                            {
                                sim.mAllowedRooms.Remove(lot.LotId);
                            }
                        }
                    }
                }                
            }

            public static bool OnAllowedInRoomCheck(int srcRoom, Sim sim)
            {
                if (sim != null && sim.LotCurrent != null)
                {
                    bool allowed = false;
                    foreach (RoomConnectionObject obj in sim.LotCurrent.GetObjectsInRoom<RoomConnectionObject>(srcRoom))
                    {
                        DoorSettings settings = GoHere.Settings.GetDoorSettings(obj.ObjectId);
                        if (settings != null)
                        {
                            allowed = settings.IsSimAllowedThrough(sim.SimDescription.SimDescriptionId);
                            if (allowed)
                            {
                                break;
                            }
                        }                        
                    }

                    //Common.Notify(sim.FullName + " allowed: " + allowed.ToString() + " to room " + srcRoom);

                    return allowed;
                }
                return true;
            }       
        }        

        public class Loader : Common.IWorldLoadFinished, Common.IExitBuildBuy
        {
            protected static void ReplaceComponent(Door door)
            {
                if (door.GetComponent<DoorPortalComponentEx>() != null) return;

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
            }
        }
    }
}
