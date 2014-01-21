using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public class DereferenceGameObject : DereferenceController<GameObject>
    {
        [Tunable, TunableComment("Whether to dump the entire reference list to log")]
        public static bool kShowFullReferencing = false;

        static Dictionary<Type, bool> sSilentDestroy = new Dictionary<Type, bool>();

        Dictionary<ObjectGuid, bool> mEventItems = null;

        Dictionary<ObjectGuid,bool> mSharedInventories = null;

        Dictionary<IGameObject, bool> mConsignment = null;

        static DereferenceGameObject()
        {
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.CookingObjects.Cake), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Miscellaneous.StuffedAnimal), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureVaseDaffodil), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Miscellaneous.RubberDucky), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Toys.BaseBall), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.CookingObjects.SnackJuice), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureWallLifePreserver), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureVaseOrchids), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureTableCowPlant), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Miscellaneous.BubbleBath), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Miscellaneous.Newspaper), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureStageFlowersPink), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Decorations.Mimics.SculptureVaseSunFlower), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Toys.Football), true);
            sSilentDestroy.Add(typeof(Sims3.Gameplay.Objects.Environment.FirePit.FirePitChair), true);
        }
        public DereferenceGameObject()
        { }

        protected Dictionary<ObjectGuid, bool> SharedInventories
        {
            get
            {
                if (mSharedInventories == null)
                {
                    mSharedInventories = new Dictionary<ObjectGuid,bool>();

                    foreach (Household house in Household.sHouseholdList)
                    {
                        if (house.SharedFamilyInventory != null)
                        {
                            Inventory inventory = house.SharedFamilyInventory.Inventory;
                            if (inventory != null)
                            {
                                foreach (GameObject obj in Inventories.QuickFind<GameObject>(inventory))
                                {
                                    mSharedInventories[obj.ObjectId] = true;
                                }
                            }
                        }

                        if (house.SharedFridgeInventory != null)
                        {
                            Inventory inventory = house.SharedFridgeInventory.Inventory;
                            if (inventory != null)
                            {
                                foreach (GameObject obj in Inventories.QuickFind<GameObject>(inventory))
                                {
                                    mSharedInventories[obj.ObjectId] = true;
                                }
                            }
                        }
                    }
                }

                    return mSharedInventories;
            }
        }

        protected Dictionary<ObjectGuid, bool> EventItems
        {
            get
            {
                if (mEventItems == null)
                {
                    mEventItems = new Dictionary<ObjectGuid, bool>();

                    foreach (EventLotMarker marker in Sims3.Gameplay.Queries.GetObjects<EventLotMarker>())
                    {
                        foreach (List<ObjectGuid> objs in marker.mLayoutItems.Values)
                        {
                            foreach (ObjectGuid guid in objs)
                            {
                                mEventItems[guid] = true;
                            }
                        }
                    }

                    foreach (SeasonalLotMarker marker in Sims3.Gameplay.Queries.GetObjects<SeasonalLotMarker>())
                    {
                        foreach (List<ObjectGuid> objs in marker.mSeasonalItems.Values)
                        {
                            foreach (ObjectGuid guid in objs)
                            {
                                mEventItems[guid] = true;
                            }
                        }
                    }
                }

                return mEventItems;
            }
        }

        protected Dictionary<IGameObject, bool> Consignment
        {
            get
            {
                if (mConsignment == null)
                {
                    mConsignment = new Dictionary<IGameObject, bool>();

                    foreach(KeyValuePair<ulong,List<ConsignmentRegister.ConsignedObject>> sim in ConsignmentRegister.sConsignedObjects)
                    {
                        foreach(ConsignmentRegister.ConsignedObject obj in sim.Value)
                        {
                            if (obj.mObject == null) continue;

                            mConsignment[obj.mObject] = true;
                        }
                    }

                    foreach (KeyValuePair<ulong, List<PotionShopConsignmentRegister.ConsignedObject>> sim in PotionShopConsignmentRegister.sConsignedObjects)
                    {
                        foreach (PotionShopConsignmentRegister.ConsignedObject obj in sim.Value)
                        {
                            if (obj.mObject == null) continue;

                            mConsignment[obj.mObject] = true;
                        }
                    }

                    foreach (KeyValuePair<ulong, List<BotShopRegister.ConsignedObject>> sim in BotShopRegister.sConsignedObjects)
                    {
                        foreach (BotShopRegister.ConsignedObject obj in sim.Value)
                        {
                            if (obj.mObject == null) continue;

                            mConsignment[obj.mObject] = true;
                        }
                    }
                }
                return mConsignment;
            }
        }

        public override void Clear()
        {
            mConsignment = null;
            mEventItems = null;
            mSharedInventories = null;

            base.Clear();
        }

        protected override void PreProcess(GameObject obj, object parent, FieldInfo field)
        { }

        protected override void Perform(GameObject obj, object referenceParent, FieldInfo field)
        {
            if (obj is Ocean)
            {
                if (LotManager.sOceanObject == obj) return;
            }
            else if (obj is Terrain)
            {
                if (Terrain.sTerrain == obj) return;
            }

            if (DereferenceManager.HasBeenDestroyed(obj))
            {
                if (obj is Sim)
                {
                    LotManager.sActorList.Remove(obj as Sim);

                    DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), true, false);
                }
                else
                {
                    GameObjectReference refObj = ObjectLookup.GetReference(new ReferenceWrapper(obj));
                    if (DereferenceManager.Perform(obj, refObj, false, false))
                    {
                        DereferenceManager.Perform(obj, refObj, true, false);

                        ErrorTrap.LogCorrection("Destroyed Object Found: " + obj.GetType());
                    }
                }
                return;
            }

            if (kShowFullReferencing)
            {
                DereferenceManager.Perform(obj, ObjectLookup.GetReference(new ReferenceWrapper(obj)), false, true);
            }

            if (obj.InWorld) return;

            if (obj.InInventory)
            {
                if (Inventories.ParentInventory(obj) == null)
                {
                    if ((obj.Parent != null) || (Consignment.ContainsKey(obj)))
                    {
                        obj.SetFlags(GameObject.FlagField.InInventory, false);

                        ErrorTrap.LogCorrection("Invalid Inventory Object Unflagged: " + obj.GetType());
                    }
                    else
                    {
                        ErrorTrap.LogCorrection("Invalid Inventory Object Found: " + obj.GetType());
                        ErrorTrap.AddToBeDeleted(obj, true);
                    }
                }

                return;
            }
            else
            {
                if (SharedInventories.ContainsKey(obj.ObjectId))
                {
                    obj.SetFlags(GameObject.FlagField.InInventory, true);

                    ErrorTrap.LogCorrection("Inventory Object Flagged: " + obj.GetType());
                    return;
                }
            }

            if (EventItems.ContainsKey(obj.ObjectId))
            {
                return;
            }

            bool hasParent = false;

            IGameObject parent = obj.Parent;
            while (parent != null)
            {
                hasParent = true;

                if (DereferenceManager.HasBeenDestroyed(parent))
                {
                    ErrorTrap.LogCorrection("Destroyed Parent Object Found: " + parent.GetType());
                    ErrorTrap.AddToBeDeleted(obj, true);

                    hasParent = false;
                    break;
                }

                parent = parent.Parent;
            }

            if (!hasParent)
            {
                ReferenceWrapper refObj = new ReferenceWrapper(obj);

                GameObjectReference reference = ObjectLookup.GetReference(refObj);
                if ((reference != null) && (reference.HasReferences))
                {
                    if (DereferenceManager.Perform(obj, ObjectLookup.GetReference(refObj), false, false))
                    {
                        IScriptProxy proxy = Simulator.GetProxy(obj.ObjectId);
                        if (proxy != null)
                        {
                            IScriptLogic logic = proxy.Target;
                            if (object.ReferenceEquals(logic, obj))
                            {
                                bool log = !sSilentDestroy.ContainsKey(obj.GetType());

                                if (log)
                                {
                                    ErrorTrap.LogCorrection("Out of World Object Found 2: " + obj.GetType());
                                }
                                else
                                {
                                    ErrorTrap.DebugLogCorrection("Out of World Object Found 3: " + obj.GetType());
                                }

                                ErrorTrap.AddToBeDeleted(obj, log);
                            }
                            else
                            {
                                ErrorTrap.DebugLogCorrection("Out of World Object Found 4: " + obj.GetType());
                                ErrorTrap.DebugLogCorrection("Out of World Object Found 5: " + logic.GetType());
                            }
                        }
                        else
                        {
                            DereferenceManager.Perform(obj, ObjectLookup.GetReference(refObj), true, false);
                        }
                    }
                }
                else
                {
                    ErrorTrap.LogCorrection("Out of World Object Found 1: " + obj.GetType());
                    ErrorTrap.AddToBeDeleted(obj, true);
                }
            }
        }
    }
}
