using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckGameObject : Check<GameObject>
    {
        protected override bool PrePerform(GameObject obj, bool postLoad)
        {
            if (obj.InInventory)
            {
                Inventory inventory = Inventory.ParentInventory(obj);
                if (inventory != null)
                {
                    if (inventory.Owner == null)
                    {
                        ErrorTrap.AddToBeDeleted(obj, true);

                        LogCorrection("Corrupt Inventory Object Deleted: " + ErrorTrap.GetName(obj));
                    }
                }
            }

            if (obj.Inventory != null)
            {
                Inventories.CheckInventory(LogCorrection, DebugLogCorrection, ErrorTrap.GetName(obj), obj.Inventory, false);
            }

            ObjectComponents.Cleanup(obj, DebugLogCorrection);
            return true;
        }
    }
}
