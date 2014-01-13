using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefInventory : Dereference<Inventory>
    {
        protected override DereferenceResult Perform(Inventory reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mItems", field, objects))
            {
                InventoryItem item = Find<InventoryItem>(objects);
                if (item != null)
                {
                    if (Performing)
                    {
                        InventoryStack stack = Find<InventoryStack>(objects);
                        if (stack != null)
                        {
                            try
                            {
                                reference.RemoveInternal(item, false, stack);
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(item.Object, e);
                            }
                        }
                    }

                    return DereferenceResult.End;
                }
            }

            if (Matches(reference, "Owner", field, objects))
            {
                Remove(ref reference.Owner);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
