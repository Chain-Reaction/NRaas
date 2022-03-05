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
            foreach (GameObject parent in Sims3.Gameplay.Queries.GetObjects<GameObject>())
            {
                CheckInventory(log, debugLog, GetName(parent), parent.Inventory, checkDestroyed);
            }

            foreach (Household house in Household.sHouseholdList)
            {
                if (house.SharedFamilyInventory != null)
                {
                    CheckInventory(log, debugLog, house.Name + " Family", house.SharedFamilyInventory.Inventory, checkDestroyed);
                }

                if (house.SharedFridgeInventory != null)
                {
                    CheckInventory(log, debugLog, house.Name + " Fridge", house.SharedFridgeInventory.Inventory, checkDestroyed);
                }
            }
        }

        public static void CheckInventory(Logger log, Logger debugLog, string name, Inventory inventory, bool checkDestroyed)
        {
            try
            {
                if (inventory == null) return;

                List<uint> badStacks = new List<uint>();

                foreach (KeyValuePair<uint, InventoryStack> stack in inventory.mItems)
                {
                    if ((stack.Value == null) || (stack.Value.List == null))
                    {
                        badStacks.Add(stack.Key);
                        continue;
                    }

                    for (int index = stack.Value.List.Count - 1; index >= 0; index--)
                    {
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
                    }

                    if (stack.Value.List.Count == 0)
                    {
                        badStacks.Add(stack.Key);
                    }
                }

                foreach (uint value in badStacks)
                {
                    inventory.mItems.Remove(value);
                }
            }
            catch (Exception e)
            {
                Common.Exception(inventory.Owner, e);
            }
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

