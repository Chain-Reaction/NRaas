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
    public class DerefSniffOut : Dereference<Terrain.SniffOut>
    {
        protected override DereferenceResult Perform(Terrain.SniffOut reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mHole", field, objects))
            {
                Remove(ref reference.mHole);
                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
