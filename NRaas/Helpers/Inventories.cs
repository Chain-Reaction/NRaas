using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Inventories
    {
        // Must be a different name than the other overrides (Intepreter crash)
        public static List<DERIVATIVE> QuickDuoFind<DERIVATIVE,BASETYPE>(Inventory ths)
            where DERIVATIVE : class, IGameObject
            where BASETYPE : GameObject
        {
            List<DERIVATIVE> retList = new List<DERIVATIVE>();
            if (ths != null)
            {
                foreach (InventoryStack stack in ths.mItems.Values)
                {
                    if (stack.List.Count == 0) continue;

                    if (stack.List[0].Object is BASETYPE)
                    {
                        foreach (InventoryItem item in stack.List)
                        {
                            DERIVATIVE local = item.Object as DERIVATIVE;
                            if (local != null)
                            {
                                retList.Add(local);
                            }
                        }
                    }
                }
            }

            return retList;
        }

        public static List<T> QuickFind<T>(Inventory ths) 
            where T : GameObject
        {
            List<T> retList = new List<T>();
            if (ths != null)
            {
                foreach (InventoryStack stack in ths.mItems.Values)
                {
                    if (stack.List.Count == 0) continue;

                    if (stack.List[0].Object is T)
                    {
                        foreach (InventoryItem item in stack.List)
                        {
                            T local = item.Object as T;
                            if (local != null)
                            {
                                retList.Add(local);
                            }
                        }
                    }
                }
            }

            return retList;
        }
        public static List<T> QuickFind<T>(Inventory ths, bool checkInUse, ItemTestFunction testFunction)
            where T : GameObject
        {
            return QuickFind<T>(ths, checkInUse, testFunction, null);
        }
        public static List<T> QuickFind<T>(Inventory ths, bool checkInUse, ItemTestFunction testFunction, object customData) 
            where T : GameObject
        {
            if ((!checkInUse) && (testFunction == null))
            {
                return QuickFind<T>(ths);
            }
            else
            {
                List<T> retList = new List<T>();
                QuickApplyFuncToAllItems<T>(
                    ths,
                    delegate(InventoryItem item)
                    {
                        if (!checkInUse || (!item.InUse && !ths.GetStackInUse(item.StackNumber)))
                        {
                            T local = item.Object as T;
                            if ((local != null) && ((testFunction == null) || testFunction(item.Object, customData)))
                            {
                                retList.Add(local);
                            }
                        }
                    },
                    false
                );
                return retList;
            }
        }

        private static void QuickApplyFuncToAllItems<T>(Inventory ths, Inventory.ItemFunction func, bool stackItemsCanChange)
            where T : GameObject
        {
            if (ths == null) return;

            if (stackItemsCanChange)
            {
                foreach (InventoryStack stack in new List<InventoryStack>(ths.mItems.Values))
                {
                    if (stack.List.Count == 0) continue;

                    if (stack.List[0].Object is T)
                    {
                        foreach (InventoryItem item in new List<InventoryItem>(stack.List))
                        {
                            func(item);
                        }
                    }
                }
            }
            else
            {
                foreach (InventoryStack stack in ths.mItems.Values)
                {
                    if (stack.List.Count == 0) continue;

                    if (stack.List[0].Object is T)
                    {
                        foreach (InventoryItem item in stack.List)
                        {
                            func(item);
                        }
                    }
                }
            }
        }

        private static string GetName(IGameObject obj)
        {
            try
            {
                string name = obj.GetLocalizedName();
                if (!string.IsNullOrEmpty(name))
                {
                    return name.Trim();
                }
            }
            catch
            { }

            return obj.GetType().ToString();
        }

        public delegate void Logger(string text);

        public static void RestoreInventoryFromList(Inventory ths, List<InventoryItem> items, bool deleteExisting)
        {
            if (deleteExisting)
            {
                ths.DestroyItems();
            }

            if (items != null)
            {
                foreach (InventoryItem item in items)
                {
                    if (!ths.TryToAdd(item.Object, false))
                    {
                        item.Object.Destroy();
                    }
                }
            }
        }

        public static void CheckInventories(Logger log, Logger debugLog, bool checkDestroyed)
        {
            CheckInventories(log, debugLog, checkDestroyed, 0);
        }
        // sleepAfter > 0 : sleeps a tick once that many milliseconds of the frame are used, also inside large inventories,
        // for callers running in their own task
        public static void CheckInventories(Logger log, Logger debugLog, bool checkDestroyed, int sleepAfter)
        {
            SleepTimer timer = new SleepTimer(sleepAfter);

            foreach (GameObject parent in Sims3.Gameplay.Queries.GetObjects<GameObject>())
            {
                // Skips the localized name lookup for the many objects without an inventory
                if (parent.Inventory == null) continue;

                if (timer.Sleep() && timer.Stopped) return;

                // Objects can go away while we sleep
                if ((timer.Slept) && (parent.HasBeenDestroyed)) continue;

                CheckInventory(log, debugLog, GetName(parent), parent.Inventory, checkDestroyed, timer);
                if (timer.Stopped) return;
            }

            foreach (Household house in new List<Household>(Household.sHouseholdList))
            {
                if (timer.Sleep() && timer.Stopped) return;

                if (house.SharedFamilyInventory != null)
                {
                    CheckInventory(log, debugLog, house.Name + " Family", house.SharedFamilyInventory.Inventory, checkDestroyed, timer);
                    if (timer.Stopped) return;
                }

                if (house.SharedFridgeInventory != null)
                {
                    CheckInventory(log, debugLog, house.Name + " Fridge", house.SharedFridgeInventory.Inventory, checkDestroyed, timer);
                    if (timer.Stopped) return;
                }
            }
        }

        // Sleeps a tick once the frame used the given milliseconds; does nothing when that is 0
        class SleepTimer
        {
            Sims3.SimIFace.StopWatch mWatch;

            uint mInterval;

            // A sleep happened since the check started
            public bool Slept;

            // The world is shutting down, the check stops
            public bool Stopped;

            public SleepTimer(int interval)
            {
                if (interval <= 0) return;

                mInterval = (uint)interval;

                mWatch = Sims3.SimIFace.StopWatch.Create(Sims3.SimIFace.StopWatch.TickStyles.Milliseconds);
                mWatch.Start();
            }

            public bool Active
            {
                get { return (mWatch != null); }
            }

            // True when it slept
            public bool Sleep()
            {
                if (mWatch == null) return false;

                if (mWatch.GetElapsedTime() < mInterval) return false;

                if (!Common.Sleep()) return false;

                mWatch.Restart();

                Slept = true;
                Stopped = GameStates.IsWorldShuttingDown;
                return true;
            }
        }

        public static void CheckInventory(Logger log, Logger debugLog, string name, Inventory inventory, bool checkDestroyed)
        {
            CheckInventory(log, debugLog, name, inventory, checkDestroyed, new SleepTimer(0));
        }
        // With an active timer the stacks and items are copied first and looked up again after each sleep, since the game can
        // change them meanwhile. Without one this is the same single-frame check as always.
        static void CheckInventory(Logger log, Logger debugLog, string name, Inventory inventory, bool checkDestroyed, SleepTimer timer)
        {
            try
            {
                if (inventory == null) return;

                List<uint> badStacks = new List<uint>();

                IEnumerable<KeyValuePair<uint, InventoryStack>> stacks = inventory.mItems;
                if (timer.Active)
                {
                    stacks = new List<KeyValuePair<uint, InventoryStack>>(inventory.mItems);
                }

                foreach (KeyValuePair<uint, InventoryStack> stack in stacks)
                {
                    if ((stack.Value == null) || (stack.Value.List == null))
                    {
                        badStacks.Add(stack.Key);
                        continue;
                    }

                    List<InventoryItem> items = null;
                    if (timer.Active)
                    {
                        // Removed or replaced by the game while we slept
                        if (!IsStack(inventory, stack)) continue;

                        items = new List<InventoryItem>(stack.Value.List);
                    }

                    for (int pos = stack.Value.List.Count - 1; pos >= 0; pos--)
                    {
                        int index = pos;
                        if (timer.Active)
                        {
                            if (timer.Sleep())
                            {
                                if (timer.Stopped) return;

                                if (!IsStack(inventory, stack)) break;
                            }

                            // Moved or taken out by the game while we slept
                            if ((index >= stack.Value.List.Count) || (stack.Value.List[index] != items[pos]))
                            {
                                index = IndexOf(stack.Value.List, items[pos]);
                                if (index < 0) continue;
                            }
                        }

                        InventoryItem item = stack.Value.List[index];

                        if (item.Object == null)
                        {
                            log(name + " Bogus Inventory Item dropped (1)");

                            stack.Value.List.RemoveAt(index);
                        }
                        else if (item.Object.ItemComp == null)
                        {
                            if (!(inventory.Owner is IHouseholdInventory))
                            {
                                log(name + " Bogus Inventory Item dropped (2) " + GetName(item.Object));

                                stack.Value.List.RemoveAt(index);
                            }
                        }
                        else if ((checkDestroyed) && (item.Object.HasBeenDestroyed))
                        {
                            log(name + " Bogus Inventory Item dropped (3) " + GetName(item.Object));

                            stack.Value.List.RemoveAt(index);
                        }
                        else
                        {
                            if (item.Object.ItemComp.InventoryParent == null)
                            {
                                item.Object.ItemComp.InventoryParent = inventory.Owner.Inventory;

                                if (!(inventory.Owner is SharedFamilyInventory))
                                {
                                    debugLog(name + " Inventory Object Reparented: " + item.Object.GetType());
                                }
                            }

                            GameObject obj = item.Object as GameObject;
                            string str = string.Empty;
                            try
                            {                                
                                str += obj.CatalogName;
                                str = string.Empty;
                            }
                            catch
                            {
                                str = obj.GetType().ToString();
                                if (str.Contains("JamJar"))
                                {
                                    try
                                    {
                                        obj.Dispose();
                                        obj.Destroy();
                                        stack.Value.List.RemoveAt(index);

                                        debugLog("Corrupt JamJar deleted from inventory");
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                        // Our own change removed the stack (jam jar): stop here, as the dictionary loop above does without a timer
                        if ((timer.Active) && (!IsStack(inventory, stack)))
                        {
                            throw new InvalidOperationException("Inventory stacks changed");
                        }
                    }

                    if (stack.Value.List.Count == 0)
                    {
                        badStacks.Add(stack.Key);
                    }
                }

                foreach (uint value in badStacks)
                {
                    // Refilled by the game while we slept
                    InventoryStack current;
                    if ((timer.Active) && (inventory.mItems.TryGetValue(value, out current)) && (current != null) && (current.List != null) && (current.List.Count > 0)) continue;

                    inventory.mItems.Remove(value);
                }
            }
            catch (Sims3.SimIFace.ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(inventory.Owner, e);
            }
        }

        static int IndexOf(Sims3.Gameplay.Utilities.ListSorted<InventoryItem> list, InventoryItem item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == item) return i;
            }

            return -1;
        }

        static bool IsStack(Inventory inventory, KeyValuePair<uint, InventoryStack> stack)
        {
            InventoryStack current;
            if (!inventory.mItems.TryGetValue(stack.Key, out current)) return false;

            return (current == stack.Value);
        }

        public static Inventory ParentInventory(IGameObject obj)
        {
            Inventory inventory = Inventory.ParentInventory(obj);
            if (inventory != null) return inventory;

            if (obj.InInventory)
            {
                foreach (Household house in Household.sHouseholdList)
                {
                    if (house.SharedFamilyInventory != null)
                    {
                        inventory = house.SharedFamilyInventory.Inventory;
                        if (inventory != null)
                        {
                            if (inventory.Contains(obj))
                            {
                                return inventory;
                            }
                        }
                    }

                    if (house.SharedFridgeInventory != null)
                    {
                        inventory = house.SharedFridgeInventory.Inventory;
                        if (inventory != null)
                        {
                            if (inventory.Contains(obj))
                            {
                                return inventory;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static bool VerifyInventory(SimDescription sim)
        {
            if (sim == null) return false;

            Sim createdSim = sim.CreatedSim;
            if (createdSim == null) return false;

            Inventory inventory = createdSim.Inventory;
            if (inventory == null) return false;

            if (inventory.mItems == null)
            {
                Common.DebugNotify("Bad Inventory : " + sim.FullName, createdSim);
                return false;
            }

            return true;
        }

        // Must be a different name than the other overrides (Intepreter crash)
        public static List<DERIVATIVE> InventoryDuoFindAll<DERIVATIVE, BASETYPE>(SimDescription sim)
            where DERIVATIVE : class, IGameObject
            where BASETYPE : GameObject
        {
            if (!VerifyInventory(sim)) return new List<DERIVATIVE>();

            return QuickDuoFind<DERIVATIVE, BASETYPE>(sim.CreatedSim.Inventory);
        }

        public static T InventoryFind<T>(SimDescription sim)
            where T : class, IGameObject
        {
            if (!VerifyInventory(sim)) return null;

            return sim.CreatedSim.Inventory.Find<T>();
        }

        public static List<T> InventoryFindAll<T>(SimDescription sim)
            where T : GameObject
        {
            if (!VerifyInventory(sim)) return new List<T>();

            return QuickFind<T>(sim.CreatedSim.Inventory);
        }
        public static List<T> InventoryFindAll<T>(SimDescription sim, bool checkInUse, ItemTestFunction func)
            where T : GameObject
        {
            if (!VerifyInventory(sim)) return new List<T>();

            return QuickFind<T>(sim.CreatedSim.Inventory, checkInUse, func);
        }

        private static bool TryToAdd(Inventory ths, IGameObject obj, bool testValidity)
        {
            if ((!testValidity) || (ths.CanAdd(obj)))
            {
                uint stackNumber = ths.FindValidStack(obj);
                return AddInternal(ths, obj, stackNumber, false);
            }
            return false;
        }

        private static bool AddInternal(Inventory ths, IGameObject obj, uint stackNumber, bool testPurge)
        {
            InventoryStack stack = ths.GetStack(ref stackNumber);
            try
            {
                return (ths.AddInternal(obj, stackNumber, stack, testPurge) != null);
            }
            catch (Exception e)
            {
                Common.Exception(ths.Owner, obj, e);
                return false;
            }
        }

        public static bool TryToMove(IGameObject obj, Sim destination)
        {
            if (destination == null) return false;

            return TryToMove(obj, destination.Inventory);
        }
        public static bool TryToMove(IGameObject obj, Sim destination, bool testValidity)
        {
            if (destination == null) return false;

            return TryToMove(obj, destination.Inventory, testValidity);
        }
        public static bool TryToMove(IGameObject obj, Inventory destination)
        {
            string failureReason;
            return TryToMove(obj, destination, true, out failureReason);
        }
        public static bool TryToMove(IGameObject obj, Inventory destination, bool testValidity)
        {
            string failureReason;
            return TryToMove(obj, destination, testValidity, out failureReason);
        }
        public static bool TryToMove(IGameObject obj, Inventory destination, bool testValidity, out string failureReason)
        {
            if (destination == null)
            {
                failureReason = "No Destination";
                return false;
            }

            Inventory source = ParentInventory(obj);

            if (source == destination)
            {
                failureReason = "Source is Destination";
                return true;
            }

            if ((testValidity) && (!destination.ValidForThisInventory(obj)))
            {
                failureReason = "Not ValidForThisInventory";
                return false;
            }

            bool flag = true;
            if (source != null)
            {
                flag = source.RemoveInternal(obj, false, false);
            }

            if (!flag)
            {
                failureReason = "RemoveInternal Fail";
                return false;
            }

            bool flag2 = TryToAdd(destination, obj, testValidity);
            if (!flag2 && (source != null))
            {
                TryToAdd(source, obj, false);
            }

            if (!flag2)
            {
                failureReason = "TryToAdd Fail";
            }
            else
            {
                failureReason = "";
            }

            return flag2;
        }

        public static bool TryToRemove(IGameObject obj, out Inventory source)
        {
            source = ParentInventory(obj);

            bool flag = true;
            if (source != null)
            {
                flag = source.RemoveInternal(obj, false, false);
            }

            return flag;
        }
    }
}

