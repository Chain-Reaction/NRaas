using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefNpcDriversManager : Dereference<NpcDriversManager>
    {
        protected override DereferenceResult Perform(NpcDriversManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mDescPools", field, objects))
            {
                Remove(reference.mDescPools, objects);

                if (Performing)
                {
                    return DereferenceResult.End;
                }
                else
                {
                    return DereferenceResult.Found;
                }
            }

            if (Matches(reference, "mNpcDrivers", field, objects))
            {
                RemoveKeys(reference.mNpcDrivers, objects);

                if (Performing)
                {
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
