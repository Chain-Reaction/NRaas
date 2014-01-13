using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class SellItem : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "SellItem";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.Household == null) return false;

            if (me.CreatedSim.Inventory == null) return false;

            return base.PrivateAllow(me);
        }

        public static bool Perform(string title, string subTitle, List<Inventory> inventories)
        {
            Dictionary<string, Item> stack = new Dictionary<string, Item>();

            foreach (Inventory inventory in inventories)
            {
                foreach (GameObject obj in Inventories.QuickFind<GameObject>(inventory))
                {
                    if ((obj is INotTransferableOnDeath) || (obj is IHiddenInInventory)) continue;

                    if (!obj.CanBeSoldBase()) continue;

                    Item list;
                    if (!stack.TryGetValue(obj.CatalogName, out list))
                    {
                        list = new Item(obj.CatalogName, obj.GetThumbnailKey());
                        stack.Add(obj.CatalogName, list);
                    }

                    list.Add(obj);
                }
            }

            stack.Add("All", new Item("(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")", ThumbnailKey.kInvalidThumbnailKey));

            List<Item> selection = new List<Item>(new CommonSelection<Item>(title, subTitle, stack.Values).SelectMultiple());
            if (selection.Count == 0) return false;

            bool all = false;
            foreach (Item item in selection)
            {
                if (item.Thumbnail == ThumbnailKey.kInvalidThumbnailKey)
                {
                    all = true;
                    break;
                }
            }

            if (all)
            {
                selection = new List<Item>(stack.Values);
            }

            foreach (Item item in selection)
            {
                int count = 1;
                if (all)
                {
                    count = item.Count;
                }
                else if (item.Count > 1)
                {
                    string text = StringInputDialog.Show(title, Common.Localize("SellItem:Prompt", false, new object[] { item.Name }), item.Count.ToString());
                    if ((text == null) || (text == "")) return false;

                    if (!int.TryParse(text, out count))
                    {
                        SimpleMessageDialog.Show(title, Common.Localize("Numeric:Error"));
                        return false;
                    }
                }

                if (count > item.Count)
                {
                    count = item.Count;
                }

                for (int i = 0; i < count; i++)
                {
                    Inventory source;
                    if (Inventories.TryToRemove(item.Value[i], out source))
                    {
                        int value = item.Value[i].Value;

                        Household house = null;

                        Sim sourceSim = source.Owner as Sim;
                        if (sourceSim != null)
                        {
                            house = sourceSim.Household;

                            Consignments.NotifySell(sourceSim.SimDescription, item.Value[i], value);
                        }
                        else
                        {
                            SharedFamilyInventory familyInventory = source.Owner as SharedFamilyInventory;
                            if (familyInventory != null)
                            {
                                house = familyInventory.OwnerHousehold;
                            }
                            else if (source.Owner.LotCurrent != null)
                            {
                                house = source.Owner.LotCurrent.Household;
                            }
                        }

                        item.Value[i].Destroy();

                        if (house != null)
                        {
                            house.ModifyFamilyFunds(value);
                        }
                    }
                }
            }

            return true;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            string subTitle = null;

            if (sims.Count == 1)
            {
                subTitle = sims[0].FullName;
            }

            List<Inventory> inventories = new List<Inventory>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                if (sim.CreatedSim == null) continue;

                inventories.Add(sim.CreatedSim.Inventory);
            }

            if (Perform(Name, subTitle, inventories))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                return OptionResult.Failure;
            }
        }

        public class Item : ValueSettingOption<List<GameObject>>
        {
            public Item()
            {
                mValue = new List<GameObject>();
            }
            public Item(string name, ThumbnailKey icon)
                : base(new List<GameObject>(), name, 0, icon)
            { }

            public void Add(GameObject obj)
            {
                Value.Add(obj);

                IncCount();
            }
        }
    }
}
