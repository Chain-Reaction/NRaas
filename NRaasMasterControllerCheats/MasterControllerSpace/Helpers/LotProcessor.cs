using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class LotProcessor : ObjectProcessor
    {
        public enum ObjectType : uint
        {
            None = 0x00,
            Local = 0x01,
            Global = 0x02,
            Inventory = 0x04,
            NotInventory = 0x08,
            Sim = 0x10,
            Family = 0x20,
            Active = 0x40,
            Inactive = 0x80,
        }

        List<Lot> mLots;

        ObjectType mLevel;

        public LotProcessor(string localizeKey, List<Lot> lots)
            : base(localizeKey)
        {
            mLots = lots;
            if (mLots == null)
            {
                mLots = new List<Lot>();
            }
        }

        protected override bool Initialize()
        {
            List<Option> allOptions = new List<Option>();
            foreach (ObjectType choice in Enum.GetValues(typeof(ObjectType)))
            {
                if (choice == ObjectType.None) continue;

                if (choice == ObjectType.Global)
                {
                    if (mLots.Count != 0) continue;
                }

                allOptions.Add(new Option(choice));
            }

            CommonSelection<Option>.Results choices = new CommonSelection<Option>(Common.Localize(LocalizeKey + ":MenuName"), allOptions).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return false;

            mLevel = ObjectType.None;

            foreach (Option choice in choices)
            {
                mLevel |= choice.Value;
            }

            return true;
        }

        protected override void GetObjects(Dictionary<string, Item> results)
        {
            if ((mLots.Count > 0) || ((mLevel & ObjectType.Local) == ObjectType.Local) || ((mLevel & ObjectType.Global) != ObjectType.Global))
            {
                List<IGameObject> list = new List<IGameObject>();

                if (mLots.Count > 0)
                {
                    foreach (Lot lot in mLots)
                    {
                        list.AddRange(lot.GetObjects<IGameObject>());

                        if ((lot.Household != null) && ((mLevel & ObjectType.Sim) != ObjectType.Sim))
                        {
                            if (lot.Household.SharedFamilyInventory != null)
                            {
                                list.AddRange(Inventories.QuickDuoFind<IGameObject,GameObject>(lot.Household.SharedFamilyInventory.Inventory));
                            }

                            if (lot.Household.SharedFridgeInventory != null)
                            {
                                list.AddRange(Inventories.QuickDuoFind<IGameObject, GameObject>(lot.Household.SharedFridgeInventory.Inventory));
                            }
                        }
                    }
                }
                else
                {
                    list.AddRange(Sims3.Gameplay.Queries.GetObjects<IGameObject>());
                }

                int index = 0;
                while (index < list.Count)
                {
                    IGameObject obj = list[index];
                    index++;

                    if ((mLevel & ObjectType.Active) == ObjectType.Active)
                    {
                        if (obj.LotCurrent != Household.ActiveHouseholdLot) continue;
                    }
                    else if ((mLevel & ObjectType.Inactive) == ObjectType.Inactive)
                    {
                        if (obj.LotCurrent == Household.ActiveHouseholdLot) continue;
                    }

                    foreach (Lot lot in mLots)
                    {
                        if ((lot != null) && ((mLevel & ObjectType.NotInventory) != ObjectType.NotInventory))
                        {
                            Inventory inventory = obj.Inventory;
                            if (inventory != null)
                            {
                                list.AddRange(Inventories.QuickDuoFind<IGameObject,GameObject>(inventory));
                            }
                        }
                    }

                    string name = GetName(obj, true);

                    bool inInventory = obj.InInventory;

                    if (!inInventory)
                    {
                        if (Inventory.ParentInventory(obj) != null)
                        {
                            inInventory = true;
                        }
                    }

                    if (inInventory)
                    {
                        if ((mLevel & ObjectType.NotInventory) == ObjectType.NotInventory) continue;

                        if ((mLevel & ObjectType.Sim) == ObjectType.Sim)
                        {
                            Inventory inventory = Inventory.ParentInventory(obj);
                            if ((inventory == null) || (!(inventory.Owner is Sim))) continue;
                        }
                        else if ((mLevel & ObjectType.Family) == ObjectType.Family)
                        {
                            Inventory inventory = Inventory.ParentInventory(obj);
                            if (inventory == null) continue;

                            if ((!(inventory.Owner is SharedFamilyInventory)) && (!(inventory.Owner is SharedFridgeInventory))) continue;
                        }
                    }
                    else
                    {
                        if ((mLevel & ObjectType.Family) == ObjectType.Family) continue;

                        if ((mLevel & ObjectType.Sim) == ObjectType.Sim)
                        {
                            if (!(obj is Sim)) continue;
                        }

                        if ((mLevel & ObjectType.Inventory) == ObjectType.Inventory) continue;
                    }

                    Item item;
                    if (!results.TryGetValue(name, out item))
                    {
                        AddItem(results, name, obj);
                    }
                    else
                    {
                        item.Add(obj);
                    }
                }
            }

            if ((mLots.Count == 0) && ((mLevel & ObjectType.Global) == ObjectType.Global))
            {
                foreach (IGameObject obj in Sims3.Gameplay.Queries.GetGlobalObjects<IGameObject>())
                {
                    string name = GetName(obj, true);

                    if (obj.InInventory)
                    {
                        // Don't double account objects stored in inventory
                        if ((mLevel & ObjectType.Local) == ObjectType.Local) continue;

                        if ((mLevel & ObjectType.NotInventory) == ObjectType.NotInventory) continue;

                        if ((mLevel & ObjectType.Sim) == ObjectType.Sim)
                        {
                            Inventory inventory = Inventory.ParentInventory(obj);
                            if ((inventory == null) || (!(inventory.Owner is Sim))) continue;
                        }
                        else if ((mLevel & ObjectType.Family) == ObjectType.Family)
                        {
                            Inventory inventory = Inventory.ParentInventory(obj);
                            if (inventory == null) continue;

                            if ((!(inventory.Owner is SharedFamilyInventory)) && (!(inventory.Owner is SharedFridgeInventory))) continue;
                        }
                    }
                    else
                    {
                        if ((mLevel & ObjectType.Sim) == ObjectType.Sim)
                        {
                            if (!(obj is Sim)) continue;
                        }

                        if ((mLevel & ObjectType.Inventory) == ObjectType.Inventory) continue;
                    }

                    GameObject gameObject = obj as GameObject;
                    if (gameObject != null)
                    {
                        if (gameObject.ActorsUsingMe.Count > 0) continue;
                    }

                    Item item;
                    if (!results.TryGetValue(name, out item))
                    {
                        AddItem(results, name, obj);
                    }
                    else
                    {
                        item.Add(obj);
                    }
                }
            }
        }

        public class Option : ValueSettingOption<ObjectType>
        {
            public Option(ObjectType type)
                : base(type, Common.Localize("ObjectStats:" + type), -1)
            { }
        }
    }
}
