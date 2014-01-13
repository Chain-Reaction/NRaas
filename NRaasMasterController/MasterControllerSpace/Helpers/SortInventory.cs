using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class SortInventory
    {
        public static Item GetSortType(string title)
        {
            return new CommonSelection<Item>(title, Common.DerivativeSearch.Find<Item>()).SelectSingle();
        }

        public static OptionResult Perform(Sims3.Gameplay.Inventory inventory, Item sort)
        {
            if (inventory == null) return OptionResult.Failure;

            if (sort == null) return OptionResult.Failure;

            Dictionary<Type, int> counts = new Dictionary<Type, int>();

            foreach (InventoryStack stack in inventory.mItems.Values)
            {
                foreach (InventoryItem item in stack.List)
                {
                    if (item.mObject == null) continue;

                    counts[item.mObject.GetType()] = stack.List.Count;

                    if (!item.mObject.InUse)
                    {
                        item.mInUse = false;
                    }
                }
            }

            List<SortObject> objs = new List<SortObject>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(inventory))
            {
                if (inventory.TryToRemove(obj))
                {
                    objs.Add(new SortObject(obj, counts[obj.GetType()]));
                }
            }

            try
            {
                objs.Sort(sort);
            }
            catch (Exception e)
            {
                Common.Exception(inventory.Owner, e);
            }

            try
            {
                inventory.IgnoreInventoryValidation = true;

                foreach (SortObject obj in objs)
                {
                    if (!Inventories.TryToMove(obj.mObj, inventory))
                    {
                        obj.mObj.Destroy();
                    }
                }
            }
            finally
            {
                inventory.IgnoreInventoryValidation = false;
            }

            return OptionResult.SuccessClose;
        }

        public class SortObject
        {
            public readonly IGameObject mObj;
            public readonly int mCount;

            public SortObject(IGameObject obj, int count)
            {
                mObj = obj;
                mCount = count;
            }
        }

        public abstract class Item : CommonOptionItem, IComparer<SortObject>
        {
            public Item(string name)
                : base(name, 0)
            { }

            public override string DisplayValue
            {
                get { return null; }
            }

            public abstract int Compare(SortObject x, SortObject y);
        }

        public class ByValueItem : Item
        {
            public ByValueItem()
                : base(Common.Localize("SortInventory:ByValue"))
            { }

            public override int Compare(SortObject x, SortObject y)
            {
                if (x.mObj.Value < y.mObj.Value) return -1;

                if (x.mObj.Value > y.mObj.Value) return 1;

                return x.mObj.GetLocalizedName().CompareTo(y.mObj.GetLocalizedName());
            }
        }

        public class ByCategoryItem : Item
        {
            public ByCategoryItem()
                : base(Common.Localize("SortInventory:ByCategory"))
            { }

            public override int Compare(SortObject x, SortObject y)
            {
                int result = x.mObj.GetType().Name.CompareTo(y.mObj.GetType().Name);
                if (result != 0) return result;

                return x.mObj.GetLocalizedName().CompareTo(y.mObj.GetLocalizedName());
            }
        }

        public class ByNameItem : Item
        {
            public ByNameItem()
                : base(Common.Localize("SortInventory:ByName"))
            { }

            public override int Compare(SortObject x, SortObject y)
            {
                return x.mObj.GetLocalizedName().CompareTo(y.mObj.GetLocalizedName());
            }
        }

        public class ByQuantityItem : Item
        {
            public ByQuantityItem()
                : base(Common.Localize("SortInventory:ByQuantity"))
            { }

            public override int Compare(SortObject x, SortObject y)
            {
                int result = x.mCount.CompareTo(y.mCount);
                if (result != 0) return result;

                return x.mObj.GetLocalizedName().CompareTo(y.mObj.GetLocalizedName());
            }
        }

        public class ByQualityItem : Item
        {
            public ByQualityItem()
                : base(Common.Localize("SortInventory:ByQuality"))
            { }

            public Quality GetQuality(IGameObject obj)
            {
                Ingredient ingredient = obj as Ingredient;
                if (ingredient != null)
                {
                    return ingredient.GetQuality();
                }
                else
                {
                    ICraft craft = obj as ICraft;
                    if (craft != null)
                    {
                        return craft.GetQuality();
                    }
                    else
                    {
                        IFoodContainer food = obj as IFoodContainer;
                        if (food != null)
                        {
                            return food.FoodQuality;
                        }
                        else
                        {
                            IHarvestable harvestable = obj as IHarvestable;
                            if (harvestable != null)
                            {
                                return harvestable.GetQuality();
                            }
                            else
                            {
                                return Quality.Neutral;
                            }
                        }
                    }
                }
            }

            public override int Compare(SortObject x, SortObject y)
            {
                IRelic leftRelic = x.mObj as IRelic;
                IRelic rightRelic = y.mObj as IRelic;

                if ((leftRelic != null) && (rightRelic != null))
                {
                    return leftRelic.Age.CompareTo(rightRelic.Age);
                }

                Quality leftQuality = GetQuality(x.mObj);
                Quality rightQuality = GetQuality(y.mObj);

                return leftQuality.CompareTo(rightQuality);
            }
        }
    }
}
