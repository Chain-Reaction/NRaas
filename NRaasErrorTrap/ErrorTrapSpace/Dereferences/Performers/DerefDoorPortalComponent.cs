using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDoorPortalComponent : Dereference<Door.DoorPortalComponent>
    {
        protected override DereferenceResult Perform(Door.DoorPortalComponent reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mLockedLaneIndices", field, objects))
            {
                try
                {
                    reference.Dispose();
                }
                catch
                { }

                RemoveKeys(reference.mLockedLaneIndices,objects);
                return DereferenceResult.Continue;
            }

            if (Matches(reference, "OwnerDoor", field, objects))
            {
                try
                {
                    reference.Dispose();
                }
                catch
                { }

                Remove(ref reference.OwnerDoor);
                return DereferenceResult.Continue;
            }

            return DereferenceResult.Failure;
        }
    }
}
