using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Helpers;
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

namespace NRaas.MasterControllerSpace.Town
{
    public class ObjectAddToInventory : OptionItem, ITownOption
    {
        public ObjectAddToInventory()
        { }

        public override string GetTitlePrefix()
        {
            return "ObjectAddToInventory";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new LotProcessor("RadiusAddToInventory", null).Perform(OnAddToInventory);
        }

        public static bool OnAddToInventory(IGameObject obj)
        {
            if (obj is PlumbBob) return false;

            if (obj is Sim) return false;

            if (obj is Mailbox) return false;

            Inventory inventory = null;

            Lot lot = obj.LotCurrent;
            if (lot != null)
            {
                if (lot.Household != null)
                {
                    inventory = lot.Household.SharedFamilyInventory.Inventory;
                }
            }

            if (inventory == null)
            {
                inventory = Household.ActiveHousehold.SharedFamilyInventory.Inventory;
            }

            Inventories.TryToMove(obj, inventory);

            return true;
        }
    }
}
