using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefFVShoppingRegisterConsignedObject : Dereference<FVShoppingRegister.ConsignedObject>
    {
        protected override DereferenceResult Perform(FVShoppingRegister.ConsignedObject reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mObject", field, objects))
            {
                if (Performing)
                {
                    Remove(ref reference.mObject);

                    foreach (FVShoppingRegister register in Sims3.Gameplay.Queries.GetObjects<FVShoppingRegister>())
                    {
                        if (register.mConsignedObjects == null) continue;

                        foreach (List<FVShoppingRegister.ConsignedObject> list in register.mConsignedObjects.Values)
                        {
                            if (list.Remove(reference)) break;
                        }
                    }
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            return DereferenceResult.Failure;
        }
    }
}
