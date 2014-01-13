using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class InventorySaver
    {
        [Tunable]
        protected static bool kInstantiator = false;

        protected static int sAttempts = 0;

        private static EventListener sBoughtObjectLister = null;

        static InventorySaver()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
        }
        public InventorySaver()
        { }

        public static void AddInteractions(Sims3.Gameplay.Objects.Electronics.Computer obj)
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.InteractionDefinition.GetType() == Version.Singleton.GetType())
                {
                    return;
                }
            }

            obj.AddInteraction(Version.Singleton);
        }

        public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string str = assembly.GetName().Name;
                if (str.ToLower () == "awesome")
                {
                    SimpleMessageDialog.Show("InventorySaver", "You appear to have AwesomeMod installed.  Be aware that this mod supercedes any changes to the inventory purger in Awesome.");
                }
            }

            List<Sims3.Gameplay.Objects.Electronics.Computer> others = new List<Sims3.Gameplay.Objects.Electronics.Computer>(Sims3.Gameplay.Queries.GetObjects<Sims3.Gameplay.Objects.Electronics.Computer>());
            foreach (Sims3.Gameplay.Objects.Electronics.Computer obj in others)
            {
                AddInteractions(obj);
            }

            sBoughtObjectLister = EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectBought));

            // Used in GiveIngredients() to add new ingredients to the fridge on household selection
            Sims3.Gameplay.CAS.Household.kNumServingsOnStartup = 0;

            sAttempts = 0;
            AlarmManager.Global.AddAlarm(1f, TimeUnit.Minutes, new AlarmTimerCallback(OnStartAlarm), "NRaasInventorySaverAlarm", AlarmType.NeverPersisted, null);
        }

        public static void OnStartAlarm()
        {
            sAttempts++;
            if (LotManager.sVenueMaintenanceAlarm != AlarmHandle.kInvalidHandle)
            {
                StyledNotification.Show(new StyledNotification.Format("InventorySaver Activated", ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kGameMessagePositive));

                AlarmManager.Global.RemoveAlarm(LotManager.sVenueMaintenanceAlarm);
                LotManager.sVenueMaintenanceAlarm = AlarmManager.Global.AddAlarmDay(LotManager.kVenueCleanupTime, ~DaysOfTheWeek.None, new AlarmTimerCallback(OnCleanUpLotsAndInventories), "Global venue cleanup alarm", AlarmType.NeverPersisted, null);
            }
            else
            {
                if (sAttempts < 30)
                {
                    AlarmManager.Global.AddAlarm(1f, TimeUnit.Minutes, new AlarmTimerCallback(OnStartAlarm), "NRaasInventorySaverAlarm", AlarmType.NeverPersisted, null);
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("InventorySaver was unable to start.  Your inventory may not be safe.", ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
                }
            }
        }

        public static void MagicallyCleanUp(Lot lot, bool bAlsoRepair)
        {
            foreach (GameObject obj2 in lot.FindAllObjectsToClean())
            {
                if (!obj2.InUse)
                {
                    if (obj2.IsCleanable)
                    {
                        obj2.Cleanable.ForceClean();
                    }
                    IThrowAwayable awayable = obj2 as IThrowAwayable;
                    if (awayable != null)
                    {
                        awayable.ThrowAwayImmediately();
                    }
                    if (obj2 is Book)
                    {
                        Book book = obj2 as Book;
                        if (book.MyShelf != null)
                        {
                            if (!book.InInventory)
                            {
                                book.MyShelf.Inventory.TryToAdd(book);
                            }
                        }
                        else
                        {
                            Lot ownerLot = book.GetOwnerLot();
                            if (ownerLot == null)
                            {
                                ownerLot = lot;
                            }

                            List<Bookshelf> shelves = new List<Bookshelf> (ownerLot.GetObjects<Bookshelf> ());

                            if (shelves.Count > 0)
                            {
                                shelves[0].Inventory.TryToAdd(book);
                            }
                            else if (ownerLot.LotId == Sim.ActiveActor.LotHome.LotId)
                            {
                                Sim.ActiveActor.Household.SharedFamilyInventory.Inventory.TryToAdd(book);
                            }
                            else
                            {
                                book.FadeOut(false, true);
                            }
                        }
                    }
                }
            }
            LotLocation[] puddles = World.GetPuddles(lot.LotCurrent.LotId, LotLocation.Invalid);
            if (puddles.Length > 0)
            {
                foreach (LotLocation location in puddles)
                {
                    PuddleManager.RemovePuddle(lot.LotCurrent.LotId, location);
                }
            }
            LotLocation[] burntTiles = World.GetBurntTiles(lot.LotCurrent.LotId, LotLocation.Invalid);
            if (burntTiles.Length > 0)
            {
                foreach (LotLocation location2 in burntTiles)
                {
                    World.SetBurnt(lot.LotId, location2, false);
                }
            }
            if (bAlsoRepair)
            {
                lot.RepairAllObjects();
            }
        }

        private static void OnCleanUpLotsAndInventories()
        {
            Lot lot = (Household.ActiveHousehold != null) ? Household.ActiveHousehold.LotHome : null;
            foreach (Lot lot2 in LotManager.AllLots)
            {
                if (lot2 != lot)
                {
                    MagicallyCleanUp(lot2, !lot2.IsWorldLot);
                    if (((lot2.Household != null)) && ((lot2.Household.SharedFridgeInventory != null) && (lot2.Household.SharedFridgeInventory.Inventory != null)))
                    {
                        //lot2.Household.SharedFridgeInventory.Inventory.DestroyItems(false);
                        lot2.Household.SharedFridgeInventory.RemoveSpoiledFood();
                    }
                }
            }
/*
            foreach (Sim sim in Actors)
            {
                if (!sim.IsSelectable)
                {
                    sim.PurgeInventory();
                }
            }
*/
        }

        protected static ListenerAction OnObjectBought(Sims3.Gameplay.EventSystem.Event e)
        {
            if (e.Id == EventTypeId.kBoughtObject)
            {
                Sims3.Gameplay.Objects.Electronics.Computer obj = e.TargetObject as Sims3.Gameplay.Objects.Electronics.Computer;
                if (obj != null)
                {
                    AddInteractions(obj);
                }
            }

            return ListenerAction.Keep;
        }

        public class Version : ImmediateInteraction<Sim, Sims3.Gameplay.Objects.Electronics.Computer>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new VersionDefinition();

            public Version()
            { }

            // Nested Types
            [DoesntRequireTuning]
            private sealed class VersionDefinition : ImmediateInteractionDefinition<Sim, Sims3.Gameplay.Objects.Electronics.Computer, Version>
            {
                public override string[] GetPath()
                {
                    return new string[] { "Inventory Saver..." };
                }

                public override string GetInteractionName(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, InteractionObjectPair interaction)
                {
                    return "Version";
                }

                public override bool Test(Sim a, Sims3.Gameplay.Objects.Electronics.Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous) return false;

                    return true;
                }
            }

            public override bool Run()
            {
                string msg = null;
                if ((sAttempts > 0) && (sAttempts < 30))
                {
                    msg = "Status: Enabled";
                }
                else
                {
                    msg = "Status: Disabled";
                }

                SimpleMessageDialog.Show("InventorySaver Version", "Version 4\n" + msg);
                return true;
            }
        }
    }
}
