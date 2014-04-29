using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupVehicles : AlarmOption, Common.IWorldLoadFinished
    {
        public override string GetTitlePrefix()
        {
            return "CleanupVehicles";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return Overwatch.Settings.mCleanupVehicles;
            }
            set
            {
                Overwatch.Settings.mCleanupVehicles = value;
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kEventSimMovedFromLot, OnLotChanged);

            new Common.AlarmTask(1f, TimeUnit.Hours, OnHourlyPerform, 1f, TimeUnit.Hours);
        }

        /*
        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);
        }
        */

        protected void OnHourlyPerform()
        {
            if (!Overwatch.Settings.mCleanupVehiclesHourly) return;

            PerformAction(false);
        }

        protected static void OnLotChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            Lot oldLot = e.TargetObject as Lot;

            if ((oldLot != null) && (!oldLot.IsResidentialLot))
            {
                foreach (ParkingSpace space in oldLot.GetObjects<ParkingSpace>())
                {
                    CarOwnable ownable = space.ReservedVehicle as CarOwnable;
                    if (ownable == null) continue;

                    if (ownable.GeneratedOwnableForNpc) continue;

                    if ((ownable == sim.GetReservedVehicle()) || (ownable == sim.GetPreferredVehicle()) || (ownable.Driver == sim))
                    {
                        Inventories.TryToMove(ownable, sim);
                    }
                }

                foreach (MooringPost post in oldLot.GetObjects<MooringPost>())
                {
                    BoatOwnable ownable = post.ReservedVehicle as BoatOwnable;
                    if (ownable == null) continue;

                    if (ownable.GeneratedOwnableForNpc) continue;

                    if ((ownable == sim.GetReservedVehicle()) || (ownable == sim.GetPreferredVehicle()) || (ownable.Driver == sim))
                    {
                        Inventories.TryToMove(ownable, sim);
                    }
                }
            }
        }

        protected static void AddOutOfWorldObjects(Dictionary<IGameObject, bool> lookup, List<InventoryItem> list)
        {
            if (list == null) return;

            foreach (InventoryItem item in list)
            {
                if (item.Object == null) continue;

                lookup[item.Object] = true;
            }
        }

        protected static bool RetainOnLot(GameObject obj)
        {
            if (obj == null) return false;

            if (obj.LotCurrent == null) return false;

            if (obj.LotCurrent.LotType != LotType.Residential) return false;

            return true;
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            Dictionary<IGameObject, bool> outOfWorldObjects = new Dictionary<IGameObject, bool>();

            Dictionary<ObjectGuid, bool> reservedObjects = new Dictionary<ObjectGuid, bool>();

            foreach (ParkingSpace space in Sims3.Gameplay.Queries.GetObjects<ParkingSpace>())
            {
                Vehicle vehicle = space.GetContainedVehicle();
                if (vehicle == null) continue;

                if (!RetainOnLot(space)) continue;

                reservedObjects[vehicle.ObjectId] = true;
            }

            foreach (MooringPost post in Sims3.Gameplay.Queries.GetObjects<MooringPost>())
            {
                Vehicle vehicle = post.GetContainedVehicle();
                if (vehicle == null) continue;

                if (!RetainOnLot(post)) continue;

                reservedObjects[vehicle.ObjectId] = true;
            }

            foreach (BikeRack rack in Sims3.Gameplay.Queries.GetObjects<BikeRack>())
            {
                if (!RetainOnLot(rack)) continue;

                foreach(Bicycle bike in rack.GetContainedBikes())
                {
                    reservedObjects[bike.ObjectId] = true;
                }
            }

            foreach (SimDescription sim in SimListing.GetResidents(false).Values)
            {
                if (sim.BoardingSchool != null)
                {
                    AddOutOfWorldObjects(outOfWorldObjects, sim.BoardingSchool.mInventoryItems);
                }

                if (sim.mInventoryItemsWhileInPassport != null)
                {
                    AddOutOfWorldObjects(outOfWorldObjects, sim.mInventoryItemsWhileInPassport);
                }

                Sim createdSim = sim.CreatedSim;
                if (createdSim != null)
                {
                    PoliceStation.GoToJail interaction = createdSim.CurrentInteraction as PoliceStation.GoToJail;
                    if ((interaction != null) && (interaction.mInmatesObjects != null))
                    {
                        foreach (IGameObject obj in interaction.mInmatesObjects)
                        {
                            outOfWorldObjects[obj] = true;
                        }
                    }

                    Vehicle reserved = createdSim.GetReservedVehicle();
                    if (reserved != null)
                    {
                        reservedObjects[reserved.ObjectId] = true;
                    }
                }

                ObjectGuid preferred = sim.mPreferredVehicleGuid;
                if (preferred != ObjectGuid.InvalidObjectGuid)
                {
                    reservedObjects[preferred] = true;
                }
            }

            if (ParentsLeavingTownSituation.sAdultsInventories != null)
            {
                foreach (List<InventoryItem> list in ParentsLeavingTownSituation.sAdultsInventories.Values)
                {
                    AddOutOfWorldObjects(outOfWorldObjects, list);
                }
            }

            int count = 0;

            foreach (Vehicle vehicle in Sims3.Gameplay.Queries.GetObjects<Vehicle>())
            {
                if (outOfWorldObjects.ContainsKey(vehicle)) continue;

                if (vehicle.HasFlags(GameObject.FlagField.IsStolen)) continue;

                if (reservedObjects.ContainsKey(vehicle.ObjectId)) continue;

                if ((!vehicle.InInventory) && (vehicle.Driver != null) && (!vehicle.Driver.HasBeenDestroyed))
                {
                    if (vehicle.Driver.LotCurrent == vehicle.LotCurrent) continue;

                    if (vehicle.Driver.IsPerformingAService) continue;
                }

                // Temporary until further review can be done
                if (vehicle is CarUFO) continue;

                if (vehicle is WindSurfboard)
                {
                    // These vehicles can be placed on the ground, so don't require a parent
                    continue;
                }

                if (vehicle is MagicBroom)
                {
                    if (vehicle.InInventory) continue;

                    if (vehicle.Parent is BroomStand) continue;
                }
                else
                {
                    IOwnableVehicle ownableVehicle = vehicle as IOwnableVehicle;
                    if (ownableVehicle != null)
                    {
                        if (ownableVehicle.PendingUse) continue;

                        CarOwnable carOwnable = vehicle as CarOwnable;
                        if (carOwnable != null) 
                        {
                            if (!carOwnable.GeneratedOwnableForNpc) continue;
                        }
                        else
                        {
                            BoatOwnable boatOwnable = vehicle as BoatOwnable;
                            if (boatOwnable != null)
                            {
                                if (!boatOwnable.GeneratedOwnableForNpc) continue;
                            }
                            else
                            {
                                if (ownableVehicle.InInventory) continue;
                            }
                        }
                    }

                    if ((vehicle is FoodTruckBase) || (vehicle is CarService))
                    {
                        if (vehicle.InWorld) continue;
                    }
                }

                if ((vehicle.InWorld) && (vehicle.InUse))
                {
                    bool found = false;

                    foreach (Sim sim in vehicle.ActorsUsingMe)
                    {
                        if (!sim.HasBeenDestroyed)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found) continue;
                }

                string parentSpace = null;
                if (vehicle.Parent != null)
                {
                    parentSpace = vehicle.Parent.GetType().ToString();
                    if (parentSpace.Contains("GalleryShopParkingSpace"))
                    {
                        continue;
                    }
                }

                {
                    string catalogName = vehicle.CatalogName;

                    bool inInventory = vehicle.InInventory;                    

                    vehicle.UnParent();

                    IUsesParkingSpace parker = vehicle as IUsesParkingSpace;

                    if (parker != null)
                    {
                        ParkingSpace space = vehicle.Parent as ParkingSpace;
                        if (space != null)
                        {
                            space.UnReserveSpot(parker);
                        }
                        else
                        {
                            MooringPost post = vehicle.Parent as MooringPost;
                            if (post != null)
                            {
                                post.UnReserveSpot(parker as Boat);
                            }
                        }
                    }

                    vehicle.Destroy();

                    if (!(vehicle is LightweightTaxi))
                    {
                        count++;
                        Overwatch.Log("Towed " + catalogName);

                        if (inInventory)
                        {
                            Overwatch.Log(" Was InInventory");
                        }

                        if (!string.IsNullOrEmpty(parentSpace))
                        {
                            Overwatch.Log(" Space: " + parentSpace);
                        }
                    }
                }
            }

            if ((count > 0) || (prompt))
            {
                Overwatch.AlarmNotify(Common.Localize("CleanupVehicles:Complete", false, new object[] { count }));
            }
        }
    }
}
