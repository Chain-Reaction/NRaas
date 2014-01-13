using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefItemComponent : Dereference<ItemComponent>
    {
        protected override DereferenceResult Perform(ItemComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "InventoryParent", field, objects))
            {
                if (Performing)
                {
                    try
                    {
                        reference.RemoveFromInventory();
                    }
                    catch
                    { }

                    Remove(ref reference.InventoryParent);
                }
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
