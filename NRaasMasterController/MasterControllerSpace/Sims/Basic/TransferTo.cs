using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
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
    public class TransferTo : DualSimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "TransferTo";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("TransferItem:Destination");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("TransferItem:Source");
        }

        protected override bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.Inventory == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (a == null) return false;

            if (b == null) return false;

            if (a == b) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            return Perform(Name, a.IsFemale, b.CreatedSim.Inventory, a.CreatedSim.Inventory);
        }

        public static bool Perform(string title, bool female, Inventory source, Inventory destination)
        {
            Dictionary<string, SellItem.Item> stack = new Dictionary<string, SellItem.Item>();

            foreach (GameObject obj in Inventories.QuickFind<GameObject>(source))
            {
                if ((obj is INotTransferableOnDeath) || (obj is IHiddenInInventory)) continue;

                if (!(destination.Owner is SharedFamilyInventory))
                {
                    if (!destination.CanAdd(obj)) continue;
                }

                SellItem.Item list;
                if (!stack.TryGetValue(obj.CatalogName, out list))
                {
                    list = new SellItem.Item(obj.CatalogName, obj.GetThumbnailKey());
                    stack.Add(obj.CatalogName, list);
                }

                list.Add(obj);
            }

            CommonSelection<SellItem.Item>.Results selection = new CommonSelection<SellItem.Item>(title, stack.Values).SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return false;

            foreach (SellItem.Item item in selection)
            {
                int count = 1;
                if (item.Count > 1)
                {
                    string text = StringInputDialog.Show(title, Common.Localize("TransferItem:Prompt", female, new object[] { item.Name }), item.Count.ToString());
                    if (string.IsNullOrEmpty(text)) return false;

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
                    Inventories.TryToMove(item.Value[i], destination);
                }
            }

            return true;
        }
    }
}
