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
    public class DerefRideHereTone : Dereference<Terrain.RideHere.RideHereTone>
    {
        protected override DereferenceResult Perform(Terrain.RideHere.RideHereTone reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mRider", field, objects))
            {
                Remove(ref reference.mRider);
                return DereferenceResult.ContinueIfReferenced;
            }

            return DereferenceResult.Failure;
        }
    }
}
